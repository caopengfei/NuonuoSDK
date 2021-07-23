using System;
using System.Collections.Generic;
using System.Text;

namespace NuonuoSDK
{
    public class NNOpenSDK
    {
        private const string tokenUrl = "https://open.nuonuo.com/accessToken";

        /// <summary>
        /// 商家获取accessToken
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public static string getMerchantToken(string appKey, string appSecret)
        {
            verify(appKey, "AppKey不能为空");
            verify(appSecret, "AppSecret不能为空");
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("client_id", appKey);
            param.Add("client_secret", appSecret);
            param.Add("grant_type", "client_credentials");
            return doPost(tokenUrl, param);
        }

        /// <summary>
        /// ISV获取accessToken
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="appSecret"></param>
        /// <param name="code"></param>
        /// <param name="taxnum"></param>
        /// <param name="redirectUri"></param>
        /// <returns></returns>
        public static string getISVToken(string appKey, string appSecret, string code, string taxnum, string redirectUri)
        {
            verify(appKey, "AppKey不能为空");
            verify(appSecret, "AppSecret不能为空");
            verify(code, "code不能为空");
            verify(taxnum, "taxnum不能为空");
            verify(redirectUri, "redirectUri不能为空");
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("client_id", appKey);
            param.Add("client_secret", appSecret);
            param.Add("grant_type", "authorization_code");
            param.Add("redirect_uri", redirectUri);
            param.Add("code", code);
            param.Add("taxNum", taxnum);
            return doPost(tokenUrl, param);
        }

        /// <summary>
        /// ISV刷新accessToken
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="userId"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public static string refreshISVToken(string refreshToken, string userId, string appSecret)
        {
            verify(userId, "userId不能为空");
            verify(appSecret, "appSecret不能为空");
            verify(refreshToken, "refreshToken不能为空");
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("refresh_token", refreshToken);
            param.Add("client_id", userId);
            param.Add("client_secret", appSecret);
            param.Add("grant_type", "refresh_token");
            return doPost(tokenUrl, param);
        }

        /// <summary>
        /// API调用（同步）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="senid"></param>
        /// <param name="appKey"></param>
        /// <param name="appSecret"></param>
        /// <param name="token"></param>
        /// <param name="taxnum"></param>
        /// <param name="method"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string sendPostSyncRequest(string url, string senid, string appKey, string appSecret, string token, string taxnum, string method, string content)
        {
            verify(senid, "senid不能为空");
            verify(token, "token不能为空");
            verify(appKey, "appKey不能为空");
            verify(method, "method不能为空");
            verify(url, "请求地址URL不能为空");
            verify(content, "content不能为空");
            verify(appSecret, "appSecret不能为空");
            try
            {
                string timestamp = getTime();
                string nonce = new Random().Next(10000, 1000000000).ToString();
                StringBuilder sb = new StringBuilder(url);
                sb.Append("?senid=").Append(senid).Append("&nonce=")
                  .Append(nonce).Append("&timestamp=").Append(timestamp)
                  .Append("&appkey=").Append(appKey);
                Uri httpUrl = new Uri(url);
                string path = httpUrl.AbsolutePath;
                Dictionary<string, string> header = new Dictionary<string, string>();
                header.Add("X-Nuonuo-Sign", ModuleSign.getSign(path, appSecret, appKey, senid, nonce, content, timestamp));
                header.Add("accessToken", token);
                header.Add("userTax", taxnum);
                header.Add("method", method);
                header.Add("sdkVer", "1.0.1");// 这个版本号不参与签名, 预留字段
                // 调用开放平台API
                return ModuleHttp.sendSyncHttp(sb.ToString(), header, content, "application/json;charset=utf-8");
            }
            catch (Exception e)
            {
                throw new NNException("发送HTTP请求异常", e);
            }
        }

        private static void verify(string v, string msg)
        {
            if (v == null || v.Length == 0 || v.ToLower().Trim().Equals("null"))
            {
                throw new NNException(msg);
            }
        }

        private static string getTime()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds / 1000).ToString();
        }

        private static string doPost(string url, Dictionary<string, string> param)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (param != null && param.Count > 0)
            {
                foreach (KeyValuePair<string, string> item in param)
                {
                    stringBuilder.AppendFormat("&{0}={1}", item.Key, item.Value);
                }
            }
            return ModuleHttp.sendSyncHttp(url, null, stringBuilder.ToString().Trim('&'), "application/x-www-form-urlencoded;charset=utf-8");
        }
    }
}
