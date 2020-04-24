using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MarketTester.Extensions
{
    public static class TimeSpanExtension
    {
        public static long GetTotalMicroSeconds(this TimeSpan timeSpan)
        {
            return timeSpan.Ticks / 10;
        }
    }
}
