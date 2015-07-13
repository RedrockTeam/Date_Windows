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
using Date.Data;
using Windows.UI.Core;

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
        List<GradeList> gradelist = new List<GradeList>();
        List<AcademyList> acalist = new List<AcademyList>();
        List<DateType> datetypelist = new List<DateType>();
        List<DateList> mdatelist = new List<DateList>();

        private int order = 0;//约会列表排序选项
        private int page = 1;//约会列表排序选项
        private int size = 40;//约会列表数量
        public bool IsLoading = false;
        public bool IsOver = false;
        private object o = new object();
        


        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            appSetting = ApplicationData.Current.LocalSettings; //本地存储



            dateScrollViewer.Height = Utils.getPhoneHeight() - 85 - 85;

            getAcademyInfor(); //获取学院列表
            getGradeInfor(); //获取年级列表
            getDatetypeInfor(); //获取约分类列表
            getDatelist(0); //获取约列表


            _timer.Interval = TimeSpan.FromSeconds(7.0);
            InitFlipView();
            _timer.Tick += ChangeImage;

            dateListView.ContainerContentChanging += dateListView_ContainerContentChanging;
        }

        /// <summary>
        /// 获取约会列表
        /// </summary>
        /// <param name="date_type"></param>
        /// <param name="page"></param>
        /// <param name="order"></param>
        private async void getDatelist(int date_type, int page = 1, int order = 0, bool isrefresh = true)
        {
            if (isrefresh)
            {
                mdatelist.Clear();
            }

            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<string, string>("uid", appSetting.Values["uid"].ToString()));
            paramList.Add(new KeyValuePair<string, string>("token", appSetting.Values["token"].ToString()));
            paramList.Add(new KeyValuePair<string, string>("date_type", date_type.ToString()));
            paramList.Add(new KeyValuePair<string, string>("page", page.ToString()));
            paramList.Add(new KeyValuePair<string, string>("order", order.ToString()));
            paramList.Add(new KeyValuePair<string, string>("size", size.ToString()));

            string datelist = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/date/datelist", paramList));
            Debug.WriteLine("datelist" + datelist);

            try
            {
                if (datelist != "")
                {
                    JObject obj = JObject.Parse(datelist);
                    if (Int32.Parse(obj["status"].ToString()) == 200)
                    {
                        JArray dateListArray = Utils.ReadJso(datelist);
                        List<DateList> mdatelistuse = new List<DateList>();
                        mdatelist.Clear();
                        for (int i = 0; i < dateListArray.Count; i++)
                        {
                            JObject jobj = (JObject)dateListArray[i];
                            DateList d = new DateList();
                            d.Head = jobj["head"].ToString();
                            d.Nickname = jobj["nickname"].ToString();
                            if (jobj["nickname"].ToString() == "1")
                                d.Gender = "ms-appx:///Assets/ic_man.png";
                            else if ((jobj["nickname"].ToString() == "2"))
                                d.Gender = "ms-appx:///Assets/ic_woman.png";
                            d.Signature = jobj["signature"].ToString();

                                d.Title = jobj["title"].ToString();

                            d.Place = jobj["place"].ToString();
                            d.Date_time = Utils.GetTime(jobj["date_time"].ToString()).ToString();
                            d.Created_at = Utils.GetTime(jobj["created_at"].ToString()).ToString();
                            if (jobj["cost_model"].ToString() == "1")
                                d.Cost_model = "AA";
                            else if ((jobj["cost_model"].ToString() == "2"))
                                d.Cost_model = "你请客";
                            else if ((jobj["cost_model"].ToString() == "3"))
                                d.Cost_model = "我买单";
                            d.Date_type = jobj["date_type"].ToString();
                            if (isrefresh)
                                mdatelistuse.Add(d);
                            else
                                mdatelist.Add(d);
                        }
                        if (isrefresh)
                            dateListView.ItemsSource = mdatelistuse;
                        else
                        {
                            if (mdatelist.Count == size)
                            {
                                ListView ll = new ListView();
                                ll.ItemTemplate = dateListView.ItemTemplate;
                                ll.ItemsSource = mdatelist;
                                ll.IsItemClickEnabled = true;

                                ll.ItemClick += dateListView_ItemClick;
                                ll.ContainerContentChanging += dateListView_ContainerContentChanging;
                                dateStackPanel.Children.Add(ll);
                            }
                            else
                                IsOver = true;
                        }
                        DateListProgressStackPanel.Visibility = Visibility.Collapsed;
                        DateListFailedStackPanel.Visibility = Visibility.Collapsed;
                    }
                    else
                        if (isrefresh)
                            DateListFailedStackPanel.Visibility = Visibility.Visible;
                }
                else
                    if (isrefresh)
                        DateListFailedStackPanel.Visibility = Visibility.Visible;
            }
            catch (Exception)
            {
                DateListFailedStackPanel.Visibility = Visibility.Visible;
            }
        }

        void dateListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            lock (o)
            {
                if (!IsOver)
                {
                    if (!IsLoading)
                    {
                        if (args.ItemIndex == dateListView.Items.Count - 1)
                        {
                            IsLoading = true;
                            Task.Factory.StartNew(async () =>
                            {
                                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                {

                                    getDatelist(0, ++page, order, false);

                                });
                                IsLoading = false;
                            });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取约会类型
        /// </summary>
        private async void getDatetypeInfor()
        {

            //约会类型
            string datetype = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/date/datetype", new List<KeyValuePair<String, String>>()));
            Debug.WriteLine("datetype" + datetype);
            if (datetype != "")
            {
                JArray datetypeArray = Utils.ReadJso(datetype);

                for (int i = 0; i < datetypeArray.Count; i++)
                {
                    JObject jobj = (JObject)datetypeArray[i];
                    var b = new DateType();
                    b.Id = Convert.ToInt32(jobj["id"].ToString());
                    b.Type = jobj["type"].ToString();
                    datetypelist.Add(b);
                }
                appSetting.Values["datetype_json"] = datetype;
            }

            //TODO:List键值对查询
            //DateType dateType = datetypelist.Find(p => p.Type.Equals("打牌"));
            //Debug.WriteLine(dateType.Id.ToString());
        }

        /// <summary>
        /// 获取年级列表
        /// </summary>
        private async void getGradeInfor()
        {
            //年级
            string grade = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/public/grade", new List<KeyValuePair<String, String>>()));
            Debug.WriteLine("grade" + grade);
            if (grade != "")
            {
                JArray gradeArray = Utils.ReadJso(grade);
                for (int i = 0; i < gradeArray.Count; i++)
                {
                    JObject jobj = (JObject)gradeArray[i];
                    var b = new GradeList
                    {
                        Id = Convert.ToInt32(jobj["id"].ToString()),
                        Name = jobj["name"].ToString()
                    };
                    gradelist.Add(b);
                }
                appSetting.Values["grade_json"] = grade;
            }
        }

        /// <summary>
        /// 获取学院列表
        /// </summary>
        private async void getAcademyInfor()
        {
            string academy = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/public/academy", new List<KeyValuePair<String, String>>()));
            Debug.WriteLine("academy" + academy);
            if (academy != "")
            {
                JArray academyArray = Utils.ReadJso(academy);
                for (int i = 0; i < academyArray.Count; i++)
                {
                    JObject jobj = (JObject)academyArray[i];
                    var b = new AcademyList
                    {
                        Id = Convert.ToInt32(jobj["id"].ToString()),
                        Name = jobj["name"].ToString()
                    };
                    acalist.Add(b);
                }
                appSetting.Values["academy_json"] = academy;
            }
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
                StatusTextBlock.Text = "";
                StatusStackPanel.Background = new SolidColorBrush(Color.FromArgb(255, 239, 239, 239));
            }
            else
                Application.Current.Exit();
        }

        private async void Login()
        {

            StatusProgressBar.Visibility = Visibility.Visible;
            StatusTextBlock.Visibility = Visibility.Visible;
            StatusStackPanel.Background = new SolidColorBrush(Color.FromArgb(255, 239, 239, 239));

            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<string, string>("username", appSetting.Values["username"].ToString()));
            paramList.Add(new KeyValuePair<string, string>("password", appSetting.Values["password"].ToString()));

            string login = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/public/login", paramList));
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
                    StatusTextBlock.Text = "";
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
                    StatusTextBlock.Text = "";

                }
            }
            else
            {
                StatusProgressBar.Visibility = Visibility.Collapsed;
                StatusStackPanel.Background = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
                StatusTextBlock.Text = "网络未连接...";

                await Task.Delay(2000);
                StatusTextBlock.Text = "";
                StatusStackPanel.Background = new SolidColorBrush(Color.FromArgb(255, 239, 239, 239));
                Frame.Navigate(typeof(LoginPage));

            }
        }

        private async void InitFlipView()
        {
            string banner = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/public/banner", new List<KeyValuePair<String, String>>()));
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

        private void typeTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MenuFlyout typeMenuFlyout = new MenuFlyout();
            bool isType = true;
            try
            {
                string datetype = appSetting.Values["datetype_json"].ToString();
                if (datetype != "")
                {
                    JArray datetypeArray = Utils.ReadJso(datetype);
                    datetypelist.Add(new DateType { Id = 0, Type = "全部分类" });
                    for (int i = 0; i < datetypeArray.Count; i++)
                    {
                        JObject jobj = (JObject)datetypeArray[i];
                        typeMenuFlyout.Items.Add(getTypeMenuFlyoutItem(jobj["type"].ToString()));
                        var b = new DateType();
                        datetypelist.Add(new DateType { Id = Convert.ToInt32(jobj["id"].ToString()), Type = jobj["type"].ToString() });
                    }
                    typeMenuFlyout.ShowAt(typeTextBlock);
                }
                else
                    isType = false;
            }
            catch (Exception)
            {
                isType = false;
            }
            if (!isType)
            {
                showStatus("分类数据加载失败", 1, Visibility.Collapsed);
                getDatetypeInfor();
            }
        }

        private MenuFlyoutItem getTypeMenuFlyoutItem(string text)
        {
            MenuFlyoutItem menuFlyoutItem = new MenuFlyoutItem();
            menuFlyoutItem.Text = text;
            menuFlyoutItem.Click += typeMenuFlyoutItem_click;
            return menuFlyoutItem;
        }

        private void typeMenuFlyoutItem_click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = sender as MenuFlyoutItem;
            typeTextBlock.Text = menuFlyoutItem.Text;
        }

        private void sortTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MenuFlyout sortMenuFlyout = new MenuFlyout();
            sortMenuFlyout.Items.Add(getsortMenuFlyoutItem("默认排序"));
            sortMenuFlyout.Items.Add(getsortMenuFlyoutItem("创建时间"));
            sortMenuFlyout.Items.Add(getsortMenuFlyoutItem("剩余时间"));
            sortMenuFlyout.Items.Add(getsortMenuFlyoutItem("参与人数"));
            sortMenuFlyout.Items.Add(getsortMenuFlyoutItem("用户信用"));
            sortMenuFlyout.ShowAt(sortTextBlock);

        }

        private MenuFlyoutItem getsortMenuFlyoutItem(string text)
        {
            MenuFlyoutItem menuFlyoutItem = new MenuFlyoutItem();
            menuFlyoutItem.Text = text;
            menuFlyoutItem.Click += sortMenuFlyoutItem_click;
            return menuFlyoutItem;
        }

        private async void sortMenuFlyoutItem_click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = sender as MenuFlyoutItem;
            if (sortTextBlock.Text != menuFlyoutItem.Text)
            {
                sortTextBlock.Text = menuFlyoutItem.Text;
                switch (menuFlyoutItem.Text)
                {
                    case "默认排序":
                        order = 0;
                        break;
                    case "创建时间":
                        order = 1;
                        break;
                    case "剩余时间":
                        order = 2;
                        break;
                    case "参与人数":
                        order = 3;
                        break;
                    case "用户信用":
                        order = 4;
                        break;
                }
                page = 1;
                List<DateList> mdatelist = new List<DateList>();
                dateListView.ItemsSource = mdatelist;
                DateListProgressStackPanel.Visibility = Visibility.Visible;
                getDatelist(0, 1, order);
            }
        }

        private async void showStatus(string text, int value, Windows.UI.Xaml.Visibility ProgressBarvisibility)
        {
            StatusStackPanel.Visibility = Visibility.Visible;
            StatusProgressBar.Visibility = ProgressBarvisibility;
            StatusTextBlock.Visibility = Visibility.Visible;
            if (value == 1)
                StatusStackPanel.Background = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
            else
                StatusStackPanel.Background = new SolidColorBrush(Color.FromArgb(255, 239, 239, 239));
            StatusTextBlock.Text = text;

            if (ProgressBarvisibility == Visibility.Collapsed)
            {
                await Task.Delay(2000);
                StatusTextBlock.Text = ""; ;
                StatusStackPanel.Visibility = Visibility.Collapsed;
                StatusProgressBar.Visibility = Visibility.Collapsed;
                StatusStackPanel.Background = new SolidColorBrush(Color.FromArgb(255, 239, 239, 239));
            }
        }

        /// <summary>
        /// DateList刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            page = 1;
            DateListProgressStackPanel.Visibility = Visibility.Visible;
            List<DateList> mdatelist = new List<DateList>();
            dateListView.ItemsSource = mdatelist;
            getDatelist(0, 1, order);
        }

        /// <summary>
        /// 失败重试点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DateListFailedStackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            DateListProgressStackPanel.Visibility = Visibility.Visible;
            DateListFailedStackPanel.Visibility = Visibility.Collapsed;
            getDatelist(0, 1, order);
        }

        private void dateListView_ItemClick(object sender, ItemClickEventArgs e)
        {

            Debug.WriteLine("你点击了：" + ((DateList)e.ClickedItem).Title);
        }




    }
}
