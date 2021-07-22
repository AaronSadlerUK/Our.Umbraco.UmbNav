using System.Collections.Generic;
using Newtonsoft.Json;
using UmbNavV8.Core.Enums;
using Umbraco.Core;

namespace UmbNavV8.Core.Models
{
    public class UmbNavItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("udi")]
        public GuidUdi Udi { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("target")]
        public string Target { get; set; }

        [JsonProperty("noopener")]
        public string Noopener { get; set; }

        [JsonProperty("noreferrer")]
        public string Noreferrer { get; set; }

        [JsonProperty("anchor")]
        public string Anchor { get; set; }

        [JsonProperty("children")]
        public IEnumerable<UmbNavItem> Children { get; set; }

        [JsonIgnore]
        public UmbNavItemType ItemType { get; set; }

        [JsonIgnore]
        public int Level { get; set; }

        [JsonProperty("culture")]
        public string Culture { get; set; }

        [JsonProperty("collapsed")]
        internal bool Collapsed { get; set; }

        [JsonProperty("hideLoggedIn")]
        internal bool HideLoggedIn { get; set; }

        [JsonProperty("hideLoggedOut")]
        internal bool HideLoggedOut { get; set; }

        [JsonProperty("url")]
        internal string Url { get; set; }

        [JsonProperty("includeChildNodes")]
        internal bool IncludeChildNodes { get; set; }
    }
}