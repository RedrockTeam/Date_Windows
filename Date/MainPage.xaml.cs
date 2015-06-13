using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.Phone.UI.Input;
using Date.DataModel;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=391641 上有介绍



namespace Date
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ApplicationDataContainer appSetting;
        ObservableCollection<Banner>  BannerList=new ObservableCollection<Banner>();
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
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;//注册重写后退按钮事件
            if (!isLogin && e.Parameter != null && e.Parameter.ToString() == "autologin")
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
                        isLogin = true;

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

            string banner = await Util.NetWork.getHttpWebRequest("/public/banner");

            if (banner != "")
            {
                JObject obj = JObject.Parse(banner);
                if (Int32.Parse(obj["status"].ToString()) != 200)
                {
                    JObject jObject = (JObject)JsonConvert.DeserializeObject(banner);
                    string json = jObject["data"].ToString();
                    JArray jArray = (JArray)JsonConvert.DeserializeObject(json);
                    for (int i = 0; i < jArray.Count; i++)
                    {
                        JObject jobj = (JObject) jArray[i];
                        var b=new Banner();
                        b.Url = jobj["url"].ToString();
                        b.Src = jobj["src"].ToString();
                        BannerList.Add(b);
                    }
                }
            }
            else
            {

            }


            InitFlipView();

        }

        private void InitFlipView()
        {
            List<Mode.FlipViewThing> flipViewThing = new List<Mode.FlipViewThing>
            {
                new Mode.FlipViewThing{FlipViewImageSource = "Assets/bg_nav3.jpg"},
                new Mode.FlipViewThing{FlipViewImageSource = "Assets/bg_nav.jpg"},  
                new Mode.FlipViewThing{FlipViewImageSource = "Assets/bg_nav2.jpg"},  
                new Mode.FlipViewThing{FlipViewImageSource = "Assets/bg_nav3.jpg"},
                new Mode.FlipViewThing{FlipViewImageSource = "Assets/bg_nav.jpg"}
            };
            dataFlipView.ItemsSource = flipViewThing;
            dataFlipView.SelectedIndex = 1;
        }

        //离开页面时，取消事件
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;//注册重写后退按钮事件
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


        private void dataFlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataFlipView.SelectedIndex == ((List<Mode.FlipViewThing>)dataFlipView.ItemsSource).Count - 1)
            {
                dataFlipView.SelectedIndex = 1;
            }
            if (dataFlipView.SelectedIndex == 0)
            {
                dataFlipView.SelectedIndex = ((List<Mode.FlipViewThing>)dataFlipView.ItemsSource).Count - 2;
            }
        }

        private void grzxGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(grzxPage));
        }


    }
}
