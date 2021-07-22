using System;
using System.Collections.Generic;
using System.Linq;
using UmbNavV8.Core.Enums;
using UmbNavV8.Core.Interfaces;
using UmbNavV8.Core.Models;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Composing;
using Umbraco.Web.PublishedCache;

namespace UmbNavV8.Core.Services
{
    public class UmbNavMenuBuilderService : IUmbNavMenuBuilderService
    {
        private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;
        private readonly ILogger _logger;
        private int _level;

        public UmbNavMenuBuilderService(IPublishedSnapshotAccessor publishedSnapshotAccessor, ILogger logger)
        {
            _publishedSnapshotAccessor = publishedSnapshotAccessor;
            _logger = logger;
        }

        public IEnumerable<UmbNavItem> BuildMenu(IEnumerable<UmbNavItem> items, bool removeNaviHideItems = false, bool removeNoopener = false, bool removeNoreferrer = false)
        {
            try
            {
                var isLoggedIn = Current.UmbracoHelper.MemberIsLoggedOn();
                items = items.ToList();

                foreach (var item in items)
                {
                    if (item.HideLoggedIn && isLoggedIn || item.HideLoggedOut && !isLoggedIn)
                    {
                        continue;
                    }

                    item.Level = _level;

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
                            item.ItemType = UmbNavItemType.Content;

                            if (removeNaviHideItems && !umbracoContent.IsVisible())
                            {
                                continue;
                            }

                            if (removeNoopener)
                            {
                                item.Noopener = null;
                            }

                            if (removeNoreferrer)
                            {
                                item.Noreferrer = null;
                            }

                            if (string.IsNullOrWhiteSpace(item.Title))
                            {
                                item.Title = umbracoContent.Name(currentCulture);
                            }
                        }
                    }

                    if (item.Children.Any())
                    {
                        _level = item.Level + 1;

                        BuildMenu(item.Children);
                    }
                }

                items = items.Where(x => x.ItemType == UmbNavItemType.Link);

                return items;
            }
            catch (Exception ex)
            {
                _logger.Error(typeof(UmbNavMenuBuilderService), ex, "Failed to build UmbNav");
                return Enumerable.Empty<UmbNavItem>();
            }
        }
    }
}
