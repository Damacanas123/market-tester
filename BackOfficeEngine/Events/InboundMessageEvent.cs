using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackOfficeEngine.Model;

namespace BackOfficeEngine.Events
{
    public class InboundMessageEventArgs : EventArgs
    {
        public IMessage msg { get; set; }
        public InboundMessageEventArgs(IMessage msg)
        {
            this.msg = msg;
        }
    }
    public delegate void InboundMessageEventHandler(object sender, InboundMessageEventArgs args);
}
