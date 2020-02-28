using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOfficeEngine.Events
{
    public class OnLogonEventArgs : EventArgs
    {
        public int ConnectorIndex { get; set; }
        public string SessionID { get; set; }
        public OnLogonEventArgs(int connectorIndex,string sessionID)
        {
            ConnectorIndex = connectorIndex;
            SessionID = sessionID;
        }
    }

    public delegate void OnLogonEventHandler(object sender, OnLogonEventArgs args);
}
