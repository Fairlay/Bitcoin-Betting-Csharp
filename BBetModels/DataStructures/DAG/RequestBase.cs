using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text;
using BBetModels.Common;
using BBetModels.DAG;
using BBetModels.DAG.SUTypeModels;
using BBetModels.DataStructures.Market;
using BBetModels.DataStructures.Order;
using BBetModels.DataStructures.User;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using RestSharp.Extensions;

namespace BBetModels
{
    // <Exec Command="del &quot;GitCommit.txt&quot;&#xD;&#xA;git log -1 --oneline&#xD;&#xA;git log -1 --oneline &gt;&gt; &quot;GitCommit.txt&quot;" />-->
    public partial class RequestBas0
    {
        private class RequestContractResolver : DefaultContractResolver
        {
            private bool _Storage;
            private Type _DataType;
            private PropertyTypes _Types;
            private Predicate<MemberInfo> _Filter;
            private JsonSerializerSettings _Settings;
            private Func<MemberInfo, IComparable> _Sorter;

            public RequestContractResolver(JsonSerializerSettings settings, PropertyTypes types, Type dataType, bool storage)
                : base()
            {
                _Types = types;
                _Storage = storage;
                _DataType = dataType;
                _Settings = settings;
                _Sorter = GetPropertyOrder;
            }

            public RequestContractResolver(JsonSerializerSettings settings, PropertyTypes types, Type dataType, Predicate<MemberInfo> filter, Func<MemberInfo, IComparable> sorter)
                : base()
            {
                _Types = types;
                _Filter = filter;
                _Sorter = sorter;
                _Settings = settings;
                _DataType = dataType;
            }

            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Cast<MemberInfo>();
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance).Cast<MemberInfo>();
                var ordered = fields.Union(props).Where(p => IsPropertyType(p, _Types)).OrderBy(_Sorter);

                var filter = new Dictionary<string, JsonProperty>();
                foreach (var member in ordered)
                {
                    if (_Filter != null && !_Filter(member))
                        continue;

                    var isReadable = true;
                    var isWriteable = true;

                    var property = member as PropertyInfo;
                    if (property != null)
                    {
                        var getter = property.GetGetMethod(true);
                        if (getter == null)
                            continue;

                        var setter = property.GetSetMethod(true);
                        if (setter == null)
                        {
                            var attribute = property.GetCustomAttribute<JsonOptionAttribute>(true);
                            if (attribute == null || ((attribute.Types & _Types) == 0))
                                continue;
                        }

                        isReadable = getter != null;
                        isWriteable = setter != null;
                    }

                    if (_Storage && member.IsFlag(ExcludeTypes.Storage))
                        continue;

                    if (!filter.ContainsKey(member.Name))
                    {
                        var propertyName = member.Name;

                        if (_Storage)
                        {
                            var storageAttr = member.GetCustomAttribute<JsonStorageAttribute>();
                            if (storageAttr != null)
                                propertyName = storageAttr.StorageName;
                        }

                        var jprop = base.CreateProperty(member, memberSerialization);

                        jprop.Readable = isReadable;
                        jprop.Writable = isWriteable;
                        jprop.PropertyName = propertyName;
                        filter.Add(member.Name, jprop);

                        if (member.Name == nameof(Data) && _DataType != null)
                            jprop.PropertyType = _DataType;
                    }
                }

                return filter.Values.ToArray();
            }

            private static IComparable GetPropertyOrder(MemberInfo propInfo)
            {
                var orderAttr = propInfo.GetCustomAttributes<JsonOptionAttribute>(true).FirstOrDefault();
                var output = orderAttr != null ? orderAttr.Order : Int32.MaxValue;
                return output;
            }

