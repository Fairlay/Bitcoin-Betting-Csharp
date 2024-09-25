using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBetting
{
    public sealed class PerformanceRes
    {
         static PerformanceResults mPerformanceResults = new PerformanceResults();
        public struct PerformanceResults
        {
            public long counter;
            public long? MaxDelay;
            public long? MinDelay;
            public double? AverageDelay;
        }

        public static string AnalyseReceivedInternal(long sentTimestamp, long receivedTimestamp)
        {
            long delay = receivedTimestamp - sentTimestamp;

            mPerformanceResults.MinDelay = (mPerformanceResults.MinDelay == null) ? delay : mPerformanceResults.MinDelay;
            if (delay < mPerformanceResults.MinDelay)
                mPerformanceResults.MinDelay = delay;

            mPerformanceResults.MaxDelay = (mPerformanceResults.MaxDelay == null) ? delay : mPerformanceResults.MaxDelay;
            if (delay > mPerformanceResults.MaxDelay)
                mPerformanceResults.MaxDelay = delay;

            mPerformanceResults.counter++;
            mPerformanceResults.AverageDelay = (mPerformanceResults.AverageDelay == null) ? delay : mPerformanceResults.AverageDelay;
            mPerformanceResults.AverageDelay = (mPerformanceResults.AverageDelay * (mPerformanceResults.counter - 1) + delay) / (mPerformanceResults.counter);

            return ("Average delay (in ticks /10): " + mPerformanceResults.AverageDelay);
          
        }
    }
}
