using BackOfficeEngine.MessageEnums;
using BackOfficeEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOfficeEngine.Events
{
    
    public class OnSessionMessageRejectEventArgs : EventArgs
    {
        public IMessage msg;
        public MessageOrigin messageOrigin;
        public OnSessionMessageRejectEventArgs(IMessage msg,MessageOrigin messageOrigin)
        {
            this.msg = msg;
            this.messageOrigin = messageOrigin;
        }
    }
    public delegate void OnSessionMessageRejectEventHandler(object sender, OnSessionMessageRejectEventArgs args);
}
