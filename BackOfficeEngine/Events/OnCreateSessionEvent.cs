using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOfficeEngine.Events
{
    public class OnCreateSessionEventArgs : EventArgs
    {
        public string ConnectorName { get; set; }
        public string SessionID { get; set; }
        public OnCreateSessionEventArgs(string connectorName, string sessionID)
        {
            ConnectorName = connectorName;
            SessionID = sessionID;
        }
    }

    public delegate void OnCreateSessionEventHandler(object sender, OnCreateSessionEventArgs args);
}
