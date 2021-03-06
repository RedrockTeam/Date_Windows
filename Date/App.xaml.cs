﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UmengSDK;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Date.Pages;

// “空白应用程序”模板在 http://go.microsoft.com/fwlink/?LinkId=391641 上有介绍

namespace Date
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    public sealed partial class App : Application
    {
        private TransitionCollection transitions;
        private ApplicationDataContainer appSetting;
        public static string CacheString = ""; //普通页面的缓存
        public static string CacheString2 = ""; //二层目录缓存
        public static string CacheString3 = ""; //三层目录缓存
        public static string gotoPage = "";
        private string exampleTaskName = "LetterBackgroundTask";

        /// <summary>
        /// 初始化单一实例应用程序对象。    这是执行的创作代码的第一行，
        /// 逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
            this.Resuming += this.OnResuming;
            appSetting = ApplicationData.Current.LocalSettings; //本地存储
            if (!appSetting.Values.ContainsKey("LetterUnRead"))
            {
                appSetting.Values["LetterUnRead"] = 0;
                appSetting.Values["isBackStart"] = true;
                addBackgroundTask();
            }
            if (bool.Parse(appSetting.Values["isBackStart"].ToString()))
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
            try
            {
                //判断是否已经注册过了
                taskRegistered = BackgroundTaskRegistration.AllTasks.Any(x => x.Value.Name == exampleTaskName);

                if (!taskRegistered)
                {
                    BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();
                    var builder = new BackgroundTaskBuilder();
                    builder.Name = exampleTaskName;
                    builder.TaskEntryPoint = "Tasks.LetterBackgroundTask";
                    //后台触发器，可多个
                    builder.SetTrigger(new SystemTrigger(SystemTriggerType.NetworkStateChange, false));
                    builder.SetTrigger(new SystemTrigger(SystemTriggerType.InternetAvailable, false));
                    //builder.SetTrigger(new MaintenanceTrigger(15, false)); //定时后台任务
                    builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));

                    BackgroundTaskRegistration task = builder.Register();
                }
                else
                {
                    var cur = BackgroundTaskRegistration.AllTasks.FirstOrDefault(x => x.Value.Name == exampleTaskName);
                    BackgroundTaskRegistration task = (BackgroundTaskRegistration)(cur.Value);
                }
            }
            catch (Exception) { }

        }

        private async void OnResuming(object sender, object e)
        {
            await UmengAnalytics.StartTrackAsync("558183b467e58ef41f0030ea", "Marketplace");
        }


        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 当启动应用程序以打开特定的文件或显示搜索结果等操作时，
        /// 将使用其他入口点。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
            //Umeng的调试模式
            //UmengSDK.UmengAnalytics.IsDebug = true;
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                // TODO: 将此值更改为适合您的应用程序的缓存大小
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: 从之前挂起的应用程序加载状态
                }

                // 将框架放在当前窗口中
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // 删除用于启动的旋转门导航。
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

                //当导航堆栈尚未还原时，导航到第一页，
                // 并通过将所需信息作为导航参数传入来配置
                // 新页面
                if (appSetting.Values.ContainsKey("username"))
                {
                    if (!rootFrame.Navigate(typeof(MainPage), "autologin"))
                    {
                        throw new Exception("Failed to create initial page");
                    }
                }
                else
                {
                    if (!rootFrame.Navigate(typeof(LoginPage), e.Arguments))
                    {
                        throw new Exception("Failed to create initial page");
                    }
                }

            }

            // 确保当前窗口处于活动状态
            Window.Current.Activate();
            await UmengAnalytics.StartTrackAsync("558183b467e58ef41f0030ea", "Marketplace");
        }

        /// <summary>
        /// 启动应用程序后还原内容转换。
        /// </summary>
        /// <param name="sender">附加了处理程序的对象。</param>
        /// <param name="e">有关导航事件的详细信息。</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。    在不知道应用程序
        /// 将被终止还是恢复的情况下保存应用程序状态，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起的请求的详细信息。</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: 保存应用程序状态并停止任何后台活动
            await UmengAnalytics.EndTrackAsync();
            deferral.Complete();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            if (args is FileOpenPickerContinuationEventArgs)
            {
                Frame rootFrame = Window.Current.Content as Frame;
                var p = rootFrame.Content as EditInfo;
                p.FilePickerEvent = (FileOpenPickerContinuationEventArgs)args;
            }
            Window.Current.Activate();
        }



    }
}