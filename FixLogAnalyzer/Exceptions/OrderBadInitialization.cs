using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixLogAnalyzer.Exceptions
{
    internal class OrderBadInitialization : Exception
    {
        public OrderBadInitialization() : base()
        {

        }
        public OrderBadInitialization(string s) : base(s)
        {

        }
    }
}
