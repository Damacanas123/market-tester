using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Threading;

using MarketTester.Model;
using MarketTester.Helper;

using BackOfficeEngine;
using BackOfficeEngine.Events;
using BackOfficeEngine.ParamPacker;

namespace MarketTester.Connection
{
    public class Connector
    {
        
        public static ObservableCollection<Channel> Channels = new ObservableCollection<Channel>();

        private object channelLock = new object();
        public ObservableCollection<Channel> ActiveChannels = new ObservableCollection<Channel>();
        public ObservableCollection<Channel> InactiveChannels = new ObservableCollection<Channel>();

        private static Connector instance;

        private Engine engine;
        private Connector() 
        {
            engine = Engine.GetInstance(500, @"C:\MATRIKS_OMS\MarketTester\BackOfficeEngine\");
            engine.OnLogonEvent += OnLogon;
            engine.OnLogoutEvent += OnLogout;
            engine.OnCreateSessionEvent += OnCreateSession;
            foreach(ConfigFile configFile  in JsonConfig.GetInstance().ConfigFiles)
            {
                Channels.Add(new Channel(configFile.FilePath,configFile.ProtocolType));
            }
        }

        public static Connector GetInstance()
        {
            if(instance == null)
            {
                instance = new Connector();
            }
            return instance;
        }

        public void Connect(Channel channel)
        {
            new Thread(() =>
            {                
                engine.Connect(channel.ConnectorIndex);
            }).Start();
        }

        
        public void ConfigureAndConnect(Channel channel)
        {
            new Task(() =>
            {
                channel.ConnectorIndex = engine.NewConnection(channel.ConfigFilePath, channel.ProtocolType);
                engine.ConfigureConnection(channel.ConnectorIndex, channel.ConfigFilePath);
                channel.IsConfigured = true;
                engine.Connect(channel.ConnectorIndex);
            }).Start();
        }

        public void OnLogon(object sender,OnLogonEventArgs args)
        {
            Channel ch = Channels.First((o) => o.ConnectorIndex == args.ConnectorIndex);
            if (!ch.IsConnected)
            {
                ch.IsConnected = true;
                lock (channelLock)
                {
                    ActiveChannels.Add(ch);
                    InactiveChannels.Remove(ch);
                }
            }
            ch.AddActive(args.SessionID);
            ch.RemoveInactive(args.SessionID);
        }

        public void OnLogout(object sender, OnLogoutEventArgs args)
        {
            Channel ch = Channels.First((o) => o.ConnectorIndex == args.ConnectorIndex);
            ch.RemoveActive(args.SessionID);
            ch.AddInactive(args.SessionID);
            if(ch.ActiveSessions.Count == 0)
            {
                ch.IsConnected = false;
                lock (channelLock)
                {
                    ActiveChannels.Add(ch);
                    InactiveChannels.Remove(ch);
                }
            }
        }

        public void OnCreateSession(object sender,OnCreateSessionEventArgs args)
        {
            Channel ch = Channels.First((o) => o.ConnectorIndex == args.ConnectorIndex);
            ch.AddInactive(args.SessionID);
        }

        public void Disconnect(Channel ch)
        {
            new Thread(() =>
            {
                engine.Disconnect(ch.ConnectorIndex);
            }).Start();
        }

        public void SendMessageNew(Channel ch,NewMessageParameters prms)
        {
            engine.SendMessageNew(prms, ch.ConnectorIndex);
        }

        public void SendMessageReplace(Channel ch, ReplaceMessageParameters prms)
        {
            engine.SendMessageReplace(prms, ch.ConnectorIndex);
        }

        public void SendMeesageCancel(Channel ch, CancelMessageParameters prms)
        {
            engine.SendMessageCancel(prms, ch.ConnectorIndex);
        }

    }
}
