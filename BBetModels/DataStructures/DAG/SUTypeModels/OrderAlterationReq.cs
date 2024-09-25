using BBetModels.DataStructures.Market;
using BBetModels.DataStructures.Order;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBetModels.DAG.SUTypeModels
{
    public class OrderAlterationReq : SUData
    {

        //Set this to true if you place a lay order on a binary market and you like to have the Amount as Liability of the Order.
        public bool LayAsL;

        //indicates whether the order shall be shared with all other nodes.   Local = true will require a lower Miner Fee
        //to think about, either by default orders are shared among all nodes -->  each user just needs one subscription to a node.
        // or each node hosts his own bunch of markets and each user must subscribe to all nodes he is interested in!
        // By default we share everything in the beginning.
        public bool Local;
 


        [JsonStorage("UO")]
        public UserOrder UserOrder;

        [JsonStorage("O")]
        public UOrder UnmatchedOrder;


        [JsonStorage("Old")]
        public Guid? OldOrderID;

    }




}
