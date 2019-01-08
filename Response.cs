using System.Collections.Generic;

namespace binance_bot
{
    class Response
    {
        public List<decimal[][]> Asks { get; set; }
        public List<decimal[][]> Bids { get; set; }

    }
}
