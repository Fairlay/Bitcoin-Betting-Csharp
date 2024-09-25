using BBetModels.DAG.SUTypeModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBetModels.DataStructures.User
{
    public class ReturnBurn
    {

        public string Txid;
        public TransferReq Transfer;
   

   
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