            private static bool IsPropertyType(MemberInfo member, PropertyTypes types)
            {
                var ignore = member.GetCustomAttribute<JsonIgnoreAttribute>();
                if (ignore != null)
                    return false;

                var attribute = member.GetCustomAttribute<JsonOptionAttribute>();
                var attributeType = attribute != null ? attribute.Types : PropertyTypes.Public;
                if (attributeType == PropertyTypes.None && types == PropertyTypes.None)
                    return true;

                var result = (types & attributeType) != PropertyTypes.None;
                return result;
            }

        }
        private class DoubleValueConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                switch (objectType.Name)
                {
                    case nameof(Double):
                    case nameof(Single):
                    case nameof(Decimal):
                        return true;

                    default:
                        break;
                }

                return false;
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (objectType == reader.ValueType)
                    return reader.Value;

                if (reader.Value == null) return null;
                var cvalue = reader.Value as IConvertible;
                var strval = cvalue != null ? cvalue.ToString(CultureInfo.InvariantCulture) : reader.Value.ToString();
                try
                {
                    switch (objectType.Name)
                    {
                        case nameof(Double):
                            var dvalue = double.Parse(strval, NumberStyles.Any, CultureInfo.InvariantCulture);
                            return dvalue;
                        case nameof(Single):
                            var svalue = float.Parse(strval, NumberStyles.Any, CultureInfo.InvariantCulture);
                            return svalue;
                        case nameof(Decimal):
                            var decvalue = decimal.Parse(strval, NumberStyles.Any, CultureInfo.InvariantCulture);
                            return decvalue;
                        default:
                            return null;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                if (IsWholeValue(value))
                    value = Convert.ToInt64(value);

                var cvalue = value as IConvertible;
                var svalue = cvalue.ToString(CultureInfo.InvariantCulture);
                if (svalue.Contains('.') && svalue.EndsWith('0'))
                    svalue = svalue.TrimEnd('.', '0');

                writer.WriteRawValue(svalue);
            }

            private static bool IsWholeValue(object value)
            {
                if (value is decimal decimalValue)
                {
                    return decimalValue == Math.Truncate(decimalValue);
                }
                else if (value is float floatValue)
                {
                    return floatValue == Math.Truncate(floatValue);
                }
                else if (value is double doubleValue)
                {
                    return doubleValue == Math.Truncate(doubleValue);
                }

                return false;
            }
        }

        private class DateTimeValueConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                switch (objectType.Name)
                {
                    case nameof(DateTime):
                        return true;

                    default:
                        break;
                }
                return false;
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (objectType == reader.ValueType)
                    return reader.Value;

                var cvalue = (long)reader.Value;
                var result = new DateTime(cvalue, DateTimeKind.Utc);
                return result;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var cvalue = (DateTime)value;
                var svalue = cvalue.Ticks.ToString();
                writer.WriteRawValue(svalue);
            }
        }

        private static Type GetRequestType(RequestTypes type)
        {
            var enumField = typeof(RequestTypes).GetField(type.ToString());
            if (enumField == null)
                throw new NotSupportedException($"Type {type} is not supported");

            var attribute = enumField.GetAttribute<StorageUnitAttribute>();
            if (attribute == null)
                return typeof(SUData);

            return attribute.Type;
        }
        private static IDictionary<string, JsonSerializerSettings> _Settings = new SortedDictionary<string, JsonSerializerSettings>();


        private static JsonSerializerSettings GetStandardSettings(PropertyTypes propertyTypes, Type dataType, bool storage)
        {
            JsonSerializerSettings settings;

            var dataTypeName = dataType.Name;
            if (dataType.IsGenericType)
                dataTypeName = String.Format("{0}[{1}]:{2}", dataType.Name, String.Join(";", dataType.GenericTypeArguments.Select(t => t.Name).ToArray()), storage);

            var key = "properties=" + propertyTypes.ToString() + ";type=" + dataTypeName + ";storage=" + storage;
            if (!_Settings.TryGetValue(key, out settings))
            {
                lock (_Settings)
                {
                    if (!_Settings.TryGetValue(key, out settings))
                    {
                        settings = new JsonSerializerSettings();
                        settings.ContractResolver = new RequestContractResolver(settings, propertyTypes, dataType, storage);
                        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        settings.DefaultValueHandling = DefaultValueHandling.Ignore;

                        settings.Converters.Add(new DoubleValueConverter());
                        settings.Converters.Add(new DateTimeValueConverter());
                        _Settings[key] = (settings);
                    }
                }
            }

            return settings;
        }

        public static string Serialize(PropertyTypes propertyType, RequestTypes requestType, bool storage, object obj)
        {
            switch (propertyType)
            {
                case PropertyTypes.Private:
                    propertyType |= PropertyTypes.Public;
                    break;
                case PropertyTypes.Hidden:
                    propertyType |= PropertyTypes.Public;
                    propertyType |= PropertyTypes.Private;
                    break;
            }
            // We need PUB :   serialize everything that has no flag. Properties to be retrieved by anyone. 
            // --> no flag or default flag = [JsonOption(PropertyType.Public)]

            // PRIVATE:   No Flag + Private.  Properties For Private Users involved in the transaction.
            // --> private flag = [JsonOption(PropertyType.Private)]

            // HIDDEN:  No Flag + Private + HIdden :  For Nodes. Properties to be saved on Disk.
            // Excluded Properties are never transmitted or saved to the DISK!
            // --> private flag = [JsonOption(PropertyType.Hidden)]

            // Example  Matched order:    Price, Amount is public. UnmatchedOrderID, IsMaker  is Private,  HiddenLastChange  Is Hidden.  Some help Property like Winnigs is Excluded.            
            // --> use [JsonIgnore] from Newtonsoft.Json.JsonIgnoreAttribute to generally skip this property

            var targetType = obj.GetType();
            var dataType = GetRequestType(requestType);
            var settings = GetStandardSettings(propertyType, dataType, storage);
            var message = JsonConvert.SerializeObject(obj, settings);
            return message;
        }
        private static JsonSerializerSettings GetSignatureSettings(PropertyTypes propertyTypes, Type dataType)
        {
            JsonSerializerSettings settings;
            var key = propertyTypes.ToString() + ";" + dataType.Name + ";Signature";
            if (!_Settings.TryGetValue(key, out settings))
            {
                lock (_Settings)
                {
                    if (!_Settings.TryGetValue(key, out settings))
                    {
                        settings = new JsonSerializerSettings();
                        settings.ContractResolver = new RequestContractResolver(settings, propertyTypes, dataType, p => p.IsNotFlag(ExcludeTypes.Signature), p => p.Name);
                        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        settings.DefaultValueHandling = DefaultValueHandling.Ignore;

                        settings.Converters.Add(new DoubleValueConverter());
                        settings.Converters.Add(new DateTimeValueConverter());
                        _Settings.Add(key, settings);
                    }
                }
            }

            return settings;
        }
        private static Type GetResponseType(RequestTypes type)
        {
            var enumField = typeof(RequestTypes).GetField(type.ToString());
            if (enumField == null)
                throw new NotSupportedException($"Type {type} is not supported");

            var attribute = enumField.GetAttribute<ResponseTypeAttribute>();
            if (attribute == null)
                return typeof(object);

            return attribute.Type;
        }
        public static JsonRespClient CreateResponse(RequestStates state, RequestTypes type, string msg)
        {
            JsonSerializerSettings settings;

            switch (state)
            {
                case RequestStates.Success:
                    var dataType = GetResponseType(type);
                    settings = GetStandardSettings(PropertyTypes.Public, dataType, false);
                    break;
                case RequestStates.Error:
                    settings = GetStandardSettings(PropertyTypes.Public, typeof(Exception), false);
                    break;
                default:
                    settings = GetStandardSettings(PropertyTypes.Public, typeof(string), false);
                    break;
            }

            object suP;

            try
            {
                var targetType = typeof(JsonRespClient);
                suP = JsonConvert.DeserializeObject(msg, targetType, settings);
            }
            catch (Exception e)
            {
                suP = new JsonRespClient()
                {
                    Type = type,
                    State = RequestStates.Error,
                    Error = e.Message,
                    Data = e,
                };
            }

            var result = (JsonRespClient)suP;
            return result;
        }

        public static int GetContentLength(object obj, RequestTypes type)
        {
            var dataType = GetRequestType(type);
            var settings = GetSignatureSettings(PropertyTypes.Public | PropertyTypes.Private, dataType);
            var content = JsonConvert.SerializeObject(obj, settings);
            return content.Length;
        }

        public static byte[] GetSignature(object obj, RequestTypes type, byte[] PrivKey, bool isETH)
        {
            var dataType = GetRequestType(type);
            var settings = GetSignatureSettings(PropertyTypes.Public | PropertyTypes.Private, dataType);
            var content = JsonConvert.SerializeObject(obj, settings);
            if (isETH)
            {
                var signer3 = new Nethereum.Signer.EthereumMessageSigner();
                var privKey4 = new Nethereum.Signer.EthECKey(PrivKey, true);
                var signature1 = signer3.EncodeUTF8AndSign(content, privKey4);

                if (signature1.StartsWith("0x")) signature1 = signature1.Substring(2);

                return StringX.GetBytesHex(signature1);



                // var privKey4 = new Nethereum.Signer.EthECKey(PrivKey, true);
                // var sign = privKey4.Sign(StringC.GetBytesUTF8(content));
                // var signByte = sign.ToDER();

                // return signByte;
            }
            else
            {

                var signature = StringX.GetSignatureEd25519(content, PrivKey);
                return signature;
            }
        }

        public string Serialize(PropertyTypes propertyType, bool storage)
        {
            return Serialize(propertyType, Type, storage, this);
        }
        public byte[] GetBytes(PropertyTypes type, bool storage)
        {
            var message = Serialize(type, storage);
            var data = Encoding.UTF8.GetBytes(message);
            return data;
        }
        public static string Serialize(PropertyTypes propertyType, RequestTypes requestType, object obj)
        {
            return Serialize(propertyType, requestType, false, obj);
        }
        public string Serialize(PropertyTypes propertyType)
        {
            return Serialize(propertyType, Type, false, this);
        }
        public byte[] GetBytes()
        {
            var message = Serialize(PropertyTypes.Public);
            var data = Encoding.UTF8.GetBytes(message);
            return data;
        }


        [JsonOption(Order = 0)]
        [JsonConverter(typeof(StringEnumConverter))]
        public RequestTypes Type;

        [JsonOption(Order = 1)]
        public long Nonce;

        public SUData Data;

        public override string ToString()
        {
            return String.Format("{0}@{1}: {2}", Type, Nonce, Data);
        }
    }
}
