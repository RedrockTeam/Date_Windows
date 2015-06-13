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
using Windows.Foundation;
using Windows.Foundation.Collections;
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

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
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

                string content = await Util.NetWork.getHttpWebRequest("/public/login", paramList);
                content = Util.Utils.ConvertUnicodeStringToChinese(content);
                Debug.WriteLine("LoginMessage" + content);

                if (content != "")
                {
                    JObject obj = JObject.Parse(content);
                    if (Int32.Parse(obj["status"].ToString()) != 200)
                    {
                        Util.Utils.Message(obj["info"].ToString());
                        LoginProgressBar.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        LoginProgressBar.Visibility = Visibility.Collapsed;
                        appSetting.Values["username"] = StuNumTextBox.Text;
                        appSetting.Values["password"] = IdNumPasswordBox.Password;
                        appSetting.Values["uid"] = obj["uid"].ToString();
                        appSetting.Values["token"] = obj["token"].ToString();

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
