using BackOfficeEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MarketTester
{
    public static class Const
    {
        public const string ResourceColorBuy = "ColorBuy";
        public const string ResourceColorSell = "ColorSell";
        public const string ResourceColorSellShort = "ColorSellShort";

        public const string ResourceNameRadioButtonLimit = "NameRadioButtonLimit";
        public const string ResourceNameRadioButtonMarket = "NameRadioButtonMarket";
        public const string ResourceNameRadioButtonMarketToLimit = "NameRadioButtonMarketToLimit";

        public static int DefaultExpectedThrottling = 16;

        public static List<ProtocolType> AvailableProtocols { get; set; } = new List<ProtocolType>()
        {
            ProtocolType.Fix50sp2,ProtocolType.Fix44,ProtocolType.Fix42,ProtocolType.Fix40,ProtocolType.Fix41,ProtocolType.Fix43,ProtocolType.Fix50
        };
    }
}
