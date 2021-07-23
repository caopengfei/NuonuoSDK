using System;
using System.Security.Cryptography;
using System.Text;

namespace NuonuoSDK
{
    sealed class ModuleSign
    {
        public static string getSign(string path, string secret, string appKey, string senid, string nonce, string body, string timestamp)
        {
            string[] split = path.Split('/');
            StringBuilder signStr = new StringBuilder();
            signStr.Append("a=" + split[3])
                .Append("&l=" + split[2])
                .Append("&p=" + split[1])
                .Append("&k=" + appKey)
                .Append("&i=" + senid)
                .Append("&n=" + nonce)
                .Append("&t=" + timestamp)
                .Append("&f=" + body);
            return hmacSha1WithBase64(signStr.ToString(), secret);
        }

        private static string hmacSha1WithBase64(string value, string key)
        {
            string res = "";
            try
            {
                byte[] rawHmac = hmacSha1(value, key);
                res = Convert.ToBase64String(rawHmac);
            }
            catch (Exception e)
            {
                throw e;
            }
            return res;
        }

        private static byte[] hmacSha1(string value, string key)
        {
            Encoding encoding = Encoding.UTF8;
            try
            {
                byte[] keyBytes = encoding.GetBytes(key);
                HMACSHA1 hmacsha1 = new HMACSHA1(keyBytes);
                byte[] messageBytes = encoding.GetBytes(value);
                byte[] hashmessage = hmacsha1.ComputeHash(messageBytes);
                return hashmessage;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
