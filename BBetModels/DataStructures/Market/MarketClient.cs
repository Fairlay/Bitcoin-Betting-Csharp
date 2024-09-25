using BBetModels.APIv1;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBetModels.DataStructures.Market
{
    public partial class Market : IComparable<Market>
    {


        public static int getFinalizedH(int creator, CATEGORY cat)
        {

            if (cat == CATEGORY.DICE) return 0;
            return 24;

        }
        //This is only called by the client
        public static Market FillMarket(Guid id, Market market, MarketTypes mtype, MarketPeriods mper, string title, string competition, int creator, string description, CATEGORY category, DateTime closing, DateTime settling, string[] runnerNames, bool inplay = false)
        {
            market.ID = id;
            market.SettleProtocol = SettlementProtocols.Default;
            market.CreationDate = DateTime.UtcNow;
            market.Period = mper;
            market.Type = mtype;
            market.Title = StringX.RemoveDiacritics(title);
            market.Creator = creator;
            market.Settler = APIv1.SC.Settler;
            market.Settler[creator] = true;

            market.ComRecip = new Dictionary<int, double>( APIv1.SC.ComRecip);
            market.ComRecip[creator] = 0.5d;

            market.Descr = StringX.RemoveDiacritics(description);
            market.ClosD = closing;
            market.SettlD = settling;
            market.Cat = category;
            market.Comp = StringX.RemoveDiacritics(competition);
            market.MainNodeID = S.MAIN_MarketNode;

      
            //
            market.SetFin = getFinalizedH(creator, market.Cat);

            int ctime = APIv1.S.MAKERCANCELTIME;

            if (market.Cat == CATEGORY.POLITICS && creator != 2) ctime = 0;
            if (market.Cat == CATEGORY.DICE)
            {
                ctime = 0;
            }
            if (inplay)
            {
                ctime = APIv1.S.MAKERCANCELTIMEINPLAY;
                if (market.Cat == APIv1.CATEGORY.SOCCER) ctime = APIv1.S.MAKERCANCELTIMEINPLAYSOCCER;
                market.Status = StatusTypes.INPLAY;
            }
            else market.Status = Market.StatusTypes.ACTIVE;
            market.Ru = new Runner[runnerNames.Length];
            for (int i = 0; i < runnerNames.Length; i++)
                market.Ru[i] = new Runner(StringX.RemoveDiacritics(runnerNames[i]), ctime);

            return market;
        }

        //This Belongs in the client
        public static Market getNewMarket2(MarketTypes mtype, MarketPeriods mper, double comm, string title, string competition, int creator, string description, CATEGORY category, DateTime closing, DateTime settling, string[] runnerNames, bool inplay = false)
        {
            var market = new Market() { ID = Guid.NewGuid() };

            market.CreationDate = DateTime.UtcNow;
            market.SettleProtocol = SettlementProtocols.Default;

            market.Period = mper;
            market.Type = mtype;
            market.Title = StringX.RemoveDiacritics(title);
            market.Creator = creator;
            market.Settler = APIv1.SC.Settler;
            market.Settler[creator] = true;

            market.ComRecip = new Dictionary<int, double>( APIv1.SC.ComRecip);
            market.ComRecip[creator] = 0.5d;

            market.Descr = StringX.RemoveDiacritics(description);
            market.ClosD = closing;
            market.SettlD = settling;
            market.Cat = category;
            market.Comp = StringX.RemoveDiacritics(competition);
            market.MainNodeID = S.MAIN_MarketNode;

            market.Comm = (decimal) comm;

            //
            market.SetFin = getFinalizedH(creator,category);

            int ctime = APIv1.S.MAKERCANCELTIME;


            if (inplay)
            {
                ctime = APIv1.S.MAKERCANCELTIMEINPLAY;
                if (market.Cat == APIv1.CATEGORY.SOCCER) ctime = APIv1.S.MAKERCANCELTIMEINPLAYSOCCER;
                market.Status = StatusTypes.INPLAY;
            }
            else market.Status = Market.StatusTypes.ACTIVE;
            market.Ru = new Runner[runnerNames.Length];
            for (int i = 0; i < runnerNames.Length; i++)
                market.Ru[i] = new Runner(StringX.RemoveDiacritics(runnerNames[i]), ctime);

            return market;
        }




    }
}
