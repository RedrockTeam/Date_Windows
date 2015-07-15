using Date.Data;
using Date.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UmengSDK;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace Date
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PersonInfoPage : Page
    {
        private ApplicationDataContainer appSetting;
        PersonInfo pi = new PersonInfo();
        private int failednum = 0;

        public PersonInfoPage()
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
            UmengAnalytics.TrackPageStart("PersonInfoPage");
            GetPerInfo((Int32)e.Parameter);
        }

        //离开页面时，取消事件
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;//注册重写后退按钮事件
            UmengAnalytics.TrackPageEnd("PersonInfoPage");

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

        private async void GetPerInfo(int User_id)
        {
            StatusProgressBar.Visibility = Visibility.Visible;
            StatusTextBlock.Text = "加载中...";
            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<string, string>("uid", appSetting.Values["uid"].ToString()));
            paramList.Add(new KeyValuePair<string, string>("get_uid", User_id.ToString()));
            paramList.Add(new KeyValuePair<string, string>("token", appSetting.Values["token"].ToString()));
            string pc = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/person/userinfo", paramList));
            Debug.WriteLine("个人信息" + pc);
            if (pc != "")
            {
                JObject obj = JObject.Parse(pc);
                pi.GetOtherAttribute(obj);
                this.DataContext = pi;
                img.ImageSource = new BitmapImage(new Uri(pi.Head));
            }
            else
            {
                if (failednum > 1)
                {
                    StatusStackPanel.Background = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
                    StatusProgressBar.Visibility = Visibility.Collapsed;
                    StatusTextBlock.Visibility = Visibility.Visible;
                    StatusTextBlock.Text = "加载失败 T_T";
                    await Task.Delay(2000);
                    StatusTextBlock.Text = "";
                    StatusTextBlock.Visibility = Visibility.Collapsed;
                    StatusStackPanel.Background = new SolidColorBrush(Color.FromArgb(255, 239, 239, 239));
                }
                else
                {
                    failednum++;
                    GetPerInfo(User_id);
                }
            }
        }
    }
}
