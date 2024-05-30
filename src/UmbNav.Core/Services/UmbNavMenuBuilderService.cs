using System;
using System.Collections.Generic;
using System.Linq;
using UmbNav.Core.Enums;
using UmbNav.Core.Interfaces;
using UmbNav.Core.Models;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Extensions;
using Serilog;
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
                var currentPublishedContentKey = Guid.Empty;
                if (_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext))
                {
                    var currentPublishedContent = umbracoContext.PublishedRequest?.PublishedContent;
                    if (currentPublishedContent != null)
                    {
                        currentPublishedContentKey = currentPublishedContent.Key;
                    }
                }

                foreach (var item in umbNavItems)
                {
                    if (item.HideLoggedIn && isLoggedIn || item.HideLoggedOut && !isLoggedIn)
                    {
                        removeItems.Add(item);
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
                        item.Key = Guid.Empty;

                        if (string.IsNullOrEmpty(item.Title) && !string.IsNullOrEmpty(item.Name))
                        {
                            item.Title = item.Name;
                        }

                        //if (!item.DisplayAsLabel)
                        //{ 
                        //    continue;
                        //}
                    }

                    var children = new List<UmbNavItem>();
                    if (item.Children != null && item.Children.Any())
                    {
                        foreach (var child in item.Children)
                        {
                            child.Parent = item;
                        }

                        children = item.Children.ToList();
                    }

                    if (item.Udi != null || item.Key != Guid.Empty)
                    {
                        IPublishedContent umbracoContent = null;
                        string currentCulture = null;
                        if (item.MenuItemType != "nolink")
                        {
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
                            }
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

                            if (!umbracoContent.IsPublished() || removeNaviHideItems && !umbracoContent.IsVisible() || removeNaviHideItems && umbracoContent.HasProperty("umbracoNavihide") && umbracoContent.Value<bool>("umbracoNavihide"))
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
                                    children.AddRange(umbracoContent.Children.Where(x => x.IsVisible() || x.HasProperty("umbracoNavihide") && x.Value<bool>("umbracoNavihide")).Select(child => new UmbNavItem
                                    {
                                        Title = child.Name,
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
                        else
                        {
                            removeItems.Add(item);
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
                _logger.Error(ex, "Failed to build UmbNav");
                return Enumerable.Empty<UmbNavItem>();
            }
        }

        private IPublishedContent GetImageUrl(UmbNavItem item)
        {
            var image = item.ImageArray[0];
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
            }
            return null;
        }
    }
}
