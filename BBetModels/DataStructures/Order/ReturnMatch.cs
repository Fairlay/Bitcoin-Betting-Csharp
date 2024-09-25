using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace BBetModels.DataStructures.Order
{
    public class ReturnMatch
    {
        public UserOrder UserOrder;
        public Match MatchedOrder;
        public Guid? UserUMOrderID;

        [JsonOption(ExcludeTypes.Hashing | ExcludeTypes.Signature | ExcludeTypes.Storage)]
        public string MarketSummary;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(UserOrder + " ");
            sb.Append(MatchedOrder + "");

            return sb.ToString();
        }
    }
}
