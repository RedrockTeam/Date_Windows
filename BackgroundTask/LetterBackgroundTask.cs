using Date.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Newtonsoft.Json.Linq;

namespace BackgroundTask
{
    public sealed class LetterBackgroundTask : IBackgroundTask
    {
        private ApplicationDataContainer appSetting;
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();  //获取 BackgroundTaskDeferral 对象，表示后台任务延期
            //执行相关异步代码
            GetLetter();

            deferral.Complete(); //所有的异步调用完成之后，释放延期，表示后台任务的完成
        }

        private async void GetLetter()
        {
             List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<string, string>("uid", appSetting.Values["uid"].ToString()));
            paramList.Add(new KeyValuePair<string, string>("token", appSetting.Values["token"].ToString()));
            string letterstatus = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/letter/letterstatus", paramList));
            Debug.WriteLine("letterstatus" + letterstatus);
            try
            {
                if (letterstatus != "")
                {
                    JObject obj = JObject.Parse(letterstatus);
                    if (Int32.Parse(obj["status"].ToString()) == 200)
                    { }
                }
            }
            catch (Exception) { Debug.WriteLine("后台任务网络异常"); }
        
        }

    }
}
