using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace BBetModels
{
    public class StorageUnitOnClient : RequestBas0
    {
        [JsonOption(PropertyTypes.Public, ExcludeTypes.Signature)]
        public byte[] SignatureUser;
    }
}
