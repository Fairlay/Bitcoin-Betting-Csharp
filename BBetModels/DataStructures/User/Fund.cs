using BBetModels.DAG.SUTypeModels;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;

namespace BBetModels.DataStructures.User
{

   
    public class Fund
    {
        [JsonOption(PropertyTypes.Private)]
        public decimal Total;

        [JsonOption(PropertyTypes.Private, ExcludeTypes.Hashing)]
        public decimal TransferedP2P;
        [JsonOption(PropertyTypes.Private)]
        public decimal Withdrawn;

        [JsonOption(PropertyTypes.Private)]
        public decimal TransferedSettlement;

        [JsonOption(PropertyTypes.Private)]
        public decimal Exchanged;

        [JsonOption(PropertyTypes.Private)]
        public decimal Used;
        [JsonOption(PropertyTypes.Private)]
        public decimal Pending;
        [JsonOption(PropertyTypes.Private)]
        public decimal Staked;



        [JsonIgnore]
        public List<SettlementChallenge> Challenges = null;

        [JsonIgnore]
        public DateTime[] HoldDA;

       
    }
}
