using NineSunScripture.model;
using NineSunScripture.util.log;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineSunScripture.util
{
    class Utils
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
            if (DateTime.Now.Hour == 9 && DateTime.Now.Minute <= 30)
            {
                Logger.Log(quotes.ToString(), LogType.Quotes);
            }
            if (DateTime.Now.Second != 0 || null == quotes)
            {
                return;
            }
            Logger.Log(quotes.ToString(), LogType.Quotes);
        }

        public static int FixQuantity(int quantity)
        {
            return (quantity / 100) * 100;
        }
    }
}
