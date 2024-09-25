using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace BBetModels.DataStructures.Order
{

    public class ReturnUOrderDetail : ReturnUOrder
    {
        public bool AddedByWS;
        public bool ConfirmedByWS;
        public bool CancelBySelf;
        public DateTime AddedDate;
        public DateTime LastChange;
    }

    public class ReturnUOrder
    {
        public UserOrder UserOrder;
        public UOrder UnmatchedOrder;

        [JsonOption(ExcludeTypes.Hashing | ExcludeTypes.Signature | ExcludeTypes.Storage)]
        public string MarketSummary;

        public string InfoMsg;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(UserOrder + " ");
            sb.Append(UnmatchedOrder + "");

            return sb.ToString();
        }
    }
}
