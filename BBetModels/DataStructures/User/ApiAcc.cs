using System;
using System.Collections.Generic;
using System.Text;

namespace BBetModels.DataStructures.User
{
    public class ApiAcc
    {
        public int ID;

        public bool CanDoAdminStuff;
        public bool CanDoSpecialRequests;

        [JsonOption(PropertyTypes.Private)]
        public bool ForceSignature;

        [JsonOption(PropertyTypes.Private)]
        public bool ForceNonce;

        [JsonOption(PropertyTypes.Hidden)]
        public long LastNonce;

        public byte[] PrivPublicKey;
        public bool IsETHKey;

        public double DailySpendingLimit;
        public double DailySpent;
        public bool ReadOnly;
        public bool TransferOnly;



    }
}
