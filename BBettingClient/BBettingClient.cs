using BBetModels;
using BBetModels.APIv1;
using BBetModels.DAG;
using BBetModels.DAG.SUTypeModels;
using BBetModels.DataStructures;
using BBetModels.DataStructures.Market;
using BBetModels.DataStructures.Order;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.WebSockets;
using Nethereum.ABI;
using Nethereum.Hex.HexConvertors.Extensions;
using System.Security;
using BBetModels.DataStructures.User;
using System.Security.Policy;
using System.Net.NetworkInformation;
using System.Net;
using System.Numerics;
using NSec.Cryptography;

namespace BBetting
{
    public partial class BBettingClient 
    {

        public static string GetUOrderKey(Guid marketid, int runnerid)
        {
            return marketid.ToString() + runnerid.ToString("####").PadLeft(4, '0');
        }
        public static (Guid, int) GetMIDFromKey(string id)
        {
            return (Guid.Parse(id.Substring(0, id.Length - 4)), int.Parse(id.Substring(id.Length - 4)));
        }

        private const int CleaningTimerInterval = 60 * 60 * 1000;
        private System.Timers.Timer _CleaningTimer;

        private Func<BBettingClient, JsonRespClient, bool> mReceiveRequest;
        public ClientWebSocket mSocketClient;

        public IPEndPoint EndPoint { get; }

        byte[] PrivKey;
        public byte[] PublicKey;
        public int ID;
        private bool mDisposedValue;
        private bool IsETH;
        private long _Nonce = 1;
        private long _NonceOffset;
        public long ValidatorNonce;

        public NonceTypes NonceType { get; set; }

        public enum NonceTypes
        {
            None,
            Ticks,
            Start
        }
        public BBettingClient(int ID, Func<BBettingClient, JsonRespClient, bool> externalReceiveRequest, string[] IP, bool isWebSocket = false, byte[] privkey = null, bool isETH = false, int valNonce = 0, int standardNode = -1)
        {
            ValidatorNonce = valNonce;
            byte[] pubkey;
            if (isETH)
            {
                IsETH = true;

                var privKey4 = new Nethereum.Signer.EthECKey(privkey, true);
                pubkey = privKey4.GetPubKey();


                var hexstr = BitConverter.ToString(pubkey).Replace("-", "");
            }
            else
            {
                if (privkey == null) privkey = StringX.CreatePrivKeyEd25519();
                pubkey = StringX.GetPublicKeyEd25519(privkey);
            }

            this.ID = ID;
            PrivKey = privkey;
            PublicKey = pubkey;
            NonceType = NonceTypes.Ticks;
            mReceiveRequest = externalReceiveRequest;

            //_Node = node;
            var uri = new Uri[IP.Length];

            for (int i = 0; i < IP.Length; i++)
            {
                if (IP[i].Split('.').Length == 4)
                {
                    uri[i] = new Uri($"ws://{IP[i]}:81");
                }
                else
                {
                    uri[i] = new Uri($"wss://{IP[i]}:82");
                }
            }

           
            if (isWebSocket)
            {
                CreateWebSocket(uri);
            }

            _CleaningTimer = new System.Timers.Timer(CleaningTimerInterval);
            _CleaningTimer.Elapsed += OnCleaningElapsed;
            _CleaningTimer.AutoReset = true;
            _CleaningTimer.Start();
        }

        //Save here all markets
        public List<CATEGORY> Sports = new List<CATEGORY>();
        public Dictionary<int, List<string>> mCompetitions = new Dictionary<int, List<string>>();
        public Dictionary<string, TransferReq> mBurns = new Dictionary<string, TransferReq>();
        public ConcurrentDictionary<Guid, Market> mMarkets = new ConcurrentDictionary<Guid, Market>();
        public ConcurrentDictionary<Guid, Market> mMarketsToSettle = new ConcurrentDictionary<Guid, Market>();
        private SortedDictionary<long, AutoResetEvent> mWaitForResponse = new SortedDictionary<long, AutoResetEvent>();
        private ConcurrentDictionary<long, JsonRespClient> mResponse = new ConcurrentDictionary<long, JsonRespClient>();

        //Need to be accessed from outside? Make it concurrent / threadsafe?!
        public ConcurrentDictionary<string, ConcurrentDictionary<Guid, ReturnUOrderDetail>> mUOrders = new ConcurrentDictionary<string, ConcurrentDictionary<Guid, ReturnUOrderDetail>>();
        public ConcurrentDictionary<Guid, ReturnMatch> mMOrders = new ConcurrentDictionary<Guid, ReturnMatch>();
        public Dictionary<Guid, ReturnMatch> mMMOrders = new Dictionary<Guid, ReturnMatch>();
        public Dictionary<Guid, TransferReq> mTransfers = new Dictionary<Guid, TransferReq>();

        public Dictionary<int, ReturnBalance> MyBalance = new Dictionary<int, ReturnBalance>();
  
 
        public long ClientTime
        {
            get { return DateTime.UtcNow.Ticks / 10; }
        }

        public IDictionary<Guid, Market> Markets
        {
            get { return mMarkets; }
        }




