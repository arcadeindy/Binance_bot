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
            int index = 0, element;
            decimal value = 0.000000000000001m;
            TimeSpan interval = new TimeSpan(0, 0, 3);
            BinanceClient.SetDefaultOptions(new BinanceClientOptions()
            {
                ApiCredentials = new ApiCredentials("API_KEY", "SECRET_API_KEY"),
                LogVerbosity = LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { Console.Out }
            });
            BinanceSocketClient.SetDefaultOptions(new BinanceSocketClientOptions()
            {
                ApiCredentials = new ApiCredentials("API_KEY", "SECRET_API_KEY"),
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
                    string binance = Get("https://api.binance.com/api/v1/depth?symbol=BNBBTC&limit=10");

                    /*-------------------------------------------------------------Продажа-------------------------------------------------------------*/

                    for (element = 0; element < 10; element++)
                    {
                        dynamic decoded = JsonConvert.DeserializeObject(binance);
                        var ask = decoded.asks[element][1];
                        sell[element] = ask;
                        if (sell[element] >= value)
                        {
                            value = sell[element];
                            index = element;
                        }
                        
                        Console.WriteLine($"ордер на продажу: {sell[element]}");
                    }
                    Console.WriteLine(index);

                    for (element = 0; element < 10; element++)
                    {
                        dynamic decoded = JsonConvert.DeserializeObject(binance);
                        var ask = decoded.asks[element][0];
                        price[element] = ask;
                        Console.WriteLine($"цена продажи: {price[element]}");
                    }
                    Console.WriteLine($"цена нужного ордера: {price[index]}");
                    var orderSellt = client.PlaceOrder("BNBBTC", OrderSide.Sell, OrderType.Limit, 1, price: price[index], timeInForce: TimeInForce.GoodTillCancel);

                    /*-------------------------------------------------------------Покупка-------------------------------------------------------------*/

                    for (element = 0; element < 10; element++)
                    {
                        dynamic decoded = JsonConvert.DeserializeObject(binance);
                        var bid = decoded.bids[element][1];
                        sell[element] = bid;
                        if (sell[element] >= value)
                        {
                            value = sell[element];
                            index = element;
                        }
                        Console.WriteLine($"ордер на покупку: {sell[element]}");
                    }
                    Console.WriteLine(index);

                    for (element = 0; element < 10; element++)
                    {
                        dynamic decoded = JsonConvert.DeserializeObject(binance);
                        var bid = decoded.bids[element][0];
                        price[element] = bid;
                        Console.WriteLine($"цена покупки: {price[element]}");
                    }
                    Console.WriteLine($"цена нужного ордера: {price[index]}");
                    var orderBuy = client.PlaceOrder("BNBBTC", OrderSide.Buy, OrderType.Limit, 1, price: price[index], timeInForce: TimeInForce.GoodTillCancel);
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
