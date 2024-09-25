using BBetModels.DataStructures.Market;
using BBetModels.DataStructures.Order;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BBetModels.DAG.SUTypeModels
{

    /// <summary>
    /// OrderMatches are only created by the miner node.  THe node must reference both unmatched ordres as ReferenceSU. After a match all referenced SUs and their parents, and parents parents and so on have to be moved from Mempool to Fixed.
    /// We cannot prevent front running. There is also no guarantee a node matches the order always with the best available order. 
    /// If front running on a node appears, the community must just create a market on a different node with higher reputation.
    /// 
    /// Maybe matching orders with not the top order can be prevented when the miner always signs received unmatched orders. The user can than prove that the mining node is malicious. 
    /// Front running can also be prevented maybe, because the Miner can not create transactions in the aftermath because of ProofOfHistory. 
    /// </summary>
    public class SubscribeMarketsByFilterReq : SUData
    {
        [JsonPropertyName("mF")]
        public MarketFilter MarketFilter;

        public bool SubscribeOrderbooks;
    }



}

