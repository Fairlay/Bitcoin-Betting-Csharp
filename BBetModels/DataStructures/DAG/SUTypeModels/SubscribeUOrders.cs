using BBetModels.DataStructures.Market;
using BBetModels.DataStructures.Order;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BBetModels.DAG.SUTypeModels
{
    public class SubscribeUOrdersReq : SUData
    {
        public DateTime FromDate = DateTime.MinValue;
        public DateTime ToDate = DateTime.MaxValue;
        public int PageSize = 100;

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public UOrder.Types[] Types;

        public bool SubscribeUpdates;
        public bool AddMarketSummary;
    }
}

