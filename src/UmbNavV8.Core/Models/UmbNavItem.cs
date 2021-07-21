using System.Collections.Generic;
using Newtonsoft.Json;
using UmbNavV8.Core.Enums;
using Umbraco.Core;

namespace UmbNavV8.Core.Models
{
    public class UmbNavItem
    {
        public int Id { get; set; }

        public GuidUdi Udi { get; set; }

        public string Title { get; set; }

        public string Target { get; set; }
        public string Noopener { get; set; }
        public string Noreferrer { get; set; }

        public string Anchor { get; set; }

        public IEnumerable<UmbNavItem> Children { get; set; }

        [JsonIgnore]
        public UmbNavItemType ItemType { get; set; }

        [JsonIgnore]
        public int Level { get; set; }

        public string Culture { get; set; }

        internal bool Collapsed { get; set; }

        internal bool HideLoggedIn { get; set; }
        
        internal bool HideLoggedOut { get; set; }
    }
}