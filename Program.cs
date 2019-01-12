using Binance.Net;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Runtime.InteropServices;

namespace binance_bot
{
    class Program
    {
        public static int trey = 1;
        public static decimal priceForOrder = 0;
        public static decimal priceBuy = 0;
        public static decimal priceSell = 0;
        public static string couple = "";
        public static decimal coins = 0.0m;

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        static void Main(string[] args)
        {

            BinanceClient.SetDefaultOptions(new BinanceClientOptions()
            {
                ApiCredentials = new ApiCredentials("API KEY", "SECRET API KEY"),
                LogVerbosity = LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { Console.Out }
            });
            BinanceSocketClient.SetDefaultOptions(new BinanceSocketClientOptions()
            {
                ApiCredentials = new ApiCredentials("API KEY", "SECRET API KEY"),
                LogVerbosity = LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { Console.Out }
            });

            Console.WriteLine("Введите торговую пару большими буквами без пробелов");
            couple = Console.ReadLine();
            Console.WriteLine("Введите количество монет, которыми будете торговать");
            coins = Convert.ToDecimal(Console.ReadLine());
            Console.WriteLine("Хотите скрыть консольное окно?");
            Console.WriteLine("0 - нет, 1 - да");
            trey = Convert.ToInt32(Console.ReadLine());
            ShowWindow(GetConsoleWindow(), 1); // 0 - свернуть в трей, 1 - показать консольное окно
            while (true)
            {
                Buy();
                Sell();
            }
        }

        protected static void Buy()
        {
            int index = 0, element;
            decimal[] percent = new decimal[3];
            decimal[] price = new decimal[10];
            decimal[] buy = new decimal[10];
            decimal value = 0.000000000000001m;
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
                string binance = Get("https://api.binance.com/api/v1/depth?symbol=" + couple + "&limit=10");
                for (element = 0; element < 10; element++)
                {
                    dynamic decoded = JsonConvert.DeserializeObject(binance);
                    var bids = decoded.bids[element][1];
                    buy[element] = bids;
                    if (buy[element] >= value)
                    {
                        value = buy[element];
                        index = element;
                    }

                    Console.WriteLine($"объем покупки: {buy[element]}");
                }
                Console.WriteLine($"индекс нужного ордера: {index}");

                for (element = 0; element < 10; element++)
                {
                    dynamic decoded = JsonConvert.DeserializeObject(binance);
                    var bids = decoded.bids[element][0];
                    price[element] = bids;
                    Console.WriteLine($"цена покупки: {price[element]}");
                }
                Console.WriteLine($"цена нужного ордера: {price[index]}");
                priceBuy = price[index];
                priceForOrder = priceBuy - Fee(priceBuy);
                priceForOrder = Math.Round(priceForOrder, 3);
                var orderBuy = client.PlaceOrder(couple, OrderSide.Buy, OrderType.Limit, coins, price: priceForOrder, timeInForce: TimeInForce.GoodTillCancel);
            }
        }

        protected static void Sell()
        {
            int index = 0, element;
            decimal[] price = new decimal[10];
            decimal[] sell = new decimal[10];
            decimal value = 0.000000000000001m;
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
                string binance = Get("https://api.binance.com/api/v1/depth?symbol="+ couple + "&limit=10");

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

                    Console.WriteLine($"объем продажи: {sell[element]}");
                }
                Console.WriteLine($"индекс нужного ордера: {index}");

                for (element = 0; element < 10; element++)
                {
                    dynamic decoded = JsonConvert.DeserializeObject(binance);
                    var ask = decoded.asks[element][0];
                    price[element] = ask;
                    Console.WriteLine($"цена продажи: {price[element]}");
                }
                Console.WriteLine($"цена нужного ордера: {price[index]}");
                priceSell = price[index];
                priceForOrder = priceSell + Fee(priceSell);
                priceForOrder = Math.Round(priceForOrder, 3);
                var orderSellt = client.PlaceOrder(couple, OrderSide.Sell, OrderType.Limit, coins, price: priceForOrder, timeInForce: TimeInForce.GoodTillCancel);
                
            }
        }
        public static decimal Fee(decimal order)
        {
            return  order / 100 * 0.075m;
        }
    }
}
