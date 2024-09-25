
using BBetModels.APIv1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BBetModels.DataStructures.Order
{
    public class UOrder : IComparable<UOrder>
    {
        public enum UOStates
        {
            Active,
            Cancelled,
            Matched,
            PartiallyMatchedAndCancelled,
            PartiallyMatched

        }

        public enum Types
        {
            Default,
            PostOnly,
            KillOrFill,
            Same
        }

        [JsonStorage("Pri")]
        public double Price { get; set; }

        //Remaining Amount. Set to same value as PrivAmount

        public Types Type { get; set; }

        [JsonOption(PropertyTypes.Hidden)]
        public long LastRealChange { get; set; }

        [JsonStorage("St")]
        public UOStates State { get; set; }

        public int Side { get; set; }

        [JsonStorage("Am")]
        public decimal Amount { get; set; }

        [JsonStorage("Cur")]
        public int Cur { get; set; }

        [JsonStorage("AmD")]
        public Dictionary<int, decimal> AmountD { get; set; }

        [JsonStorage("RemAm")]
        public decimal RemAmount { get; set; }

        [JsonStorage("RemAmD")]
        public Dictionary<int, decimal> RemAmountD { get; set; }

        /// Order ID.  Set Oid to 0 if you like to create a new order
        public Guid ID { get; set; }

        [JsonOption(ExcludeTypes.Signature)]
        public DateTime CreationDate { get; set; }

        public string Note { get; set; }

        public DateTime CancelAt { get; set; }

        [JsonStorage("mCT")]
        //This is the Maker cancel time.  How much time you have as maker of the bet to void the bet after the intial matching. 
        public int makerCT { get; set; }

        [JsonStorage("UID")]
        [JsonOption(ExcludeTypes.Signature)]
        public int UserID { get; set; }

        public UOrder()
        {
        }

        public UOrder(bool adv, UOrder umo)
        {
            ID = umo.ID;
            UserID = umo.UserID;
            Side = umo.Side;
            Price = umo.Price;
            Amount = umo.Amount;
            RemAmount = umo.Amount;

            Type = umo.Type;
            Note = umo.Note;
            makerCT = umo.makerCT;
            Cur = umo.Cur;
            State = UOStates.Active;

            if (adv)
            {
                AmountD = new Dictionary<int, decimal>(umo.AmountD);
                RemAmountD = new Dictionary<int, decimal>(AmountD);
            }
        }

        public UOrder(Guid id, bool layliability, int uid, int bidorask, double price, decimal amount, Types typ, string note, int mCTime)
        {
            this.ID = id;
            UserID = uid;

            Side = bidorask;
            Price = price;
            Amount = amount;
            if (layliability) Amount = Math.Round(amount / ((decimal)price - 1m), 10);
            RemAmount = amount;
            Note = note;

            makerCT = mCTime;
            Type = typ;

            State = UOStates.Active;
        }

        public UOrder(bool adv, decimal maxAm, Guid id, int uid, int bidorask, double price, decimal amount, Types typ, string note, int mCTime)
        {
            this.ID = id;
            UserID = uid;

            Side = bidorask;
            Price = price;
            AmountD = new Dictionary<int, decimal>();
            AmountD[0] = amount;
            AmountD[1] = amount  >= maxAm? maxAm*20 : amount * 20m;
            AmountD[2] = amount >= maxAm ? maxAm : amount * 1m;
            //AmountD[2] = maxAm;
            if (bidorask == 0 && amount > 0 && price * (double)amount > (double)maxAm)
            {
                AmountD[1] = Math.Round(maxAm * 20 / (decimal)price);
                AmountD[2] = Math.Floor(maxAm  / (decimal)price);
                //            if (price * (double) maxAm > 50) AmountD[2] = Math.Floor(maxAm * 50 / (decimal)price) * 0.1m;
            }


            RemAmountD = new Dictionary<int, decimal>(AmountD);
            Note = note;

            makerCT = mCTime;
            Type = typ;

            State = UOStates.Active;
        }

        public decimal getAmount(int cur = -1)
        {
            if (AmountD != null)
            {
                if (cur == -1) return AmountD[Cur];
                return AmountD[cur];

            }
            return Amount;
        }

        public decimal getRemAmount(int cur = -1)
        {
            if (AmountD != null)
            {
                if (cur == -1) return RemAmountD[Cur];
                return RemAmountD[cur];

            }
            return RemAmount;
        }

        public bool IsAdvanced()
        {
            if (AmountD != null || RemAmountD != null)
            {
                return true;

            }
            return false;
        }

        public void setRemAmount(int cur, decimal val)
        {
            if (RemAmountD != null)
            {
                RemAmountD[cur] = val;

            }
            else RemAmount = val;
        }

        public int CompareTo(UOrder other)
        {

            if (this == null) return 1;
            if (other == null) return -1;

            if (this.Price != other.Price)
            {
                if (Side == 1)
                {
                    return this.Price.CompareTo(other.Price);
                }
                else
                {
                    return other.Price.CompareTo(this.Price);
                }
            }
            else return this.CreationDate.CompareTo(other.CreationDate);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Side + " ");
            sb.Append(Price.ToString(S.DefaultCulture) + " ");
            sb.Append(getRemAmount().ToString(S.DefaultCulture) + " ");
            sb.Append(Note + " ");

            return sb.ToString();
        }
    }
}