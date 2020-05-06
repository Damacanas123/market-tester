using BackOfficeEngine.Helper;
using QuickFix.Fields;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MarketTester.Converters
{
    public class MessageTypeColorConverter : IValueConverter
    {
        //background to UI
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string msg = value.ToString();
            string msgType = Fix.GetTag(msg, "35");
            SolidColorBrush toBeReturned = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            if (string.IsNullOrWhiteSpace(msgType))
            {
                return toBeReturned;
            }
            
            switch (msgType)
            {
                case MsgType.NEWORDERSINGLE:
                    toBeReturned = new SolidColorBrush(Color.FromRgb(82, 75, 38));
                    break;
                case MsgType.ORDERCANCELREPLACEREQUEST:
                    toBeReturned = new SolidColorBrush(Color.FromRgb(55, 85, 32));
                    break;
                case MsgType.ORDERCANCELREQUEST:
                    toBeReturned = new SolidColorBrush(Color.FromRgb(82, 14, 14));
                    break;
                case MsgType.EXECUTIONREPORT:
                    toBeReturned = new SolidColorBrush(Color.FromRgb(14, 82, 61));
                    break;
                case MsgType.ORDERCANCELREJECT:
                    toBeReturned = new SolidColorBrush(Color.FromRgb(14, 26, 82));
                    break;
                case MsgType.HEARTBEAT:
                    toBeReturned = new SolidColorBrush(Color.FromRgb(72, 14, 82));
                    break;
                case MsgType.BUSINESSMESSAGEREJECT:
                    toBeReturned = new SolidColorBrush(Color.FromRgb(28, 47, 77));
                    break;
            }
            return toBeReturned;
        }

        //UI to background
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
