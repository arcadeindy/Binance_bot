using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace binance_bot
{
    public partial class Response
    {
        [JsonProperty("lastUpdateId")]
        public long LastUpdateId { get; set; }

        [JsonProperty("bids")]
        public List<List<Ask>> Bids { get; set; }

        [JsonProperty("asks")]
        public List<List<Ask>> Asks { get; set; }
    }

    public partial struct Ask
    {
        public List<object> AnythingArray;
        public string String;

        public static implicit operator Ask(List<object> AnythingArray) => new Ask { AnythingArray = AnythingArray };
        public static implicit operator Ask(string String) => new Ask { String = String };
    }

}
