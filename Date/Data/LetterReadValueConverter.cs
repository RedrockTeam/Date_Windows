using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Date.Data
{
    public class LetterReadValueConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int? isRead = value as int?;
            if (isRead == 1)
            {
                return "White";
            }
            else
            {
                return "#FFD800";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
