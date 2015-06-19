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
        private bool isExit = false; //用于双击退出
        List<Banner> BannerList = new List<Banner>();
        private string hubSectionChange = "DateListHubSection";
        DispatcherTimer _timer = new DispatcherTimer();//定义一个定时器

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            appSetting = ApplicationData.Current.LocalSettings; //本地存储


            _timer.Interval = TimeSpan.FromSeconds(7.0);
            InitFlipView();
            _timer.Tick += ChangeImage;


        }

        private void ChangeImage(object sender, object e)
        {
            try
            {
                dataFlipView.SelectionChanged -= dataFlipView_SelectionChanged;
                if (dataFlipView.Items != null && dataFlipView.Items.Count > 1 && dataFlipView.SelectedIndex < dataFlipView.Items.Count - 1)
                {

                    dataFlipView.SelectedIndex++;
                }
                else
                {
                    dataFlipView.SelectedIndex = 1;
                }
                Debug.WriteLine(dataFlipView.SelectedIndex);
                dataFlipView.SelectionChanged += dataFlipView_SelectionChanged;
            }
            catch (Exception) { }
        }



        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await Utils.ShowSystemTrayAsync(Color.FromArgb(255, 255, 61, 61), Colors.White, text: "约");

            Frame.BackStack.Clear();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;//注册重写后退按钮事件
            if (!isLogin && e.Parameter != null && e.Parameter.ToString() == "autologin")
            {
                Login();
            }
            _timer.Start();
            UmengSDK.UmengAnalytics.TrackPageStart("MainPage");
        }


        //离开页面时，取消事件
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;//注册重写后退按钮事件
            UmengSDK.UmengAnalytics.TrackPageEnd("MainPage");
            _timer.Stop();
        }
        private async void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)//重写后退按钮，如果要对所有页面使用，可以放在App.Xaml.cs的APP初始化函数中重写。
        {
            e.Handled = true;
            if (!isExit)
            {
                isExit = true;
                StatusStackPanel.Background = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
                StatusTextBlock.Visibility = Visibility.Visible;
                StatusTextBlock.Text = "再次点击返回键退出...";
                await Task.Delay(2000);
                isExit = false;
                StatusTextBlock.Visibility = Visibility.Collapsed;
                StatusStackPanel.Background = null;
            }
            else
                Application.Current.Exit();
        }

        private async void Login()
        {

            StatusProgressBar.Visibility = Visibility.Visible;
            StatusTextBlock.Visibility = Visibility.Visible;
            StatusStackPanel.Background = null;

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
                    StatusProgressBar.Visibility = Visibility.Collapsed;
                    StatusTextBlock.Text = "登陆失败...";

                    await Task.Delay(2000);
                    StatusTextBlock.Visibility = Visibility.Collapsed;
                    Frame.Navigate(typeof(LoginPage));
                }
                else
                {
                    appSetting.Values["uid"] = obj["uid"].ToString();
                    appSetting.Values["token"] = obj["token"].ToString();
                    isLogin = true;

                    StatusProgressBar.Visibility = Visibility.Collapsed;
                    StatusTextBlock.Text = "登陆成功...";

                    await Task.Delay(2000);
                    StatusTextBlock.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                StatusProgressBar.Visibility = Visibility.Collapsed;
                StatusStackPanel.Background = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
                StatusTextBlock.Text = "网络未连接...";

                await Task.Delay(2000);
                StatusTextBlock.Visibility = Visibility.Collapsed;

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
                    if (Math.Abs(HoldPlaceImg.Opacity) > 0)
                    {
                        Hideimg.Begin();
                    }
                    else
                    {
                        HoldPlaceImg.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                }
                try
                {
                    dataFlipView.ItemsSource = BannerList;
                    //HoldPlaceImg.Visibility = Visibility.Collapsed;
                    dataFlipView.SelectedIndex = 1;
                }
                catch (Exception) { }
            }
        }



        private void dataFlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var itemsSource = (List<Banner>)dataFlipView.ItemsSource;
            if (itemsSource == null || itemsSource.Count == 1) return;
            else
            {
                Debug.WriteLine(dataFlipView.SelectedIndex);

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
