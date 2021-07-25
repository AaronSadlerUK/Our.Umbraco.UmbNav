using System;
using System.Collections.Generic;
using System.Linq;
using UmbNav.Core.Enums;
using UmbNav.Core.Interfaces;
using UmbNav.Core.Models;
#if NETCOREAPP
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core;
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
        private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public UmbNavMenuBuilderService(IPublishedSnapshotAccessor publishedSnapshotAccessor, ILogger logger, IHttpContextAccessor httpContextAccessor)
        {
            _publishedSnapshotAccessor = publishedSnapshotAccessor;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
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

                    if (!string.IsNullOrEmpty(item.ImageUrl))
                    {
                        item.ImageUrl = GetImageUrl(item);
                    }

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


                            if (!removeIncludeChildNodes && item.IncludeChildNodes && umbracoContent.Children != null && umbracoContent.Children.Any())
                            {
                                var children = item.Children.ToList();
                                if (removeNaviHideItems)
                                {
                                    children.AddRange(umbracoContent.Children.Where(x => x.IsVisible()).Select(child => new UmbNavItem
                                    {
                                        Title = child.Name,
                                        Id = child.Id,
                                        Udi = new GuidUdi("document", child.Key),
                                        ItemType = UmbNavItemType.Content,
                                        Level = level + 1,
                                        Url = child.Url(currentCulture)
                                    }));
                                }
                                else
                                {
                                    children.AddRange(umbracoContent.Children.Select(child => new UmbNavItem
                                    {
                                        Title = child.Name,
                                        Id = child.Id,
                                        Udi = new GuidUdi("document", child.Key),
                                        ItemType = UmbNavItemType.Content,
                                        Level = level + 1,
                                        Url = child.Url(currentCulture)
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

        private string GetImageUrl(UmbNavItem item)
        {
            if (int.TryParse(item.ImageUrl, out var imageId))
            {
                var mediaItem = _publishedSnapshotAccessor.PublishedSnapshot.Media.GetById(imageId);

                return mediaItem != null ? mediaItem.Url() : string.Empty;
            }

#if NETCOREAPP
            if (UdiParser.TryParse(item.ImageUrl, out var imageUdi))
#else
            if (Udi.TryParse(item.ImageUrl, out var imageUdi))
#endif
            {
                var mediaItem = _publishedSnapshotAccessor.PublishedSnapshot.Media.GetById(imageUdi);

                return mediaItem != null ? mediaItem.Url() : string.Empty;
            }

            if (Guid.TryParse(item.ImageUrl, out var imageGuid))
            {
                var mediaItem = _publishedSnapshotAccessor.PublishedSnapshot.Media.GetById(imageGuid);

                return mediaItem != null ? mediaItem.Url() : string.Empty;
            }

            return item.ImageUrl;
        }
    }
}
