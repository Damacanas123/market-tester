using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackOfficeEngine.Model;
using BackOfficeEngine.ParamPacker;
namespace BackOfficeEngine.Connection
{
    internal interface IConnector
    {
        List<IConnectorSubscriber> subscribers { get; }
        void Subscribe(IConnectorSubscriber subscriber);
        void ConfigureConnection(string configFilePath);
        void ConfigureConnection(string configFilePath,BISTCredentialParams credentialParams);
        void Connect();
        void Disconnect();
        void SendMsgOrderEntry(IMessage msg);
        
        
    }
}
