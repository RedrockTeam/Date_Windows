using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Date.Util
{
    class NetWork
    {
        public static async Task<string> getHttpWebRequest(string api, List<KeyValuePair<String, String>> paramList)
        {
            string content = "";
            return await Task.Run(() =>
            {
                HttpClient httpClient = new HttpClient();
                string uri = "http://106.184.7.12:8002/index.php/api" + api;
                HttpResponseMessage response = httpClient.PostAsync(new Uri(uri), new FormUrlEncodedContent(paramList)).Result;
                content = response.Content.ReadAsStringAsync().Result;
                return content;
            });
        }

    }



}
