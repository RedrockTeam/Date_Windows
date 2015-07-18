using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Date.Util
{
    class CreatedTimeValueConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {

            DateTime createdTime = System.Convert.ToDateTime(value as string);
            DateTime nowDateTime = DateTime.Now;
            TimeSpan timespan = nowDateTime - createdTime;
            if (timespan.Days > 365)
            {
                return createdTime.ToString();
            }
            else if (timespan.Days > 7)
            {
                return createdTime.Month+"月"+createdTime.Day+"日";
            }
            else if (timespan.Days > 1)
            {
                return timespan.Days + "天前";
            }
            else if (timespan.Hours > 1)
            {
                return timespan.Hours + "小时前";
            }
            else if(timespan.Minutes>1)
            {
                return timespan.Minutes + "分钟前";
            }
            else
            {
                return "刚刚";
            }


            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
