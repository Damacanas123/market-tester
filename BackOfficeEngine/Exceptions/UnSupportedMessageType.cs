using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOfficeEngine.Exceptions
{
    public class UnSupportedMessageType: Exception
    {
        public UnSupportedMessageType(string s): base(s)
        {

        }
    }
}
