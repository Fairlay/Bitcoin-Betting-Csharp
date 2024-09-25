using BBetModels.DataStructures.Order;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBetModels
{
    // a u
    class Account
    {
        

   
        public class ReturnMOrder
        {
            public UserOrder UserOrder;
            public Match MatchedOrder;
            public long UserUMOrderID;

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(UserOrder + " ");
                sb.Append(MatchedOrder + "");

                return sb.ToString();
            }
        }

        public class ReturnUOrder
        {
            public UserOrder UserOrder;
            public UOrder UnmatchedOrder;

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(UserOrder + " ");
                sb.Append(UnmatchedOrder + "");

                return sb.ToString();
            }

        }





       

  
    }
}
