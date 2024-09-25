using BBetModels.DataStructures.Market;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBetModels.DataStructures.Order
{
    public class UserOrder
    {
        [JsonOption(ExcludeTypes.Signature)]
        public int Side;

        [JsonStorage("M")]
        public Guid MarketID;

        [JsonStorage("R")]
        public int RunnerID;

        [JsonStorage("O")]
        [JsonOption(ExcludeTypes.Signature)]
        public Guid? OrderID;

        public bool Insurance;

        
        [ JsonOption(PropertyTypes.Private, ExcludeTypes.Storage | ExcludeTypes.Hashing)]
        public string Note;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Side + " ");
            sb.Append(MarketID + " ");
            sb.Append(RunnerID + "-");

            return sb.ToString();
        }
    }
}
