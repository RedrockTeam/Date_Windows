using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Date.Data
{
    class GenderConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string g = value as string;
            if (g == "1")
            {
                return "/Assets/ic_man.png";
            }
            else
            {
                return "/Assets/ic_woman.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
