using Umbraco.Core.PropertyEditors;

namespace UmbNavV8.Core.PropertyEditors
{
    public class UmbNavV8Configuration
    {
        [ConfigurationField("maxDepth", "Max Depth", "number", Description = "The maximum number of levels in the navigation")]
        public int MaxDepth { get; set; }

        [ConfigurationField("removeNaviHideItems", "Remove NaviHide Items", "boolean", Description = "Remove items where umbracoNaviHide is true")]
        public bool RemoveNaviHideItems { get; set; }

        [ConfigurationField("expandOnHover", "Allow Expand On Hover", "boolean", Description = "Expand the tree item on hover")]
        public bool ExpandOnHover { get; set; }

        [ConfigurationField("expandOnHoverTimeout", "Expand On Hover", "number", Description = "The delay to wait before the tree item expands")]
        public int ExpandOnHoverTimeout { get; set; }

        [ConfigurationField("hideNoopener", "Hide noopener Toggle", "boolean", Description = "Hide the ability to toggle noopener")]
        public bool HideNoopener { get; set; }

        [ConfigurationField("hideNoreferrer", "Hide noreferrer Toggle", "boolean", Description = "Hide the ability to toggle noreferrer")]
        public bool HideNoreferrer { get; set; }

        [ConfigurationField("allowDisplay", "Allow Display", "boolean", Description = "Allow the ability to hide menu items based on member authentication status")]
        public bool AllowDisplay { get; set; }

        [ConfigurationField("hideLabel", "Hide Label", "boolean", Description = "Hide the property label and let the item list span the full width of the editor window.")]
        public bool HideLabel { get; set; }
    }
}
