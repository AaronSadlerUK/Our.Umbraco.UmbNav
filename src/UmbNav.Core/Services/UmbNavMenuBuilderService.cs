using System;
using System.Collections.Generic;
using System.Linq;
using UmbNav.Core.Enums;
using UmbNav.Core.Interfaces;
using UmbNav.Core.Models;
#if NETCOREAPP
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Extensions;
using Serilog;
#else
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;
#endif
namespace UmbNav.Core.Services
{
    public class UmbNavMenuBuilderService : IUmbNavMenuBuilderService
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public UmbNavMenuBuilderService(IPublishedSnapshotAccessor publishedSnapshotAccessor, ILogger logger, IHttpContextAccessor httpContextAccessor, IUmbracoContextAccessor umbracoContextAccessor)
        {
            _publishedSnapshotAccessor = publishedSnapshotAccessor;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        public IEnumerable<UmbNavItem> BuildMenu(IEnumerable<UmbNavItem> items, int level = 0, bool removeNaviHideItems = false,
            bool removeNoopener = false, bool removeNoreferrer = false, bool removeIncludeChildNodes = false)
        {
            try
            {
                var isLoggedIn = _httpContextAccessor.HttpContext.User != null && _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
                items = items.ToList();

                foreach (var item in items)
                {
                    if (item.HideLoggedIn && isLoggedIn || item.HideLoggedOut && !isLoggedIn)
                    {
                        continue;
                    }

                    if (item.ImageArray != null && item.ImageArray.Any())
                    {
                        item.Image = GetImageUrl(item);
                    }

                    if (item.MenuItemType is "nolink")
                    {
                        item.ItemType = UmbNavItemType.Label;
                        continue;
                    }


                    var currentPublishedContent = _umbracoContextAccessor.UmbracoContext.PublishedRequest.PublishedContent;
                    if (currentPublishedContent.Key == item.Key)
                    {
                        item.IsActive = true;
                    }

                    if (item.Key != Guid.Empty)
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
                            item.Content = umbracoContent;

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


                            if (!removeIncludeChildNodes && item.IncludeChildNodes && umbracoContent.Children != null && umbracoContent.Children.Any())
                            {
                                var children = item.Children.ToList();
                                if (removeNaviHideItems)
                                {
                                    children.AddRange(umbracoContent.Children.Where(x => x.IsVisible()).Select(child => new UmbNavItem
                                    {
                                        Title = child.Name,
                                        Id = child.Id,
                                        Key = child.Key,
                                        Udi = new GuidUdi("document", child.Key),
                                        ItemType = UmbNavItemType.Content,
                                        Level = level + 1,
                                        Url = child.Url(currentCulture),
                                        IsActive = child.Key == currentPublishedContent.Key
                                    }));
                                }
                                else
                                {
                                    children.AddRange(umbracoContent.Children.Select(child => new UmbNavItem
                                    {
                                        Title = child.Name,
                                        Id = child.Id,
                                        Key = child.Key,
                                        Udi = new GuidUdi("document", child.Key),
                                        ItemType = UmbNavItemType.Content,
                                        Level = level + 1,
                                        Url = child.Url(currentCulture),
                                        IsActive = child.Key == currentPublishedContent.Key
                                    }));
                                }

                                item.Children = children;
                            }
                        }
                    }

                    if (item.Children.Any())
                    {
                        BuildMenu(item.Children, level + 1, true);
                    }

                    item.Level = level;
                }

                //items = items.Where(x => x.ItemType == UmbNavItemType.Link);

                return items;
            }
            catch (Exception ex)
            {
#if NETCOREAPP
                _logger.Error(ex, "Failed to build UmbNav");
#else
                _logger.Error(typeof(UmbNavMenuBuilderService), ex, "Failed to build UmbNav");
#endif
                return Enumerable.Empty<UmbNavItem>();
            }
        }

        private IPublishedContent GetImageUrl(UmbNavItem item)
        {
            var image = item.ImageArray[0];
            IPublishedContent publishedImage = null;
#if NETCOREAPP
            if (UdiParser.TryParse(image.Udi, out var imageUdi))
#else
            if (Udi.TryParse(image.Udi, out var imageUdi))
#endif
            {
                var mediaItem = _publishedSnapshotAccessor.PublishedSnapshot.Media.GetById(imageUdi);

                publishedImage = mediaItem;
            }
            else if (item.Key != Guid.Empty)
            {
                var mediaItem = _publishedSnapshotAccessor.PublishedSnapshot.Media.GetById(item.Key);

                publishedImage = mediaItem;
            }
            else if (item.Id != default)
            {
                var mediaItem = _publishedSnapshotAccessor.PublishedSnapshot.Media.GetById(item.Id);

                publishedImage = mediaItem;
            }

            return publishedImage;
        }
    }
}
