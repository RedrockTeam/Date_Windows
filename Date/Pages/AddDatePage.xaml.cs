using Date.Data;
using Date.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI;
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

        private string DateTimeStamp;

        private int gender_limit = 0;
        int[] grade_limit = new int[] { 0, 0, 0, 0 };
        private int grade_choose = 0;
        private int cost_model = 0;

        List<DateType> datetypelist = new List<DateType>();

        public AddDatePage()
        {
            this.InitializeComponent();
            initScrollViewer();
            appSetting = ApplicationData.Current.LocalSettings; //本地存储
        }

        private void initScrollViewer()
        {
            AddDateScrollViewer.Height = Utils.getPhoneHeight() - 60 - 85;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;//注册重写后退按钮事件
            await Utils.ShowSystemTrayAsync(Color.FromArgb(255, 255, 61, 61), Colors.White, text: "约");
            UmengSDK.UmengAnalytics.TrackPageStart("AddDatePage");
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;//注册重写后退按钮事件
            UmengSDK.UmengAnalytics.TrackPageEnd("AddDatePage");
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
            DateTimeStamp = Utils.GetTimeStamp(addDate, addTime);
            Debug.WriteLine(DateTimeStamp);
        }

        private void AddDateCostGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MenuFlyout costMenuFlyout = new MenuFlyout();

            costMenuFlyout.Items.Add(getCostMenuFlyoutItem("AA制"));
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
            switch (menuFlyoutItem.Text)
            {
                case "AA":
                    cost_model = 1;
                    break;
                case "你请客":
                    cost_model = 2;
                    break;
                case "我买单":
                    cost_model = 3;
                    break;
            }
        }

        private async void AddDateTypeGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MenuFlyout typeMenuFlyout = new MenuFlyout();
            bool isType = true;
            try
            {
                string datetype = appSetting.Values["datetype_json"].ToString();
                if (datetype != "")
                {
                    JArray datetypeArray = Utils.ReadJso(datetype);

                    for (int i = 0; i < datetypeArray.Count; i++)
                    {
                        JObject jobj = (JObject)datetypeArray[i];
                        typeMenuFlyout.Items.Add(getTypeMenuFlyoutItem(jobj["type"].ToString()));
                        var b = new DateType();
                        datetypelist.Add(new DateType { Id = Convert.ToInt32(jobj["id"].ToString()), Type = jobj["type"].ToString() });
                    }
                    typeMenuFlyout.ShowAt(AddDateTypeGrid);
                }
                else
                    isType = false;

            }
            catch (Exception)
            {
                Debug.WriteLine("发布约，分类数据加载异常");
                isType = false;
            }
            if (!isType)
            {
                showStatus("分类数据加载失败", 1, Visibility.Collapsed);
                appSetting.Values["datetype_json"] = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/date/datetype", new List<KeyValuePair<String, String>>()));
            }

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
            switch (menuFlyoutItem.Text)
            {
                case "男":
                    gender_limit = 1;
                    break;
                case "女":
                    gender_limit = 2;
                    break;
                default:
                    gender_limit = 0;
                    break;
            }
        }

        private void AddDateGradeGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AddDateGradeFlyout.ShowAt(AddDateGradeGrid);
            AddDateGradeTextBox.Text = "";
            grade_choose = 0;
        }

        private void FlyoutButton_Click(object sender, RoutedEventArgs e)
        {
            AddDateGradeFlyout.Hide();
        }

        private void AddDateGradeFlyout_Closed(object sender, object e)
        {
            if (oneCheckBox.IsChecked == true)
            {
                AddDateGradeTextBox.Text = AddDateGradeTextBox.Text + "大一 ";
                grade_limit[0] = 1;
                grade_choose++;
            }
            if (twoCheckBox.IsChecked == true)
            {
                AddDateGradeTextBox.Text = AddDateGradeTextBox.Text + "大二 ";
                grade_limit[1] = 1;
                grade_choose++;
            }
            if (threeCheckBox.IsChecked == true)
            {
                AddDateGradeTextBox.Text = AddDateGradeTextBox.Text + "大三 ";
                grade_limit[2] = 1;
                grade_choose++;
            }
            if (fourCheckBox.IsChecked == true)
            {
                AddDateGradeTextBox.Text = AddDateGradeTextBox.Text + "大四 ";
                grade_limit[3] = 1;
                grade_choose++;
            }
        }

        private async void PublishAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddDateTypeTextBox.Text != "" && AddDateTitleTextBox.Text != "" && AddDateContentTextBox.Text != "" && AddDateTimeTextBox.Text != "" && AddDatePlaceTextBox.Text != "" && Int32.Parse(AddDatePeopleTextBox.Text.ToString()) > 0 && AddDateTypeTextBox.Text != "" && AddDateSexTextBox.Text != "" && AddDateGradeTextBox.Text != "")
            {
                //这里提交
                Debug.WriteLine("完成");

                showStatus("提交中...", 0, Visibility.Visible);

                List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
                paramList.Add(new KeyValuePair<string, string>("uid", appSetting.Values["uid"].ToString()));
                paramList.Add(new KeyValuePair<string, string>("token", appSetting.Values["token"].ToString()));
                paramList.Add(new KeyValuePair<string, string>("date_type", (datetypelist.Find(p => p.Type.Equals("打牌"))).Id.ToString()));
                paramList.Add(new KeyValuePair<string, string>("title", AddDateTitleTextBox.Text));
                paramList.Add(new KeyValuePair<string, string>("content", AddDateContentTextBox.Text));
                paramList.Add(new KeyValuePair<string, string>("date_time", DateTimeStamp));
                paramList.Add(new KeyValuePair<string, string>("date_place", AddDatePlaceTextBox.Text));
                paramList.Add(new KeyValuePair<string, string>("date_people", AddDatePeopleTextBox.Text));
                paramList.Add(new KeyValuePair<string, string>("gender_limit", gender_limit.ToString()));

                if (grade_choose != 4)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (grade_limit[i] == 1)
                            paramList.Add(new KeyValuePair<string, string>("grade_limit[]", (i+1).ToString()));
                    }
                    paramList.Add(new KeyValuePair<string, string>("grade_select_model", "2"));
                }
                paramList.Add(new KeyValuePair<string, string>("cost_model", cost_model.ToString()));

                string AddDate = await NetWork.getHttpWebRequest("/date/createdate", paramList);
                AddDate = Util.Utils.ConvertUnicodeStringToChinese(AddDate);
                Debug.WriteLine(AddDate);


                JObject obj = JObject.Parse(AddDate);
                if (Int32.Parse(obj["status"].ToString()) != 200)
                {
                    Utils.Message(obj["info"].ToString());
                    StatusTextBlock.Visibility = Visibility.Collapsed;
                    StatusProgressBar.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //Utils.Toast("发布成功");
                    showStatus("发布成功", 1, Visibility.Collapsed);
                    await Task.Delay(1500);
                    Frame rootFrame = Window.Current.Content as Frame;
                    rootFrame.GoBack();
                }

            }
            else
            {
                Utils.Message("信息不完全");
            }
        }



    }
}
