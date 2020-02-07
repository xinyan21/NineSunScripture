using NineSunScripture.model;
using NineSunScripture.util.log;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Windows.Documents;

namespace NineSunScripture.util
{
    internal class Utils
    {
        /// <summary>
        /// 判断是不是周末/节假日
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns>周末和节假日返回true，工作日返回false</returns>
        public static bool IsHolidayByDate(DateTime date)
        {
            var isHoliday = false;
            var webClient = new System.Net.WebClient();
            var PostVars = new System.Collections.Specialized.NameValueCollection
            {
                { "d", date.ToString("yyyyMMdd") }//参数
            };
            try
            {
                var day = date.DayOfWeek;

                //判断是否为周末
                if (day == DayOfWeek.Sunday || day == DayOfWeek.Saturday)
                    return true;

                //0为工作日，1为周末，2为法定节假日
                /* var byteResult = await webClient.UploadValuesTaskAsync("http://tool.bitefu.net/jiari/",
                     "POST", PostVars);//请求地址,传参方式,参数集合
                 var result = Encoding.UTF8.GetString(byteResult);//获取返回值
                 if (result == "1" || result == "2")
                     isHoliday = true;*/
            }
            catch
            {
                isHoliday = false;
            }
            return isHoliday;
        }

        public static Bitmap RotateImage(Image image, float angle)
        {
            if (image == null)
                throw new ArgumentNullException("image");
            float dx = image.Width / 2.0f;
            float dy = image.Height / 2.0f;

            Bitmap rotatedBmp = new Bitmap(image.Width, image.Height);
            rotatedBmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            Graphics g = Graphics.FromImage(rotatedBmp);
            g.TranslateTransform(dx, dy);
            g.RotateTransform(angle);
            g.TranslateTransform(-dx, -dy);
            g.DrawImage(image, new PointF(0, 0));
            return rotatedBmp;
        }

        public static void SamplingLogQuotes(Quotes quotes)
        {
            if (null == quotes)
            {
                return;
            }
            if (DateTime.Now.Hour == 9 && DateTime.Now.Minute <= 30)
            {
                Logger.Log(quotes.ToString(), LogType.Quotes);
            }
            if (DateTime.Now.Second != 0)
            {
                return;
            }
            Logger.Log("Sampling->" + quotes.ToString(), LogType.Quotes);
        }

        public static int FixQuantity(int quantity)
        {
            return (quantity / 100) * 100;
        }

        public static float FormatTo2Digits(float value)
        {
            return (float)Math.Round(value, 2);
        }

        /// <summary>
        /// 检测dll是否存在
        /// </summary>
        /// <returns></returns>
        public static bool DetectTHSDll()
        {
            string path = Environment.CurrentDirectory + @"\ths.dll";
            bool isExist = File.Exists(path);
            return isExist;
        }

        public static void SendEmail(string title, string content, string to = "xinyan621@outlook.com")
        {
            //实例化一个发送邮件类。
            MailMessage mailMessage = new MailMessage
            {
                //发件人邮箱地址，方法重载不同，可以根据需求自行选择。
                From = new MailAddress("460313911@qq.com"),
                //收件人邮箱地址。
                //邮件标题。
                Subject = title,
                //邮件内容。
                Body = content
            };
            mailMessage.To.Add(new MailAddress(to));

            //实例化一个SmtpClient类。
            SmtpClient client = new SmtpClient
            {
                //在这里我使用的是qq邮箱，所以是smtp.qq.com，如果你使用的是126邮箱，那么就是smtp.126.com。
                Host = "smtp.qq.com",
                //使用安全加密连接。
                EnableSsl = true,
                //不和请求一块发送。
                UseDefaultCredentials = false,
                //验证发件人身份(发件人的邮箱，邮箱里的生成授权码);
                Credentials = new NetworkCredential("460313911@qq.com", "ymegzdrsirmscaib")
            };
            //发送
            client.Send(mailMessage);
        }

        public static bool IfNeedToSubOByOPrice(Quotes quotes)
        {
            if (null == quotes)
            {
                return false;
            }
            if (quotes.Code.StartsWith("6"))
            {
                return false;
            }
            if (quotes.PreClose > 0 && quotes.LatestPrice / quotes.PreClose > 1.085)
            {
                return true;
            }

            return false;
        }

        public static bool IsListEmpty(List<Quotes> list)
        {
            if (null != list && list.Count > 0)
            {
                return false;
            }
            return true;
        }

        public static bool IsListEmpty(List<Position> list)
        {
            if (null != list && list.Count > 0)
            {
                return false;
            }
            return true;
        }

        public static bool IsListEmpty(List<Order> list)
        {
            if (null != list && list.Count > 0)
            {
                return false;
            }
            return true;
        }
    }
}