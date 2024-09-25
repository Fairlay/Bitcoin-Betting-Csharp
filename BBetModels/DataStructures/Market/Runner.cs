using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace BBetModels.DataStructures.Market
{

    public partial class Runner
    {
        [JsonIgnore]
        public bool IsZombie
        {
            get { return OrdBook == null || OrdBook.IsZombie; }
        }
        [JsonOption(ExcludeTypes.Hashing | ExcludeTypes.Signature)]
        public OrderBookSimple OrdBook;


        //this is just here temporary
        [JsonOption(ExcludeTypes.Hashing | ExcludeTypes.Signature)]
        public OrderBookSimple Orderbook;


        public string Name { get; set; }

        public int mCT { get; set; }

        public double RedA { get; set; }

        public Runner(string name, int mctime)
        {
            mCT = mctime;
            Name = name;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name + " ");

            return sb.ToString();
        }
    }
}
