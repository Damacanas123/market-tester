using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackOfficeEngine.Model;

namespace BackOfficeEngine.Events
{
    public class UnknownClOrdIDReceivedEventArgs : EventArgs 
    {
        public IMessage msg { get; set; }
        public UnknownClOrdIDReceivedEventArgs(IMessage msg)
        {
            this.msg = msg;
        }
    }
    public delegate void UnknownClOrdIDReceivedEventHandler(object sender, UnknownClOrdIDReceivedEventArgs args);
}
