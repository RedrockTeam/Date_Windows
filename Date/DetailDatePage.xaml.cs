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

namespace Date
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DetailDatePage : Page
    {
        private ApplicationDataContainer appSetting;
        DateDetail dd=new DateDetail();
        public DetailDatePage()
        {
            appSetting = ApplicationData.Current.LocalSettings; //本地存储
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;//注册重写后退按钮事件
            UmengSDK.UmengAnalytics.TrackPageStart("DetailDatePage");

            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<string, string>("uid", appSetting.Values["uid"].ToString()));
            paramList.Add(new KeyValuePair<string, string>("date_id", "118"));
            paramList.Add(new KeyValuePair<string, string>("token", appSetting.Values["token"].ToString()));
            string pc = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/date/detaildate", paramList));
            Debug.WriteLine("个人信息" + pc);
            if (pc != "")
            {
                JObject obj = JObject.Parse(pc);
                if (Int32.Parse(obj["status"].ToString()) == 200)
                {
                    dd.GetAttribute(obj);
                    JArray mydatelist = JArray.Parse(obj["data"]["mydate"].ToString());
                    for (int i = 0; i < mydatelist.Count; i++)
                    {
                        JObject temp = JObject.Parse(mydatelist[i].ToString());
                        MyDate md = new MyDate();
                        md.GetAttribute(temp);
                        //MyDates.Add(md);

                    }
                }

            }







        }

        //离开页面时，取消事件
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;//注册重写后退按钮事件
            UmengSDK.UmengAnalytics.TrackPageEnd("DetailDatePage");
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

    }
}
