using System;
using System.Collections.Generic;
using System.Text;

namespace BBetModels.DAG.SUTypeModels
{
    public class AccountCreationReq : SUData
    {
        public enum CREATIONMODE
        {
            DIRECT,
            ISSUEINVITE,
            REDEEMINVITE,
            DELETEINVITE,
            REVOKEINVITE
        }

        public CREATIONMODE CreationMode;
        public string InviteCode;
        public string InviteCodeHash;
        public int NewAccountID;
        public byte[] PubKey;
        public bool IsETH;
        public bool BanSub;

        public decimal StartingBalance;
    }


    public class SubAccountCreation : SUData
    {
        public DataStructures.User.ApiAcc SubUser;
        public int Id;


    }
}
