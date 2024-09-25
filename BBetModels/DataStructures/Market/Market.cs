using BBetModels.APIv1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BBetModels.DataStructures.Market
{
    public partial class Market : IComparable<Market>
    {
        [JsonIgnore]
        [JsonOption(PropertyTypes.Hidden)]
        public bool NeedsClosingTimeUpdate;
        


        public static string makeOrderBStr(OrderBook[] obs)
        {
            string ob = "";

            for (int i = 0; i < obs.Length; i++)
            {
                if (obs[i] != null && (obs.Length != 2 || (obs[i].Asks != null && obs[i].Asks.Count > 0) || (obs[i].Bids != null && obs[i].Bids.Count > 0))) ob += JsonConvert.SerializeObject(obs[i]) + "~";
                else ob += "~";
            }
            if (ob.Length > 1) ob = ob.Remove(ob.Length - 1);

            return ob;
            // 
        }
        [JsonIgnore]
        [JsonOption(PropertyTypes.Hidden)]
        public string Ru0Bid
        {
            get
            {
                if (Ru[0].OrdBook == null || Ru[0].OrdBook.Bids == null || Ru[0].OrdBook.Bids.Count == 0) return "-";
                var firstord = Ru[0].OrdBook.Bids.First();

                return firstord[0] + " (" + firstord[1] + ")";
            }
        }
        [JsonIgnore]
        [JsonOption(PropertyTypes.Hidden)]
        public string Ru1Bid
        {
            get
            {
                if (Ru[1].OrdBook == null || Ru[1].OrdBook.Bids == null || Ru[1].OrdBook.Bids.Count == 0) return "-";
                var firstord = Ru[1].OrdBook.Bids.First();

                return firstord[0] + " (" + firstord[1] + ")";
            }
        }
        [JsonIgnore]
        [JsonOption(PropertyTypes.Hidden)]
        public string Ru0Ask
        {
            get
            {
                if (Ru[0].OrdBook == null || Ru[0].OrdBook.Asks == null || Ru[0].OrdBook.Asks.Count == 0) return "-";
                var firstord = Ru[0].OrdBook.Asks.First();

                return firstord[0] + " (" + firstord[1] + ")";
            }
        }


        [JsonIgnore]
        [JsonOption(PropertyTypes.Hidden)]
        public string Ru0Name
        {
            get
            {
                return Ru[0].Name;
            }
        }


        [JsonIgnore]
        [JsonOption(PropertyTypes.Hidden)]
        public string Ru1Name
        {
            get
            {
                return Ru[1].Name;
            }
        }


        [JsonIgnore]
        [JsonOption(PropertyTypes.Hidden)]
        public string ID2
        {
            get
            {
                return ID.ToString();
            }
        }

        [JsonIgnore]
        [JsonOption(PropertyTypes.None)]
        public string OrdBStr
        {
            get
            {
                var str = "";
                foreach (var el in Ru)
                {
                    if (el.OrdBook == null) str += " ^ ";
                    else str += el.OrdBook.ToString() + " ^ ";
                }
                return str.TrimEnd(' ', '^');
            }
            set
            {

            }
        }

        [JsonIgnore]
        [JsonOption(PropertyTypes.None)]
        public bool IsZombie
        {
            get
            {
                foreach(var el in Ru)
                {
                    if (!el.IsZombie) return false;
                }

                return true;
            }
        }

        /// <summary>
        ///   The MainNode should be the one, that each one is sending his Orders too. 
        ///  It has Prescedence in processing orders  if the timeline is somehow uncertain
        /// The MainNode will have all unmatched Orders and the market in the mempool only.
        /// And only it's backup nodes will query these Orders. Only after a market is settled, It shall be referenced by other SU  and become part of the 
        /// "MainChain" of the DAG.
        ///  If a MainNode abuses it's privlieges (front running orders)  Users will just stop using that Node and create a market with a different MainNode.
        /// </summary>
        /// 


        [JsonStorage("NID")] 
        public int MainNodeID { get; set; }



        public Guid ID;
        public string Comp { get; set; }


      


        [JsonStorage("CrD")]
        public DateTime CreationDate
        {
            get; set;
        }


        [JsonStorage("D")]
        public string Descr { get; set; }


        //Runners  +  Spread Home V / Total V

        [JsonStorage("T")]
        public string Title { get; set; }
        public CATEGORY Cat { get; set; }


        [JsonOption(ExcludeTypes.Hashing)]
        [JsonStorage("ClD")]
        public DateTime ClosD { get; set; }

        [JsonOption(ExcludeTypes.Hashing)]
        [JsonStorage("SeD")]
        public DateTime SettlD { get; set; }

        //Has influence on how often Simulate Matching must be called


        [JsonStorage("St")]
        public StatusTypes Status { get; set; }


        [JsonStorage("SeP")]
        public SettlementProtocols SettleProtocol { get; set; }
        public Runner[] Ru { get; set; }

        public MarketTypes Type { get; set; }

        [JsonStorage("Pe")]
        public MarketPeriods Period { get; set; }

        [JsonStorage("SeT")]
        public SettleTypes SettlT { get; set; }

        public decimal Comm { get; set; }

        [JsonStorage("Cr")]
        public long Creator { get; set; }

        [JsonStorage("Set")]
        public Dictionary<int, bool> Settler { get; set; }

        [JsonStorage("CR")]
        public Dictionary<int, double> ComRecip { get; set; }

        public double MinVal { get; set; }
        public double MaxVal { get; set; }

        /// <summary>
        /// 1st base Currency
        /// </summary>
        public int Cur { get; set; }

        /// <summary>
        /// 2nd currency / Target Currency. In case of  mBTC /mETH     this is mETH!
        /// </summary>
        /// 
        public int CurB { get; set; }


        /// <summary>
        /// determines after how many hours the market settlement should be considered finalized.
        /// </summary>
        public int SetFin { get; set; }

        /// <summary>
        /// 1 :  only creator may post maker orders;
        /// </summary>
        public int Flag { get; set; }



        [JsonStorage("evID")]
        public long evID { get; set; }

      
        public Market(DateTime date)
        {

        }
        public Market()
        {

        }



        public int getMaintainerID()
        {

            if (MainNodeID == 1) return 104;
            return MainNodeID;
        }



        public int CompareTo(Market other)
        {
            return this.ID.CompareTo(other.ID);

        }


        public static int CompareClosingTime(Market a, Market b)
        {
            if (a.ClosD == b.ClosD)
            {

                return a.CompareTo(b);

            }

            return a.ClosD.CompareTo(b.ClosD);

        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Comp + " ");
            sb.Append(Title + " ");
            sb.Append(Period + " ");
            sb.Append(Type + " ");
            sb.Append(Ru[0].Name + " ");
            sb.Append(ClosD + " ");
            sb.Append(OrdBStr + " ");

            return sb.ToString();
        }

        public string ToString(int rid)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(Comp + " ");
            sb.Append(Title + " ");
            sb.Append(Period + " ");
            sb.Append(Type + " ");
            sb.Append(Ru[rid].Name + " ");
            sb.Append(ClosD + " ");
            sb.Append(OrdBStr + " ");

            return sb.ToString();


        }
       
        public string ToStringRunner()
        {

            StringBuilder sb = new StringBuilder();
            foreach (var ru in Ru)
            {
                sb.Append(ru.ToString() + " ");

            }

            return sb.ToString();


        }


    }
}
