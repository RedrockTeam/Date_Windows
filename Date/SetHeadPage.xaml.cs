using Date.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace Date
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SetHeadPage : Page
    {
        private ApplicationDataContainer appSetting;

        public SetHeadPage()
        {
            this.InitializeComponent();
            headScrollViewer.Width = Utils.getPhoneWidth();
            headScrollViewer.Height = Utils.getPhoneWidth();
            appSetting = ApplicationData.Current.LocalSettings; //本地存储
            var statusBar = StatusBar.GetForCurrentView();
            statusBar.HideAsync();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;//注册重写后退按钮事件
            BitmapImage bitmapImage = new BitmapImage(new Uri(((StorageFile)e.Parameter).Path));
            headImage.Source = bitmapImage;
        }

        //离开页面时，取消事件
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


        private async void AcceptAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            showStatus("保存中...", 1, Visibility.Visible);
            string head = "";
            try
            {

                HttpClient _httpClient = new HttpClient();
                CancellationTokenSource _cts = new CancellationTokenSource();


                RenderTargetBitmap mapBitmap = new RenderTargetBitmap();
                await mapBitmap.RenderAsync(headScrollViewer);
                var pixelBuffer = await mapBitmap.GetPixelsAsync();
                IStorageFolder applicationFolder = ApplicationData.Current.LocalFolder;
                IStorageFile saveFile = await applicationFolder.CreateFileAsync("head.png", CreationCollisionOption.OpenIfExists);
                using (var fileStream = await saveFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);

                    encoder.SetPixelData(
                        BitmapPixelFormat.Bgra8,
                        BitmapAlphaMode.Ignore,
                        (uint)mapBitmap.PixelWidth,
                        (uint)mapBitmap.PixelHeight,
                        DisplayInformation.GetForCurrentView().LogicalDpi,
                        DisplayInformation.GetForCurrentView().LogicalDpi,
                        pixelBuffer.ToArray());
                    await encoder.FlushAsync();
                }

                HttpStringContent uidStringContent = new HttpStringContent(appSetting.Values["uid"].ToString());
                HttpStringContent tokenStringContent = new HttpStringContent(appSetting.Values["token"].ToString());


                // 构造需要上传的文件数据
                IRandomAccessStreamWithContentType stream1 = await saveFile.OpenReadAsync();
                HttpStreamContent streamContent = new HttpStreamContent(stream1);
                HttpMultipartFormDataContent fileContent = new HttpMultipartFormDataContent();

                fileContent.Add(streamContent, "photo", "head.png");
                fileContent.Add(uidStringContent, "uid");
                fileContent.Add(tokenStringContent, "token");

                HttpResponseMessage response = await _httpClient.PostAsync(new Uri("http://106.184.7.12:8002/index.php/api/person/uploadimg"), fileContent).AsTask(_cts.Token);
                head = Utils.ConvertUnicodeStringToChinese(await response.Content.ReadAsStringAsync().AsTask(_cts.Token));
                Debug.WriteLine(head);
            }
            catch (Exception) { }
            if (head != "")
            {
                JObject obj = JObject.Parse(head);
                if (Int32.Parse(obj["status"].ToString()) != 200)
                {
                    Utils.Message(obj["info"].ToString());
                    StatusProgressBar.Visibility = Visibility.Collapsed;
                    StatusTextBlock.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //Utils.Toast("发布成功");
                    showStatus("保存成功", 1, Visibility.Collapsed);
                    await Task.Delay(1500);
                    Frame rootFrame = Window.Current.Content as Frame;
                    rootFrame.GoBack();
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

        private async void showStatus(string text, int value, Windows.UI.Xaml.Visibility ProgressBarvisibility)
        {
            StatusStackPanel.Visibility = Visibility.Visible;
            StatusProgressBar.Visibility = ProgressBarvisibility;
            StatusTextBlock.Visibility = Visibility.Visible;
            if (value == 1)
                StatusStackPanel.Background = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
            else
                StatusStackPanel.Background = null;
            StatusTextBlock.Text = text;

            if (ProgressBarvisibility == Visibility.Collapsed)
            {
                await Task.Delay(2000);
                StatusTextBlock.Visibility = Visibility.Collapsed;
                StatusStackPanel.Visibility = Visibility.Collapsed;
                StatusProgressBar.Visibility = Visibility.Collapsed;
                StatusStackPanel.Background = null;
            }
        }


    }
}
