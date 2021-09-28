using Newtonsoft.Json;

namespace UmbNav.Core.Models
{
    public class ImageItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("udi")]
        public string Udi { get; set; }
    }
}