using System;
using System.IO;
using System.Net;
using System.Text;

namespace NineSunScripture.util
{
    public class Http
    {
        public CookieContainer cookies = new CookieContainer();

        /// <summary>
        /// GET方法(自动维护cookie)
        /// </summary>
        public string Get(string url, string referer = "", int timeout = 2000, Encoding encode = null)
        {
            string dat;
            HttpWebResponse res = null;
            HttpWebRequest req = null;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);
                req.CookieContainer = cookies;
                req.AllowAutoRedirect = false;
                req.Timeout = timeout;
                req.Referer = referer;
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0;%20WOW64; rv:47.0) Gecko/20100101 Firefox/47.0";
                res = (HttpWebResponse)req.GetResponse();
                cookies.Add(res.Cookies);
                dat = new StreamReader(res.GetResponseStream(), encode ?? Encoding.UTF8).ReadToEnd();
                res.Close();
                req.Abort();
            }
            catch
            {
                return null;
            }
            return dat;
        }

        /// <summary>
        /// Post方法(自动维护cookie)
        /// </summary>
        public static string Post(string url, string postdata, CookieContainer cookie = null, string referer = "", int timeout = 2000, Encoding encode = null)
        {
            string html = null;
            HttpWebRequest request;
            HttpWebResponse response;
            if (encode == null) encode = Encoding.UTF8;
            try
            {
                byte[] byteArray = encode.GetBytes(postdata); // 转化
                request = (HttpWebRequest)WebRequest.Create(new Uri(url));
                if (cookie == null) cookie = new CookieContainer();
                request.CookieContainer = cookie;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; InfoPath.1)";
                request.Method = "POST";
                request.Referer = referer;
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                request.Timeout = timeout;
                Stream newStream = request.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);    //写入参数
                newStream.Close();
                response = (HttpWebResponse)request.GetResponse();
                cookie.Add(response.Cookies);
                StreamReader str = new StreamReader(response.GetResponseStream(), encode);
                html = str.ReadToEnd();
            }
            catch
            {
                return "";
            }
            return html;
        }
    }
}
