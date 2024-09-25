using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace BBetModels.DAG
{
    /// <summary>
    /// This is the data each User signs.  It can be MarketCreation, OrderCreation,...
    /// It will be put into a StorageUnit(Pub)  together with the Users signature.
    /// Additional data fields of the StorageUnit will be filled by the Miner.
    /// </summary>
    public class SUData
    {
        public int UserID;
        public int ApiID; //default 0

        //Miner Fee can only be deducted if the SU makes it into the DAG.    To prevent spam, a Node will require a minimum fee.
        //If some user is spamming orders or markets, he can include these orders anyway
        public double MinerFee;
        public string MinerFeeStr;
        public int NodeID;
        
        public DateTime CreatedByUser;
       
        /// <summary>
        /// Any User can force a reference TXID, so that this transaction is only mined when the referenced TX was mined before!
        /// </summary>
        public string ReferenceSUHash;

        public double getMinerFee()
        {
            if (MinerFee > 0) return MinerFee;
            if(String.IsNullOrEmpty(MinerFeeStr)) return 0;
            Double.TryParse(MinerFeeStr, System.Globalization.CultureInfo.InvariantCulture, out var minerfee);
            
            return minerfee;

        }
    }
}
