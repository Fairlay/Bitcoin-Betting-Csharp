using BBetModels.DataStructures;
using BBetModels.DataStructures.Order;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBetModels.DAG.SUTypeModels
{
   

    public class AmendMatchReq : SUData
    {
        public Guid mid;
       
        public int rid;
        public Guid orderid;
        public double am;
        public BBetModels.DataStructures.Order.Match.MOState moState;
        public double Red;
        public MatchedOrderFlag Flag;
    }
}