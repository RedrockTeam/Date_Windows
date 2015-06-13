using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Date.DataModel
{
    public class Banner
    {
        private string url;

        public string Url
        {
            get { return url; }
            set { url = value; }
        }
        private string src;

        public string Src
        {
            get { return src; }
            set { src = value; }
        }
    }
}
