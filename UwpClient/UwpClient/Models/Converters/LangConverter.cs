using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace UwpClient.Models.Converters
{
    public class LangConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var lang = value as string;
            switch (lang)
            {
                case "en-US": return 0;
                case "hu-HU": return 1;
                default: return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var lang = value as int?;
            switch (lang)
            {
                case 0: return "en-US";
                case 1: return "hu-HU";
                default: return "en-US";
            }
        }
    }
}
