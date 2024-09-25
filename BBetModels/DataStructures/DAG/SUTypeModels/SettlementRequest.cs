using System;
using System.Collections.Generic;
using System.Text;

namespace BBetModels.DAG.SUTypeModels
{
    public class SettlementReq : SUData
    {
        public Guid Mid;
        public bool Unsettle;
        public int Runner;
        public int Win;
        public bool Half;
        public double Dec;
        public double ORed;
        public int[] VoidRunners;
        public DateTime ClosD;
        public Guid? MO;

        public int Level;



        public override string ToString()
        {
            return Mid + Unsettle.ToString() + Runner.ToString() + Win.ToString() + Half.ToString() + Dec.ToString() + ORed.ToString() + MO.ToString();
        }

    }

}
