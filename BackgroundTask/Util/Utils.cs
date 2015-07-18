﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Date.DataModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.UI.Xaml.Media.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;

namespace Date.Util
{
    internal class Utils
    {

        /// <summary>
        /// Toast
        /// </summary>
        /// <param name="text"></param>
        public static async void Toast(string text)
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

        public static async Task ShowSystemTrayAsync(Color backgroundColor, Color foregroundColor, double opacity = 1,
            string text = "", bool isIndeterminate = false)
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
        public static async void Message(string text, string title = "错误")
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
        /// 时间转时间戳
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetTimeStamp(DateTimeOffset date, TimeSpan time)
        {
            DateTime nowdate = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, 0);
            TimeSpan ts = nowdate.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// 时间戳转时间
        /// </summary>
        /// <param name="timeStamp">Unix时间戳格式</param>
        /// <returns>C#格式时间</returns>
        public static DateTime GetTime(string timeStamp)
        {
            DateTime dtStart = new DateTime(1970, 1, 1,8,0,0);
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        public static JArray ReadJso(string jsonstring)
        {
            if (jsonstring != "")
            {
                JObject obj = JObject.Parse(jsonstring);
                if (Int32.Parse(obj["status"].ToString()) == 200)
                {
                    JObject jObject = (JObject)JsonConvert.DeserializeObject(jsonstring);
                    string json = jObject["data"].ToString();
                    JArray jArray = (JArray)JsonConvert.DeserializeObject(json);
                    return jArray;
                }
                else
                {
                    Message("请求失败","失败");
                    return null;
                }
            }

            else
            {
                Message("网络错误！", "错误");
                return null;
            }
        }

        //public static async void GetImages(BitmapImage bitmap,string filename)
        //{

        //    WriteableBitmap wb=new WriteableBitmap(bitmap.PixelWidth,bitmap.PixelHeight);
        // //   wb.SetSourceAsync();
        //    RandomAccessStreamReference rasr=RandomAccessStreamReference.CreateFromUri(bitmap.UriSource);
        //    var streamWithcontent = await rasr.OpenReadAsync();
        //    byte[] buffer=new byte[streamWithcontent.Size];
        //    await streamWithcontent.ReadAsync(buffer.AsBuffer(), (uint) streamWithcontent.Size, InputStreamOptions.None);
        //    //IRandomAccessStream stream = new Stream();
        //   // editab wbmp=new WriteableBitmap(128,128);
        //    //wbmp.SetSourceAsync(stream);
        //    StorageFolder folder = ApplicationData.Current.LocalFolder;
        //    StorageFile file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
        //   // IBuffer buffer = RandomAccessStreamToBuffer(bitmap);
        //    MemoryStream ss=new MemoryStream();
        //    bitmap.

        //}
        //public static IBuffer RandomAccessStreamToBuffer(IRandomAccessStream randomstream)
        //{

        //    Stream stream = WindowsRuntimeStreamExtensions.AsStreamForRead(randomstream.GetInputStreamAt(0));

        //    MemoryStream memoryStream = new MemoryStream();

        //    if (stream != null)
        //    {

        //        byte[] bytes = ConvertStreamTobyte(stream);

        //        if (bytes != null)
        //        {

        //            var binaryWriter = new BinaryWriter(memoryStream);

        //            binaryWriter.Write(bytes);

        //        }

        //    }

        //    IBuffer buffer = WindowsRuntimeBufferExtensions.GetWindowsRuntimeBuffer(memoryStream, 0, (int)memoryStream.Length);

        //    return buffer;

        //}
        //public static byte[] ConvertStreamTobyte(Stream input)
        //{

        //    byte[] buffer = new byte[16 * 1024];



        //    using (MemoryStream ms = new MemoryStream())
        //    {

        //        int read;

        //        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
        //        {

        //            ms.Write(buffer, 0, read);

        //        }

        //        return ms.ToArray();

        //    }

        //}
    }
}
