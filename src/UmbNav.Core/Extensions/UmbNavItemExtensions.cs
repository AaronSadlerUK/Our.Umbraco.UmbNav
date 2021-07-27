using System;
using System.Collections.Generic;
using UmbNav.Core.Enums;
using UmbNav.Core.Models;
#if NETCOREAPP
using Umbraco.Cms.Core.Models.PublishedContent;
using HtmlHelper = Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelper;
using TagBuilder = Microsoft.AspNetCore.Mvc.Rendering.TagBuilder;
using Microsoft.AspNetCore.Html;
#else
using System.Web;
using Umbraco.Core.Models.PublishedContent;
using System.Web.Mvc;
#endif

namespace UmbNav.Core.Extensions
{
    public static class UmbNavItemExtensions
    {
#if NETCOREAPP
        [Obsolete("I see your using Umbraco V9, Why not use the TagHelper <umbnavitem> instead.")]
        public static IHtmlContent GetLinkHtml(this UmbNavItem item, string cssClass = null, string id = null, string culture = null, UrlMode mode = UrlMode.Default, string labelTagName = "span", object htmlAttributes = null)
#else
        public static HtmlString GetLinkHtml(this UmbNavItem item, string cssClass = null, string id = null, string culture = null, UrlMode mode = UrlMode.Default, string labelTagName = "span", object htmlAttributes = null)
#endif
        {
            var htmlAttributesConverted = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            var tagBuilder = item.ItemType == UmbNavItemType.Label
                ? new TagBuilder(labelTagName)
                : new TagBuilder("a");

#if NETCOREAPP
            tagBuilder.InnerHtml.Append(item.Title);
#else
            tagBuilder.InnerHtml = item.Title;
#endif

            if (!string.IsNullOrEmpty(cssClass))
            {
                tagBuilder.AddCssClass(cssClass);
            }
           
            if (!string.IsNullOrEmpty(item.CustomClasses))
            {
                tagBuilder.AddCssClass(item.CustomClasses);
            }

            if (!string.IsNullOrEmpty(id))
            {
                tagBuilder.Attributes.Add("id", id);
            }

            tagBuilder.MergeAttributes(htmlAttributesConverted);

            if (item.ItemType == UmbNavItemType.Label)
            {
#if NETCOREAPP
                return tagBuilder;
#else
            return MvcHtmlString.Create(tagBuilder.ToString());
#endif
            }

            tagBuilder.Attributes.Add("href", item.Url(culture, mode));


            if (!string.IsNullOrEmpty(item.Target))
            {
                tagBuilder.Attributes.Add("target", item.Target);
            }

            if (!string.IsNullOrEmpty(item.Noopener) || !string.IsNullOrEmpty(item.Noreferrer))
            {
                var rel = new List<string>();

                if (!string.IsNullOrEmpty(item.Noopener))
                {
                    rel.Add(item.Noopener);
                }

                if (!string.IsNullOrEmpty(item.Noreferrer))
                {
                    rel.Add(item.Noreferrer);
                }

                if (htmlAttributesConverted.ContainsKey("rel"))
                {
                    var originalRelValue = htmlAttributesConverted["rel"] as string;
                    htmlAttributesConverted["rel"] = string.Format("{0} {1}", originalRelValue, string.Join(" ", rel));
                }
                else
                {
                    htmlAttributesConverted.Add("rel", string.Join(" ", rel));
                }
            }

#if NETCOREAPP
            return tagBuilder;
#else
            return MvcHtmlString.Create(tagBuilder.ToString());
#endif
        }

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