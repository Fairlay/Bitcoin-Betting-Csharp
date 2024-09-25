using System;
using System.Collections.Generic;
using System.Text;

namespace BBetModels.DAG.SUTypeModels
{
    //anyone can request a SettleChallenge or request an update of the Closing Time before the market is even settled
    public class SettlementChallenge : SUData
    {
        public string TX;
        public int? Rid;
        public int[] VoidRunners;
        public DateTime ClosD;
        public decimal Collateral;
        public string Reason;
        public int Level;
        
    }
}
