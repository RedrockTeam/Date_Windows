using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Date.Util
{
    class Utils
    {

        /// <summary>
        /// Toast
        /// </summary>
        /// <param name="text"></param>
        public async static void Toast(string text)
        {
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
            XmlNodeList elements = toastXml.GetElementsByTagName("text");
            elements[0].AppendChild(toastXml.CreateTextNode(text));
            ToastNotification toast = new ToastNotification(toastXml);
            //toast.Activated += toast_Activated;//点击
            //toast.Dismissed += toast_Dismissed;//消失
            //toast.Failed += toast_Failed;//消除
            ToastNotificationManager.CreateToastNotifier().Show(toast);


            //从通知中心删除
            await Task.Delay(3000);
            ToastNotificationManager.History.Clear();

        }

        /// <summary>
        ///UNICODE字符转为中文 
        /// </summary>
        /// <param name="unicodeString"></param>
        /// <returns></returns>
        public static string ConvertUnicodeStringToChinese(string unicodeString)
        {
            if (string.IsNullOrEmpty(unicodeString))
                return string.Empty;

            string outStr = unicodeString;

            Regex re = new Regex("\\\\u[0123456789abcdef]{4}", RegexOptions.IgnoreCase);
            MatchCollection mc = re.Matches(unicodeString);
            foreach (Match ma in mc)
            {
                outStr = outStr.Replace(ma.Value, ConverUnicodeStringToChar(ma.Value).ToString());
            }
            return outStr;
        }

        private static char ConverUnicodeStringToChar(string str)
        {
            char outStr = Char.MinValue;
            outStr = (char)int.Parse(str.Remove(0, 2), System.Globalization.NumberStyles.HexNumber);
            return outStr;
        }

        public static async Task ShowSystemTrayAsync(Color backgroundColor, Color foregroundColor, double opacity = 1, string text = "", bool isIndeterminate = false)
        {
            StatusBar statusBar = StatusBar.GetForCurrentView();
            statusBar.BackgroundColor = backgroundColor;
            statusBar.ForegroundColor = foregroundColor;
            statusBar.BackgroundOpacity = opacity; 

            statusBar.ProgressIndicator.Text = text;
            if (!isIndeterminate)
            {
                statusBar.ProgressIndicator.ProgressValue = 0;
            }
            await statusBar.ProgressIndicator.ShowAsync();
        }

        /// <summary>
        /// 弹出对话框
        /// </summary>
        /// <param name="text"></param>
        public async static void Message(string text,string title="错误")
        {
            await new MessageDialog(text, title).ShowAsync();
        }

        /// <summary>
        /// 屏幕高度
        /// </summary>
        /// <returns></returns>
        public static double getPhoneHeight()
        {
            return Window.Current.Bounds.Height;
        }

        /// <summary>
        /// 屏幕宽度
        /// </summary>
        /// <returns></returns>
        public static double getPhoneWidth()
        {
            return Window.Current.Bounds.Width;
        }

        /// <summary>
        /// 时间戳
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetTimeStamp(DateTimeOffset date, TimeSpan time)
        {
            DateTime nowdate = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, 0);
            TimeSpan ts = nowdate.ToUniversalTime() -new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }


    }


}
