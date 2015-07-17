using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Date.Data
{
    class DateStatusValueConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string s = value as string;
            if (s == "0")
            {
                return "已结束";
            }
            else if(s=="1")
            {
                return "受理中";
            }
            else if (s=="2")
            {
                return "正常";
            }
            else
            {
                return "未知";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
