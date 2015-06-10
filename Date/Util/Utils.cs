using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Popups;

namespace Date.Util
{
    class Utils
    {


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

        //UNICODE字符转为中文 
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

        public async static void Message(string text)
        {
            await new MessageDialog(text).ShowAsync();
        }


    }
}
