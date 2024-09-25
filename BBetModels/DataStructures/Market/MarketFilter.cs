using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBetModels.DataStructures.User;
using BBetModels.APIv1;

namespace BBetModels.DataStructures.Market
{
    public class MarketFilter
    {
        public enum MarketFilterSorting
        {
            MIX,   //MIX:    MatchedVolume  + Closing < 1h ? 10 : <3h ? 5: <12h  ? 3 : <24h ? 2 : <48h ? 1 :0       + OpenOrdersVolume >500 ? 10: >300: ; 
            MATCHEDVOLUME,
            CLOSING,
            OPENORDERSVOLUME
        }

        public MarketFilter()
        {
            PageSize = 200;
        }

        public CATEGORY Cat;
        //General searches in Runner, Title, Description and Competition
        public HashSet<string> GeneralAND;
        public HashSet<string> GeneralNOT;
        public HashSet<string> GeneralOR;
        public HashSet<long> evIDs;

        public HashSet<string> RunnerAND;
        public Market.StatusTypes? Status;
        public HashSet<string> TitleAND;
        public string Comp;
        public string Descr;

        public HashSet<Market.MarketTypes> TypeOR;
        public HashSet<Market.MarketPeriods> PeriodOR;
        public HashSet<Market.SettleTypes> SettleOR;
        public bool ToSettle;
        public bool OnlyMyCreatedMarkets;


        public DateTime ChangedAfter;
        public DateTime SoftChangedAfter;
        public bool OnlyActive;
        public decimal MaxMargin;
        public bool NoZombie;

        public DateTime FromClosT;
        public DateTime ToClosT;

        // page rage is given as "1..5 7 9..12"
        public string PageRange;
        public int PageSize;
        public int MaxResults;

        public MarketFilterSorting Sorting;

     
        public static string rtReplace(string name)
        {
            int j = name.IndexOf('(');
            int j2 = name.IndexOf(')');
            int j3 = name.LastIndexOf(')');
            // REMOVE COUNTRY in brackets!
            if (j > 5 && j2 > j && j3 == j2 && j2 >= name.Length - 2)
            {
                var text = name.Substring(j + 1, j2 - j - 1);
                if (text != null && text.Length == 3 && text.ToUpper() == text)
                {
                    name = name.Substring(0, j);
                }
            }
            return name.ToLower().Replace(" (n)", "").Replace(".", "").Replace("'", "").Replace("-", "").Replace("  ", " ").Trim();
        }

    }
}
