﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BBetModels.DataStructures.Market
{
    public partial class Market : IComparable<Market>
    {
        public enum SettleTypes
        {
            BINARY,
            CFD,
            CFDINVERSE,
            EXCHANGETOCUR

        }

        public enum MarketTypes
        {
            M_ODDS,
            OVER_UNDER,
            OUTRIGHT,
            GAMESPREAD,
            SETSPREAD,
            CORRECT_SCORE,
            FUTURE,
            BASICPREDICTION,
            RESERVED2,
            RESERVED3,
            RESERVED4,
            RESERVED5,
            RESERVED6
        }

        public enum MarketPeriods
        {
            UNDEFINED,
            FT,             //All
            FIRST_SET,     //Tennis, Badminton , Volleyball  , Tabletennis? 
            SECOND_SET,
            THIRD_SET,
            FOURTH_SET,
            FIFTH_SET,
            FIRST_HALF,   //Soccer, Rugby Union, Basketball, Baseball, Handball
            SECOND_HALF,
            FIRST_QUARTER,  //Basketball
            SECOND_QUARTER,
            THIRD_QUARTER,
            FOURTH_QUARTER,
            FIRST_PERIOD,  //Hockey only
            SECOND_PERIOD,
            THIRD_PERIOD,
            FIRST_INNING,   //Baseball only
            SECOND_INNING,
            THIRD_INNING,
            FOURTH_INNING,
            FIFTH_INNING,
            SIXTH_INNING,
            SEVENTH_INNING,
            EIGHTH_INNING,
            NINTH_INNING,
            FIRST_MAP,   //Esports only
            SECOND_MAP,
            THIRD_MAP,
            FOURTH_MAP,
            FIFTH_MAP,
            SIXTH_MAP,
            SEVENTH_MAP,
            EIGHTH_MAP,
            NINTH_MAP,
            FT_WITH_OVERTIME,  //Soccer, Hockey 
            FIRST_OVERTIME,  //SOCCER 
            SECOND_OVERTIME,//SOCCER 
            PENALTY ,
            REGULATION_TIME, //HOCKEY
            TENNIS_GAME,
          


        }

        public enum StatusTypes
        {
            ACTIVE,
            INPLAY,
            SUSPENDED,
            CLOSED,
            SETTLED,
            FINALIZED,
            CANCELLED
        }
    }
}
