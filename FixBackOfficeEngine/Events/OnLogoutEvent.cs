using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOfficeEngine.Events
{
    public class OnLogoutEventArgs
    {
        public int ConnectorIndex { get; set; }
        public string SessionID { get; set; }
        public OnLogoutEventArgs(int connectorIndex, string sessionID)
        {
            ConnectorIndex = connectorIndex;
            SessionID = sessionID;
        }
    }

    public delegate void OnLogoutEventHandler(object sender, OnLogoutEventArgs args);
}
