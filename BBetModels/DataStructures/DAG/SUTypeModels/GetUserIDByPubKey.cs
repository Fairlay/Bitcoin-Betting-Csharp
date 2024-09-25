using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBetModels.DAG.SUTypeModels
{
    public class GetUserIDByPubKeyReq : SUData
    {
        public byte[] PublicKey;
    }
}
