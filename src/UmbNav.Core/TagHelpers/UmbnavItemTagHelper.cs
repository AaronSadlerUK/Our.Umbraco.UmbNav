#if NETCOREAPP
using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using UmbNav.Core.Enums;
using UmbNav.Core.Extensions;
using UmbNav.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace UmbNav.Core.TagHelpers
{
    public class UmbnavitemTagHelper : TagHelper
    {
        public UmbNavItem MenuItem { get; set; }
        public UrlMode Mode { get; set; }
        public string Culture { get; set; }
        public string LabelTagName { get; set; } = "span";
        public string ActiveClass { get; set; }
        private bool IsLabel { get { return MenuItem.ItemType == UmbNavItemType.Label; } }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = IsLabel ? LabelTagName : "a";
            output.Content.SetContent(MenuItem.Title);

            if (!IsLabel)
            {
                output.Attributes.SetAttribute("href", MenuItem.Url(Culture, Mode));
            }
            
            if (!string.IsNullOrEmpty(MenuItem.CustomClasses))
            {
                output.AddClass(MenuItem.CustomClasses, HtmlEncoder.Default);
            }

            if (!string.IsNullOrEmpty(ActiveClass) && MenuItem.IsActive)
            {
                output.AddClass(ActiveClass, HtmlEncoder.Default);
            }

            if (!string.IsNullOrEmpty(MenuItem.Target) && !IsLabel)
            {
                output.Attributes.SetAttribute("target", MenuItem.Target);
            }

            if ((!string.IsNullOrEmpty(MenuItem.Noopener) || !string.IsNullOrEmpty(MenuItem.Noreferrer)) && !IsLabel)
            {
                var rel = new List<string>();

                if (!string.IsNullOrEmpty(MenuItem.Noopener))
                {
                    rel.Add(MenuItem.Noopener);
                }

                if (!string.IsNullOrEmpty(MenuItem.Noreferrer))
                {
                    rel.Add(MenuItem.Noreferrer);
                }

                if (output.Attributes["rel"] != null)
                {
                    var originalRelValue = output.Attributes["rel"].Value;
                    output.Attributes.SetAttribute("rel", string.Format("{0} {1}", originalRelValue, string.Join(" ", rel)));
                }
                else
                {
                    output.Attributes.SetAttribute("rel", string.Join(" ", rel));
                }
            }

        }
    }
}
#endif
