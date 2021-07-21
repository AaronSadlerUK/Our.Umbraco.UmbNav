using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UmbNavV8.Core.Enums;
using UmbNavV8.Core.Models;
using UmbNavV8.Core.PropertyEditors;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;

namespace UmbNavV8.Core.ValueConverters
{
    public class UmbNavV8ValueConverter : PropertyValueConverterBase
    {
        private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;
        private readonly ILogger _logger;

        private bool _removeNaviHideItems;
        private bool _removeNoopener;
        private bool _removeNoreferrer;

        public UmbNavV8ValueConverter(IPublishedSnapshotAccessor publishedSnapshotAccessor, ILogger logger)
        {
            _publishedSnapshotAccessor = publishedSnapshotAccessor;
            _logger = logger;
        }

        public override bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals(Constants.PropertyEditorAlias);
        }

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(IEnumerable<UmbNavItem>);

        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            if (inter == null)
            {
                return Enumerable.Empty<UmbNavItem>();
            }

            var configuration = propertyType.DataType.ConfigurationAs<UmbNavV8Configuration>();

            if (configuration != null)
            {
                _removeNaviHideItems = configuration.RemoveNaviHideItems;
                _removeNoopener = configuration.HideNoopener;
                _removeNoreferrer = configuration.HideNoreferrer;
            }

            try
            {
                var items = JsonConvert.DeserializeObject<IEnumerable<UmbNavInternalItem>>(inter.ToString());

                return BuildMenu(items);
            }
            catch (Exception ex)
            {
                _logger.Error<UmbNavV8ValueConverter>("Failed to convert UmbNav {ex}", ex);
            }

            return Enumerable.Empty<UmbNavItem>();
        }

        private IEnumerable<UmbNavItem> BuildMenu(IEnumerable<UmbNavItem> items, int level = 0)
        {
            items = items.ToList();

            foreach (var item in items)
            {
                item.Level = level;

                // it's likely a content item
                if (item.Id > 0)
                {
                    IPublishedContent umbracoContent;
                    string currentCulture;

                    if (item.Udi != null)
                    {
                        currentCulture = _publishedSnapshotAccessor.PublishedSnapshot.Content.GetById(item.Udi)?.GetCultureFromDomains();
                        umbracoContent = _publishedSnapshotAccessor.PublishedSnapshot.Content.GetById(item.Udi);
                    }
                    else
                    {
                        currentCulture = _publishedSnapshotAccessor.PublishedSnapshot.Content.GetById(item.Id)?.GetCultureFromDomains();
                        umbracoContent = _publishedSnapshotAccessor.PublishedSnapshot.Content.GetById(item.Id);
                    }

                    if (umbracoContent != null)
                    {
                        // set item type
                        item.ItemType = UmbNavItemType.Content;

                        // skip item if umbracoNaviHide enabled
                        if (_removeNaviHideItems && !umbracoContent.IsVisible())
                        {
                            continue;
                        }

                        if (_removeNoopener)
                        {
                            item.Noopener = null;
                        }

                        if (_removeNoreferrer)
                        {
                            item.Noreferrer = null;
                        }

                        //// set content to node
                        //item.Content = umbracoContent;

                        // set title to node name if no override is set
                        if (string.IsNullOrWhiteSpace(item.Title))
                        {
                            item.Title = umbracoContent.Name(currentCulture);
                        }

                        //if (!string.IsNullOrEmpty(item.Anchor))
                        //{
                        //    item.Url = umbracoContent.Url(currentCulture) + $"{item.Anchor}";
                        //}
                        //else
                        //{
                        //    item.Url = umbracoContent.Url(currentCulture);
                        //}
                        // set url to most recent from published cache
                    }
                }

                // process child items
                if (item.Children.Any())
                {
                    var childLevel = item.Level + 1;

                    BuildMenu(item.Children, childLevel);
                }
            }

            // remove unpublished content items
            items = items.Where(x => x.ItemType == UmbNavItemType.Link);

            return items;
        }
    }
}