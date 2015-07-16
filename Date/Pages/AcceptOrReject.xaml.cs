using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Date.Data;
using Date.Util;
using Newtonsoft.Json.Linq;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace Date.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AcceptOrReject : Page
    {
        private ApplicationDataContainer appSetting;
        DateLetter dl = new DateLetter();
        public AcceptOrReject()
        {
            appSetting = ApplicationData.Current.LocalSettings; //本地存储
            this.InitializeComponent();
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;//注册重写后退按钮事件
            dl = e.Parameter as DateLetter;
            this.DataContext = dl;
            if (dl.User_gender == 1)
            {
                GenderImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/ic_man.png", UriKind.Absolute));
            }
            else
            {
                GenderImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/ic_woman.png", UriKind.Absolute));
            }

            if (dl.User_date_status==0)
            {
                this.AorR.Text = "已接受";
                CommandBar.Visibility=Visibility.Collapsed;
            }
            if (dl.User_date_status == 1)
            {
                this.AorR.Text = "已拒绝";
                CommandBar.Visibility = Visibility.Collapsed;
            }

        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;//注册重写后退按钮事件
            UmengSDK.UmengAnalytics.TrackPageEnd("AcceptPage");

        }
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)//重写后退按钮，如果要对所有页面使用，可以放在App.Xaml.cs的APP初始化函数中重写。
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
                e.Handled = true;
            }
        }
        private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(PersonInfoPage), dl.User_id);
        }

        private async void AcceptBarButton_Click(object sender, RoutedEventArgs e)
        {
            string content = await GetResponse("1");
            try
            {
                if (content != "")
                {
                    JObject obj = JObject.Parse(content);
                    if (Int32.Parse(obj["status"].ToString()) == 200)
                    {
                        this.AorR.Text = "已接受";
                    } 
                    else if (Int32.Parse(obj["status"].ToString()) ==403)
                    {
                        this.AorR.Text = "你没有权限";
                    }
                    else 
                    {
                        this.AorR.Text = "没有这条约会记录啦~";
                    }
                    await Task.Delay(200);
                    Frame.GoBack();
                }

            }
            catch (Exception)
            {
                Debug.WriteLine("接受私信错误");

            }
        }

        private async Task<string> GetResponse(string action)
        {
            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<string, string>("uid", appSetting.Values["uid"].ToString()));
            paramList.Add(new KeyValuePair<string, string>("token", appSetting.Values["token"].ToString()));
            paramList.Add(new KeyValuePair<string, string>("to_id", dl.User_id.ToString()));
            paramList.Add(new KeyValuePair<string, string>("date_id", dl.Date_id.ToString()));
            paramList.Add(new KeyValuePair<string, string>("action", action));
            string content =
                Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/letter/dateaction", paramList));
            return content;
        }

        private async void RejectBarButton_Click(object sender, RoutedEventArgs e)
        {
            string content = await GetResponse("2");
            try
            {
                if (content != "")
                {
                    JObject obj = JObject.Parse(content);
                    if (Int32.Parse(obj["status"].ToString()) == 200)
                    {
                        this.AorR.Text = "已拒绝";
                    }
                    else if (Int32.Parse(obj["status"].ToString()) == 403)
                    {
                        this.AorR.Text = "你没有权限";
                    }
                    else
                    {
                        this.AorR.Text = "没有这条约会记录啦~";
                    }
                    await Task.Delay(200);
                    Frame.GoBack();
                }

            }
            catch (Exception)
            {
                Debug.WriteLine("拒绝私信错误");
            }
        }


    }
}
