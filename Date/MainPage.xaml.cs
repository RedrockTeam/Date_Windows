using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=391641 上有介绍



namespace Date
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ApplicationDataContainer appSetting;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
             Util.Utils.ShowSystemTrayAsync(Colors.Red, Colors.White, text: "约");
            appSetting = ApplicationData.Current.LocalSettings; //本地存储

        }



        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected  override void OnNavigatedTo(NavigationEventArgs e)
        {
            await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
            if (e.Parameter != null && e.Parameter.ToString() == "autologin")
            {
                LoginProgressBar.Visibility = Visibility.Visible;
                LoginTextBlock.Visibility = Visibility.Visible;
                LoginStackPanel.Background = null;

                List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
                paramList.Add(new KeyValuePair<string, string>("username", appSetting.Values["username"].ToString()));
                paramList.Add(new KeyValuePair<string, string>("password", appSetting.Values["password"].ToString()));

                string content = await Util.NetWork.getHttpWebRequest("/public/login", paramList);
                content = Util.Utils.ConvertUnicodeStringToChinese(content);

                if (content != "")
                {
                    JObject obj = JObject.Parse(content);
                    if (Int32.Parse(obj["status"].ToString()) != 200)
                    {
                        Util.Utils.Message(obj["info"].ToString());
                        LoginProgressBar.Visibility = Visibility.Collapsed;
                        LoginTextBlock.Text = "登陆失败...";

                        await Task.Delay(2000);
                        LoginTextBlock.Visibility = Visibility.Collapsed;
                        Frame.Navigate(typeof(LoginPage));
                    }
                    else
                    {
                        appSetting.Values["uid"] = obj["uid"].ToString();
                        appSetting.Values["token"] = obj["token"].ToString();

                        LoginProgressBar.Visibility = Visibility.Collapsed;
                        LoginTextBlock.Text = "登陆成功...";

                        await Task.Delay(2000);
                        LoginTextBlock.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    LoginProgressBar.Visibility = Visibility.Collapsed;
                    LoginStackPanel.Background = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
                    LoginTextBlock.Text = "网络未连接...";

                    await Task.Delay(2000);
                    LoginTextBlock.Visibility = Visibility.Collapsed;
                    LoginStackPanel.Visibility = Visibility.Collapsed;

                }
            }

            //string banner = await Util.NetWork.getHttpWebRequest("/public/banner");
            //if (banner != "")
            //{
            //    JObject obj = JObject.Parse(banner);
            //    if (Int32.Parse(obj["status"].ToString()) != 200)
            //    {
            //        JArray ja = (JArray)JsonConvert.DeserializeObject(banner);
            //    }
            //}
            //else
            //{

            //}
        }


        private void dataFlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }


    }
}
