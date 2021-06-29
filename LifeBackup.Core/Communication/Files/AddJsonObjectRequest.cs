using Newtonsoft.Json;
using System;

namespace LifeBackup.Core.Communication.Files
{
    public class AddJsonObjectRequest
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("timesent")]
        public DateTime TimeSent { get; set; }
    }
}
