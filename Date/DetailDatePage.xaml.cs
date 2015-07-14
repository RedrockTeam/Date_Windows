using Date.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace Date
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DetailDatePage : Page
    {
        public DetailDatePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;//注册重写后退按钮事件
            UmengSDK.UmengAnalytics.TrackPageStart("DetailDatePage");

            var datelistNavigate = (DateList)e.Parameter;
            DetailHeadImage.ImageSource = new BitmapImage(new Uri(datelistNavigate.Head, UriKind.Absolute));
            DetailNameTextBlock.Text = datelistNavigate.Nickname;
            if (datelistNavigate.Gender == "1")
                DetailGenderImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/ic_man.png", UriKind.Absolute));
            else if ((datelistNavigate.Gender == "2"))
                DetailGenderImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/ic_woman.png", UriKind.Absolute));
            DetailSignatureTextBlock.Text = datelistNavigate.Signature;
            DetailTitleTextBlock.Text = datelistNavigate.Title;
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
