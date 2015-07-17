using Date.Data;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json.Linq;
using Windows.UI.Popups;
using Windows.UI;
using System.Threading.Tasks;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace Date
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DetailDatePage : Page
    {
        private ApplicationDataContainer appSetting;
        DateDetail dd = new DateDetail();
        List<GradeList> gradelist = new List<GradeList>();
        List<JoinedOnes> joinedOnes = new List<JoinedOnes>();
        DateList datelistNavigate = new DateList();
        private bool isCollected = false;
        private int failednum = 0;
        public DetailDatePage()
        {
            appSetting = ApplicationData.Current.LocalSettings; //本地存储
            this.InitializeComponent();
            getGradeInfor();
            DateDetailScrollViewer.Height = Utils.getPhoneHeight() - 60 - 85;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;//注册重写后退按钮事件
            UmengSDK.UmengAnalytics.TrackPageStart("DetailDatePage");

            //先显示传进来的数据
            datelistNavigate = (DateList)e.Parameter;
            DetailNameTextBlock.Text = datelistNavigate.nickname;
            if (datelistNavigate.gender == "1")
                DetailGenderImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/ic_man.png", UriKind.Absolute));
            else if ((datelistNavigate.gender == "2"))
                DetailGenderImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/ic_woman.png", UriKind.Absolute));
            DetailSignatureTextBlock.Text = datelistNavigate.signature;
            DetailTitleTextBlock.Text = datelistNavigate.title;
            DetailPlaceTextBlock.Text = datelistNavigate.place;
            DetailTimeTextBlock.Text = datelistNavigate.date_time;
            DetailCostTextBlock.Text = datelistNavigate.cost_model;
            DetailHeadImage.ImageSource = new BitmapImage(new Uri(datelistNavigate.head, UriKind.Absolute));

            if (e.NavigationMode == NavigationMode.Back)
                getDateInfo(2);
            else
                getDateInfo(1);
        }

        private async void getDateInfo(int cc)
        {
            StatusProgressBar.Visibility = Visibility.Visible;
            StatusTextBlock.Text = "加载中...";
            string detaildate = "";
            if (cc == 1)
            {//一把下面这段放到一个函数里，网络请求这个就出错。求解
                List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
                paramList.Add(new KeyValuePair<string, string>("uid", appSetting.Values["uid"].ToString()));
                paramList.Add(new KeyValuePair<string, string>("token", appSetting.Values["token"].ToString()));
                paramList.Add(new KeyValuePair<string, string>("date_id", datelistNavigate.date_id.ToString()));
                detaildate = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/date/detaildate", paramList));
                App.CacheString = detaildate;
            }
            else
                detaildate = App.CacheString;
            Debug.WriteLine("约会详情" + detaildate);
            if (detaildate != "")
            {
                JObject obj = JObject.Parse(detaildate);
                if (Int32.Parse(obj["status"].ToString()) == 200)
                {
                    dd.GetAttribute(obj);
                    //获取详细信息，存在dd里
                    GetDetail();
                }
            }
            else
            {
                if (failednum > 1)
                {
                    StatusStackPanel.Background = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
                    StatusProgressBar.Visibility = Visibility.Collapsed;
                    StatusTextBlock.Visibility = Visibility.Visible;
                    StatusTextBlock.Text = "加载失败 T_T";
                    await Task.Delay(2000);
                    StatusTextBlock.Text = "";
                    StatusTextBlock.Visibility = Visibility.Collapsed;
                    StatusStackPanel.Background = new SolidColorBrush(Color.FromArgb(255, 239, 239, 239));
                }
                else
                {
                    failednum++;
                    getDateInfo(1);
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

        private void GetDetail()
        {
            DetailContentTextBlock.Text = dd.Content;
            DetailSignatureTextBlock.Text = dd.Signature;
            if (dd.Grade_limit.Length == 4)
                DetailGradeTextBlock.Text = "不限";
            else
            {
                DetailGradeTextBlock.Text = "";
                if (Array.IndexOf(dd.Grade_limit, 1) != -1)
                {
                    GradeList g = gradelist.Find(p => p.Id.Equals(1));
                    DetailGradeTextBlock.Text = DetailGradeTextBlock.Text + g.Name + " ";
                }
                if (Array.IndexOf(dd.Grade_limit, 2) != -1)
                {
                    GradeList g = gradelist.Find(p => p.Id.Equals(2));
                    DetailGradeTextBlock.Text = DetailGradeTextBlock.Text + g.Name + " ";
                }
                if (Array.IndexOf(dd.Grade_limit, 3) != -1)
                {
                    GradeList g = gradelist.Find(p => p.Id.Equals(3));
                    DetailGradeTextBlock.Text = DetailGradeTextBlock.Text + g.Name + " ";
                }
                if (Array.IndexOf(dd.Grade_limit, 4) != -1)
                {
                    GradeList g = gradelist.Find(p => p.Id.Equals(4));
                    DetailGradeTextBlock.Text = DetailGradeTextBlock.Text + g.Name;
                }
            }

            if (dd.Gender_limit == 0)
                DetailGenderNeedTextBlock.Text = "不限";
            else if (dd.Gender_limit == 1)
                DetailGenderNeedTextBlock.Text = "男";
            else if (dd.Gender_limit == 2)
                DetailGenderNeedTextBlock.Text = "女";
            DetailNumTextBlock.Text = dd.People_limit.ToString();

            JoinedOnes[] join = dd.Joined;
            if (join.Length == 0)
                NullGrid.Visibility = Visibility.Visible;
            else
            {
                for (int i = 0; i < join.Length; i++)
                {
                    joinedOnes.Add(new JoinedOnes { Head = join[i].Head, Nickname = join[i].Nickname, User_id = join[i].User_id });
                    if (join[i].User_id == Int32.Parse(appSetting.Values["uid"].ToString()))
                    {
                        EnrollAppBarToggleButton.IsEnabled = false;
                        EnrollAppBarToggleButton.Label = "已报名";
                    }
                }
                NullGrid.Visibility = Visibility.Collapsed;
                JoinedGridView.ItemsSource = joinedOnes;
            }
            StatusProgressBar.Visibility = Visibility.Collapsed;
            StatusTextBlock.Visibility = Visibility.Collapsed;
            StatusTextBlock.Text = "";
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
            else
            {
                grade = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/public/grade", new List<KeyValuePair<String, String>>()));
                getGradeInfor();
            }
        }

        //已加入的Item点击事件
        private void JoinedGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Debug.WriteLine("你点击了" + ((JoinedOnes)e.ClickedItem).Nickname);
            if (((JoinedOnes)e.ClickedItem).User_id.ToString() == appSetting.Values["uid"].ToString())
                Frame.Navigate(typeof(grzxPage));
            else
                Frame.Navigate(typeof(PersonInfoPage), ((JoinedOnes)e.ClickedItem).User_id);
        }

        private async void CollectAppBarToggleButton_Click(object sender, RoutedEventArgs e)
        {
            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<string, string>("uid", appSetting.Values["uid"].ToString()));
            paramList.Add(new KeyValuePair<string, string>("token", appSetting.Values["token"].ToString()));
            paramList.Add(new KeyValuePair<string, string>("date_id", datelistNavigate.date_id.ToString()));
            if (!isCollected)
            {
                string collect = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/person/collect", paramList));
                if (collect != "")
                {
                    try
                    {
                        JObject obj = JObject.Parse(collect);
                        if (Int32.Parse(obj["status"].ToString()) == 200)
                        {
                            await new MessageDialog("收藏成功").ShowAsync();
                            CollectAppBarToggleButton.Icon = new SymbolIcon(Symbol.UnFavorite);
                            CollectAppBarToggleButton.Label = "取消收藏";
                            isCollected = true;
                        }
                        else
                        {
                            await new MessageDialog(obj["info"].ToString()).ShowAsync();
                            if (Int32.Parse(obj["status"].ToString()) == 403)
                                isCollected = true;
                        }
                    }
                    catch (Exception) { Debug.WriteLine("约会详情，收藏约会异常"); }
                }
                else
                    await new MessageDialog("收藏失败").ShowAsync();
            }
            else
            {
                string rmcollecttion = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/person/rmcollecttion", paramList));
                if (rmcollecttion != "")
                {
                    try
                    {
                        JObject obj = JObject.Parse(rmcollecttion);
                        if (Int32.Parse(obj["status"].ToString()) == 200)
                        {
                            await new MessageDialog("取消收藏成功").ShowAsync();
                            CollectAppBarToggleButton.Icon = new SymbolIcon(Symbol.Favorite);
                            CollectAppBarToggleButton.Label = "收藏";
                            isCollected = false;
                        }
                        else
                        {
                            await new MessageDialog(obj["info"].ToString()).ShowAsync();
                        }
                    }
                    catch (Exception) { Debug.WriteLine("约会详情，取消收藏异常"); }
                }
                else
                    await new MessageDialog("取消收藏失败").ShowAsync();
            }
        }

        private async void EnrollAppBarToggleButton_Click(object sender, RoutedEventArgs e)
        {
            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<string, string>("uid", appSetting.Values["uid"].ToString()));
            paramList.Add(new KeyValuePair<string, string>("token", appSetting.Values["token"].ToString()));
            paramList.Add(new KeyValuePair<string, string>("date_id", datelistNavigate.date_id.ToString()));
            string report = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/date/report", paramList));
            if (report != "")
            {
                try
                {
                    JObject obj = JObject.Parse(report);
                    if (Int32.Parse(obj["status"].ToString()) == 200)
                    {
                        await new MessageDialog("报名成功").ShowAsync();
                        EnrollAppBarToggleButton.IsEnabled = false;
                        List<JoinedOnes> joinedOnesnew = new List<JoinedOnes>();
                        joinedOnesnew.AddRange(joinedOnes);
                        joinedOnesnew.Add(new JoinedOnes { Head = appSetting.Values["head"].ToString(), Nickname = appSetting.Values["nickname"].ToString() });
                        JoinedGridView.ItemsSource = joinedOnesnew;
                    }
                    else
                    {
                        await new MessageDialog(obj["info"].ToString()).ShowAsync();
                    }
                }
                catch (Exception) { Debug.WriteLine("约会详情，报名异常"); }
            }
        }


    }
}
