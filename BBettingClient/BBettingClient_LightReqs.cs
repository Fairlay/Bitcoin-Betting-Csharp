using BBetModels;
using BBetModels.DAG;
using BBetModels.DAG.SUTypeModels;
using BBetModels.DataStructures;
using BBetModels.DataStructures.Market;
using BBetModels.DataStructures.Order;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BBetting
{
    public partial class BBettingClient 
    {

        private bool SendBaseRequest(RequestBas0 su)
        {
            if (mSocketClient != null && mSocketClient.State == WebSocketState.Open)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                var message = su.Serialize(PropertyTypes.Public);

                if (su.Type != RequestTypes.ChangeMarketTimes)
                {
                    
                    Console.WriteLine("[{0}] sending: {1} ", DateTime.UtcNow.ToString(), message);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }


                var bytes = Encoding.UTF8.GetBytes(message);
                var token = new CancellationToken();
                try
                {
                    var task = mSocketClient.SendAsync(bytes, WebSocketMessageType.Text, true, token);
                    bool success = task.Wait(3000, token);
                    if (!success)
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    mSocketClient.Dispose();
                    mSocketClient = new ClientWebSocket();

                    return false;
                }

                var result = !token.IsCancellationRequested;
                return result;
            }


            return false;
        }


        public bool SendRequestSimple(RequestTypes ltype, SUData msg = null, int userid = -1, int? timeout = null)
        {
           
            long nonce = getNonce();

            if (msg != null)
            {
                msg.UserID = userid;
                msg.NodeID = BBetModels.APIv1.S.MAIN_MarketNode;
            }
            var lr = new ReadRequestClient()
            {
                Type = ltype,
                //UserID = userid,
                //NodeID = nodeid,
                RequestTime = DateTime.UtcNow,
                Data = msg,
                Nonce = nonce
            };

            bool result;
            if (timeout.HasValue)
            {
                var resetEvent = new AutoResetEvent(false);
                mWaitForResponse[nonce] = resetEvent;
                result = SendBaseRequest(lr);

                if (timeout.Value == int.MaxValue)
                {
                    resetEvent.WaitOne();
                }
                else
                {
                    resetEvent.WaitOne(timeout.Value);
                }
            }
            else
            {
                result = SendBaseRequest(lr);
            }

            return result;
        }

        public JsonRespClient SendRequestWait(RequestTypes ltype, SUData msg, int userid, int? timeout)
        {
            var nonce = getNonce();
            var nodeid = BBetModels.APIv1.S.MAIN_MarketNode;

            if (msg != null)
            {
                msg.UserID = userid == -1 ? ID : userid;
                msg.NodeID = nodeid;
            }
            var lr = new ReadRequestClient()
            {
                Type = ltype,
                //UserID = userid,
                //NodeID = nodeid,
                RequestTime = DateTime.UtcNow,
                Data = msg,
                Nonce = nonce
            };
            bool result;

            var resetEvent = new AutoResetEvent(false);
            mWaitForResponse[nonce] = resetEvent;
            result = SendBaseRequest(lr);

            if (timeout.Value == int.MaxValue)
            {
                resetEvent.WaitOne();
            }
            else
            {
                resetEvent.WaitOne(timeout.Value);
            }

            if (mResponse.ContainsKey(nonce))
            {

                return mResponse[nonce];
            }
            return null;

        }


        public string GetAccSettings()
        {
            var result = SendRequestWait(RequestTypes.GetAccountSettings, new SUData(), ID, 2000);
            if (result == null) return "";
            else return JsonConvert.SerializeObject(result);
        }
    }
}
