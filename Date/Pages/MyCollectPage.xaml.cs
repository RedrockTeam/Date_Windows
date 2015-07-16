using Date.Data;
using Date.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UmengSDK;
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
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace Date
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MyCollectPage : Page
    {
        private ApplicationDataContainer appSetting;
        List<DateList> mdatelist = new List<DateList>();
        private int ch;
        public MyCollectPage()
        {
            appSetting = ApplicationData.Current.LocalSettings; //本地存储
            this.InitializeComponent();
            MyCollectScrollViewer.Height = Utils.getPhoneHeight() - 60 - 20;
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;//注册重写后退按钮事件
            UmengAnalytics.TrackPageStart("MyCollectPage");
            if ((Int32)e.Parameter == 1)
                TitleTextBlock.Text = "我的收藏";
            else
                TitleTextBlock.Text = "我的约过";
            ch = (Int32)e.Parameter;
            if (e.NavigationMode == NavigationMode.Back)
            {
                getMyCollect(ch, 2);
            }
            else
                getMyCollect(ch, 1);
        }

        //离开页面时，取消事件
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;//注册重写后退按钮事件
            UmengAnalytics.TrackPageEnd("MyCollectPage");
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

        /// <summary>
        /// 收藏的网络请求
        /// </summary>
        private async void getMyCollect(int ch, int cc)
        {
            string collect = "";
            DateListProgressStackPanel.Visibility = Visibility.Visible;
            if (cc == 1)
            {
                List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
                paramList.Add(new KeyValuePair<string, string>("uid", appSetting.Values["uid"].ToString()));
                paramList.Add(new KeyValuePair<string, string>("token", appSetting.Values["token"].ToString()));
                if (ch == 1)
                    collect = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/person/collection", paramList));
                else
                    collect = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/person/join", paramList));
                App.CacheString = collect;
            }
            else
                collect = App.CacheString;
            Debug.WriteLine("collect" + collect);

            try
            {
                if (collect != "")
                {
                    JObject obj = JObject.Parse(collect);
                    if (Int32.Parse(obj["status"].ToString()) == 200)
                    {
                        JArray dateListArray = Utils.ReadJso(collect);
                        for (int i = 0; i < dateListArray.Count; i++)
                        {
                            JObject jobj = (JObject)dateListArray[i];
                            DateList d = new DateList();
                            d.Date_id = (Int32)jobj["date_id"];
                            d.Head = jobj["head"].ToString();
                            d.Nickname = jobj["nickname"].ToString();
                            if (jobj["gender"].ToString() == "1")
                                d.Gender = "ms-appx:///Assets/ic_man.png";
                            else if ((jobj["gender"].ToString() == "2"))
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
                            else
                                d.Cost_model = "AA";
                            mdatelist.Add(d);
                        }
                        dateListView.ItemsSource = mdatelist;
                        DateListProgressStackPanel.Visibility = Visibility.Collapsed;
                        DateListFailedStackPanel.Visibility = Visibility.Collapsed;
                    }
                    else
                        DateListFailedStackPanel.Visibility = Visibility.Visible;
                }
                else
                    DateListFailedStackPanel.Visibility = Visibility.Visible;
            }
            catch (Exception)
            {
                Debug.WriteLine("主页，列表网络异常");
                DateListFailedStackPanel.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 请求失败点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DateListFailedStackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            getMyCollect(ch,1);
        }

        private void dateListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Debug.WriteLine("你点击了：" + ((DateList)e.ClickedItem).Title);
            DateList datelistNavigate = new DateList(((DateList)e.ClickedItem).Date_id, ((DateList)e.ClickedItem).Head, ((DateList)e.ClickedItem).Nickname, ((DateList)e.ClickedItem).Gender, ((DateList)e.ClickedItem).Signature, ((DateList)e.ClickedItem).Title, ((DateList)e.ClickedItem).Place, ((DateList)e.ClickedItem).Date_time, ((DateList)e.ClickedItem).Cost_model);
            Frame.Navigate(typeof(DetailDatePage), datelistNavigate);
        }
    }
}
