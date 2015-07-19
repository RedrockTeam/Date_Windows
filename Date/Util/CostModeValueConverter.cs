using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Date.Util
{
    class CostModeValueConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string c = value as string;
            if (c== "1")
                return  "AA";
            else if (c== "2")
                return "你请客";
            else if (c== "3")
                return "我买单";
            else
            {
                return "AA";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
