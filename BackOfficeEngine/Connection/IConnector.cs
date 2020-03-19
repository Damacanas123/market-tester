using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackOfficeEngine.Model;

namespace BackOfficeEngine.Connection
{
    internal interface IConnector
    {
        List<IConnectorSubscriber> subscribers { get; }
        void Subscribe(IConnectorSubscriber subscriber);
        void ConfigureConnection(string configFilePath);
        void Connect();
        void Disconnect();
        void SendMsgOrderEntry(IMessage msg);
        
        
    }
}
