using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using BackOfficeEngine;
using BackOfficeEngine.Events;
using BackOfficeEngine.ParamPacker;
using BackOfficeEngine.MessageEnums;
using BackOfficeEngine.Model;

namespace MockRunner
{
    class Program
    {
        static Engine engine;
        static int connectorIndex;
        static void Main(string[] args)
        {
            engine = Engine.GetInstance(500, @"C:\MATRIKS_OMS\BackOfficeEngine");
            connectorIndex = engine.NewConnection(@"C:\MATRIKS_OMS\TestClient\static\Gate2.txt", ProtocolType.Fix50sp2);
            engine.InboundMessageEvent += OnInboundMessage;
            engine.OnLogonEvent += Engine_OnLogonEvent;
            Thread.Sleep(2000);
            
            
        }
        static string symbol = "ANHYT.E";
        static decimal price = 7m;
        static decimal qty = 5m;
        static string primaryChannel = "FC221";
        static string account = "1";
        private static void Engine_OnLogonEvent(object sender, OnLogonEventArgs args)
        {
            Console.WriteLine("Logon : " + args.ConnectorIndex + args.SessionID);
            if (args.SessionID.Contains(primaryChannel))
            {
                IMessage newMsg;
                string nonProtocolOrdID;
                (newMsg,nonProtocolOrdID) = engine.PrepareMessageNew(new NewMessageParameters(ProtocolType.Fix50sp2, account, symbol, qty, Side.Buy, TimeInForce.Day, OrdType.Limit, price));
                SetMandatoryFields(newMsg);
                engine.SendMessage(newMsg,connectorIndex);
                Thread.Sleep(3000);
                //IMessage replaceMsg = engine.PrepareMessageReplace(new ReplaceMessageParameters(nonProtocolOrdID, qty, price + 0.01m));
                //replaceMsg.SetSymbol(symbol);
                //SetMandatoryFields(replaceMsg);
                //engine.SendMessage(replaceMsg, connectorIndex);
                Thread.Sleep(3000);
                //IMessage cancelMessage = engine.PrepareMessageCancel(new CancelMessageParameters(nonProtocolOrdID));
                //SetMandatoryFields(cancelMessage);
                //engine.SendMessage(cancelMessage, connectorIndex);
                (newMsg, nonProtocolOrdID) = engine.PrepareMessageNew(new NewMessageParameters(ProtocolType.Fix50sp2, account, symbol, qty , Side.Sell, TimeInForce.Day, OrdType.Limit, price));
                SetMandatoryFields(newMsg);
                engine.SendMessage(newMsg,connectorIndex);

                Account acc = Account.GetAccount(account);
                Console.WriteLine("Positions");
                foreach(Position pos in acc.Positions)
                {
                    Console.WriteLine(pos);
                }
            }
        }

        private static void SetMandatoryFields(IMessage msg)
        {
            if(msg.GetMsgType() != MsgType.Cancel)
            {
                msg.SetGenericField(70, "dummyAlloc");
                msg.SetGenericField(528, "A");
            }
            
            
        }

        public static void OnInboundMessage(object sender,InboundMessageEventArgs args)
        {
            Thread.Sleep(2000);
            Console.WriteLine("Inside invoked event function" + args.msg);
            Console.WriteLine(Account.Accounts.Count);
        }
    }
}
