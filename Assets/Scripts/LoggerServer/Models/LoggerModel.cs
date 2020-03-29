using Valve.Newtonsoft.Json;

namespace LoggerServer.Models
{
    using System.Collections.Generic;

    public class LoggerModel
    {
        [JsonProperty(PropertyName = "movements")]
        public IList<MovementModel> Movements { get; set; }

        [JsonProperty(PropertyName = "buttons")]
        public IList<ButtonModel> Buttons { get; set; }
    }
}
