using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

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
    }
}
