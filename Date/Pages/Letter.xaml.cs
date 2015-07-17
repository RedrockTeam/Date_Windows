using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using Date.Data;
using Date.Util;
using Newtonsoft.Json.Linq;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace Date.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Letter : Page
    {
        private ApplicationDataContainer appSetting;
        ObservableCollection<DateLetter> dl = new ObservableCollection<DateLetter>();
        private int letterpage = 1;
        public Letter()
        {
            this.InitializeComponent();
            appSetting = ApplicationData.Current.LocalSettings; //本地存储
            MyLitterScrollViewer.Height = Utils.getPhoneHeight() - 60 - 20;
        }



        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;//注册重写后退按钮事件
            UmengSDK.UmengAnalytics.TrackPageStart("Letter");

            if (e.NavigationMode == NavigationMode.Back)
            {
                getLetter(1);
            }
            else
                getLetter(1);

        }

        private async void getLetter(int cc, int page = 1)
        {
            DateListProgressStackPanel.Visibility = Visibility.Visible;
            DateListFailedStackPanel.Visibility = Visibility.Collapsed;
            string content = "";
            if (cc == 1)
            {
                List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
                paramList.Add(new KeyValuePair<string, string>("uid", appSetting.Values["uid"].ToString()));
                paramList.Add(new KeyValuePair<string, string>("token", appSetting.Values["token"].ToString()));
                paramList.Add(new KeyValuePair<string, string>("page", page.ToString()));
                paramList.Add(new KeyValuePair<string, string>("size","10"));
                content = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/letter/getletter", paramList));
                Debug.WriteLine("content" + content);
            }
            else if (cc == 2)
                content = App.CacheString;
            try
            {
                if (content != "")
                {
                    JObject obj = JObject.Parse(content);
                    if (Int32.Parse(obj["status"].ToString()) == 200)
                    {
                        JArray dateListArray = Utils.ReadJso(content);
                        for (int i = 0; i < dateListArray.Count; i++)
                        {
                            DateLetter d = new DateLetter();
                            JObject jobj = (JObject)dateListArray[i];
                            d.GetAttribute(jobj);
                            dl.Add(d);
                        }
                        if(dl.Count<=10)
                        this.letterListView.ItemsSource = dl;
                        App.CacheString = content;
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
                DateListProgressStackPanel.Visibility = Visibility.Collapsed;
                Debug.WriteLine("私信，列表网络异常");
                DateListFailedStackPanel.Visibility = Visibility.Visible;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;//注册重写后退按钮事件
            UmengSDK.UmengAnalytics.TrackPageEnd("Letter");
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

        private void LetterListView_OnItemClickListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(AcceptOrReject), e.ClickedItem as DateLetter);
        }

        private void DateListFailedStackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            getLetter(1);
        }

        private async void AllReadButton_Click(object sender, RoutedEventArgs e)
        {
            string content = "";
            DateListProgressStackPanel.Visibility = Visibility.Visible;
            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<string, string>("uid", appSetting.Values["uid"].ToString()));
            paramList.Add(new KeyValuePair<string, string>("token", appSetting.Values["token"].ToString()));
            for (int i = 0; i < dl.Count; i++)
            {
                if (dl[i].Letter_status == 1)
                {
                    paramList.Add(new KeyValuePair<string, string>("letter_id", dl[i].Letter_id.ToString()));
                    content = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/letter/detailletter", paramList));
                    Debug.WriteLine("content" + content);
                }
            }
            if (letterpage == 1)
            {
                getLetter(1);
            }
            else
            {
                getLetter(1, letterpage - 1);
            }
            DateListProgressStackPanel.Visibility = Visibility.Collapsed;
        }

        private void MoreButton_Click(object sender, RoutedEventArgs e)
        {
            int count = dl.Count;
            DateListProgressStackPanel.Visibility = Visibility.Visible;
            getLetter(1,++letterpage);
            DateListProgressStackPanel.Visibility = Visibility.Visible;
            if (dl.Count - count < 10)
            {
                NomoreItemstip.Text = "没有啦>_<";
                NomoreItemstip.Visibility=Visibility.Visible;
            }
        }

       
    }
}
