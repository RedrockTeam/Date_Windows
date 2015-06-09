using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (StuNumTextBox.Text == "" || IdNumPasswordBox.Password=="")
            {
                Util.Utils.Toast("参数错误");
            }
            else
            {
                LoginProgressBar.Visibility = Visibility.Visible;
               

                List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
                paramList.Add(new KeyValuePair<string, string>("username", "2013211594"));
                paramList.Add(new KeyValuePair<string, string>("password", "160155"));

                string content = await Util.NetWork.getHttpWebRequest("/public/login", paramList);

                content = Util.Utils.ConvertUnicodeStringToChinese(content);

                Debug.WriteLine("aaaaaaaaaaaa"+content);

                Frame.Navigate(typeof(MainPage));


            }
        }
    }
}
