using BBetModels.DataStructures.Market;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBetModels.DAG.SUTypeModels
{
    public class MarketCreationReq : SUData
    {
        [JsonStorage("M")]
        public Market Market;
        //indicates whether the order shall be kept local.   Local = true will require a lower Miner Fee
        public bool Local;

    }
}
