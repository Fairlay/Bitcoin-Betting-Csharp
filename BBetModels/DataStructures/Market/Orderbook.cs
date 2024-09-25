using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.ComponentModel;

namespace BBetModels.DataStructures.Market
{
    public class OrderBook : OrderBookSimple
    {

        public override bool Equals(object obj)
        {
            var other = obj as OrderBook;
            if (other == null)
                return false;

            var bids = Bids ?? new List<double[]>();
            var asks = Asks ?? new List<double[]>();

            var obids = other.Bids ?? new List<double[]>();
            var oasks = other.Asks ?? new List<double[]>();



            if (bids.Count != obids.Count)
                return false;

            if (asks.Count != oasks.Count)
                return false;

            if (!bids.SequenceEqual(obids))
                return false;

            if (!asks.SequenceEqual(oasks))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public class OrderBookSimple : OrderBookSimplest
    {
        public Guid MarketID;

        [DefaultValue(-1)]
        public int RunnerID;

        public override string ToString()
        {
            return base.ToString();
        }

    }
    public class OrderBookSimplest
    {
        public List<double[]> Bids;
        public List<double[]> Asks;

        [JsonIgnore]
        public bool IsZombie
        {
            get
            {
                return (Bids == null || Bids.Count == 0) && (Asks == null || Asks.Count == 0);
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
