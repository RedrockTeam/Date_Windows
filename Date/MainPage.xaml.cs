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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Phone.UI.Input;
using System.Diagnostics;
using Date.DataModel;
using Date.Util;
using System.Collections.ObjectModel;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=391641 上有介绍



namespace Date
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ApplicationDataContainer appSetting;
        private bool isLogin = false;
        List<Banner> BannerList = new List<Banner>();
        private string hubSectionChange = "DateListHubSection";

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            appSetting = ApplicationData.Current.LocalSettings; //本地存储


        }



        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await Util.Utils.ShowSystemTrayAsync(Colors.Red, Colors.White, text: "约");
            //这里要改，注销再登陆，按返回，又要登录了
            Frame.BackStack.Clear();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;//注册重写后退按钮事件
            if (!isLogin && e.Parameter != null && e.Parameter.ToString() == "autologin")
            {
                Login();
            }



            InitFlipView();

        }

        private async void Login()
        {

            LoginProgressBar.Visibility = Visibility.Visible;
            LoginTextBlock.Visibility = Visibility.Visible;
            LoginStackPanel.Background = null;

            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<string, string>("username", appSetting.Values["username"].ToString()));
            paramList.Add(new KeyValuePair<string, string>("password", appSetting.Values["password"].ToString()));

            string login = await NetWork.getHttpWebRequest("/public/login", paramList);
            login = Util.Utils.ConvertUnicodeStringToChinese(login);
            Debug.WriteLine("login" + login);

            if (login != "")
            {
                JObject obj = JObject.Parse(login);
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

        private async void InitFlipView()
        {
            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            string banner = await NetWork.getHttpWebRequest("/public/banner", paramList);
            Debug.WriteLine("banner" + banner);
            if (banner != "")
            {
                JObject obj = JObject.Parse(banner);
                if (Int32.Parse(obj["status"].ToString()) == 200)
                {
                    JObject jObject = (JObject)JsonConvert.DeserializeObject(banner);
                    string json = jObject["data"].ToString();
                    JArray jArray = (JArray)JsonConvert.DeserializeObject(json);
                    BannerList.Add(new Banner { Url = ((JObject)jArray[jArray.Count - 1])["url"].ToString(), Src = ((JObject)jArray[jArray.Count - 1])["src"].ToString() });
                    for (int i = 0; i < jArray.Count; i++)
                    {
                        JObject jobj = (JObject)jArray[i];
                        var b = new Banner();
                        b.Url = jobj["url"].ToString();
                        b.Src = jobj["src"].ToString();
                        BannerList.Add(b);
                    }
                    BannerList.Add(new Banner { Url = ((JObject)jArray[0])["url"].ToString(), Src = ((JObject)jArray[0])["src"].ToString() });
                    Hideimg.Begin();
                }
            }
            else
            {

            }
            dataFlipView.ItemsSource = BannerList;
            //HoldPlaceImg.Visibility = Visibility.Collapsed;
            dataFlipView.SelectedIndex = 1;
        }

        //离开页面时，取消事件
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;//注册重写后退按钮事件
        }
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)//重写后退按钮，如果要对所有页面使用，可以放在App.Xaml.cs的APP初始化函数中重写。
        {
            Application.Current.Exit();
        }


        private void dataFlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var itemsSource = (List<Banner>)dataFlipView.ItemsSource;
            if (itemsSource == null) return;
            else
            {
                if (dataFlipView.SelectedIndex == itemsSource.Count - 1)
                {
                    dataFlipView.SelectedIndex = 1;
                }
                if (dataFlipView.SelectedIndex == 0)
                {
                    dataFlipView.SelectedIndex = itemsSource.Count - 2;
                }
            }
        }

        private void grzxGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(grzxPage));
        }

        private void DateHub_SectionsInViewChanged(object sender, SectionsInViewChangedEventArgs e)
        {
            var hubSection = DateHub.SectionsInView[0];
            Debug.WriteLine(hubSection.Name);
            CommandBar commandbar = ((CommandBar)this.BottomAppBar);

            if (hubSection.Name != hubSectionChange)
            {
                ((CommandBar)this.BottomAppBar).PrimaryCommands.Clear();
                //((CommandBar)this.BottomAppBar).SecondaryCommands.Clear();
                switch (hubSection.Name)
                {
                    case "DateListHubSection":
                        commandbar.ClosedDisplayMode = AppBarClosedDisplayMode.Compact;
                        AppBarButton BarAddButton = new AppBarButton();
                        BarAddButton.Icon = new SymbolIcon(Symbol.Add);
                        BarAddButton.Label = "新建";
                        BarAddButton.Click += AddAppBarButton_Click;
                        commandbar.PrimaryCommands.Add(BarAddButton);

                        break;
                    case "MeHubSection":
                        commandbar.ClosedDisplayMode = AppBarClosedDisplayMode.Minimal;
                        break;
                    case "MoreHubSection":
                        commandbar.ClosedDisplayMode = AppBarClosedDisplayMode.Minimal;
                        break;
                }
            }
            hubSectionChange = hubSection.Name;
        }

        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddDatePage));
        }




    }
}
