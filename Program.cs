using Binance.Net;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace binance_bot
{
    class Program
    {
        static void Main(string[] args)
        {
            TimeSpan interval = new TimeSpan(0, 0, 1);

            BinanceClient.SetDefaultOptions(new BinanceClientOptions()
            {
                ApiCredentials = new ApiCredentials("API_KEY", "SECRET_KEY"),
                LogVerbosity = LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { Console.Out }
            });

            BinanceSocketClient.SetDefaultOptions(new BinanceSocketClientOptions()
            {
                ApiCredentials = new ApiCredentials("API_KEY", "SECRET_KEY"),
                LogVerbosity = LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { Console.Out }
            });
/*
            using (var client = new BinanceClient())
            {
                while (true)
                {
                    Thread.Sleep(interval);
                    var orderBook = client.GetOrderBook("BNBBTC", 10);

                }
            }
*/          
            using (var client = new WebClient())
            {
                responseString = client.DownloadString("https://api.binance.com/api/v1/depth?symbol=ETHBTC&limit=10");
            }
            var response = JsonConvert.DeserializeObject<Response>(responseString);
            Console.WriteLine(response);
        }
    }
}
