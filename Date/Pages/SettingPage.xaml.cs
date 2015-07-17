using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UmengSDK;
using Windows.ApplicationModel.Background;
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

namespace Date.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        private ApplicationDataContainer appSetting;
        private string exampleTaskName = "LetterBackgroundTask";

        public SettingPage()
        {
            appSetting = ApplicationData.Current.LocalSettings; //本地存储
            this.InitializeComponent();
            IsBackToggleSwitch.IsOn = bool.Parse(appSetting.Values["isBackStart"].ToString());
            if(bool.Parse(appSetting.Values["isBackStart"].ToString()))
            {
                addBackgroundTask();
            }
        }

        /// <summary>
        /// 增加后台任务
        /// </summary>
        private async void addBackgroundTask()
        {
            bool taskRegistered = false;
            //判断是否已经注册过了
            taskRegistered = BackgroundTaskRegistration.AllTasks.Any(x => x.Value.Name == exampleTaskName); 

            if (!taskRegistered)
            {
                BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();
                var builder = new BackgroundTaskBuilder();
                builder.Name = exampleTaskName;
                builder.TaskEntryPoint = "Tasks.LetterBackgroundTask";
                //后台触发器，可多个
                builder.SetTrigger(new SystemTrigger(SystemTriggerType.NetworkStateChange,false));
                builder.SetTrigger(new SystemTrigger(SystemTriggerType.InternetAvailable, false));
                //builder.SetTrigger(new MaintenanceTrigger(15, false)); //定时后台任务
                builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));

                BackgroundTaskRegistration task = builder.Register();
                task.Completed += task_Completed;
            }
            else
            {
                var cur = BackgroundTaskRegistration.AllTasks.FirstOrDefault(x => x.Value.Name == exampleTaskName);
                BackgroundTaskRegistration task = (BackgroundTaskRegistration)(cur.Value);
                task.Completed += task_Completed;
            }

        }

        /// <summary>
        /// 删除后台任务
        /// </summary>
        private void deleteBackgroundTask()
        {
            foreach (KeyValuePair<Guid, IBackgroundTaskRegistration> task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == exampleTaskName)
                {
                    task.Value.Unregister(true);
                    break;
                }
            }
        }

        //后台任务完成时调用，若在后台，则应用恢复时调用
        //若要处理UI，在UI线程中调用
        async void task_Completed(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
        }


        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;//注册重写后退按钮事件
            UmengAnalytics.TrackPageStart("SettingPage");
        }

        //离开页面时，取消事件
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;//注册重写后退按钮事件
            UmengAnalytics.TrackPageEnd("SettingPage");
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

        private void IsBackToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            try
            {
                appSetting.Values["isBackStart"] = IsBackToggleSwitch.IsOn;
                if (IsBackToggleSwitch.IsOn)
                    addBackgroundTask();
                else
                    deleteBackgroundTask();
            }
            catch (Exception) { Debug.WriteLine("设置，开关切换异常"); }
        }

        
    }
}
