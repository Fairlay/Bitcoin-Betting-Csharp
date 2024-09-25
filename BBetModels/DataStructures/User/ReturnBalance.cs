using System;
using System.Collections.Generic;
using System.Text;

namespace BBetModels.DataStructures.User
{
    public class ReturnBalance
    {
        public double ReservedFunds;
        public double MaxFunds;
        public double UsedFunds;
        public double AvailableFunds;
        public double SettleUsed;
        public double CreatorUsed;
        public double OnHold;
        public double Pending;
        public double Staked;
        public double Collateral;
        public int RemainingRequests;

        public override bool Equals(object obj)
        {
            var other = (ReturnBalance)obj;
            if (other == null)
                return false;

            if (ReservedFunds != other.ReservedFunds)
                return false;

            if (UsedFunds != other.UsedFunds)
                return false;

            if (AvailableFunds != other.AvailableFunds)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
