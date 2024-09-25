using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBetModels
{
    public class Config
    {

        public byte[] PrivKey
        {
            get
            {
                return StringX.GetBytesHex(PrivateKey);
            }
        }
        public int UserID { get; set; }
        public string PrivateKey { get; set; }
        public string PublicAddress { get; set; }
        public string ApiKeyEtherscan { get; set; }
        public string SmartContractAddress { get; set; }

        public string CertificateFile { get; set; }
        public string CertificatePassword { get; set; }

        public string[] InitAddresses { get; set; }
        public int[] InitUserID { get; set; }

        public int ReplacementNodeId { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public int LogALLTXLastXDays { get; set; }

        public bool AutoConnectNodes { get; set; }
        public string ExplorerDomain { get; set; }
        public int  MainNodeId { get; set; }
        public bool DoNotPushNonSelfmined { get; set; }
        public bool RejectFromUnknownNode { get; set; }
    }


    public class PublicNode
    {
        public int ID;
        public string URI;
        public string Name;
        public string Version;
        public byte[] PublicKey;

        public DateTime StartTime;
        public DateTime CurrentTime;

        public double Longitude;
        public double Latitude;

        public int ReplacementNodeId;
    }
}
