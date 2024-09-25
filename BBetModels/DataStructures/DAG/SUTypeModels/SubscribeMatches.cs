
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BBetModels.DAG.SUTypeModels
{
    public class SubscribeMatchesReq : SUData
    {
        public DateTime FromDate = DateTime.MinValue;
        public DateTime ToDate = DateTime.MaxValue;
        public int PageSize = 100;

        public bool SubscribeUpdates;
        public bool AddMarketSummary;
    }
}

