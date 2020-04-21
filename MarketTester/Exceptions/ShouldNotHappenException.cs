using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketTester.Exceptions
{
    public class ShouldNotHappenException : Exception
    {
        public ShouldNotHappenException(string message) : base(message)
        {

        }
    }
}
