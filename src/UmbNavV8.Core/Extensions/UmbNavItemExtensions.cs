using System;
using UmbNavV8.Core.Models;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.Composing;

namespace UmbNavV8.Core.Extensions
{
    public static class UmbNavItemExtensions
    {
        public static string Url(this UmbNavItem item, string culture = null, UrlMode mode = UrlMode.Default)
        {
            while (true)
            {
                var umbracoContext = Current.UmbracoContext;

                if (umbracoContext == null) throw new InvalidOperationException("Cannot resolve a Url when Current.UmbracoContext is null.");
                if (umbracoContext.UrlProvider == null) throw new InvalidOperationException("Cannot resolve a Url when Current.UmbracoContext.UrlProvider is null.");


                if (item.Udi != null)
                {
                    var contentItem = GuidUdi.TryParse(item.Udi.ToString(), out var udi)
                        ? Current.UmbracoContext.Content.GetById(item.Udi)
                        : Current.UmbracoContext.Content.GetById(item.Id);

                    if (contentItem != null)
                    {
                        switch (contentItem.ContentType.ItemType)
                        {
                            case PublishedItemType.Content:

                                string url;
                                if (!string.IsNullOrEmpty(item.Anchor))
                                {
                                    url = umbracoContext.UrlProvider.GetUrl(contentItem, mode, culture) + item.Anchor;
                                }
                                else
                                {
                                    url = umbracoContext.UrlProvider.GetUrl(contentItem, mode, culture);
                                }

                                return url;

                            case PublishedItemType.Media:
                                return umbracoContext.UrlProvider.GetMediaUrl(contentItem, mode, culture);

                            default:
                                throw new NotSupportedException();
                        }
                    }
                }

                culture = null;
                mode = UrlMode.Default;
            }
        }
    }
}