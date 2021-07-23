using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#if NETCOREAPP
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Deploy;
using Umbraco.Cms.Core.Models;
using Umbraco.Extensions;

#else
using Umbraco.Core;
using Umbraco.Core.Deploy;
using Umbraco.Core.Models;
#endif

namespace UmbNavV8.Core.ValueConnectors
{
    /// <summary>
    /// Represents a value connector for Umbraco UmbNav property editor
    /// </summary>
    /// <seealso cref="IValueConnector" />
    public class UmbNavV8ValueConnector : IValueConnector
    {

#if NETCOREAPP
        public string ToArtifact(object value, IPropertyType propertyType, ICollection<ArtifactDependency> dependencies)
#else
        public string ToArtifact(object value, PropertyType propertyType, ICollection<ArtifactDependency> dependencies)
#endif
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

#if NETCOREAPP
        public object FromArtifact(string value, IPropertyType propertyType, object currentValue)
#else
        public object FromArtifact(string value, PropertyType propertyType, object currentValue)
#endif
        {
            if (AppSettingsManager.GetDisableUmbracoCloudSync())
                return null;

            return value;
        }

        public IEnumerable<string> PropertyEditorAliases => new[] { Constants.PropertyEditorAlias };


        private static JArray ParseLinks(JArray links, ICollection<ArtifactDependency> dependencies)
        {
            foreach (var link in links)
            {
                if (!AppSettingsManager.GetDisableUmbracoCloudDependencySync())
                {
#if NETCOREAPP
                    var validUdi = UdiParser.TryParse(link.Value<string>("udi"), out var guidUdi);
#else
                    var validUdi = GuidUdi.TryParse(link.Value<string>("udi"), out var guidUdi);
#endif
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