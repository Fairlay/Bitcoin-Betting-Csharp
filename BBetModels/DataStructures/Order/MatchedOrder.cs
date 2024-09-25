using BBetModels.APIv1;
using System;
using System.Text;

namespace BBetModels.DataStructures.Order
{

    public enum MatchedOrderFlag
    {
        Default,
        AmountTooBig,
        PriceTooBig,
        LiveDanger,
        Rejected,
        Internal1,
        Internal2,
        Internal3,
        Settle,
        NoSettle,
        Reversed,
        ReverseSettled
    }

    public partial class Match :  IComparable<Match>
    {
        public enum MOState
        {
            DEFAULT,
            MATCHED,
            RUNNERWON,
            RUNNERHALFWON,
            RUNNERLOST,
            RUNNERHALFLOST,
            MAKERVOIDED,
            VOIDED,
            PENDING,
            DECIMALRESULT,
            DECIMALRESULTTOBASE,
            SETTLED

        }

        [JsonOption(PropertyTypes.Private)]
        public bool[] IsMaker;

        [JsonOption(PropertyTypes.Private)]
        public int[] UserID;
        [JsonOption(PropertyTypes.Private)]
        public Guid[] UMOrderID;


        public double DecResult { get; set; }
        public MatchedOrderFlag R { get; set; }
        public Guid ID { get; set; }


        [JsonStorage("CrD")]
        public DateTime CreationDate { get; set; }



        [JsonStorage("St")]
        public MOState State;



        [JsonStorage("Pri")]
        public double Price { get; set; }

        [JsonStorage("Am")]
        public decimal Amount { get; set; }
        public double Red { get; set; }

        [JsonStorage("Cur")]
        public int Cur { get; set; }



        [JsonOption(PropertyTypes.Private)]
        public int MakerCancelTime;

        [JsonOption(PropertyTypes.Hidden, ExcludeTypes.Hashing | ExcludeTypes.Signature | ExcludeTypes.Storage)]
    

        public Match()
        {

        }


        [JsonOption(PropertyTypes.None)]
        public decimal ExcludedWinnings
        {
            get { return ((decimal)Price - 1m) * Amount; }
        }

        public int CompareTo(Match other)
        {
            return this.ID.CompareTo(other.ID);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Price.ToString(S.DefaultCulture) + " ");
            sb.Append(Amount.ToString(S.DefaultCulture) + " ");
            sb.Append(CreationDate + " ");
            sb.Append(State + " ");

            return sb.ToString();
        }


    }
}
