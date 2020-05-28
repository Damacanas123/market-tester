using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickFix;
using BackOfficeEngine.Model;
using BackOfficeEngine.MessageEnums;

namespace BackOfficeEngine.Connection
{
    internal interface IConnectorSubscriber
    {
        void OnLogon(IConnector connector, string sessionID);

        void OnLogout(IConnector connector, string sessionID);

        void OnCreateSession(IConnector connector, string sessionID);

        void EnqueueMessageThatContainsClOrdID(IConnector connector, IMessage msg);

        void OnSessionMessageReject(IConnector connector, IMessage msg, MessageOrigin messageOrigin);

        void OnApplicationMessageReject(IConnector connector, IMessage msg, MessageOrigin messageOrigin);

        void OnMessage(string message, string connectionName,string sessionID,DateTime timeStamp);
    }
}
