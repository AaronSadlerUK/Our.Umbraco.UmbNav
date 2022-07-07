using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Deploy;
using Umbraco.Cms.Core.Models;
using Umbraco.Extensions;

namespace UmbNav.Core.ValueConnectors
{
    /// <summary>
    /// Represents a value connector for Umbraco UmbNav property editor
    /// </summary>
    /// <seealso cref="IValueConnector" />
    public class UmbNavValueConnector : IValueConnector
    {
        public string ToArtifact(object value, IPropertyType propertyType, ICollection<ArtifactDependency> dependencies)
        {
            if (AppSettingsManager.GetDisableUmbracoCloudSync())
                return null;

            var svalue = value as string;
            if (string.IsNullOrWhiteSpace(svalue) || !svalue.DetectIsJson())
            {
                return svalue;
            }

            var rootLinks = ParseLinks(JArray.Parse(svalue), dependencies);

            return rootLinks.ToString(Formatting.None);
        }

        public object FromArtifact(string value, IPropertyType propertyType, object currentValue)
        {
            if (AppSettingsManager.GetDisableUmbracoCloudSync())
                return null;

            return value;
        }

        public IEnumerable<string> PropertyEditorAliases => new[] { UmbNavConstants.PropertyEditorAlias };


        private static JArray ParseLinks(JArray links, ICollection<ArtifactDependency> dependencies)
        {
            foreach (var link in links)
            {
                if (!AppSettingsManager.GetDisableUmbracoCloudDependencySync())
                {
                    var validUdi = UdiParser.TryParse(link.Value<string>("udi"), out var guidUdi);
                    if (validUdi)
                    {
                        dependencies.Add(new ArtifactDependency(guidUdi, false, ArtifactDependencyMode.Exist));
                    }
                }

                var children = link.Value<JArray>("children");
                if (children != null)
                {
                    link["children"] = ParseLinks(children, dependencies);
                }
            }

            return links;
        }
    }
}