using Valve.Newtonsoft.Json;

namespace LoggerServer.Models
{
    public class UserResponseModel
    {
        [JsonProperty(PropertyName = "id")]
        public string UserId { get; set; }
    }
}
