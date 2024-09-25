using System;
using System.Collections.Generic;
using System.Text;

namespace BBetModels.DataStructures.User
{

    public partial class CommR
    {
        public double[] PaidInCMonth = new double[13];
        public double PermRed;
        public double Red;

        public int getVIPLevel(double feespaid)
        {
            int red = 0;
            if (feespaid <= 0.05d) return 0;
            else if (feespaid > 50) red = 10;
            else if (feespaid > 30) red = 9;
            else if (feespaid > 20) red = 9;
            else if (feespaid > 15) red = 8;
            else if (feespaid > 10) red = 8;
            else if (feespaid > 5) red = 7;
            else if (feespaid > 2.5d) red = 6;
            else if (feespaid > 1.5d) red = 5;
            else if (feespaid > 1d) red = 5;
            else if (feespaid > 0.5d) red = 4;
            else if (feespaid > 0.25d) red = 3;
            else if (feespaid > 0.15d) red = 2;
            else if (feespaid > 0.1d) red = 2;
            else if (feespaid > 0.05d) red = 1;
            else red = 0;
            return red;

        }
        public double getExcludedRed(DateTime currTime, double feespaid_new)
        {

            double red;

            var feespaid = 0d;

            //switch / case better

            if(currTime > new DateTime(2024,8,1))
            {
                feespaid = feespaid_new;

                if (feespaid <= 0.05d) red = 0d;
                else if (feespaid > 50) red = 0.6d;
                else if (feespaid > 30) red = 0.57d;
                else if (feespaid > 20) red = 0.54d;
                else if (feespaid > 15) red = 0.50d;
                else if (feespaid > 10) red = 0.45d;
                else if (feespaid > 5) red = 0.4d;
                else if (feespaid > 2.5d) red = 0.30d;
                else if (feespaid > 1.5d) red = 0.25d;
                else if (feespaid > 1d) red = 0.2d;
                else if (feespaid > 0.5d) red = 0.15d;
                else if (feespaid > 0.25d) red = 0.1d;
                else if (feespaid > 0.15d) red = 0.075d;
                else if (feespaid > 0.1d) red = 0.05d;
                else if (feespaid > 0.05d) red = 0.02d;
                else red = 0d;

            }
            else
            {
                var currmonth = currTime.Month;
                var currmonthFactor = (double)currTime.Day / (double)DateTime.DaysInMonth(currTime.Year, currTime.Month);


                //PaidInCMonth  are the effective fees paid in each respective month.
                feespaid += 0.2d * (1 - currmonthFactor * 0.5d) * PaidInCMonth[currmonth - 3 < 1 ? currmonth + 9 : currmonth - 3];
                feespaid += 0.3d * (1 - currmonthFactor * 0.5d) * PaidInCMonth[currmonth - 2 < 1 ? currmonth + 10 : currmonth - 2];
                feespaid += 0.5d * (1 - currmonthFactor * 0.5d) * PaidInCMonth[currmonth - 1 < 1 ? currmonth + 11 : currmonth - 1];
                feespaid += 0.5d * PaidInCMonth[currmonth];


                if (feespaid <= 0.02d) red = 0d;
                else if (feespaid > 10) red = 0.9d;
                else if (feespaid > 6) red = 0.85d;
                else if (feespaid > 4) red = 0.80d;
                else if (feespaid > 3) red = 0.75d;
                else if (feespaid > 2) red = 0.7d;
                else if (feespaid > 1) red = 0.6d;
                else if (feespaid > 0.5d) red = 0.5d;
                else if (feespaid > 0.3d) red = 0.40d;
                else if (feespaid > 0.2d) red = 0.3d;
                else if (feespaid > 0.1d) red = 0.25d;
                else if (feespaid > 0.05d) red = 0.2d;
                else if (feespaid > 0.03d) red = 0.15d;
                else if (feespaid > 0.02d) red = 0.1d;
                else if (feespaid > 0.01d) red = 0.05d;
                else red = 0d;

            }


            red += (1 - red) * PermRed;


            return   red > 1 ? 1d :  red;
        }



    }


}
