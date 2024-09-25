using NSec.Cryptography;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BBetModels
{
    public class StringX
    {
        public static Random RC = new Random();
        public static byte[] GetBytesHex(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
        public static string getStringHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }
        public static string RemoveDiacritics(string text)
        {
            if (text == null) return null;
            text = text.Replace("ı", "i").Replace("\"", "").Replace("ø", "o").Replace("đ", "d").Replace("?", "").Replace("Å", "A").Replace("ə", "e").Replace("\"", "").Replace("\\", "").Replace("ł", "l").Replace("Ł", "L").Replace("Đ", "D").Replace("Ð", "D").Replace("ộ", "o");

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {



                    stringBuilder.Append(c);
                }
            }

            var ret = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

            stringBuilder = new StringBuilder();

            foreach (var c in ret)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                int cv = Convert.ToInt32(c);
                if (cv < 130)
                {
                    stringBuilder.Append(c);
                }
            }


            return stringBuilder.ToString();
        }

        static SignatureAlgorithm Ed2519 = SignatureAlgorithm.Ed25519;
        public static byte[] GetSignatureEd25519(string data, byte[] PrivKey)
        {

            // create a new key pair
            var key = Key.Import(Ed2519, PrivKey, KeyBlobFormat.NSecPrivateKey);

            // generate some data to be signed
            var datab = Encoding.UTF8.GetBytes(data);

            // sign the data using the private key
            var signature = Ed2519.Sign(key, datab);

            // verify the data using the signature and the public key


            return signature;
        }
        public static string SubstringMy(string source, string preChars, string postChars)
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

        public static byte[] CreatePrivKeyEd25519()
        {
            var key = Key.Create(Ed2519, new KeyCreationParameters() { ExportPolicy = KeyExportPolicies.AllowPlaintextExport });
            return key.Export(KeyBlobFormat.NSecPrivateKey);
        }
        public static byte[] GetPublicKeyEd25519(byte[] privkey)
        {
            var key = Key.Import(Ed2519, privkey, KeyBlobFormat.NSecPrivateKey);
            var keyexport = key.Export(KeyBlobFormat.NSecPublicKey);


            return keyexport;
        }
        public static string GetHashString(string inputString)
        {
            if (inputString == null) return "nullhash";

            var hashx = GetHash(inputString);

            return GetHashString(hashx);
        }
        public static string GetHashString(byte[] hashx)
        {
            StringBuilder sb = new StringBuilder();

            foreach (byte b in hashx)
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
        private static byte[] GetHash(string inputString)
        {
            System.Security.Cryptography.HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[RC.Next(s.Length)]).ToArray());
        }
        public static byte[] GetBytesUTF8(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }
    }
}
