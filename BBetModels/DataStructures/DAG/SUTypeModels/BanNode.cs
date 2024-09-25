using System;
using System.Collections.Generic;
using System.Text;

namespace BBetModels.DAG.SUTypeModels
{
    public class BanNodeReq : SUData
    {
        public int IDToBan;
        public DateTime BanTill;
        public DateTime BanFrom;
        public bool UnBan;
    }
}