using SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace UwpClient.Models.Converters
{
    public class GenderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var gender = value as Gender?;
            switch (gender)
            {
                case Gender.Male: return 0;
                case Gender.Female: return 1;
                default: return -1;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var gender = value as int?;
            switch (gender)
            {
                case 0: return Gender.Male;
                case 1: return Gender.Female;
                default: return null;
            }
        }
    }
}
