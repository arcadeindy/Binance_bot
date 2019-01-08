using Binance.Net;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using System.Net;

namespace binance_bot
{
    class Program
    {
        static void Main(string[] args)
        {
            decimal[] price = new decimal[10];
            decimal[] sell = new decimal[10];
            int index = 0;
            decimal value = 0.000000000000001m;
            int element, inside;
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
            while (true)
            {
                using (var client = new BinanceClient())
                {
                    string Get(string uri)
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        using (Stream stream = response.GetResponseStream())
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                    for (inside = 1, element = 0; element < 10; element++)
                    {
                        string binance = Get("https://api.binance.com/api/v1/depth?symbol=BNBBTC&limit=10");
                        dynamic decoded = JsonConvert.DeserializeObject(binance);
                        var bid = decoded.bids[element][inside];
                        sell[element] = bid;
                        if (sell[element] >= value)
                        {
                            value = sell[element];
                            index++;
                        }
                        Console.WriteLine($"ордер на продажу: {sell[element]}");
                    }
                    Console.WriteLine(index);
                    for (inside = 0, element = 0; element < 10; element++)
                    {
                        string binance = Get("https://api.binance.com/api/v1/depth?symbol=BNBBTC&limit=10");
                        dynamic decoded = JsonConvert.DeserializeObject(binance);
                        var bid = decoded.bids[element][inside];
                        price[element] = bid;
                        Console.WriteLine($"цена продажи: {price[element]}");
                        if (element == 9)
                            Console.WriteLine($"цена нужного ордера: {price[index]}");
                    }
                    var orderSellt = client.PlaceOrder("BNBBTC", OrderSide.Sell, OrderType.Limit, 1, price: price[index], timeInForce: TimeInForce.GoodTillCancel);
                    Thread.Sleep(3000);
                    var cancelSell = client.CancelOrder("BNBBTC", orderSellt.Data.OrderId);
                    for (inside = 1, element = 0; element < 10; element++)
                    {
                        string binance = Get("https://api.binance.com/api/v1/depth?symbol=BNBBTC&limit=10");
                        dynamic decoded = JsonConvert.DeserializeObject(binance);
                        var ask = decoded.asks[element][inside];
                        sell[element] = ask;
                        if (sell[element] >= value)
                        {
                            value = sell[element];
                            index++;
                        }
                        Console.WriteLine($"ордер на покупку: {sell[element]}");
                    }
                    for (inside = 0, element = 0; element < 10; element++)
                    {
                        string binance = Get("https://api.binance.com/api/v1/depth?symbol=BNBBTC&limit=10");
                        dynamic decoded = JsonConvert.DeserializeObject(binance);
                        var ask = decoded.asks[element][inside];
                        price[element] = ask;
                        Console.WriteLine($"цена покупки: {price[element]}");
                        if (element == 9)
                            Console.WriteLine($"цена нужного ордера: {price[index]}");
                    }
                    var orderBuy = client.PlaceOrder("BNBBTC", OrderSide.Buy, OrderType.Limit, 1, price: price[index], timeInForce: TimeInForce.GoodTillCancel);
                    Thread.Sleep(3000);
                    var cancelByu = client.CancelOrder("BNBBTC", orderBuy.Data.OrderId);
                }
            }
        }
    }
}
