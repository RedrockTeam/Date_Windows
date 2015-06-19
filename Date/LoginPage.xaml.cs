using Date.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace Date
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        private ApplicationDataContainer appSetting;

        public LoginPage()
        {
            this.InitializeComponent();
            appSetting = ApplicationData.Current.LocalSettings; //本地存储

        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;//注册重写后退按钮事件
            UmengSDK.UmengAnalytics.TrackPageStart("LoginPage");
        }

        //离开页面时，取消事件
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;//注册重写后退按钮事件
            UmengSDK.UmengAnalytics.TrackPageEnd("LoginPage");
        }



        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)//重写后退按钮，如果要对所有页面使用，可以放在App.Xaml.cs的APP初始化函数中重写。
        {
            Application.Current.Exit();
        }


        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (StuNumTextBox.Text == "" || IdNumPasswordBox.Password == "")
            {
                Util.Utils.Toast("参数错误");
            }
            else
            {
                LoginProgressBar.Visibility = Visibility.Visible;

                List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
                paramList.Add(new KeyValuePair<string, string>("username", StuNumTextBox.Text));
                paramList.Add(new KeyValuePair<string, string>("password", IdNumPasswordBox.Password));

                string content = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/public/login", paramList));
                Debug.WriteLine("LoginMessage" + content);

                if (content != "")
                {
                    JObject obj = JObject.Parse(content);
                    if (Int32.Parse(obj["status"].ToString()) != 200)
                    {
                        Utils.Message(obj["info"].ToString());
                        LoginProgressBar.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        LoginProgressBar.Visibility = Visibility.Collapsed;
                        appSetting.Values["username"] = StuNumTextBox.Text;
                        appSetting.Values["password"] = IdNumPasswordBox.Password;
                        appSetting.Values["uid"] = obj["uid"].ToString();
                        appSetting.Values["token"] = obj["token"].ToString();

                        //Umeng统计

                        UmengSDK.UmengAnalytics.TrackEvent("Login");

                        Frame.Navigate(typeof(MainPage));
                    }
                }
                else
                {
                    LoginProgressBar.Visibility = Visibility.Collapsed;
                    LoginStackPanel.Visibility = Visibility.Visible;
                    LoginStackPanel.Background = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
                    await Task.Delay(2000);
                    LoginStackPanel.Visibility = Visibility.Collapsed;
                }


            }
        }
    }
}
