using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
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
using UmengSDK;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace Date.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class EditInfo : Page
    {
        private ApplicationDataContainer appSetting;
        List<AcademyList> acalist = new List<AcademyList>();
        List<GradeList> gradelist = new List<GradeList>();
        private FileOpenPickerContinuationEventArgs _filePickerEventArgs = null;

        public EditInfo()
        {
            appSetting = ApplicationData.Current.LocalSettings; //本地存储
            this.InitializeComponent();

        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;//注册重写后退按钮事件
            PersonInfo pi = e.Parameter as PersonInfo;
            getAca();
            getGradeInfor();
            this.DataContext = pi;
            if (e.NavigationMode == NavigationMode.New)
            {
                sethead(pi);
            }
            else
            {

                SetHead(pi);
            }
            canModify(pi);
        }
        private void SetHead(PersonInfo p)
        {
            if (appSetting.Values.ContainsKey("IsHeadExistOffline") && bool.Parse(appSetting.Values["IsHeadExistOffline"].ToString()))
            {
                SetHeadPage();
            }
            else
            {
                sethead(p);
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
        void sethead(PersonInfo pi)
        {
            img.ImageSource = new BitmapImage(new Uri(pi.Head));
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

        void canModify(PersonInfo p)
        {
            if (p.Gender != "")
            {
                Gbox.Visibility = Visibility.Collapsed;
            }
            if (p.Grade != "")
            {
                Grbox.Visibility = Visibility.Collapsed;
            }
            if (p.Academy != "")
            {
                Acabox.Visibility = Visibility.Collapsed;
            }
        }
        private void getAca()
        {
            string academy = appSetting.Values["academy_json"].ToString();
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
                Acabox.ItemsSource = acalist;
            }
        }
        private async void getGradeInfor()
        {
            //年级
            string grade = appSetting.Values["grade_json"].ToString();
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
            }
            Grbox.ItemsSource = gradelist;
        }
        private void ico_Tapped(object sender, TappedRoutedEventArgs e)
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
                Frame.Navigate(typeof(SetHeadPage), file);
                BitmapImage bitmapImage = new BitmapImage(new Uri(file.Path));
                img.ImageSource = bitmapImage;
                //IsHeadExistOffline 
                appSetting.Values["IsHeadExistOffline"] = true;

            }

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            int nklong = nkname.Text.Length;
            if (nklong > 10 || nklong < 1)
            {
                await new MessageDialog("昵称长度在1到10之间！").ShowAsync();
                return;
            }
            int sglong = signa.Text.Length;
            if (sglong > 140)
            {
                await new MessageDialog("个性签名太长啦！").ShowAsync();
                return;
            }
            string tele = tel.Text;
            if (!Regex.IsMatch(tele, @"^(0|86|17951)?(13[0-9]|15[012356789]|17[678]|18[0-9]|14[57])[0-9]{8}$"))
            {
                await new MessageDialog("电话不对哦~").ShowAsync();
                return;
            }
            string qqn = qq.Text;
            if (!Regex.IsMatch(qqn, @"^\d{5,10}$"))
            {
                await new MessageDialog("QQ号不对哦~").ShowAsync();
                return;
            }
            StatusStackPanel.Visibility=Visibility.Visible;
            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<string, string>("uid", appSetting.Values["uid"].ToString()));
            paramList.Add(new KeyValuePair<string, string>("token", appSetting.Values["token"].ToString()));
            paramList.Add(new KeyValuePair<string, string>("nickname", nkname.Text));
            paramList.Add(new KeyValuePair<string, string>("signature", signa.Text));
            paramList.Add(new KeyValuePair<string, string>("telephone", tel.Text));
            paramList.Add(new KeyValuePair<string, string>("qq", qq.Text));
            paramList.Add(new KeyValuePair<string, string>("weixin", weixin.Text));
            if (Gbox.Visibility == Visibility.Visible)
                paramList.Add(new KeyValuePair<string, string>("gender", (Gbox.SelectedIndex + 1).ToString()));
            if (Grbox.Visibility == Visibility.Visible)
                paramList.Add(new KeyValuePair<string, string>("grade", (Grbox.SelectedIndex + 1).ToString()));
            if (Acabox.Visibility == Visibility.Visible)
                paramList.Add(new KeyValuePair<string, string>("academy", (Acabox.SelectedIndex + 1).ToString()));
            string result = "";
            result = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/person/editdata", paramList));
            JObject jo = JObject.Parse(result);
            if (!result.Contains("null") && jo["status"].ToString() == "200")
            {
                StatusTextBlock.Text = "修改成功!";
                await Task.Delay(2000);
                StatusStackPanel.Visibility = Visibility.Collapsed;
                Frame.Navigate(typeof (grzxPage));
                //修改成功
            }
            else
            {
                StatusTextBlock.Text = "修改失败!";
                await Task.Delay(2000);
                StatusStackPanel.Visibility = Visibility.Collapsed;
                Frame.Navigate(typeof(grzxPage));
                //修改失败
            }
        }


    }
}
