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

        public IEnumerable<UmbNavInternalItem> BuildMenu(IEnumerable<UmbNavInternalItem> items, int level = 0, bool removeNaviHideItems = false,
            bool removeNoopener = false, bool removeNoreferrer = false, bool removeIncludeChildNodes = false)
        {
            var umbNavItems = items.ToList();
            var removeItems = new List<UmbNavInternalItem>();
            try
            {
                var isLoggedIn = _httpContextAccessor.HttpContext.User != null && _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;

                foreach (var item in umbNavItems)
                {
                    var currentPublishedContentKey = Guid.Empty;
#if NETCOREAPP
                    if (_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext))
                    {
                        var currentPublishedContent = umbracoContext.PublishedRequest.PublishedContent;
                        currentPublishedContentKey = currentPublishedContent.Key;
                    }
#else
                    var currentPublishedContent = _umbracoContextAccessor.UmbracoContext.PublishedRequest.PublishedContent;
                    currentPublishedContentKey = currentPublishedContent.Key;
#endif
                    if (item.HideLoggedIn && isLoggedIn || item.HideLoggedOut && !isLoggedIn)
                    {
                        continue;
                    }

                    if (item.MenuItemType is "nolink" || item.DisplayAsLabel)
                    {
                        item.ItemType = UmbNavItemType.Label;
                        item.Anchor = null;
                        item.Url = null;
                        item.Target = null;
                        item.Noopener = null;
                        item.Noreferrer = null;
                        item.IncludeChildNodes = false;
                        item.Udi = null;

                        //if (!item.DisplayAsLabel)
                        //{ 
                        //    continue;
                        //}
                    }

                    var children = new List<UmbNavInternalItem>();
                    if (item.Children != null && item.Children.Any())
                    {
                        children = item.InternalChildren.ToList();
                    }

                    if (item.Udi != null || item.Key != Guid.Empty || item.Id > 0)
                    {
                        IPublishedContent umbracoContent = null;
                        string currentCulture = null;
                        if (item.MenuItemType != "nolink")
                        {
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
                        }

                        if (umbracoContent != null)
                        {
                            if (!item.DisplayAsLabel)
                            {
                                item.ItemType = UmbNavItemType.Content;
                            }
                            item.Content = umbracoContent;

                            if (umbracoContent.Key == currentPublishedContentKey)
                            {
                                item.IsActive = true;
                            }

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
                                if (removeNaviHideItems)
                                {
                                    children.AddRange(umbracoContent.Children.Where(x => x.IsVisible() || x.HasProperty("umbracoNavihide") && x.Value<bool>("umbracoNavihide")).Select(child => new UmbNavInternalItem
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
                                    children.AddRange(umbracoContent.Children.Select(child => new UmbNavInternalItem
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
                            }
                        }
                    }

                    if (item.ImageArray != null && item.ImageArray.Any())
                    {
                        item.Image = GetImageUrl(item);
                    }

                    if (children != null && children.Any())
                    {
                        var childItems = BuildMenu(children, level + 1, removeNaviHideItems).ToList();
                        if (!children.Equals(childItems))
                        {
                            children = childItems;
                        }

                        item.Children = children;
                    }

                    item.Level = level;

                    if (item.DisplayAsLabel && item.Content == null || item.MenuItemType is "link" && item.Content == null)
                    {
                        item.ItemType = UmbNavItemType.Link;
                    }
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
                return Enumerable.Empty<UmbNavInternalItem>();
            }
        }

        public IEnumerable<UmbNavItem> BuildRenderingMenu(IEnumerable<UmbNavInternalItem> internalItems)
        {
            var umbNav = new List<UmbNavItem>();
            foreach (var internalItem in internalItems)
            {
                var children = BuildRenderingMenu(internalItem.InternalChildren);
                umbNav.Add(new UmbNavItem
                {
                    Anchor = internalItem.Anchor,
                    Children = children,
                    Content = internalItem.Content,
                    Culture = internalItem.Culture,
                    CustomClasses = internalItem.CustomClasses,
                    DisplayAsLabel = internalItem.DisplayAsLabel,
                    Id = internalItem.Id,
                    Image = internalItem.Image,
                    IsActive = internalItem.IsActive,
                    ItemType = internalItem.ItemType,
                    Key = internalItem.Key,
                    Level = internalItem.Level,
                    Noopener = internalItem.Noopener,
                    Noreferrer = internalItem.Noreferrer,
                    Target = internalItem.Target,
                    Title = internalItem.Title,
                    Udi = internalItem.Udi,
                    Url = internalItem.Url
                });
            }

            return umbNav;
        }

        private IPublishedContent GetImageUrl(UmbNavInternalItem item)
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
