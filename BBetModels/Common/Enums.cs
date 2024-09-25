using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBetModels.DAG;
using BBetModels.DAG.SUTypeModels;
using BBetModels.DataStructures;
using BBetModels.DataStructures.Market;
using BBetModels.DataStructures.Order;
using BBetModels.DataStructures.User;

namespace BBetModels
{
    public enum RequestStates
    {
        None,
        Success,
        Error,
        Forward
    }

    public enum ClientStates
    {
        None,
        Connecting,
        Open,
        Aborted,
        Closed,
    }

    public enum MessagePriority
    {
        Default,
        High,
    }

    //None  never export
    //Hidden only export for complete state
    // Private   only export for authenticated users
    // Public export for everyone
    //HiddenButPrivate2   Never Export but for StateHash 
    [Flags]
    public enum PropertyTypes
    {
        None = 0x00,

        Public = 0x01,
        Private = 0x02,
        Hidden = 0x04,
        All = 0x07
    }

    //None          no exclusions
    //Hashing        property is excluded from hashing
    //Storage     property is not stored in blockchain
    //Signature   property is ignored creating signature
    [Flags]
    public enum ExcludeTypes
    {
        None = 0x00,

        Hashing = 0x01,
        Storage = 0x02,
        Signature = 0x04,

        All = 0x07,
    }

    public enum SettlementProtocols
    {
        Default,
        Other
    }

    public enum RequestTypes : int
    {
        // Default
        Default = 0,

        // Subscribe & GET Requests
        [ResponseType(typeof(Dictionary<int, ReturnBalance>))]
        SubscribeBalance = -1,

        [StorageUnit(typeof(SubscribeMatchesReq))]
        [ResponseType(typeof(List<ReturnMatch>))]
        SubscribeMatches = -2,

        [StorageUnit(typeof(SubscribeUOrdersReq))]
        [ResponseType(typeof(List<ReturnUOrder>))]
        SubscribeUOrders = -3,

        [StorageUnit(typeof(SubscribeMarketsByFilterReq))]
        [ResponseType(typeof(List<Market>))]
        SubscribeMarketsByFilter = -4,

        [ResponseType(typeof(Dictionary<int, Dictionary<string, int>>))]
        SubscribeCompetitions = -6,

        [ResponseType(typeof(List<string[]>))]
        SubscribeSports = -7,
        SubscribeMaster = -8,

        [ResponseType(typeof(DateTime))]
        ReturnHeartbeat = -10,

        [ResponseType(typeof(OrderBookSimple))]
        ReturnOrderbook = -11,

        ReturnExtendedOrderbook = -12,

        [StorageUnit(typeof(MarketsToSettleReq))]
        [ResponseType(typeof(List<Guid>))]
        MarketsToSettle = -13,

        [StorageUnit(typeof(GetFreeUserIDReq))]
        [ResponseType(typeof(int))]
        GetFreeUserID = -14,

        [StorageUnit(typeof(GetUserIDByPubKeyReq))]
        [ResponseType(typeof(int))]
        GetUserIDFromPubKey = -15,

   

        [ResponseType(typeof(User))]
        GetUser = -18,

        [ResponseType(typeof(Market[]))]
        GetMarkets = -19,

        [StorageUnit(typeof(GetMarketByIdReq))]
        [ResponseType(typeof(Market))]
        GetMarketByID = -20,

        [ResponseType(typeof(PublicNode[]))]
        GetNodes = -21,

   

    

        [StorageUnit(typeof(GetCurrenciesReq))]
        [ResponseType(typeof(Currency[]))]
        GetCurrencies = -23,

        [StorageUnit(typeof(SubscribeBurnsReq))]
        [ResponseType(typeof(ReturnBurn[]))]
        SubscribeBurns = -24,

        [StorageUnit(typeof(GetBurnValidationsReq))]
        [ResponseType(typeof(BurnValidationReq[]))]
        GetBurnValidations = -25,


        [StorageUnit(typeof(SubscribeTransfersReq))]
        [ResponseType(typeof(TransferReq[]))]
        SubscribeTransfers = -26,


        [ResponseType(typeof(uint[]))]
        GetMerkleHash = -27,
        [ResponseType(typeof(PublicNode))]
        GetHeartbeat = -28,

        [StorageUnit(typeof(GetWholeBalanceReq))]
        [ResponseType(typeof(double))]
        GetWholeBalance = -29,



        [ResponseType(typeof(AccountSettingReq))]
        GetAccountSettings = -30,


        [StorageUnit(typeof(GetStatementReq))]
        [ResponseType(typeof(Statement[]))]
        GetStatement = -31,

        [ResponseType(typeof(Dictionary<int, BanNodeReq>))]
        GetBannedNodes = -32,

        // Blockchain
        [StorageUnit(typeof(SUData))]
        Genesis = 1, // Genesis Unit

        //LATER Accounts can only be created by existing Accounts and this shall only be done based on a trust basis
        // If an account is banned in the future also the referring account might get banned as well.
        [StorageUnit(typeof(AccountCreationReq))]
        AccountCreation = 2,

        [StorageUnit(typeof(MarketCreationReq))]
        MarketCreation = 3,

        [StorageUnit(typeof(CurrencyIssuanceReq))]
        CurrencyIssuance = 4,

        [StorageUnit(typeof(TransferReq))]
        Transfer = 5,

        [StorageUnit(typeof(OrderAlterationReq))]
        [ResponseType(typeof(ReturnUOrder))]
        OrderAlteration = 7,

        OrderCancellationConfirmation = 8,

        [StorageUnit(typeof(AmendMatchReq))]
        OrderMatchFinal = 19,

        [StorageUnit(typeof(SettlementReq))]
        [ResponseType(typeof(object))]
        Settlement = 10,

        [StorageUnit(typeof(SettlementChallenge))]
        SettlementChallenge = 11,

        [StorageUnit(typeof(ChangeMarketTimeReq))]
        ChangeMarketTimes = 12,

        ChallengeNodeMatch = 13,
        VoteNodeDown = 14,
        ProposeBan = 21,
        VoteBan = 20,

        [StorageUnit(typeof(AmendMatchReq))]
        [ResponseType(typeof(ReturnMatch))]
        AmendMatch = 15,

        [StorageUnit(typeof(BurnValidationReq))]
        [ResponseType(typeof(BurnValidationReq))]
        BurnValidation = 16,

        MintRequest = 17,

        [StorageUnit(typeof(AccountSettingReq))]
        AccountSetting = 18,


        [StorageUnit(typeof(AccountSettingReq))]
        AccountSettingPriv = 28,

        [StorageUnit(typeof(BanNodeReq))]
        BanNode = 29
    }
}
