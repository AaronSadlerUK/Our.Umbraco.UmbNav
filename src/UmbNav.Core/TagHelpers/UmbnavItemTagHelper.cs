#if NETCOREAPP
using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.TagHelpers;
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

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";
            output.Attributes.SetAttribute("href", MenuItem.Url(Culture, Mode));
            output.Content.SetContent(MenuItem.Title);

            if (!string.IsNullOrEmpty(MenuItem.Target))
            {
                output.Attributes.SetAttribute("target", MenuItem.Target);
            }

            if (!string.IsNullOrEmpty(MenuItem.Noopener) || !string.IsNullOrEmpty(MenuItem.Noreferrer))
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

            if (!string.IsNullOrEmpty(MenuItem.CustomClasses))
            {
                if (output.Attributes["class"] != null)
                {
                    var originalRelValue = output.Attributes["class"].Value;
                    output.Attributes.SetAttribute("class", string.Format("{0} {1}", originalRelValue, string.Join(" ", MenuItem.CustomClasses)));
                }
                else
                {
                    output.Attributes.SetAttribute("rel", string.Join(" ", MenuItem.CustomClasses));
                }
            }

        }
    }
}
#endif