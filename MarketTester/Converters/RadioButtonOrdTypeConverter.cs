using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;


using BackOfficeEngine.MessageEnums;

namespace MarketTester.Converters
{
    public class RadioButtonOrdTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            OrdType ordType = (OrdType)value;
            string radioButtonName = parameter.ToString();
            if(radioButtonName == App.Current.Resources[Const.ResourceNameRadioButtonLimit].ToString())
            {
                if (ordType == OrdType.Limit)
                {
                    return true;
                }
                else return false;
            }
            else if (radioButtonName == App.Current.Resources[Const.ResourceNameRadioButtonMarket].ToString())
            {
                if (ordType == OrdType.Market)
                {
                    return true;
                }
                else return false;
            }
            else
            {
                if (ordType == OrdType.MarketToLimit)
                {
                    return true;
                }
                else return false;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string radioButtonName = parameter.ToString();
            if (radioButtonName == App.Current.Resources[Const.ResourceNameRadioButtonLimit].ToString())
            {
                return OrdType.Limit;
            }
            else if (radioButtonName == App.Current.Resources[Const.ResourceNameRadioButtonMarket].ToString())
            {
                return OrdType.Market;
            }
            else
            {
                return OrdType.MarketToLimit;
            }
        }
    }
}
