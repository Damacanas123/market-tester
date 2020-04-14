using BackOfficeEngine.MessageEnums;
using BackOfficeEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOfficeEngine.Events
{
    public class OnApplicationMessageRejectEventArgs : EventArgs
    {
        public IMessage msg;
        public MessageOrigin messageOrigin;
        public OnApplicationMessageRejectEventArgs(IMessage msg, MessageOrigin messageOrigin)
        {
            this.msg = msg;
            this.messageOrigin = messageOrigin;
        }
    }
    public delegate void OnApplicationMessageRejectEventHandler(object sender, OnApplicationMessageRejectEventArgs args);
}
