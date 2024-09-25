//#define LogTraffic
using BBetModels.DAG;
using BBetModels.DAG.SUTypeModels;
using BBetModels.DataStructures;
using BBetModels.DataStructures.Market;
using BBetModels.DataStructures.Order;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BBetModels.APIv1;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Net.WebSockets;
using System.Collections;
using BBetModels.DataStructures.User;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using BBetModels;
using BBetModels.Common;

namespace BBetting
{
    public partial class BBettingClient 
    {
        public Uri[] URI;
        #region WebSockets Methods

        public bool Reconnecting = false;
        public DateTime LastOrderReceived = DateTime.UtcNow;

        public int Use_IP = 0;
        private void CreateWebSocket(Uri[] uri)
        {
            //  var token = new CancellationTokenSource();
            mSocketClient = new ClientWebSocket();
            //  mSocketClient.ConnectAsync(uri[Use_IP], token.Token).Wait();
            URI = uri;
            Task.Delay(1000).Wait();

            for (int i = 0; i < 8; i++)
            {

                bool suc = ReConnect(force:true, random:false);
                if (suc) return;
                Task.Delay(10000).Wait();
            }
        }

        private bool ValidateCertificate(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
        {
            var result = false;

            if (certificate is X509Certificate2)
            {
                var certificate2 = (X509Certificate2)certificate;
                result = certificate2.Verify();
            }

            return result;
        }
       
        public bool ReConnect(bool force, bool random)
        {
            if (force || (mSocketClient != null && mSocketClient.State != WebSocketState.Open))
            {
                if (SchedulerX.IsDue("ReconnectWS", 30000))
                {
                    try
                    {
                        if (random)
                        {
                            if (StringX.RC.NextDouble() > 0.7)
                            {
                                Use_IP++;
                                if (Use_IP >= URI.Length) Use_IP = 0;
                            }
                            Reconnecting = true;
                            mSocketClient.Dispose();
                            Task.Delay(1000).Wait();
                        }


                        //MesssageHandler.SendMessage2(" Reconnect " + Use_IP);

                        mSocketClient = new ClientWebSocket();
                        mSocketClient.Options.RemoteCertificateValidationCallback = ValidateCertificate;

                        var token = new CancellationTokenSource();
                        mSocketClient.ConnectAsync(URI[Use_IP], token.Token).Wait();

                        if (mSocketClient.State != WebSocketState.Open)
                        {
                            Console.WriteLine("WebSocket not connected: " + mSocketClient.State);
                            return false;
                        }

                        Console.WriteLine("WebSocket connected: " + mSocketClient.State  + " to: " + URI[Use_IP]);

                        Task.Factory.StartNew(StartReceivingMessages);
                        //SubscribeAll();
                    }
                    catch (Exception ex)
                    {

                        //if (!force) MesssageHandler.SendMessage2($" error on Reconnect: {ex.Message}");
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }
       
        private async void StartReceivingMessages()
        {
            var rcvBuffer = new byte[65536];
            var messageBuilder = new StringBuilder();

            await Task.Delay(1000);
            while (mSocketClient != null && mSocketClient.State == WebSocketState.Open)
            {
                try
                {
                    var token = new CancellationToken();
                    var rcvResult = await mSocketClient.ReceiveAsync(rcvBuffer, token);
                    switch (rcvResult.MessageType)
                    {
                        case WebSocketMessageType.Close:
                            {
                                var messageStr = rcvResult.CloseStatusDescription;
                                await mSocketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "See you", CancellationToken.None);
                                Console.WriteLine("WebSocket closed: " + messageStr);
                            }
                            break;

                        case WebSocketMessageType.Text:
                            var currentStr = Encoding.UTF8.GetString(rcvBuffer, 0, rcvResult.Count);
                            messageBuilder.Append(currentStr);

                            if (rcvResult.EndOfMessage)
                            {
                                var messageStr = messageBuilder.ToString();
                                messageBuilder.Clear();

                                if (messageStr != null)
                                {
                                    ReceiveServerAnswer(messageStr);
                                }

                                messageStr = null;
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    var messageStr = messageBuilder.ToString();
                    //MesssageHandler.SendMessage2("Exc on Receive " + messageStr + " ex " + ex.ToString(), 345472, 0.1, 1, 30);
                    var jerror = new JsonRespClient { State = RequestStates.Error, Error = ex.Message, Data = ex, };
                    ReceiveServerAnswer(jerror.ToString());
                    messageBuilder.Clear();
                }
            }
        }
        int orders_rec = 0;
     
        private bool ReceiveServerAnswer(string msg)
        {
            if (String.IsNullOrWhiteSpace(msg))
                return false;

            var stateStr =  msg.Substring("\"State\":\"", "\"", 100);
            if (string.IsNullOrEmpty(stateStr))
                return false;

            var state = stateStr.TryParseEnum(RequestStates.None);
            var typeStr = msg.Substring("\"Type\":\"", "\"", 100);

            if (string.IsNullOrEmpty(typeStr))
            {
                switch (state)
                {
                    case RequestStates.Success:
                    case RequestStates.Error:
                        mReceiveRequest(this, new JsonRespClient() { Data = msg });
                        break;

                }

                return false;
            }

            var result = false;
            var type = typeStr.TryParseEnum(RequestTypes.Default);
            JsonRespClient resp = null;
            try
            {

                resp = RequestBas0.CreateResponse(state, type, msg);

            }
            catch (Exception)
            {

                return false;
            }
            switch (state)
            {
                case RequestStates.Success:
                    {
                        switch (resp.Type)
                        {
                            case RequestTypes.SubscribeSports:
                                {
                                    var sdata = (List<string[]>)resp.Data;
                                    Sports.Clear();
                                    Sports.AddRange(sdata.Select(m => m[1].TryParseEnum(CATEGORY.RESERVED6)));
                                }
                                break;
                            case RequestTypes.GetWholeBalance:
                                {
                                          }
                                break;
                            case RequestTypes.SubscribeCompetitions:
                                {
                                    var allKeys = mCompetitions.Keys.ToList();
                                    var cdata = (Dictionary<int, Dictionary<string, int>>)resp.Data;
                                    foreach (var item in cdata)
                                    {
                                        List<string> competitions;
                                        if (!mCompetitions.TryGetValue(item.Key, out competitions))
                                        {
                                            competitions = new List<string>();
                                            mCompetitions.TryAdd(item.Key, competitions);
                                        }
                                        else
                                        {
                                            allKeys.Remove(item.Key);
                                        }

                                        competitions.Clear();
                                        competitions.AddRange(item.Value.Keys);
                                        competitions.Sort();
                                    }

                                    foreach (var key in allKeys)
                                        mCompetitions.Remove(key);
                                }
                                break;
                            case RequestTypes.SubscribeBurns:
                                {
                                    var cdata = (ReturnBurn[])resp.Data;
                                    if (cdata != null && cdata.Length > 0)
                                    {
                                        foreach (var burn in cdata)
                                        {
                                            if (!mBurns.ContainsKey(burn.Txid))
                                            {
                                                //process it
                                            }
                                            mBurns[burn.Txid] = burn.Transfer;
                                        }
                                    }

                                }
                                break;
                            case RequestTypes.SubscribeBalance:
                                {
                                    var bdata = (Dictionary<int, ReturnBalance>)resp.Data;
                                    MyBalance = bdata;
                                }
                                break;
                            case RequestTypes.SubscribeUOrders:
                                {
                                    var marray = (IList<ReturnUOrder>)resp.Data;
                                    if (marray != null && marray.Count > 0)
                                    {
                                        if (marray.Count > 3)
                                        {
                                            //MesssageHandler.SendMessage2(" uoders rec" + orders_rec, 1536, 0.1, double.MaxValue);

                                            orders_rec++;
                                            LastOrderReceived = DateTime.UtcNow;
                                        }
                                        for (int i = 0; i < marray.Count; i++)
                                        {
                                            var mdata = new ReturnUOrderDetail() { AddedByWS = true, AddedDate = DateTime.UtcNow, MarketSummary = marray[i].MarketSummary, UnmatchedOrder = marray[i].UnmatchedOrder, UserOrder = marray[i].UserOrder };
                                            if (mdata.UserOrder == null)
                                                continue;

                                            var okey = GetUOrderKey(mdata.UserOrder.MarketID, mdata.UserOrder.RunnerID);
                                            if (!mUOrders.ContainsKey(okey)) mUOrders[okey] = new ConcurrentDictionary<Guid, ReturnUOrderDetail>();

                                            var marketOrders = mUOrders[okey];

                                            var removemO = new List<Guid>();
                                            int ct = 0;
                                            foreach (var ord in marketOrders)
                                            {
                                                if (ord.Value.UnmatchedOrder.State != UOrder.UOStates.Active)
                                                {
                                                    removemO.Add(ord.Key);
                                                    continue;
                                                }
                                                ct++;

                                            }



                                            //if order is cancelled, delete the current order only if the IDs are the same

                                            if (marketOrders.ContainsKey(mdata.UnmatchedOrder.ID))
                                            {
                                                var saved_order = marketOrders[mdata.UnmatchedOrder.ID];
                                                saved_order.ConfirmedByWS = true;

                                                if (saved_order.UnmatchedOrder.State != mdata.UnmatchedOrder.State)
                                                {
                                                    if (saved_order.UnmatchedOrder.State == UOrder.UOStates.Cancelled)
                                                    {

                                                    }
                                                }

                                                if (saved_order.UnmatchedOrder.State != UOrder.UOStates.Active || mdata.UnmatchedOrder.State != UOrder.UOStates.Active)
                                                {
                                                    marketOrders.TryRemove(mdata.UnmatchedOrder.ID, out var dummy);
                                                }


                                            }
                                            else
                                            {
                                                if (mdata.UnmatchedOrder.State == UOrder.UOStates.Active)
                                                {
                                                    marketOrders[mdata.UnmatchedOrder.ID] = mdata;
                                                }
                                            }

                                            foreach (var el in removemO)
                                            {
                                                marketOrders.TryRemove(el, out var dumm1);
                                            }



                                            ct = 0;
                                            foreach (var ord in marketOrders)
                                            {

                                                if (ord.Value.UnmatchedOrder.State != UOrder.UOStates.Active) continue;
                                                ct++;

                                            }

                                            if (ct == 0)
                                            {
                                                mUOrders.TryRemove(okey, out var dummy3);
                                            }
                                            if (ct > 2)
                                            {

                                            }

                                        }
                                    }
                                }
                                break;
                            case RequestTypes.SubscribeTransfers:
                                {
                                    var marray = (IList<TransferReq>)resp.Data;
                                    if (marray != null && marray.Count > 0)
                                    {
                                        //   MesssageHandler.SendMessage2(" receive WS Order " + msg, 0, 1, 0);
                                        for (int i = 0; i < marray.Count; i++)
                                        {
                                            var transfer = marray[i];
                                            mTransfers[transfer.ID] = transfer;


                                        }
                                    }
                                }
                                break;
                            case RequestTypes.SubscribeMatches:
                                {
                                    if (resp.Data != null && resp.Data is IEnumerable)
                                    {
                                        foreach (var el in (IEnumerable)resp.Data)
                                        {
                                            var rm = (ReturnMatch)el;
                                            addMatchedOrder(rm);

                                        }
                                    }
                                    //var mdata = (ReturnMatch[])resp.Data;


                                    //if (mdata != null && mdata.Length > 0)
                                    //{
                                    //    for (int i = 0; i < mdata.Length; i++)
                                    //        addMatchedOrder(mdata[i]);
                                    //}
                                }
                                break;
                            case RequestTypes.AmendMatch:
                                {
                                    
                                }
                                break;

                            case RequestTypes.SubscribeMarketsByFilter:
                                {
                                    var mdata = (IList<Market>)resp.Data;
                                    if (mdata.Count > 10) LastOrderReceived = DateTime.UtcNow;
                                    foreach (var item in mdata)
                                    {
                                        mMarkets.AddOrUpdate(item.ID, (item as Market), (Guid id, Market m) => m);

                                        if (item.Ru != null)
                                        {
                                            foreach (var ru in item.Ru)
                                            {
                                                if (ru.Orderbook != null && ru.OrdBook == null)
                                                {
                                                    ru.OrdBook = ru.Orderbook;
                                                }
                                            }
                                        }
                                    }
                                }
                                break;

                            case RequestTypes.ReturnOrderbook:
                                {
                                    var obdata = (OrderBookSimple)resp.Data;

                                    if (obdata.MarketID.ToString().Contains("d8205"))
                                    {

                                    }
                                    if (mMarkets.ContainsKey(obdata.MarketID))
                                    {
                                        var m = mMarkets[obdata.MarketID];
                                        var ru = m.Ru[obdata.RunnerID];
                                        if (ru.OrdBook == null || ru.OrdBook.Bids == null || ru.OrdBook.Asks == null || (ru.OrdBook.Bids.Count == 0 && ru.OrdBook.Asks.Count == 0))
                                        {

                                        }
                                        ru.OrdBook = obdata;
                                    }
                                }
                                break;

                            case RequestTypes.MarketCreation:
                                {
                                    var mdata = (MarketCreationReq)resp.Data;
                                    if (mdata != null && mdata.Market != null)
                                        mMarkets.AddOrUpdate(mdata.Market.ID, mdata.Market, (Guid id, Market m) => m);
                                }
                                break;
                            case RequestTypes.MarketsToSettle:
                                {
                                    var mdata = (MarketsToSettleReturn)resp.Data;
                                    if (mdata != null && mdata.MarketIDsToSettle != null)
                                    {
                                        foreach (var mid in mdata.MarketIDsToSettle)
                                        {
                                            Market m = null;
                                            if (mMarkets.ContainsKey(mid)) m = mMarkets[mid];

                                            mMarketsToSettle[mid] = m;

                                        }
                                    }
                                }
                                break;

                            case RequestTypes.ReturnHeartbeat:
                                {
                                    var serverNone = resp.Nonce;
                                    var clientNonce = DateTime.UtcNow.Ticks / 10;
                                    var span = TimeSpan.FromTicks((resp.Nonce - clientNonce) * 10);

                                    _NonceOffset = resp.Nonce - clientNonce;
                                }
                                break;

                            default:
                                break;
                        }

                        AutoResetEvent autoReset;
                        if (mWaitForResponse.TryGetValue(resp.Nonce, out autoReset))
                        {
                            mResponse[resp.Nonce] = resp;
                            autoReset.Set();
                        }



                        if (mReceiveRequest != null)
                        {
                            result = mReceiveRequest(this, resp);
                        }
                    }
                    break;

                case RequestStates.Error:
                    if (resp.Type == RequestTypes.Settlement)
                    {
                        if (resp.Error != null && resp.Error.Contains("already settled."))
                        {
                            var mid = StringX.SubstringMy(resp.Error, "Market ", " already settled");

                            if (Guid.TryParse(mid, out var midguid))
                            {
                                AlreadySettledMarkets[midguid] = true;

                            }

                        }
                    }
                    if (resp.Type == RequestTypes.ChangeMarketTimes)
                    {
                        if (resp.Error != null && resp.Error.Contains("already settled."))
                        {
                            var mid = StringX.SubstringMy(resp.Error, "Market ", " already settled");

                            if (Guid.TryParse(mid, out var midguid))
                            {
                                AlreadySettledMarkets[midguid] = true;

                            }

                        }
                    }

                    mReceiveRequest(this, resp);
                    break;

                default:
                    break;
            }

            return result;
        }

        public Dictionary<Guid, bool> AlreadySettledMarkets = new Dictionary<Guid, bool>();

        private void CloseWebSocket()
        {
            if (mSocketClient != null)
            {
                var ctoken = new CancellationTokenSource();

                try
                {

                    var task = mSocketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "Bye bye", ctoken.Token);
                    task.Wait(ctoken.Token);
                }
                catch (Exception)
                {


                }

                mSocketClient.Dispose();
            }
        }
        #endregion

    
        public ClientStates State
        {
            get { return mDisposedValue ? ClientStates.Closed : ClientStates.Open; }
        }
  
       


        protected virtual void Dispose(bool disposing)
        {
            if (!mDisposedValue)
            {
                CloseWebSocket();
                mDisposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    

        #region Cleaning Dicts
        private bool m_IsCleaing = false;
        private void OnCleaningElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Clear all markets that are Closed for more than 24hours
            // Clear all ChangeMarketTime requests that are for markets closed more than 24h ago
            // Clear all unmatched orders, that were on markets closed for more than 1h;

            if (m_IsCleaing)
                return;

            m_IsCleaing = true;


            int run = 0;
            while (run < 2)
            {
                try
                {


                    var allUOrders = mUOrders.ToArray();
                    foreach (var item in allUOrders)
                    {

                        foreach (var ord in item.Value.Values)
                        {

                            var isRemoved = false;
                            if (ord == null)
                                continue;

                            if (ord.UserOrder == null)
                            {
                                isRemoved = true;
                                continue;
                            }

                            Market market;
                            if (!mMarkets.TryGetValue(ord.UserOrder.MarketID, out market) || market == null)
                            {
                                isRemoved = true;
                            }
                            else
                            {
                                if (market.ClosD < DateTime.UtcNow.AddHours(-24))
                                    isRemoved = true;
                            }


                            if (isRemoved)
                                mUOrders[item.Key].TryRemove(ord.UnmatchedOrder.ID, out var dummy1);

                        }

                    }


                    var allMOrders = mMOrders.ToArray();
                    foreach (var item in allMOrders)
                    {
                        var isRemoved = false;
                        if (item.Value.UserOrder == null)
                        {
                            isRemoved = true;
                            continue;
                        }
                        if (item.Value.MatchedOrder.State == Match.MOState.PENDING || item.Value.MatchedOrder.State == Match.MOState.MATCHED)
                        {
                            continue;
                        }

                        Market market;
                        if (!mMarkets.TryGetValue(item.Value.UserOrder.MarketID, out market) || market == null)
                        {
                            isRemoved = true;
                        }
                        else
                        {
                            if (market.ClosD < DateTime.UtcNow.AddHours(-72))
                                isRemoved = true;
                        }

                        if (isRemoved)
                        {
                            mMOrders.TryRemove(item.Key, out var x);
                            if (mMMOrders.ContainsKey(item.Value.UserOrder.MarketID)) mMMOrders.Remove(item.Value.UserOrder.MarketID);

                        }
                    }

                    var allMarkets = mMarkets.Values.ToArray();
                    foreach (var market in allMarkets)
                    {
                        var isRemoved = false;
                        if (market.ClosD < DateTime.UtcNow.AddHours(-24))
                            isRemoved = true;

                        if (isRemoved)
                        {
                            Market result;
                            mMarkets.TryRemove(market.ID, out result);
                        }
                    }
                    run++;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(5);
                }
                finally
                {
                    run++;
                }
            }

            m_IsCleaing = false;
        }
        #endregion
    }
}
