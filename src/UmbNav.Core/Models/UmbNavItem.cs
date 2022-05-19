using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UmbNav.Core.Enums;
#if NETCOREAPP
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
#else
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
#endif

namespace UmbNav.Core.Models
{
    public class UmbNavItem
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        [Obsolete("This is here purely for legacy reasons, please use the Key or Udi property as this is unreliable")]
        public int Id { get; set; } = 0;

        [JsonProperty("udi")]
        public GuidUdi Udi { get; set; }

        [JsonProperty("key")]
        public Guid Key { get; set; }

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
        public IPublishedContent Content { get; set; }

        [JsonIgnore]
        public UmbNavItemType ItemType { get; set; }

        [JsonIgnore]
        public int Level { get; set; }

        [JsonProperty("culture")]
        public string Culture { get; set; }

        [JsonProperty("url")]
        internal string Url { get; set; }

        [JsonProperty("customClasses")]
        public string CustomClasses { get; set; }

        [JsonIgnore]
        public IPublishedContent Image { get; set; }

        [JsonIgnore]
        public bool IsActive { get; set; }

        [JsonProperty("displayAsLabel")]
        public bool DisplayAsLabel { get; set; }
    }
}
