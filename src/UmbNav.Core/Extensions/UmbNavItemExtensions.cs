using System;
using System.Collections.Generic;
using UmbNav.Core.Enums;
using UmbNav.Core.Models;
using Umbraco.Extensions;
using Umbraco.Cms.Core.Models.PublishedContent;
using HtmlHelper = Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelper;
using TagBuilder = Microsoft.AspNetCore.Mvc.Rendering.TagBuilder;
using Microsoft.AspNetCore.Html;

namespace UmbNav.Core.Extensions
{
    public static class UmbNavItemExtensions
    {
        [Obsolete("I see your using Umbraco V9, Why not use the TagHelper <umbnavitem> instead.")]
        public static IHtmlContent GetLinkHtml(this UmbNavItem item, string cssClass = null, string id = null, string culture = null, UrlMode mode = UrlMode.Default, string labelTagName = "span", object htmlAttributes = null, string activeClass = null)
        {
            var htmlAttributesConverted = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            var tagBuilder = item.ItemType == UmbNavItemType.Label
                ? new TagBuilder(labelTagName)
                : new TagBuilder("a");

            tagBuilder.InnerHtml.Append(item.Title);

            if (!string.IsNullOrEmpty(cssClass))
            {
                tagBuilder.AddCssClass(cssClass);
            }

            if (!string.IsNullOrEmpty(item.CustomClasses))
            {
                tagBuilder.AddCssClass(item.CustomClasses);
            }

            if (!string.IsNullOrEmpty(activeClass) && item.IsActive)
            {
                tagBuilder.AddCssClass(activeClass);
            }

            if (!string.IsNullOrEmpty(id))
            {
                tagBuilder.Attributes.Add("id", id);
            }

            tagBuilder.MergeAttributes(htmlAttributesConverted);

            if (item.ItemType == UmbNavItemType.Label)
            {
                return tagBuilder;
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

            return tagBuilder;
        }

        [Obsolete("I see your using Umbraco V9, Why not use the TagHelper <umbnavitem> instead.")]
        public static IHtmlContent GetItemHtml(this UmbNavItem item, string cssClass = null, string id = null, string culture = null, UrlMode mode = UrlMode.Default, string labelTagName = "span", object htmlAttributes = null, string activeClass = null)
        {
            return GetLinkHtml(item, cssClass, id, culture, mode, labelTagName, htmlAttributes, activeClass);
        }

        public static string Url(this UmbNavItem item, string culture = null, UrlMode mode = UrlMode.Default)
        {
            if (item.Content != null)
            {
                switch (item.Content.ContentType.ItemType)
                {
                    case PublishedItemType.Content:

                        string url;
                        if (!string.IsNullOrEmpty(item.Anchor))
                        {
                            url = item.Content.Url(culture, mode) + item.Anchor;
                        }
                        else
                        {
                            url = item.Content.Url(culture, mode);
                        }

                        return url;

                    case PublishedItemType.Media:
                        return item.Content.Url(culture, mode);

                    default:
                        throw new NotSupportedException();
                }
            }

            return item.Url;
        }

        public static bool IsActive(this UmbNavItem item, IPublishedContent currentPage, bool checkAncestors = false)
        {
            if (item.Key == currentPage.Key)
            {
                return true;
            }

            if (checkAncestors)
            {
                if (item.Content.IsAncestorOrSelf(currentPage) && item.Content != currentPage.Root())
                {
                    return true;
                }
            }

            return false;
        }
    }
}