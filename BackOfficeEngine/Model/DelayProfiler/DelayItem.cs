using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOfficeEngine.Model
{
    public class DelayItem
    {
        public IMessage requestMessage { get; set; }
        public IMessage ackMessage { get; set; }
    }
}
