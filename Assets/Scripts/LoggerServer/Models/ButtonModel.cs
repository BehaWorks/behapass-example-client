using Valve.Newtonsoft.Json;

namespace LoggerServer.Models
{
    public class ButtonModel
    {
        [JsonProperty(PropertyName = "session_id")]
        public string SessionId { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public double Timestamp { get; set; }

        [JsonProperty(PropertyName = "controller_id")]
        public string ControllerId { get; set; }

        [JsonProperty(PropertyName = "trigger")]
        public double? Trigger { get; set; }

        [JsonProperty(PropertyName = "trackpad_x")]
        public double? TrackpadX { get; set; }

        [JsonProperty(PropertyName = "trackpad_y")]
        public double? TrackpadY { get; set; }

        [JsonProperty(PropertyName = "button_pressed")]
        public int? ButtonPressed { get; set; }

        [JsonProperty(PropertyName = "button_touched")]
        public int? ButtonTouched { get; set; }

        [JsonProperty(PropertyName = "menu_button")]
        public bool? MenuButton { get; set; }

        [JsonProperty(PropertyName = "trackpad_pressed")]
        public bool? TrackpadPressed { get; set; }

        [JsonProperty(PropertyName = "trackpad_touched")]
        public bool? TrackpadTouched { get; set; }

        [JsonProperty(PropertyName = "grip_button")]
        public bool? GripButton { get; set; }
    }
}
