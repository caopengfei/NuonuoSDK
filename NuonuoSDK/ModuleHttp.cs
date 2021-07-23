using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace NuonuoSDK
{
    sealed class ModuleHttp
    {
        public static string sendSyncHttp(string url, Dictionary<string, string> header, string body, string contentType)
        {
            Encoding encoding = Encoding.UTF8;
            //创建Web访问对象
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = contentType;
            request.Timeout = 30000;
            request.ServicePoint.Expect100Continue = false;
            setHeaders(request, header);
            byte[] buffer = encoding.GetBytes(body);
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            //通过Web访问对象获取响应内容
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                //通过响应内容流创建StreamReader对象，因为StreamReader更高级更快
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    return reader.ReadToEnd();//利用StreamReader就可以从响应内容从头读到尾
                }
            }
            catch (WebException e)
            {
                response = (HttpWebResponse)e.Response;
                using (Stream data = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(data))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        private static void setHeaders(HttpWebRequest request, Dictionary<string, string> header)
        {
            if (header != null && header.Count > 0)
            {
                Dictionary<string, string>.KeyCollection keys = header.Keys;
                foreach (string key in keys)
                {
                    request.Headers.Add(key, header[key]);
                }
            }
        }
    }
}
