using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBetModels.DAG.SUTypeModels
{
    public class MarketsToSettleReq : SUData
    {


        public MarketsToSettleReq()
        {
            CreationTime = DateTime.UtcNow;
        }



        public DateTime CreationTime
        {
            get; set;
        }
    }
    public class MarketsToSettleReturn : SUData
    {
        public Guid[] MarketIDsToSettle;

        public MarketsToSettleReturn()
        {
            CreationTime = DateTime.UtcNow;
        }



        public DateTime CreationTime
        {
            get; set;
        }
    }
}
