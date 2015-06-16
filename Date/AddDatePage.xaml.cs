using Date.Util;
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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace Date
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AddDatePage : Page
    {
        private ApplicationDataContainer appSetting;

        private DateTimeOffset addDate;
        private TimeSpan addTime;

        public AddDatePage()
        {
            this.InitializeComponent();
            initScrollViewer();
            appSetting = ApplicationData.Current.LocalSettings; //本地存储

        }

        private void initScrollViewer()
        {
            AddDateScrollViewer.Height = Utils.getPhoneHeight() - 60 - 70;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;//注册重写后退按钮事件
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

        private void AddDateTimeGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            DatePickerFlyout datePickerFlyout = new DatePickerFlyout();
            datePickerFlyout.MinYear = DateTime.Now;
            datePickerFlyout.DatePicked += datePickerFlyout_DatePicked;
            datePickerFlyout.ShowAt(Frame);
        }

        private void datePickerFlyout_DatePicked(DatePickerFlyout sender, DatePickedEventArgs args)
        {
            TimePickerFlyout timePickerFlyout = new TimePickerFlyout();
            timePickerFlyout.TimePicked += timePickerFlyout_TimePicked;
            timePickerFlyout.ShowAt(Frame);
            addDate = args.NewDate;
            Debug.WriteLine(addDate.Date.ToString());
        }

        private void timePickerFlyout_TimePicked(TimePickerFlyout sender, TimePickedEventArgs args)
        {
            addTime = args.NewTime;
            Debug.WriteLine(addTime.ToString());
            AddDateTimeTextBox.Text = addDate.Date.Year + "年" + addDate.Date.Month + "月" + addDate.Date.Day + "日  " + addTime.Hours + "点" + addTime.Minutes + "分";
        }

        private void AddDateCostGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MenuFlyout costMenuFlyout = new MenuFlyout();

            costMenuFlyout.Items.Add(getCostMenuFlyoutItem("AA"));
            costMenuFlyout.Items.Add(getCostMenuFlyoutItem("你请客"));
            costMenuFlyout.Items.Add(getCostMenuFlyoutItem("我买单"));
            costMenuFlyout.ShowAt(AddDateCostGrid);
        }

        private MenuFlyoutItem getCostMenuFlyoutItem(string text)
        {
            MenuFlyoutItem menuFlyoutItem = new MenuFlyoutItem();
            menuFlyoutItem.Text = text;
            menuFlyoutItem.Click += costMenuFlyoutItem_click;
            return menuFlyoutItem;
        }

        private void costMenuFlyoutItem_click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = sender as MenuFlyoutItem;
            AddDateCostTextBox.Text = menuFlyoutItem.Text;
        }

        private void AddDateTypeGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MenuFlyout typeMenuFlyout = new MenuFlyout();

            typeMenuFlyout.Items.Add(getTypeMenuFlyoutItem("出行"));
            typeMenuFlyout.Items.Add(getTypeMenuFlyoutItem("饮食"));
            typeMenuFlyout.Items.Add(getTypeMenuFlyoutItem("娱乐"));
            typeMenuFlyout.Items.Add(getTypeMenuFlyoutItem("棋牌"));
            typeMenuFlyout.Items.Add(getTypeMenuFlyoutItem("活动"));
            typeMenuFlyout.Items.Add(getTypeMenuFlyoutItem("竞赛"));
            typeMenuFlyout.Items.Add(getTypeMenuFlyoutItem("运动"));
            typeMenuFlyout.Items.Add(getTypeMenuFlyoutItem("自习"));
            typeMenuFlyout.Items.Add(getTypeMenuFlyoutItem("其他"));

            typeMenuFlyout.ShowAt(AddDateTypeGrid);
        }

        private MenuFlyoutItem getTypeMenuFlyoutItem(string text)
        {
            MenuFlyoutItem menuFlyoutItem = new MenuFlyoutItem();
            menuFlyoutItem.Text = text;
            menuFlyoutItem.Click += typeMenuFlyoutItem_click;
            return menuFlyoutItem;
        }

        private void typeMenuFlyoutItem_click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = sender as MenuFlyoutItem;
            AddDateTypeTextBox.Text = menuFlyoutItem.Text;
        }

        private void AddDateSexGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MenuFlyout sexMenuFlyout = new MenuFlyout();

            sexMenuFlyout.Items.Add(getSexMenuFlyoutItem("不限"));
            sexMenuFlyout.Items.Add(getSexMenuFlyoutItem("男"));
            sexMenuFlyout.Items.Add(getSexMenuFlyoutItem("女"));
            sexMenuFlyout.ShowAt(AddDateSexGrid);
        }

        private MenuFlyoutItem getSexMenuFlyoutItem(string text)
        {
            MenuFlyoutItem menuFlyoutItem = new MenuFlyoutItem();
            menuFlyoutItem.Text = text;
            menuFlyoutItem.Click += SexMenuFlyoutItem_click;
            return menuFlyoutItem;
        }

        private void SexMenuFlyoutItem_click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = sender as MenuFlyoutItem;
            AddDateSexTextBox.Text = menuFlyoutItem.Text;
        }

        private void AddDateGradeGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MenuFlyout GradeMenuFlyout = new MenuFlyout();

            GradeMenuFlyout.Items.Add(getGradeMenuFlyoutItem("大一"));
            GradeMenuFlyout.Items.Add(getGradeMenuFlyoutItem("大二"));
            GradeMenuFlyout.Items.Add(getGradeMenuFlyoutItem("大三"));
            GradeMenuFlyout.Items.Add(getGradeMenuFlyoutItem("大四"));
            GradeMenuFlyout.ShowAt(AddDateGradeGrid);
        }

        private MenuFlyoutItem getGradeMenuFlyoutItem(string text)
        {
            MenuFlyoutItem menuFlyoutItem = new MenuFlyoutItem();
            menuFlyoutItem.Text = text;
            menuFlyoutItem.Click += GradeMenuFlyoutItem_click;
            return menuFlyoutItem;
        }

        private void GradeMenuFlyoutItem_click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = sender as MenuFlyoutItem;
            AddDateGradeTextBox.Text = menuFlyoutItem.Text;
        }


        private async void PublishAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddDateTypeTextBox.Text != "" && AddDateTitleTextBox.Text != "" && AddDateContentTextBox.Text != "" && AddDateTimeTextBox.Text != "" && AddDatePlaceTextBox.Text != "" && Int32.Parse(AddDatePeopleTextBox.Text.ToString()) >0 && AddDateTypeTextBox.Text != "" && AddDateSexTextBox.Text != "" && AddDateGradeTextBox.Text != "")
            {
                //这里提交
                Debug.WriteLine("完成");

                List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
                paramList.Add(new KeyValuePair<string, string>("uid", appSetting.Values["uid"].ToString()));
                paramList.Add(new KeyValuePair<string, string>("token", appSetting.Values["token"].ToString()));
                paramList.Add(new KeyValuePair<string, string>("date_type", appSetting.Values["token"].ToString()));
                paramList.Add(new KeyValuePair<string, string>("title", appSetting.Values["token"].ToString()));
                paramList.Add(new KeyValuePair<string, string>("content", appSetting.Values["token"].ToString()));
                paramList.Add(new KeyValuePair<string, string>("date_time", appSetting.Values["token"].ToString()));
                paramList.Add(new KeyValuePair<string, string>("date_place", appSetting.Values["token"].ToString()));
                paramList.Add(new KeyValuePair<string, string>("date_people", appSetting.Values["token"].ToString()));
                paramList.Add(new KeyValuePair<string, string>("gender_limit", appSetting.Values["token"].ToString()));
                paramList.Add(new KeyValuePair<string, string>("grade_limit", appSetting.Values["token"].ToString()));
                paramList.Add(new KeyValuePair<string, string>("grade_select_model", appSetting.Values["token"].ToString()));
                paramList.Add(new KeyValuePair<string, string>("cost_model", appSetting.Values["token"].ToString()));

                string content = await NetWork.getHttpWebRequest("/public/login", paramList);
                content = Util.Utils.ConvertUnicodeStringToChinese(content);

                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.GoBack();
            }
        }
    }
}
