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
        private DateTimeOffset addDate;
        private TimeSpan addTime;

        public AddDatePage()
        {
            this.InitializeComponent();
            initScrollViewer();
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
            MenuFlyoutItem AAmenuFlyoutItem = new MenuFlyoutItem();
            MenuFlyoutItem YoumenuFlyoutItem = new MenuFlyoutItem();
            MenuFlyoutItem MemenuFlyoutItem = new MenuFlyoutItem();

            AAmenuFlyoutItem.Text = "AA";
            AAmenuFlyoutItem.Click += menuFlyoutItem_click;
            costMenuFlyout.Items.Add(AAmenuFlyoutItem);

            YoumenuFlyoutItem.Text = "你买单";
            YoumenuFlyoutItem.Click += menuFlyoutItem_click;
            costMenuFlyout.Items.Add(YoumenuFlyoutItem);

            MemenuFlyoutItem.Text = "我请客";
            MemenuFlyoutItem.Click += menuFlyoutItem_click;
            costMenuFlyout.Items.Add(MemenuFlyoutItem);

            costMenuFlyout.ShowAt(AddDateCostGrid);
        }

        private void menuFlyoutItem_click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = sender as MenuFlyoutItem;
            AddDateCostTextBox.Text = menuFlyoutItem.Text;
        }


    }
}
