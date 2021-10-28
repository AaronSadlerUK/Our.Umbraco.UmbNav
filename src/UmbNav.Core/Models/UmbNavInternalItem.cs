using System.Collections.Generic;
using Newtonsoft.Json;

namespace UmbNav.Core.Models
{
    public class UmbNavInternalItem : UmbNavItem
    {
        [JsonProperty("collapsed")]
        internal bool Collapsed { get; set; }

        [JsonProperty("hideLoggedIn")]
        internal bool HideLoggedIn { get; set; }

        [JsonProperty("hideLoggedOut")]
        internal bool HideLoggedOut { get; set; }

        [JsonProperty("url")]
        public new string Url { internal get; set; }

        [JsonProperty("image")]
        internal ImageItem[] ImageArray { get; set; }

        [JsonProperty("includeChildNodes")]
        internal bool IncludeChildNodes { get; set; }

        [JsonProperty("children")]
        internal IEnumerable<UmbNavInternalItem> InternalChildren { get; set; }

        [JsonProperty("itemType")] 
        internal string MenuItemType { get; set; } = "link";
    }
}
