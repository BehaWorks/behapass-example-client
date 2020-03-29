using Valve.Newtonsoft.Json;

namespace LoggerServer.Models
{
    public class UserRequestModel
    {
        [JsonProperty(PropertyName = "data")]
        public string UserName { get; set; }
    }
}