        public void SubscribeAll(MarketFilter mf = null, bool sports_comp=true)
        {
            
            //when mf is null, we query all markets
            if (mf == null) mf = new MarketFilter() { MaxResults = 0, PageSize = 100, OnlyActive = true, };

            if (sports_comp)
            {
                SendRequestSimple(RequestTypes.SubscribeSports, null, ID);
                Task.Delay(1000).Wait();
                SendRequestSimple(RequestTypes.SubscribeCompetitions, null, ID);
                Task.Delay(1000).Wait();
            }
            SendRequestSimple(RequestTypes.SubscribeMatches, new SubscribeMatchesReq() {  AddMarketSummary=true, FromDate = DateTime.UtcNow.AddDays(-31) }, ID);
            Task.Delay(3000).Wait();
            SendRequestSimple(RequestTypes.SubscribeUOrders, new SubscribeUOrdersReq() { AddMarketSummary = true }, ID);

            LastOrderReceived = DateTime.UtcNow;
            Task.Delay(2000).Wait();
            while (LastOrderReceived > DateTime.UtcNow.AddSeconds(-10))
            {
                Task.Delay(1000).Wait();
            }

            if (sports_comp)
            {
                SendRequestSimple(RequestTypes.SubscribeCompetitions, null, ID);
                Task.Delay(300).Wait();
            }
            SendRequestSimple(RequestTypes.SubscribeBalance, new SUData(), ID);
            Task.Delay(300).Wait();
            SendRequestSimple(RequestTypes.SubscribeMarketsByFilter, new SubscribeMarketsByFilterReq() { MarketFilter = mf, SubscribeOrderbooks = true }, ID);
        }
        public void SubscribeMost(MarketFilter mf)
        {
            //when mf is null, we query all markets
            if (mf == null) mf = new MarketFilter() { MaxResults = 0, PageSize = 100, OnlyActive = true, };

            SendRequestSimple(RequestTypes.SubscribeMatches, new SubscribeMatchesReq() { AddMarketSummary = true }, ID, 1);
            Task.Delay(300).Wait();
            SendRequestSimple(RequestTypes.SubscribeUOrders, new SubscribeUOrdersReq() { AddMarketSummary = true }, ID, 1);
            Task.Delay(300).Wait();
            SendRequestSimple(RequestTypes.SubscribeBalance, null, ID, 1);
            Task.Delay(300).Wait();
            SendRequestSimple(RequestTypes.SubscribeMarketsByFilter, new SubscribeMarketsByFilterReq() { MarketFilter = mf, SubscribeOrderbooks = true }, ID, 1);
        }

        public void SubscribePriv()
        {
            //when mf is null, we query all markets

            SendRequestSimple(RequestTypes.SubscribeMatches, new SubscribeMatchesReq() { AddMarketSummary = true, FromDate = DateTime.UtcNow.AddDays(-31) }, ID);
            Task.Delay(4300).Wait();
            SendRequestSimple(RequestTypes.SubscribeUOrders, new SubscribeUOrdersReq() { AddMarketSummary = true, SubscribeUpdates = true }, ID);

            LastOrderReceived = DateTime.UtcNow;
            Task.Delay(3000).Wait();
            while (LastOrderReceived > DateTime.UtcNow.AddSeconds(-10))
            {
                Task.Delay(1000).Wait();
            }

            SendRequestSimple(RequestTypes.SubscribeBalance, new SUData(), ID);
            Task.Delay(300).Wait();
            SendRequestSimple(RequestTypes.SubscribeTransfers, new SubscribeTransfersReq() { FromDate = DateTime.UtcNow.AddDays(-5) }, ID);
            Task.Delay(300).Wait();
        }
        public void SubscribeBurns()
        {
            //when mf is null, we query all markets

            SendRequestSimple(RequestTypes.SubscribeBurns, new SubscribeBurnsReq() { FromDate = DateTime.UtcNow.AddDays(-1) }, ID);
            Task.Delay(300).Wait();

        }
        public void SubscribeMarkets(MarketFilter mf)
        {
            //when mf is null, we query all markets

            SendRequestSimple(RequestTypes.SubscribeMarketsByFilter, new SubscribeMarketsByFilterReq() { MarketFilter = mf, SubscribeOrderbooks = true }, ID, 1);
            Task.Delay(300).Wait();

        }

        public virtual void addMatchedOrder(ReturnMatch mo)
        {
            //todo handle

            mMOrders[mo.MatchedOrder.ID] = mo;
            var mid = mo.UserOrder.MarketID;

            mMMOrders[mid] = mo;
        


        }

       


        public bool reconnectSuccessfulAndNoConnection(bool force=false)
        {

            var suc = ReConnect(force, random:true);

            return suc;

        }



       
        private long getNonce()
        {
            switch (NonceType)
            {
                case NonceTypes.Start:
                    _Nonce++;
                    return _Nonce;

                case NonceTypes.Ticks:
                    var current = ClientTime;
                    var corrected = current + _NonceOffset;
                    return corrected;

                default:
                    return 0;
            }
        }
    }
}
