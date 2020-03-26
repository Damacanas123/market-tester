using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOfficeEngine.Events
{
    public class OnLogoutEventArgs
    {
        public string ConnectorName { get; set; }
        public string SessionID { get; set; }
        public OnLogoutEventArgs(string connectorName, string sessionID)
        {
            ConnectorName = connectorName;
            SessionID = sessionID;
        }
    }

    public delegate void OnLogoutEventHandler(object sender, OnLogoutEventArgs args);
}
