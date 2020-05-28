using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackOfficeEngine.Model;

namespace BackOfficeEngine.Events
{
    public class OnMessageEventArgs : EventArgs
    {
        public string msg { get; set; }
        public string connectionName { get; set; }
        public string sessionID { get; set; }
        public DateTime timeStamp{ get; set; }
        public OnMessageEventArgs(string msg,string connectionName,string sessionID,DateTime timeStamp)
        {
            this.msg = msg;
            this.connectionName = connectionName;
            this.sessionID = sessionID;
            this.timeStamp = timeStamp;
        }
    }
    public delegate void OnMessageEventHandler(object sender, OnMessageEventArgs args);
}
