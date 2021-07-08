using Umbraco.Core.PropertyEditors;

namespace UmbNavV8.Core.PropertyEditors
{
    public class UmbNavV8Configuration
    {
        [ConfigurationField("maxDepth", "Max Depth", "number", Description = "The maximum number of levels in the navigation")]
        public int MaxDepth { get; set; }

        [ConfigurationField("removeNaviHideItems", "Remove NaviHide Items", "boolean", Description = "Remove items where umbracoNaviHide is true")]
        public bool RemoveNaviHideItems { get; set; }
    }
}
