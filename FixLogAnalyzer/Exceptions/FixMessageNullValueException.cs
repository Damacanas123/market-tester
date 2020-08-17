using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixLogAnalyzer.Exceptions
{
    public class FixMessageNullValueException : Exception
    {
        public FixMessageNullValueException() : base()
        {

        }
        public FixMessageNullValueException(string s) : base(s)
        {

        }
    }
}
