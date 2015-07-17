using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.ApplicationModel.Background;
using Windows.Storage;

namespace BackgroundTask
{
    public sealed class LetterBackgroundTask : IBackgroundTask
    {
        private ApplicationDataContainer appSetting;
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();  //获取 BackgroundTaskDeferral 对象，表示后台任务延期
            //执行相关异步代码
            deferral.Complete(); //所有的异步调用完成之后，释放延期，表示后台任务的完成
        }

    }
}
