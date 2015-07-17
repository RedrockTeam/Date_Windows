using Date.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using Windows.Web.Http;
using System.Threading;

namespace Tasks
{
    public sealed class LetterBackgroundTask : IBackgroundTask
    {
        private ApplicationDataContainer appSetting;
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            appSetting = ApplicationData.Current.LocalSettings; //本地存储

            string letterstatus = "";
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();  //获取 BackgroundTaskDeferral 对象，表示后台任务延期

            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<string, string>("uid", appSetting.Values["uid"].ToString()));
            paramList.Add(new KeyValuePair<string, string>("token", appSetting.Values["token"].ToString()));
            letterstatus = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/letter/letterstatus", paramList));

            Debug.WriteLine("letterstatus" + letterstatus);
            try
            {
                if (letterstatus != "" && letterstatus.IndexOf("成功") != -1)
                {
                    Debug.WriteLine(letterstatus.IndexOf("letter"));
                    Debug.WriteLine(letterstatus.IndexOf("}"));
                    int letterUnRead = Int32.Parse(letterstatus.Substring(letterstatus.IndexOf("letter") + 9, letterstatus.IndexOf("}") - (letterstatus.IndexOf("letter") + 10 )));
                    if (letterUnRead > Int32.Parse(appSetting.Values["LetterUnRead"].ToString()))
                    {
                        Utils.Toast("你有" + letterUnRead + "条新的私信");
                        appSetting.Values["LetterUnRead"] = letterUnRead;
                    }
                }
            }
            catch (Exception) { Debug.WriteLine("后台任务网络异常"); }



            Debug.WriteLine("111111111111111111111111111111111");

            deferral.Complete(); //所有的异步调用完成之后，释放延期，表示后台任务的完成
        }


    }
}
