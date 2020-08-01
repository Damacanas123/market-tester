using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MarketTester.Converters
{
    public class FixStringConverter : IValueConverter
    {
        private const char Seperator = '|';
        //class to UI
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string msg = value.ToString();
            msg = msg.Replace('\u0001', Seperator);            
            return msg;
        }

        //from UI to class
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string msg = value.ToString();
            msg = msg.Replace(Seperator, '\u0001');
            return msg;
        }
    }
}
