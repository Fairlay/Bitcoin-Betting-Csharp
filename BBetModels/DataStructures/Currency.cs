using System;
using System.Collections.Generic;
using System.Text;

namespace BBetModels.DataStructures
{
    public class Currency
    {
        public int ID;
        public string Name;
        public string Symbol;
        public string ColdWalletAddress;
        public decimal TotalBalance;
        public int Maintainer;
        public bool CrossChainPayments;
        public bool DisallowIssuingByMaintainer;

    }
}
