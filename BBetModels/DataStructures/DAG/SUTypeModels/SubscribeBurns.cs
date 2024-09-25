using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BBetModels.DAG.SUTypeModels
{
    public class SubscribeBurnsReq : SUData
    {
        public DateTime FromDate = DateTime.UtcNow.AddDays(-1);
        public DateTime ToDate = DateTime.MaxValue;
        public int PageSize = 100;

    }
    
}

