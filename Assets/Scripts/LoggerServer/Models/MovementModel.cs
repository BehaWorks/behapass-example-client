using Valve.Newtonsoft.Json;

namespace LoggerServer.Models
{
    public class MovementModel
    {
        [JsonProperty(PropertyName = "session_id")]
        public string SessionId { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public double Timestamp { get; set; }

        [JsonProperty(PropertyName = "controller_id")]
        public string DeviceId { get; set; }

        [JsonProperty(PropertyName = "x")]
        public double X { get; set; }

        [JsonProperty(PropertyName = "y")]
        public double Y { get; set; }

        [JsonProperty(PropertyName = "z")]
        public double Z { get; set; }

        [JsonProperty(PropertyName = "yaw")]
        public double? Yaw { get; set; }

        [JsonProperty(PropertyName = "pitch")]
        public double? Pitch { get; set; }

        [JsonProperty(PropertyName = "roll")]
        public double? Roll { get; set; }

        [JsonProperty(PropertyName = "r_x")]
        public double RotationX { get; set; }

        [JsonProperty(PropertyName = "r_y")]
        public double RotationY { get; set; }

        [JsonProperty(PropertyName = "r_z")]
        public double RotationZ { get; set; }
    }
}
