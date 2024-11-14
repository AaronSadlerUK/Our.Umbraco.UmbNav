using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        public IEnumerable<string> PropertyEditorAliases => new[] { "UmbNav" };

#if NET8_0_OR_GREATER
        public async Task<string?> ToArtifactAsync(object? value, IPropertyType propertyType, ICollection<ArtifactDependency> dependencies, IContextCache contextCache)
        {
            if (AppSettingsManager.GetDisableUmbracoCloudSync())
                return null;

            var svalue = value as string;
            if (string.IsNullOrWhiteSpace(svalue) || !svalue.DetectIsJson())
            {
                return svalue;
            }

            var rootLinks = await Task.Run(() => ParseLinks(JArray.Parse(svalue)));
            return JsonConvert.SerializeObject(rootLinks);
        }

        public async Task<object?> FromArtifactAsync(string? value, IPropertyType propertyType, object? currentValue, IContextCache contextCache)
        {
            if (string.IsNullOrWhiteSpace(value) || !value.DetectIsJson())
            {
                return value;
            }

            var rootLinks = await Task.Run(() => ParseLinks(JArray.Parse(value)));
            return JsonConvert.SerializeObject(rootLinks);
        }

        public string? ToArtifact(object? value, IPropertyType propertyType, ICollection<ArtifactDependency> dependencies, IContextCache contextCache)
        {
            var svalue = value as string;
            if (string.IsNullOrWhiteSpace(svalue) || !svalue.DetectIsJson())
            {
                return svalue;
            }

            var rootLinks = ParseLinks(JArray.Parse(svalue));
            return JsonConvert.SerializeObject(rootLinks);
        }

        public object? FromArtifact(string? value, IPropertyType propertyType, object? currentValue, IContextCache contextCache)
        {
            if (string.IsNullOrWhiteSpace(value) || !value.DetectIsJson())
            {
                return value;
            }

            var rootLinks = ParseLinks(JArray.Parse(value));
            return JsonConvert.SerializeObject(rootLinks);
        }
#else
        public string? ToArtifact(object? value, IPropertyType propertyType, ICollection<ArtifactDependency> dependencies)
        {
            var svalue = value as string;
            if (string.IsNullOrWhiteSpace(svalue) || !svalue.DetectIsJson())
            {
                return svalue;
            }

            var rootLinks = ParseLinks(JArray.Parse(svalue));
            return JsonConvert.SerializeObject(rootLinks);
        }

        public object? FromArtifact(string? value, IPropertyType propertyType, object? currentValue)
        {
            if (string.IsNullOrWhiteSpace(value) || !value.DetectIsJson())
            {
                return value;
            }

            var rootLinks = ParseLinks(JArray.Parse(value));
            return JsonConvert.SerializeObject(rootLinks);
        }
#endif

        private JArray ParseLinks(JArray jsonArray)
        {
            // Placeholder ParseLinks method implementation
            return jsonArray;
        }
    }
}
