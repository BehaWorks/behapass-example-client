using Valve.Newtonsoft.Json;

namespace LoggerServer.Models
{
    public class LookupModel
    {
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "distance")]
        public double Distance { get; set; }
    }
}
