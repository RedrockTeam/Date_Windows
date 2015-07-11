using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
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
    public sealed partial class grzxPage : Page
    {
        private ApplicationDataContainer appSetting;
        private FileOpenPickerContinuationEventArgs _filePickerEventArgs = null;
        private bool IsHeadExistOffline = false;
        private PersonInfo personInfo;
        public grzxPage()
        {
            appSetting = ApplicationData.Current.LocalSettings; //本地存储
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;//注册重写后退按钮事件
            UmengSDK.UmengAnalytics.TrackPageStart("grzxPage");
            SetHead();
        }

        private void SetHead()
        {
            if (IsHeadExistOffline == true)
            {
                SetHeadPage();
            }
            else
            {
                GetHeadImg();
            }
        }
        private async void SetHeadPage()
        {
            try
            {
                IStorageFolder applicationFolder = ApplicationData.Current.LocalFolder;
                IStorageFile storageFileRE = await applicationFolder.GetFileAsync("head.png");
                img.ImageSource = new BitmapImage(new Uri(storageFileRE.Path));
            }
            catch (Exception)
            {
            }
        }

        private async void GetHeadImg()
        {
            try
            {

                List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
                paramList.Add(new KeyValuePair<string, string>("uid", appSetting.Values["uid"].ToString()));
                paramList.Add(new KeyValuePair<string, string>("get_uid", appSetting.Values["uid"].ToString()));
                paramList.Add(new KeyValuePair<string, string>("token", appSetting.Values["token"].ToString()));
                string personinfo = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/person/userinfo", paramList));
                Debug.WriteLine("个人信息" + personinfo);
                if (personinfo != "")
                {
                    JObject obj = JObject.Parse(personinfo);

                        JArray PersoninfoArray = Utils.ReadJso(personinfo);

                        int id = Int32.Parse(obj["id"].ToString());
                        string nickname = obj["nickname"].ToString();
                        string signature = obj["signature"].ToString();
                        string gender = obj["gender"].ToString();
                        string telephone = obj["telephone"].ToString();
                        string grade = obj["grade"].ToString();
                        string academy = obj["academy"].ToString();
                        string qq = obj["qq"].ToString();
                        string weixin = obj["weixin"].ToString();
                        PersonInfo p=new PersonInfo(id,nickname,signature,gender,grade,academy,telephone,qq,weixin);
                        personInfo = p;
                    
                }
            }
            catch (Exception)
            {
            }
        }

        //离开页面时，取消事件
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;//注册重写后退按钮事件
            UmengSDK.UmengAnalytics.TrackPageEnd("grzxPage");

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
            Frame.Navigate(typeof(LoginPage));
        }

        private async void headAppBarButton_Click(object sender, RoutedEventArgs e)
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

        public async void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            if ((args.ContinuationData["Operation"] as string) == "img" && args.Files != null && args.Files.Count > 0)
            {
                StorageFile file = args.Files[0];
                Frame.Navigate(typeof (SetHeadPage),file);
                BitmapImage bitmapImage = new BitmapImage(new Uri(file.Path));
                img.ImageSource = bitmapImage;
            }
        }

    }
}
