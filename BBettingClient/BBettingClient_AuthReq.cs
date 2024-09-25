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
        public ConcurrentDictionary<long, (DateTime, Market)> ChangeMarketTimeRequests = new ConcurrentDictionary<long, (DateTime, Market)>();

        public (bool, long) SendAuthRequest(RequestTypes sutype, SUData sd, int userid = -1)
        {
            sd.NodeID = S.MAIN_MarketNode;
            sd.UserID = userid == -1 ? ID : userid;
            //TODO do time syncing  GET  NODE TIME  and add offset!
            if (sd.CreatedByUser == default(DateTime)) sd.CreatedByUser = DateTime.UtcNow;
            sd.MinerFee = 0.00000000001d;
            var contLen = RequestBas0.GetContentLength(sd, sutype) + 10;
            sd.MinerFee = Math.Round((double)contLen * 0.0001d * 0.0001, 12);

            byte[] sign = RequestBas0.GetSignature(sd, sutype, PrivKey, IsETH);
            var su = new StorageUnitOnClient()
            {
                Type = sutype,
                Data = sd,

                SignatureUser = sign,
                Nonce = getNonce()

            };


            var result = SendBaseRequest(su);
            return (result, su.Nonce);

        }

        private (bool, JsonRespClient) SendAuthRequestAndWait(RequestTypes sutype, SUData sd, int userid = -1)
        {
            sd.NodeID = S.MAIN_MarketNode;
            sd.UserID = userid == -1 ? ID : userid;
            //TODO do time syncing  GET  NODE TIME  and add offset!
            if (sd.CreatedByUser == default(DateTime)) sd.CreatedByUser = DateTime.UtcNow;
            sd.MinerFee = 0.00000000001d;
            var contLen = RequestBas0.GetContentLength(sd, sutype) + 10;
            sd.MinerFee = Math.Round((double)contLen * 0.0001d * 0.0001, 12);

            byte[] sign = RequestBas0.GetSignature(sd, sutype, PrivKey, IsETH);
            var su = new StorageUnitOnClient()
            {
                Type = sutype,
                Data = sd,

                SignatureUser = sign,
                Nonce = getNonce()

            };

            bool result;

            var resetEvent = new AutoResetEvent(false);
            mWaitForResponse[su.Nonce] = resetEvent;
            result = SendBaseRequest(su);


            resetEvent.WaitOne(5000);


            if (mResponse.ContainsKey(su.Nonce))
            {

                return (String.IsNullOrEmpty(mResponse[su.Nonce].Error), mResponse[su.Nonce]);
            }
            return (false, null);

        }

        public bool CreateMarket(Market market, int userid = -1, bool wait = false)
        {
           
            if (market.MainNodeID == 0) market.MainNodeID = S.MAIN_MarketNode;
            var sd = new MarketCreationReq()
            {
                Market = market,
                CreatedByUser = DateTime.UtcNow

            };
            sd.Market.CreationDate = sd.CreatedByUser;

            if (wait)
            {
                var (suc, resultx) = SendAuthRequestAndWait(RequestTypes.MarketCreation, sd, userid);
                if (suc) return true;
                if (resultx != null)
                {

                    //MesssageHandler.SendMessage2("Could not create market  " + resultx.Error);
                }
                //MesssageHandler.SendMessage2("Could not create market  no response " );
                return false;
            }
            else
            {
                var (result, nonce) = SendAuthRequest(RequestTypes.MarketCreation, sd, userid);

                return result;
            }



        }
     
        public bool ChangeMarketTime(Guid marketId, Market mx, DateTime closeTime, DateTime settleTime = default(DateTime))
        {
            var sd = new ChangeMarketTimeReq()
            {
                Mid = marketId,
                ClosD = closeTime,
                SetlD = settleTime
            };

            var (result, nonce) = SendAuthRequest(RequestTypes.ChangeMarketTimes, sd, -1);

            ChangeMarketTimeRequests[nonce] = (closeTime, mx);

            return result;
        }

        public Guid? SendOrderAlteration(Guid marketid, bool inplay, int CatID, Guid newguid, int ruid, int boa, double pri, double am, Guid? orderid = null)
        {

            int ctime = S.MAKERCANCELTIME;


            if (inplay)
            {
                ctime = S.MAKERCANCELTIMEINPLAY;
                if (CatID == (int)CATEGORY.SOCCER) ctime = S.MAKERCANCELTIMEINPLAYSOCCER;

            }
            if (CatID == (int)CATEGORY.BITCOIN) ctime = 0;
            if (CatID == (int)CATEGORY.DICE) ctime = 0;
            if (CatID == (int)CATEGORY.POLITICS && ID != 2) ctime = 0;



            var umo = pri == 0 ? null : new UOrder(newguid, false, ID, boa, pri, (decimal)am, UOrder.Types.Default, null, ctime) { };

            var uso = new UserOrder() { MarketID = marketid, Side = boa, Insurance = false, OrderID = newguid, RunnerID = ruid };

            var sd = new OrderAlterationReq() { UnmatchedOrder = umo, UserOrder = uso, OldOrderID = orderid };


            //sd.NodeID = nodeid;
            //sd.UserID = ID;
            //byte[] sign = StringC2.GetSignatureEd25519(JsonConvert.SerializeObject(sd), PrivKey);

            //var su = new StorageUnitPub() { Type = SUType.MarketCreation, Data = sd, SignatureUser = sign };
            //sendRequest(JsonConvert.SerializeObject(su));

            //return true;

            var (result, nonce) = SendAuthRequest(RequestTypes.OrderAlteration, sd, -1);
            if (!result) return null;
            return newguid;
        }
        public Guid? SendOrderAlterationAdv(Guid marketid, bool inplay, int CatID, Guid newguid, int ruid, int boa, double pri, double am, decimal max_am, Guid? orderid = null)
        {

            int ctime = S.MAKERCANCELTIME;


            if (inplay)
            {
                ctime = S.MAKERCANCELTIMEINPLAY;
                if (CatID == (int)CATEGORY.SOCCER) ctime = S.MAKERCANCELTIMEINPLAYSOCCER;

            }
            if (CatID == (int)CATEGORY.BITCOIN) ctime = 0;



            var umo = pri == 0 ? null : new UOrder(true, max_am, newguid, ID, boa, pri, (decimal)am, UOrder.Types.Default, null, ctime) { };

            var uso = new UserOrder() { MarketID = marketid, Side = boa, Insurance = false, OrderID = newguid, RunnerID = ruid };

            var sd = new OrderAlterationReq() { UnmatchedOrder = umo, UserOrder = uso, OldOrderID = orderid };


            //sd.NodeID = nodeid;
            //sd.UserID = ID;
            //byte[] sign = StringC2.GetSignatureEd25519(JsonConvert.SerializeObject(sd), PrivKey);

            //var su = new StorageUnitPub() { Type = SUType.MarketCreation, Data = sd, SignatureUser = sign };
            //sendRequest(JsonConvert.SerializeObject(su));

            //return true;

            var (result, nonce) = SendAuthRequest(RequestTypes.OrderAlteration, sd, -1);
            if (!result) return null;
            return newguid;
        }

        public int cancelOnMarket(Market m, int sleep = 0)
        {
            int max_runner = m.Ru.Length;
            if (mMarkets.ContainsKey(m.ID))
            {
                if (mMarkets[m.ID] != null)
                {
                    var ru = mMarkets[m.ID].Ru.Length;
                    if (ru > 2) max_runner = ru;
                }
            }
            int ct = 0;
            for (int i = 0; i < max_runner; i++)
            {
                ct += cancelOnMarket(m.ID, i, sleep);
            }
            return ct;

        }
        public int cancelOnMarket(Guid lmid, int run, int sleep = 1)
        {
            var okey = GetUOrderKey(lmid, run);
            int ctcancel = 0;
            if (mUOrders.ContainsKey(okey))
            {
                var moxfs = mUOrders[okey];
                var market_orders = mUOrders[okey].ToArray();



                foreach (var ord in market_orders)
                {
                    if (ord.Value == null || ord.Value.UnmatchedOrder.State != UOrder.UOStates.Active) continue;


                    var xsf = SendOrderAlteration(lmid, false, 0, Guid.Empty, run, 0, 0d, 0d, ord.Value.UnmatchedOrder.ID);
                    ctcancel++;

                    System.Threading.Thread.Sleep(sleep);


                    ord.Value.UnmatchedOrder.State = UOrder.UOStates.Cancelled;
                    ord.Value.CancelBySelf = true;
                    ord.Value.LastChange = DateTime.UtcNow;

                  

                }

            }

            return ctcancel;
        }
        public int cancelAll(int sleep=2)
        {
            int ctcancel = 0;
            foreach (var or in mUOrders)
            {
                var market_orders = or.Value.ToArray();

                foreach (var ord in market_orders)
                {
                    if (ord.Value == null || ord.Value.UnmatchedOrder.State != UOrder.UOStates.Active) continue;


                    var xsf = SendOrderAlteration(ord.Value.UserOrder.MarketID, false, 0, Guid.Empty, ord.Value.UserOrder.RunnerID, 0, 0d, 0d, ord.Value.UnmatchedOrder.ID);
                    ctcancel++;
                    Task.Delay(sleep).Wait();

                    ord.Value.UnmatchedOrder.State = UOrder.UOStates.Cancelled;
                    ord.Value.CancelBySelf = true;
                    ord.Value.LastChange = DateTime.UtcNow;

                    // if (xsf == null)
                    // {
                    //mUOrders[okey].TryRemove(ord.Key, out var dummy);
                    //}


                }

            }


            return ctcancel;
        }
      
        public bool AmendMatch(Guid mid, int rid, Guid orderid, double am, MatchedOrderFlag moflag, Match.MOState state, double red)
        {
            var amr = new AmendMatchReq() { am = am, Flag = moflag, mid = mid, moState = state, orderid = orderid, Red = red, rid = rid };
            var (result, nonce) = SendAuthRequest(RequestTypes.AmendMatch, amr, -1);
            return result;
        }
        public bool SendSettleRequest(SettlementReq sreq)
        {
            var (result, nonce) = SendAuthRequest(RequestTypes.Settlement, sreq, -1);
            return result;
        }

       

         public string IssueInvite(decimal am = 0m)
        {
            if (am <= 0) am = S.MinimalInitialAccountFunding;
            var InviteCode = StringX.RandomString(12);

            var hash = StringX.GetHashString(InviteCode).ToUpper();
            var sd = new AccountCreationReq()
            {

                CreationMode = AccountCreationReq.CREATIONMODE.ISSUEINVITE,
                StartingBalance = am,
                InviteCodeHash = hash

            };

            var (result, nonce) = SendAuthRequest(RequestTypes.AccountCreation, sd, -1);

            return InviteCode;
        }

        public (bool, long) SendTransfer(double am, string descr, int toU, TransferReq.TransferType transfertype, Guid guid, int cur = 0)
        {
            //Burn
            //SendTransfer(1, "withdrewaladdress", 0, TransferReq.TransferType.Burn, Guid.NewGuid(), 0, 1);


            var sd = new TransferReq()
            {
                Amount = am,
                Cur = cur,
                From = ID,
                ID = guid,
                Reference = descr,
                To = toU,
                TType = transfertype
            };

            var (result, nonce) = SendAuthRequest(RequestTypes.Transfer, sd, ID);
            return (result, nonce);
        }
       
        #region special
        public bool IssueCurrency(int id, double am)
        {
            var cu = new Currency() { ID = 0, TotalBalance = (decimal)am, Maintainer = ID, ColdWalletAddress = "To be entered", Name = "Milli-Bitcoin", Symbol = "mBTC" };
            if (id == 1) cu = new Currency() { ID = 1, TotalBalance = (decimal)am, Maintainer = ID, ColdWalletAddress = "0x48EAE4259EC1498Cc9092732EdC6e829611d42C3", Name = "Milli-Ether (Smart Contract)", Symbol = "mETH", DisallowIssuingByMaintainer = true, CrossChainPayments = true };
            if (id == 2) cu = new Currency() { ID = 2, TotalBalance = (decimal)am, Maintainer = ID, ColdWalletAddress = "0x48EAE4259EC1498Cc9092732EdC6e829611d42C3", Name = "Wrapped Milli-Bitcoin (Smart Contract)", Symbol = "mWBTC", DisallowIssuingByMaintainer = true, CrossChainPayments = true };
            if (id == 3) cu = new Currency() { ID = 3, TotalBalance = (decimal)am, Maintainer = ID, ColdWalletAddress = "0x48EAE4259EC1498Cc9092732EdC6e829611d42C3", Name = "USDC (Smart Contract)", Symbol = "USDC", DisallowIssuingByMaintainer = true, CrossChainPayments = true };
            var sd = new CurrencyIssuanceReq() { ApiID = 0, Currency = cu };
            var (result, nonce) = SendAuthRequest(RequestTypes.CurrencyIssuance, sd, -1);
            return result;
        }
        public bool makeDeFiDeposit(int id, double am, string txhash)
        {

            var dfs = new DeFiDeposit() { Amount = am, TXID = txhash, UserID = id };
            var cu = new Currency { ID = 1 };
            var sd = new CurrencyIssuanceReq() { ApiID = 0, Deposit = dfs, Currency = cu };

            var (result, nonce) = SendAuthRequest(RequestTypes.CurrencyIssuance, sd, -1);
            return result;
        }

        public bool RegisterSelf(int usedUserId = 0, byte[] pubkey = null)
        {
            var accC = new AccountCreationReq()
            {
                NewAccountID = ID,
                PubKey = pubkey != null ? pubkey : PublicKey,
                UserID = usedUserId, //exception not needed for first 100 accounts
            };

            var (result, nonce) = SendAuthRequest(RequestTypes.AccountCreation, accC, -1);

            ChangeAccountSettings(true, 10000 * 1000, "", 0);



            //sd.NodeID = nodeid;//exception not needed for first 100 accounts

            //byte[] sign = usedUserId ==0 ? null : StringC2.GetSignatureEd25519(JsonConvert.SerializeObject(sd), PrivKey);

            //var su = new StorageUnitPub() { Type = SUType.AccountCreation, Data = sd, SignatureUser = sign };
            //sendRequest(JsonConvert.SerializeObject(su));

            return result;
        }


        public bool Register(int newID, byte[] PubKey = null, bool isETH = false)
        {

            var sd = new AccountCreationReq()
            {
                NewAccountID = newID,
                PubKey = (PubKey != null) ? PubKey : PublicKey,
                UserID = ID, //exception not needed for first 100 accounts
                IsETH = isETH

            };

            var (result, nonce) = SendAuthRequest(RequestTypes.AccountCreation, sd, -1);

            return result;
        }
        public bool ChangeAccountSettings(bool force_confirm, long absenceCancelms, string udp_ip, int udp_port)
        {
            var accS = new AccountSettingReq()
            {
                AccountID = ID,
                AbsenceCancelMS = 10000 * 1000,
                ForceConfirmMatched = force_confirm,
                UDP_IP = udp_ip,
                UDP_Port = udp_port
            };
            var (result, nonce) = SendAuthRequest(RequestTypes.AccountSetting, accS, -1);
            return true;
        }
        //This has to be called within InterNodeCommunication
        public (bool, string) ValidateBurn(TransferReq trReq, string txid, string privkeyValidator)
        {

            var privKey4b = new Nethereum.Signer.EthECKey(privkeyValidator);

            var pubKeyValidator = privKey4b.GetPubKeyNoPrefix();

            var hexstr = BitConverter.ToString(pubKeyValidator).Replace("-", "");
            var sd = new BurnValidationReq()
            {
                Address = trReq.Reference,
                Amount = trReq.Amount,
                Cur = trReq.Cur,
                TXID = txid,
                Nonce = ValidatorNonce,
                TargetUserID = trReq.UserID
            };

            //platform  0 BTC, 1 mETH, 2mWBTC, 3 USDC
            //smart contract 0 WBTC, 1 ETH, 2 USDC

            var newcurr = sd.Cur == 1 ? 1 : sd.Cur == 2 ? 0 : sd.Cur == 3 ? 2 : -1;


            var signer = new Nethereum.Signer.EthereumMessageSigner();
            var privKey = new Nethereum.Signer.EthECKey(privkeyValidator);

            var abiEncode = new ABIEncode();

            var power = sd.Cur == 1 ? 15 : sd.Cur == 2 ? 5 : sd.Cur == 3 ? 6 : 0;
            var result2 = abiEncode.GetSha3ABIEncodedPacked(new ABIValue("uint256", (long)(sd.Amount * Math.Pow(10, power))),
              new ABIValue("uint256", sd.Nonce),
              new ABIValue("uint8", newcurr),
                new ABIValue("bytes", (txid).HexToByteArray()),   //new ABIValue("bytes", ("0x" + txid).HexToByteArray()),
               new ABIValue("address", sd.Address));

            var signature = signer.Sign(result2, privKey);


            sd.SignatureValidator = signature;

            var byt = StringX.GetBytesUTF8(signature);

            var amTest = (long)(sd.Amount * Math.Pow(10, power));

            Console.WriteLine(" send Burn Validation " + JsonConvert.SerializeObject(sd));

            var (result, nonce) = SendAuthRequest(RequestTypes.BurnValidation, sd, -1);




            ValidatorNonce++;
            return (result, JsonConvert.SerializeObject(sd));


        }
        #endregion
    }
}
