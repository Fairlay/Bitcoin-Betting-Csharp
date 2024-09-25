using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBetModels.DAG.SUTypeModels
{
    public class GetFreeUserIDReq : SUData
    {
    }
    public class GetWholeBalanceReq : SUData
    {
        public   int Cur;
    }
    public class GetCurrenciesReq : SUData
    {
    }
    public class GetBurnValidationsReq : SUData
    {
        public int MaxResults = 100;
    }
    public class GetStatementReq : SUData
    {
        public int MaxResults = 100;
        public int Cur;
        public bool ShowTiny;
        public DAG.SUTypeModels.TransferReq.TransferType TransferTypeFilter;
    }
}
