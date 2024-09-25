using BBetModels.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBetModels.DAG.SUTypeModels
{
    public class CurrencyIssuanceReq : SUData
    {
        public Currency Currency;
        public DeFiDeposit Deposit;
    }

    public class DeFiDeposit
    {
        public string TXID;
        public int UserID;
        public double Amount;
        //For future open deposits
        public byte[] PubKey;
        public int CurrencyID;
    }
}
