using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketTester.Exceptions
{
    public class ParallelRunException : Exception
    {
        public ParallelRunException(string message) : base(message)
        {

        }
    }
}
