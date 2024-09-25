using BBetModels.APIv1;
using BBetModels.DataStructures.Order;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBetModels.DataStructures.User
{
    public partial class User
    {
        public int ID;

        [JsonOption(PropertyTypes.Private, ExcludeTypes.Hashing | ExcludeTypes.Signature)]
        public int Requests;


        [JsonOption(PropertyTypes.Hidden)]
        public List<UserOrder> MOrders;
        [JsonOption(PropertyTypes.Hidden)]
        public List<UserOrder> MOrdersFinalized;

        public string Name;

        public long[] SettleDel;

        [JsonOption(PropertyTypes.Private)]
        public bool NoOnHold;
        [JsonOption(PropertyTypes.Private)]
        public int ReplacementNodeID;
        [JsonOption(PropertyTypes.Private)]
        public double Longitude;
        [JsonOption(PropertyTypes.Private)]
        public double Latitude;

        public Dictionary<int, ApiAcc> SubUsers = new Dictionary<int, ApiAcc>();


        [JsonOption(PropertyTypes.Private)]
        public bool ForceConfirmMatched;

        [JsonOption(PropertyTypes.Private)]
        public long AbsenceCancelMS = 0;

        [JsonOption(PropertyTypes.Private)]
        public string CBIP;


       


        public string Screenname = "Unknown";
        public string ExcludedPOR;
        public DateTime AccountCreation;


        [JsonOption(PropertyTypes.Hidden)]
        public CommR HiddenCommR;



        public User(int id)
        {
            ID = id;
        }


       
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ID + " tot:");



            return sb.ToString();
        }
    }
}
