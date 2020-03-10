using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using BackOfficeEngine.MessageEnums;

namespace MarketTester.Converters
{
    public class SideConverter : IValueConverter
    {
        private const string BUY = "Buy";
        private const string SELL = "Sell";
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((Side)value)
            {
                case Side.Buy:
                    return BUY;
                case Side.Sell:
                    return SELL;
                default:
                    throw new Exception("Unknown side type");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((string)value)
            {
                case BUY:
                    return Side.Buy;
                case SELL:
                    return Side.Sell;
                default:
                    return Side.UNKNOWN;
            }
        }
    }
}
