using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BBetModels.DAG.SUTypeModels
{
    public class TransferReq : SUData
    {
        public enum TransferType
        {
            P2P,
            CommPaid,
            FeeReceived,
            FeeReversed,
            Settlement,
            SettlementReversed,
            Exchange,
            Withdrawal,
            Deposit,
            CurrencyIssuing,
            Burn,
            DirectDebit,
            Penalty,
            Staking,
            ReferralReceived,
            ReferralReversed,

            MinerFeePaid,
            MinerFeeRec,

        }
        public enum ChainType
        {
            ETH,
            FAIRLAY

        }
        public TransferReq()
        {
        }
        public TransferReq(DateTime time, int from, int to, string descr, TransferType trType, double am, int cur)
        {
            ID = Guid.NewGuid();
            From = from;
            To = to;
            Reference = descr;
            TType = trType;
            Amount = am;
            Cur = cur;
        }

        public int Cur;
        public Guid ID;
        public int From;
        public bool IsStaking
        {
            get
            {
                return TType == TransferType.Staking;
            }
        }
        public int To;
        public string Reference;
        //10=cashouts with withdrawal address!

        public TransferType TType;
        public double Amount;
        public ChainType Chain;
        public bool Burn
        {
            get
            {
                return TType == TransferType.Burn;
            }
        }

        //[JsonOption(PropertyTypes.None, ExcludeTypes.Hashing | ExcludeTypes.Signature)]     
        public DateTime CreationTime
        {
            get; set;
        }
        [JsonOption(PropertyTypes.None, ExcludeTypes.All)]
        public DateTime Created;
    }

    public class BurnValidationReq : SUData
    {
        //A Validator will validate any burn request when the user signature is valid, the blockchain is valid and was not altered and when a 2/3 majority  of staking Nodes have referenced the Transaction
        public BurnValidationReq()
        {

        }
        public BurnValidationReq(DateTime time, string add, double am, int cur)
        {

            CreationTime = DateTime.UtcNow;

            Address = add;
            Amount = am;
            Cur = cur;
        }

        public string TXID;
        //0 == mBTC / 1=METH / 2=USDC
        public int Cur;

        public double Amount;
        public long Nonce;
        public string Address;

        
        public string SignatureValidator;
        public int TargetUserID;

        [JsonOption(PropertyTypes.None, ExcludeTypes.Hashing | ExcludeTypes.Signature)]
        public DateTime CreationTime
        {
            get; set;
        }
    }
}
