using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BBetModels.Common
{
    public static class CommonExtensions
    {
        public static string[] MemoryUnits = { "B", "KB", "MB", "GB", "TB", "PT", "EB" };
        public static T FindNode<T>(this IEnumerable<T> nodeList, int nodeId)
            where T : PublicNode
        {
            if (nodeList == null)
                return null;

            foreach (var node in nodeList)
            {
                if (node.ID == nodeId)
                    return node;
            }

            return null;
        }
        public static string ToEmptyString(this object item)
        {
            return item != null ? item.ToString() : string.Empty;
        }
        public static string ToFileSize(this long size, int Digits = 2)
        {
            var index = 0;
            var fsize = (double)size;
            while (fsize > 1023 && index < 6)
            {
                fsize /= 1024;
                index++;
            }

            if (index == 0)
                return String.Format("{0} {1}", size, MemoryUnits[index]);

            var rsize = Math.Round(fsize, Digits).ToString("F" + Digits, CultureInfo.InvariantCulture);
            var result = String.Format("{0} {1}", rsize, MemoryUnits[index]);
            return result;
        }

        #region Flags Setting
        public static bool IsFlag<T>(this T flags, T value)
            where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            var flagValue = Convert.ToInt32(flags);
            var valueValue = Convert.ToInt32(value);
            var result = (flagValue & valueValue) != 0;
            return result;
        }

        public static bool IsNotFlag<T>(this T flags, T value)
            where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            var flagValue = Convert.ToInt32(flags);
            var valueValue = Convert.ToInt32(value);
            var result = (flagValue & valueValue) == 0;
            return result;
        }

        public static T SetFlag<T>(this T flags, T flag, bool value)
            where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            var flagValue = Convert.ToInt32(flags);
            var valueValue = Convert.ToInt32(flag);
            var isFlagSet = (flagValue & valueValue) != 0;

            var result = flags;
            if (value != isFlagSet)
            {
                var factor = value ? +1 : -1;
                var newValue = flagValue + factor * valueValue;
                result = (T)Enum.ToObject(typeof(T), newValue);
            }
            return result;
        }

        public static bool IsFlag(this MemberInfo member, ExcludeTypes flags)
        {
            if (member == null)
                return false;

            var attribute = member.GetCustomAttribute<JsonOptionAttribute>();
            var result = attribute != null && attribute.Excludes.IsFlag(flags);
            return result;
        }

        public static bool IsNotFlag(this MemberInfo member, ExcludeTypes flags)
        {
            if (member == null)
                return true;

            var attribute = member.GetCustomAttribute<JsonOptionAttribute>();
            var result = attribute == null || attribute.Excludes.IsNotFlag(flags);
            return result;
        }
        #endregion

        public static string Substring(this string source, string preChars, string postChars)
        {
            string result = null;
            if (source != null)
            {
                var preCharsLength = preChars != null ? preChars.Length : 0;
                var startIndex = preChars != null ? source.IndexOf(preChars) : 0;
                if (startIndex >= 0)
                {
                    var substring = source.Substring(startIndex + preCharsLength);
                    if (postChars != null)
                    {
                        var endIndex = substring.IndexOf(postChars);
                        if (endIndex >= 0)
                            result = substring.Substring(0, endIndex);
                    }
                    else
                    {
                        result = substring;
                    }
                }
            }
            return result;
        }

        public static string Substring(this string source, string preChars, string postChars, int maxLength)
        {
            string result = null;

            if (source != null)
            {
                var maxCount = Math.Min(maxLength, source.Length);
                var preCharsLength = preChars != null ? preChars.Length : 0;
                var startIndex = preChars != null ? source.IndexOf(preChars, 0, maxCount) : 0;
                if (startIndex >= 0)
                {
                    if (postChars != null)
                    {
                        var endIndex = source.IndexOf(postChars, startIndex + preCharsLength, maxCount - startIndex - preCharsLength);
                        if (endIndex >= 0)
                            result = source.Substring(startIndex + preCharsLength, endIndex - startIndex - preCharsLength);
                    }
                    else
                    {
                        result = source.Substring(0, startIndex);
                    }
                }
            }
            return result;
        }

        public static Nullable<T> TryParseEnum<T>(this string source)
          where T : struct
        {
            var result = default(Nullable<T>);
            if (source == null)
                return result;

            var names = Enum.GetNames(typeof(T));
            if (names.Contains(source))
                result = (T)Enum.Parse(typeof(T), source);

            return result;
        }

        public static T TryParseEnum<T>(this string source, T defaultValue)
        {
            if (source == null)
                return defaultValue;

            T result = defaultValue;
            var names = Enum.GetNames(typeof(T));
            if (names.Contains(source))
                result = (T)Enum.Parse(typeof(T), source);

            return result;
        }
    }
}
