﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using CacheModeDemo.Common;
using Date.Data;
using Date.Pages;
using Date.Util;
using Newtonsoft.Json.Linq;
using UmengSDK;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace Date
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class grzxPage : Page
    {
        private ApplicationDataContainer appSetting;
        private FileOpenPickerContinuationEventArgs _filePickerEventArgs = null;
        List<MyDate> MyDates = new List<MyDate>();
        PersonInfo pi = new PersonInfo();
        public grzxPage()
        {
            appSetting = ApplicationData.Current.LocalSettings; //本地存储
            this.InitializeComponent();
        }

        protected  override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;//注册重写后退按钮事件
            UmengAnalytics.TrackPageStart("grzxPage");
            if (e.NavigationMode == NavigationMode.Back)
            {
                GetPerInfo(2);
                if (App.gotoPage == "detail")
                    MyCenter.SelectedIndex = 1;
                else
                    MyCenter.SelectedIndex = 0;
            }
            else
            {
                GetPerInfo(1);
                MyCenter.SelectedIndex = 0;
            }
            SetHead();
            if (Frame.BackStack[Frame.BackStackDepth - 1].SourcePageType.ToString() == "Date.Pages.EditInfo")
            {
                Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
                Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
            }
        }

        private void SetHead()
        {
            if (appSetting.Values.ContainsKey("head") || (appSetting.Values.ContainsKey("IsHeadExistOffline") && bool.Parse(appSetting.Values["IsHeadExistOffline"].ToString())))
            {
                SetHeadPage();
            }
            else
            {
                try
                {
                    img.ImageSource = new BitmapImage(new Uri(pi.Head));
                }
                catch (Exception)
                {
                    Debug.WriteLine("头像设置异常");
                }
            }

        }
        private async void SetHeadPage()
        {
            try
            {
                nkname.Text = appSetting.Values["nickname"].ToString();
                IStorageFolder applicationFolder = ApplicationData.Current.LocalFolder;
                IStorageFile storageFileRE = await applicationFolder.GetFileAsync("head.png");
                img.ImageSource = new BitmapImage(new Uri(storageFileRE.Path));
            }
            catch (Exception)
            {
            }
        }
        //离开页面时，取消事件
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;//注册重写后退按钮事件
            UmengAnalytics.TrackPageEnd("grzxPage");

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

        //退出登录
        private void logoutAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            appSetting.Values.Remove("username");
            appSetting.Values.Remove("password");
            appSetting.Values.Remove("uid");
            appSetting.Values.Remove("token");
            appSetting.Values.Remove("nickname");
            appSetting.Values.Remove("head");
            appSetting.Values.Remove("IsHeadExistOffline");
            Frame.Navigate(typeof(LoginPage));
        }

        private void headAppBarButton_Click(object sender, RoutedEventArgs e)
        {

            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".bmp");
            openPicker.FileTypeFilter.Add(".gif");
            openPicker.ContinuationData["Operation"] = "img";

            openPicker.PickSingleFileAndContinue();

        }

        public FileOpenPickerContinuationEventArgs FilePickerEvent
        {
            get { return _filePickerEventArgs; }
            set
            {
                _filePickerEventArgs = value;
                ContinueFileOpenPicker(_filePickerEventArgs);
            }
        }

        public void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            if ((args.ContinuationData["Operation"] as string) == "img" && args.Files != null && args.Files.Count > 0)
            {
                StorageFile file = args.Files[0];
                App.gotoPage = "head";
                Frame.Navigate(typeof(SetHeadPage), file);
                BitmapImage bitmapImage = new BitmapImage(new Uri(file.Path));
                img.ImageSource = bitmapImage;
                //IsHeadExistOffline 
                appSetting.Values["IsHeadExistOffline"] = true;

            }

        }

        private async void GetPerInfo(int cc)
        {
            string pc = "";
            if (cc == 1)
            {
                List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
                paramList.Add(new KeyValuePair<string, string>("uid", appSetting.Values["uid"].ToString()));
                paramList.Add(new KeyValuePair<string, string>("get_uid", appSetting.Values["uid"].ToString()));
                paramList.Add(new KeyValuePair<string, string>("token", appSetting.Values["token"].ToString()));
                pc = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/person/userinfo", paramList));
                App.CacheString2 = pc;
            }
            else
                pc = App.CacheString2;
            Debug.WriteLine("个人信息" + pc);
            if (pc != "")
            {
                JObject obj = JObject.Parse(pc);
                if (Int32.Parse(obj["status"].ToString()) == 200)
                {
                    pi.GetAttribute(obj);
                    if (appSetting.Values["savehead"] == null)
                    {
                        Size downloadSize = new Size(ico.Width, ico.Height);
                        await Utils.DownloadAndScale("head.png", pi.Head, downloadSize);
                        appSetting.Values["savehead"] = "1";
                    }
                    if (appSetting.Values["head"].ToString() != pi.Head)
                    {
                        appSetting.Values["head"] = pi.Head;
                        Size downloadSize = new Size(ico.Width, ico.Height);
                        await Utils.DownloadAndScale("head.png", pi.Head, downloadSize);
                    }
                    if (appSetting.Values["nickname"].ToString() != pi.Nickname)
                    {
                        appSetting.Values["nickname"] = pi.Nickname;
                        nkname.Text = appSetting.Values["nickname"].ToString();
                    }

                    JArray mydatelist = JArray.Parse(obj["data"]["mydate"].ToString());
                    for (int i = 0; i < mydatelist.Count; i++)
                    {
                        JObject temp = JObject.Parse(mydatelist[i].ToString());
                        MyDate md = new MyDate();
                        md.GetAttribute(temp);
                        MyDates.Add(md);
                    }
                }
                this.DataContext = pi;
                MyDatesList.ItemsSource = MyDates;
                if (img.ImageSource == null)
                    SetHead();
                //img.ImageSource = new BitmapImage(new Uri(pi.Head));
            }
            else
            {
                await new MessageDialog("请检查网络！").ShowAsync();
            }
        }

        private void MyCenter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MyCenter.SelectedIndex == 0)
            {
                BaseInfo.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                DateHistory.Foreground = new SolidColorBrush(Color.FromArgb(255, 220, 220, 220));

            }
            else
            {
                BaseInfo.Foreground = new SolidColorBrush(Color.FromArgb(255, 220, 220, 220));
                DateHistory.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

            }
        }

        private void MyDatesList_ItemClick(object sender, ItemClickEventArgs e)
        {
            Debug.WriteLine("你点击了：" + ((MyDate)e.ClickedItem).Title);
            DateList datelistNavigate = new DateList(Int32.Parse(((MyDate)e.ClickedItem).Date_id), ((MyDate)e.ClickedItem).Head, ((MyDate)e.ClickedItem).Nickname, ((MyDate)e.ClickedItem).Gender, "加载中...", ((MyDate)e.ClickedItem).Title, ((MyDate)e.ClickedItem).Place, ((MyDate)e.ClickedItem).Date_time, ((MyDate)e.ClickedItem).Cost_model);
            App.gotoPage = "detail";
            Frame.Navigate(typeof(DetailDatePage), datelistNavigate);
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            App.gotoPage = "edit";
            Frame.Navigate(typeof(EditInfo), pi);
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            GetPerInfo(1);
        }
    }
}

