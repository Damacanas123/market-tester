using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MarketTester.Converters
{
    public class IsConnectedConverter : IValueConverter
    {
        //class to UI
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return App.Current.Resources[ResourceKeys.StringIsConnected].ToString();
            }
            else
            {
                return App.Current.Resources[ResourceKeys.StringIsDisconnected].ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
