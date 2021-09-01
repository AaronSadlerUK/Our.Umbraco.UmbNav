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
            _umbracoContextAccessor = umbracoContextAccessor;
            _publishedSnapshotAccessor = publishedSnapshotAccessor;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<UmbNavItem> BuildMenu(IEnumerable<UmbNavItem> items, int level = 0, bool removeNaviHideItems = false,
            bool removeNoopener = false, bool removeNoreferrer = false, bool removeIncludeChildNodes = false)
        {
            var umbNavItems = items.ToList();
            var removeItems = new List<UmbNavItem>();
            try
            {
                var isLoggedIn = _httpContextAccessor.HttpContext.User != null && _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;

                foreach (var item in umbNavItems)
                {
                    if (item.HideLoggedIn && isLoggedIn || item.HideLoggedOut && !isLoggedIn)
                    {
                        continue;
                    }

                    if (item.MenuItemType is "nolink")
                    {
                        item.ItemType = UmbNavItemType.Label;
                        continue;
                    }

                    var currentPublishedContentKey = new Guid();
#if NETCOREAPP
                    if (_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext))
                    {
                        var currentPublishedContent = umbracoContext.PublishedRequest.PublishedContent;
                        currentPublishedContentKey = currentPublishedContent.Key;
                        if (currentPublishedContent.Key == item.Key)
                        {
                            item.IsActive = true;
                        }
                    }
#else
                    var currentPublishedContent = _umbracoContextAccessor.UmbracoContext.PublishedRequest.PublishedContent;
                    currentPublishedContentKey = currentPublishedContent.Key;
                    if (currentPublishedContent.Key == item.Key)
                    {
                        item.IsActive = true;
                    }
#endif

                    if (item.Udi != null || item.Key != Guid.Empty || item.Id > 0)
                    {
                        IPublishedContent umbracoContent = null;
                        string currentCulture = null;
#if NETCOREAPP
                        if (_publishedSnapshotAccessor.TryGetPublishedSnapshot(out var publishedSnapshot))
                        {
                            if (item.Udi != null)
                            {
                                currentCulture = publishedSnapshot.Content.GetById(item.Udi)?.GetCultureFromDomains();
                                umbracoContent = publishedSnapshot.Content.GetById(item.Udi);
                            }
                            else if (item.Key != Guid.Empty)
                            {
                                currentCulture = publishedSnapshot.Content.GetById(item.Key)?.GetCultureFromDomains();
                                umbracoContent = publishedSnapshot.Content.GetById(item.Key);
                            }
                            else
                            {
                                currentCulture = publishedSnapshot.Content.GetById(item.Id)?.GetCultureFromDomains();
                                umbracoContent = publishedSnapshot.Content.GetById(item.Id);
                            }
                        }
#else
                        if (item.Udi != null)
                        {
                            currentCulture = _publishedSnapshotAccessor.PublishedSnapshot.Content.GetById(item.Udi)?.GetCultureFromDomains();
                            umbracoContent = _publishedSnapshotAccessor.PublishedSnapshot.Content.GetById(item.Udi);
                        }
                        else if (item.Key != Guid.Empty)
                        {
                            currentCulture = _publishedSnapshotAccessor.PublishedSnapshot.Content.GetById(item.Key)?.GetCultureFromDomains();
                            umbracoContent = _publishedSnapshotAccessor.PublishedSnapshot.Content.GetById(item.Key);
                        }
                        else
                        {
                            currentCulture = _publishedSnapshotAccessor.PublishedSnapshot.Content.GetById(item.Id)?.GetCultureFromDomains();
                            umbracoContent = _publishedSnapshotAccessor.PublishedSnapshot.Content.GetById(item.Id);
                        }
#endif

                        if (umbracoContent != null)
                        {
                            item.ItemType = UmbNavItemType.Content;
                            item.Content = umbracoContent;

                            if (removeNaviHideItems && !umbracoContent.IsVisible() || removeNaviHideItems && umbracoContent.HasProperty("umbracoNavihide") && umbracoContent.Value<bool>("umbracoNavihide"))
                            {
                                removeItems.Add(item);
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
                                    children.AddRange(umbracoContent.Children.Where(x => x.IsVisible() ||  x.HasProperty("umbracoNavihide") && x.Value<bool>("umbracoNavihide")).Select(child => new UmbNavItem
                                    {
                                        Title = child.Name,
                                        Id = child.Id,
                                        Key = child.Key,
                                        Udi = new GuidUdi("document", child.Key),
                                        ItemType = UmbNavItemType.Content,
                                        Level = level + 1,
                                        Url = child.Url(currentCulture),
                                        IsActive = child.Key == currentPublishedContentKey
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
                                        IsActive = child.Key == currentPublishedContentKey
                                    }));
                                }

                                item.Children = children;
                            }
                        }
                    }

                    if (item.ImageArray != null && item.ImageArray.Any())
                    {
                        item.Image = GetImageUrl(item);
                    }

                    var childItems = item.Children.ToList();
                    if (childItems != null && childItems.Any())
                    {
                        var children = BuildMenu(childItems, level + 1, removeNaviHideItems);
                        if (!children.Equals(childItems))
                        {
                            item.Children = children;
                        }
                    }

                    item.Level = level;
                }
                //items = items.Where(x => x.ItemType == UmbNavItemType.Link);
                foreach (var removeItem in removeItems)
                {
                    umbNavItems.Remove(removeItem);
                }
                return umbNavItems;
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
#if NETCOREAPP
            if (_publishedSnapshotAccessor.TryGetPublishedSnapshot(out var publishedSnapshot))
            {
                if (UdiParser.TryParse(image.Udi, out var imageUdi))
                {
                    return publishedSnapshot.Media.GetById(imageUdi);
                }
                else if (item.Key != Guid.Empty)
                {
                    return publishedSnapshot.Media.GetById(item.Key);
                }
                else if (item.Id != default)
                {
                    return publishedSnapshot.Media.GetById(item.Id);
                }
            }
#else
            if (Udi.TryParse(image.Udi, out var imageUdi))
            {
                return _publishedSnapshotAccessor.PublishedSnapshot.Media.GetById(imageUdi);
            }
            else if (item.Key != Guid.Empty)
            {
                return _publishedSnapshotAccessor.PublishedSnapshot.Media.GetById(item.Key);
            }
            else if (item.Id != default)
            {
                return _publishedSnapshotAccessor.PublishedSnapshot.Media.GetById(item.Id);
            }
#endif
            return null;
        }
    }
}
