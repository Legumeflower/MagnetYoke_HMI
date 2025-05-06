using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Brushes = System.Windows.Media.Brushes;

namespace IConverter
{
    public class Connect_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)parameter == "text")
            {
                if ((bool)value == true) return "Disconnect";
                else return "Connect";
            }

            if ((string)parameter == "color")
            {
                if ((bool)value == true) return Brushes.Pink;
                else return Brushes.MediumSeaGreen;
            }

            return null;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
