using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
namespace BBetModels
{
    public class SchedulerX
    {
        public static Dictionary<string, SchedulerX> _MySchedulers = new Dictionary<string, SchedulerX>();
        public DateTime LastCall;
        public int IntervalMS;
        public string Name;
        public bool StillRunning;
        public SchedulerX(string name, int interval)
        {
            IntervalMS = interval;
            Name = name;
            LastCall = DateTime.UtcNow;

        }
   
      
        public static bool IsDue(string name, int msinterval, bool reqFinish = false)
        {
            try
            {


                if (!_MySchedulers.ContainsKey(name))
                {
                    _MySchedulers[name] = new SchedulerX(name, msinterval);
                    return true;
                }
                var mys = _MySchedulers[name];

                int factor = 1;
                if (reqFinish)
                {
                    if (mys.StillRunning)
                    {
                        factor = 50;
                    }

                }
                if (mys.LastCall < DateTime.UtcNow.AddMilliseconds(-msinterval * factor))
                {
                    mys.LastCall = DateTime.UtcNow;
                    mys.StillRunning = true;
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }
        public static void SetFinish(string name)
        {
            if (!_MySchedulers.ContainsKey(name))
            {
                return;
            }
            var mys = _MySchedulers[name];
            mys.StillRunning = false;

        }
        public static void DoAgainInSec(string name, int msinterval, int sec)
        {
            if (!_MySchedulers.ContainsKey(name))
            {
                return;
            }

            var mys = _MySchedulers[name];

            if (mys.LastCall > DateTime.UtcNow.AddMilliseconds(-msinterval).AddSeconds(sec))
            {
                mys.LastCall = DateTime.UtcNow.AddMilliseconds(-msinterval).AddSeconds(sec);
            }


        }


    }

}
