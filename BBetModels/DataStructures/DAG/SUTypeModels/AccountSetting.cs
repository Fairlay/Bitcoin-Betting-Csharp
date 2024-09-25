using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBetModels.DAG.SUTypeModels
{
    public class AccountSettingReq : SUData
    {
        public int AccountID;
        public bool? ForceConfirmMatched;
        public bool? DisableOnHold;
        public long? AbsenceCancelMS;
        public int? ReplacementNodeID;
        public double? Longitude;
        public double? Latitude;

        public DateTime? SelfExclude;

        public string UDP_IP;
        public int UDP_Port;
        public double CommPaid;
        public double CommReduction;
        public double ReferralBonus;
        public int ReferralLevel;
        public int VIPLevel;
    }
}