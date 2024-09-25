using BBetModels;
using BBetModels.DataStructures.Market;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using BBetModels.DAG.SUTypeModels;
using BBetModels.APIv1;
using BBetModels.DataStructures.User;
using Nethereum.Model;
using NBitcoin.Secp256k1;
using Org.BouncyCastle.Crypto.Paddings;
using System.IO;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices.ComTypes;
using BBetModels.DataStructures.Order;

namespace BBetting
{
    class Program
    {
   

        static Config _Config;
        static object mOutputLock = new object();

        static void Main(string[] args)
        {           


            var domain = AppDomain.CurrentDomain;
            domain.UnhandledException += OnUnhandledException;


            var filename = "config.json";
            if (args.Length > 1 && args[1].Contains(".json"))  filename = args[0];
     
            if (!File.Exists(filename))
            {
                Console.WriteLine($"File {filename} does not exist");
                return;
            }


            try
            {
                var json = File.ReadAllText(filename);
                _Config = JsonConvert.DeserializeObject<Config>(json);

                Console.WriteLine("File " + filename + " read UserID: " + _Config.UserID);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"File could not be read " + ex.ToString());
                return;
            }

            byte[] privKey = StringX.GetBytesHex(_Config.PrivateKey);

            if(_Config.MainNodeId!=0) S.MAIN_MarketNode = _Config.MainNodeId;


            var client1 = new BBettingClient(_Config.UserID, receiveMessageRequest,  _Config.InitAddresses, true, privKey, isETH: true, standardNode: S.MAIN_MarketNode);


            Task.Delay(2000).Wait();
            client1.SubscribePriv();


            client1.SubscribeMarkets(new MarketFilter() { Cat= CATEGORY.TENNIS,  OnlyActive= true, Status= Market.StatusTypes.INPLAY, PageSize=100 });

            Task.Delay(5000).Wait();


            Console.WriteLine("Balances:");
            
            foreach (var fund in client1.MyBalance)
            {
                Console.WriteLine(" Bala " + fund.Key + ": " + fund.Value.ReservedFunds + " avail: " + fund.Value.AvailableFunds);
            }

            Console.WriteLine("Open Orders:");
            foreach (var uodic in client1.mUOrders)
            {
                foreach (var uo in uodic.Value)
                {
                    Console.WriteLine(" Bala " + uo.Value.MarketSummary + ": " + uo.Value.UnmatchedOrder.Amount + " @ " + uo.Value.UnmatchedOrder.Price);
                }
            }

            Console.WriteLine("All Matches:");
            foreach (var mo in client1.mMMOrders)
            {
               
                    Console.WriteLine(" Bala " + mo.Value.MarketSummary + ": " + mo.Value.MatchedOrder.Amount + " @ " + mo.Value.MatchedOrder.Price);
                
            }



            Console.WriteLine("All Handball Markets:");
            foreach (var ma in client1.Markets)
            {
                Console.WriteLine(" Market " + ma.Value.Descr + " " + ma.Value.Title + " " + ma.Value.ClosD);
            }


            Console.ReadLine();

        }

        public static object GetRandomEnumValue(Type t)
        {
            var randomGenerator = new Random();
            var randomAsInt = randomGenerator.Next(Enum.GetNames(t).Length);
            return Enum.ToObject(t, randomAsInt);

        }

        private static bool receiveMessageRequest(BBettingClient client, JsonRespClient resp)
        {
            if (resp == null)
                return false;

            var currentTimestamp = client.ClientTime;

            lock (mOutputLock)
            {
            
                switch (resp.State)
                {
                    case RequestStates.Success:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case RequestStates.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }

                Console.WriteLine(resp.ToString());
                Console.ForegroundColor = ConsoleColor.White;

                switch (resp.Type)
                {
                    case RequestTypes.ReturnHeartbeat:
      
                        break;
                    default:
                        var res = PerformanceRes.AnalyseReceivedInternal(resp.Nonce, currentTimestamp);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine(res);
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }
            }

            return true;
        }


  
        static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception)e.ExceptionObject;
            Console.WriteLine("UnhandledException: " + exception.Message);
            Console.WriteLine("   TraceStack: " + exception.StackTrace);
        }


    }
}
