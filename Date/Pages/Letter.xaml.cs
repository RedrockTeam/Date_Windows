using System;
using System.Collections.Generic;
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
        List<DateLetter>  dl=new List<DateLetter>();

        public Letter()
        {
            this.InitializeComponent();
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

            getLetter();
            
        }

        private async void getLetter()
        {
            DateListProgressStackPanel.Visibility = Visibility.Visible;
            DateListFailedStackPanel.Visibility = Visibility.Collapsed;

            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<string, string>("uid", appSetting.Values["uid"].ToString()));
            paramList.Add(new KeyValuePair<string, string>("token", appSetting.Values["token"].ToString()));
            paramList.Add(new KeyValuePair<string, string>("page", "1"));
            paramList.Add(new KeyValuePair<string, string>("size", "10"));
            string content = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/letter/getletter", paramList));
            Debug.WriteLine("content" + content);
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
                        this.letterListView.ItemsSource = dl;

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
            Frame.Navigate(typeof (AcceptOrReject), e.ClickedItem as DateLetter);
        }

        private void DateListFailedStackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            getLetter();
        }
    }
}
