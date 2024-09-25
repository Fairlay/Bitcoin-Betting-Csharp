using BBetModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBetModels
{
    public class JsonOptionAttribute : Attribute
    {
        public JsonOptionAttribute(PropertyTypes types, ExcludeTypes excludes)
        {
            Types = types;
            Excludes = excludes;
            Order = Int32.MaxValue;
        }

        public JsonOptionAttribute(PropertyTypes types)
        {
            Types = types;
            Excludes = ExcludeTypes.None;
            Order = Int32.MaxValue;
        }

        public JsonOptionAttribute(ExcludeTypes excludes)
        {
            Types = PropertyTypes.Public;
            Excludes = excludes;
            Order = Int32.MaxValue;
        }

        public JsonOptionAttribute(int order)
        {
            Types = PropertyTypes.Public;
            Excludes = ExcludeTypes.None;
            Order = order;
        }

        public JsonOptionAttribute()
        {
            Types = PropertyTypes.Public;
            Excludes = ExcludeTypes.None;
            Order = Int32.MaxValue;
        }

        public PropertyTypes Types { get; set; }

        public ExcludeTypes Excludes { get; set; }

        public int Order { get; set; }
    }

    public class JsonStorageAttribute : Attribute
    {
        public JsonStorageAttribute(string storageName)
        {
            StorageName = storageName;
        }

        public string StorageName { get; set; }
    }

    public class StorageUnitAttribute : Attribute
    {
        public StorageUnitAttribute(Type requestType)
        {
            Type = requestType;
        }

        public Type Type { get; set; }
    }

    public class ResponseTypeAttribute : Attribute
    {
        public ResponseTypeAttribute(Type responseType)
        {
            Type = responseType;
        }

        public Type Type { get; set; }
    }

    public class RequestException : Exception
    {
        public RequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public RequestException(string message)
            : base(message)
        {
        }
    }
    public class RequestWarning : Exception
    {
        public RequestWarning(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public RequestWarning(string message)
            : base(message)
        {
        }
    }

    public struct DataRate
    {
        private long mTotal;
        private double mRate;
        private DateTime mCurrent;

        public long Total
        {
            get { return mTotal; }
        }

        public double Rate
        {
            get
            {
                var current = DateTime.UtcNow;
                var timediff = current - mCurrent;

                if (mRate == Double.NaN || timediff.TotalSeconds > 60)
                    return 0d;

                var quote = timediff.TotalSeconds / 60;
                var crate = mRate * (1 - quote);
                return crate;
            }
        }

        public string RateStr
        {
            get { return String.Format("{0}/s", ((long)Rate).ToFileSize()); }
        }

        public DataRate()
        {
            mCurrent = DateTime.UtcNow;
            mRate = Double.NaN;
            mTotal = 0;
        }

        private void Process(long length)
        {
            var current = DateTime.UtcNow;
            var timediff = current - mCurrent;
            mCurrent = current;
            mTotal += length;

            if (mRate == Double.NaN || timediff.TotalSeconds > 60)
            {
                mRate = length / timediff.TotalSeconds;
            }
            else
            {
                var quote = timediff.TotalSeconds / 60;
                var crate = length / timediff.TotalSeconds;
                mRate = mRate * (1 - quote) + crate * quote;
            }
        }

        public static DataRate operator +(DataRate rate, long length)
        {
            rate.Process(length);
            return rate;
        }
    }
}
