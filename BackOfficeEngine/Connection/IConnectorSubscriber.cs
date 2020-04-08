using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickFix;
using BackOfficeEngine.Model;

namespace BackOfficeEngine.Connection
{
    internal interface IConnectorSubscriber
    {
        void OnInboundMessage(IConnector connector, string sessionID, IMessage msg);

        void OnLogon(IConnector connector, string sessionID);

        void OnLogout(IConnector connector, string sessionID);

        void OnCreateSession(IConnector connector, string sessionID);

        void EnqueueMessage(IConnector connector, IMessage msg);
    }
}
