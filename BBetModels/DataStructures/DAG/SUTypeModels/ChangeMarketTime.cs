using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBetModels.DAG.SUTypeModels
{
    public class ChangeMarketTimeReq : SUData
    {
        public Guid Mid;
        public DateTime? ClosD;
        public DateTime? SetlD;
    }
}
