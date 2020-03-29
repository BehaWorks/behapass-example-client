using Valve.Newtonsoft.Json;

namespace LoggerServer.Models
{
    public class UserMovementsResponseModel
    {
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "remaining")]
        public int Remaining { get; set; }
    }
}
