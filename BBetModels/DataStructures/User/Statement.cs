using System;
using System.Collections.Generic;
using System.Text;

namespace BBetModels.DataStructures.User
{
    public class Statement
    {
        public Statement(DateTime time, double am, double bank, string descr, DAG.SUTypeModels.TransferReq.TransferType ttype, int cur)
        {
            ID = time.Ticks/1000;
            Descr = descr;
            Cur = cur;
            Bank = bank;
            T = ttype;
            Am = am;
        }


        public long ID;
        public string Descr;
        //10=cashouts with withdrawal address!

        public DAG.SUTypeModels.TransferReq.TransferType T;
        public double Am;
        public double Bank;
        public int Cur;

        [JsonOption(PropertyTypes.None)]
        public DateTime ExcludedCreationTime
        {
            get
            {
                return new DateTime(ID*1000);
            }
            set
            {

            }
        }

    }
}
