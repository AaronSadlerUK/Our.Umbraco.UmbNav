using System;
using UmbNav.Core.Models;
#if NETCOREAPP
using Umbraco.Cms.Core.Models.PublishedContent;
#else
using Umbraco.Core.Models.PublishedContent;

#endif

namespace UmbNav.Core.Extensions
{
    public static class UmbNavItemExtensions
    {
        public static string Url(this UmbNavItem item, string culture = null, UrlMode mode = UrlMode.Default)
        {
            if (item.Udi != null)
            {
                var contentItem = item.PublishedContentItem;
                if (contentItem != null)
                {
                    switch (contentItem.ContentType.ItemType)
                    {
                        case PublishedItemType.Content:

                            string url;
                            if (!string.IsNullOrEmpty(item.Anchor))
                            {
                                url = item.Url(culture, mode) + item.Anchor;
                            }
                            else
                            {
                                url = item.Url(culture, mode);
                            }

                            return url;

                        case PublishedItemType.Media:
                            return item.Url(culture, mode);

                        default:
                            throw new NotSupportedException();
                    }
                }
            }

            return item.Url;
        }
    }
}