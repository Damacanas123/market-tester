﻿using System;
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
using MarketTester.ViewModel.Manager;
using BackOfficeEngine.MessageEnums;

using QuickFix.Fields;
using BackOfficeEngine.Model;

namespace MarketTester.Connection
{
    public class Connector
    {
        
        public static ObservableCollection<Channel> Channels = new ObservableCollection<Channel>();

        private object channelLock = new object();
        public static ObservableCollection<Channel> ActiveChannels = new ObservableCollection<Channel>();
        public static ObservableCollection<Channel> InactiveChannels = new ObservableCollection<Channel>();

        private static Connector instance;

        private Engine engine;
        private Connector() 
        {
            engine = Engine.GetInstance(500, @"C:\MATRIKS_OMS\MarketTester\BackOfficeEngine\");
            engine.OnLogonEvent += OnLogon;
            engine.OnLogoutEvent += OnLogout;
            engine.OnCreateSessionEvent += OnCreateSession;
            engine.SessionMessageRejectEvent += OnSessionMessageReject;
            engine.ApplicationMessageRejectEvent += OnApplicationMessageReject;
            foreach(ConfigFile configFile  in JsonConfig.GetInstance().ConfigFiles)
            {
                Channel channel = new Channel(configFile.FilePath, configFile.ProtocolType);
                Channels.Add(channel);
                if(configFile.CredentialParams != null)
                {
                    channel.credentialParams = configFile.CredentialParams;
                }
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
                channel.IsConnectingDisconnecting = true;
                engine.Connect(channel.ConnectorName);
            }).Start();
        }

        
        public void ConfigureAndConnect(Channel channel)
        {
            new Task(() =>
            {
                channel.ConnectorName = engine.NewConnection(channel.ConfigFilePath, channel.ProtocolType);
                if(channel.credentialParams == null)
                {
                    engine.ConfigureConnection(channel.ConnectorName, channel.ConfigFilePath);
                }
                else
                {
                    engine.ConfigureConnection(channel.ConnectorName, channel.ConfigFilePath,channel.credentialParams);
                }                
                channel.IsConfigured = true;
                Connect(channel);
            }).Start();
        }

        public void OnLogon(object sender,OnLogonEventArgs args)
        {
            Channel ch = Channels.First((o) => o.ConnectorName == args.ConnectorName);

            lock (channelLock)
            {                
                if (!ch.IsConnected)
                {
                    ch.IsConnectingDisconnecting = false;
                    ch.IsConnected = true;
                    ActiveChannels.Add(ch);
                    InactiveChannels.Remove(ch);
                }
            }
            ch.AddActive(args.SessionID);
            ch.RemoveInactive(args.SessionID);
        }

        public void OnLogout(object sender, OnLogoutEventArgs args)
        {
            Channel ch = Channels.First((o) => o.ConnectorName == args.ConnectorName);
            lock (channelLock)
            {
                ch.RemoveActive(args.SessionID);
                ch.AddInactive(args.SessionID);
                if(ch.ActiveSessions.Count == 0)
                {
                    ch.IsConnectingDisconnecting = false;
                    ch.IsConnected = false;
                    ActiveChannels.Add(ch);
                    InactiveChannels.Remove(ch);
                }
            }
        }

        public void OnCreateSession(object sender,OnCreateSessionEventArgs args)
        {
            Channel ch = Channels.First((o) => o.ConnectorName == args.ConnectorName);
            ch.AddInactive(args.SessionID);
        }

        public void Disconnect(Channel ch)
        {
            new Thread(() =>
            {
                ch.IsConnectingDisconnecting = true;
                engine.Disconnect(ch.ConnectorName);
            }).Start();
        }

        public void SendMessageNew(Channel ch,NewMessageParameters prms)
        {
            engine.SendMessageNew(prms, ch.ConnectorName);
        }

        public void SendMessageReplace(string connectorName, ReplaceMessageParameters prms)
        {
            engine.SendMessageReplace(prms, connectorName);
        }

        public void SendMessageCancel(string connectorName, CancelMessageParameters prms)
        {
            engine.SendMessageCancel(prms, connectorName);
        }

        public void SendMessage(string connectorName,string fixMsg,bool overrideSessionTags)
        {
            engine.SendMessage(fixMsg, connectorName, overrideSessionTags);
        }

        private void OnSessionMessageReject(object sender,OnSessionMessageRejectEventArgs args)
        {   
            string infoMsg = App.Current.Resources[ResourceKeys.StringSession].ToString();
            infoMsg += MessageRejectCommon(args.messageOrigin, args.msg);
            InfoManager.PublishInfo(Enumeration.EInfo.Primary, infoMsg);
        }

        private void OnApplicationMessageReject(object sender, OnApplicationMessageRejectEventArgs args)
        {
            string infoMsg = App.Current.Resources[ResourceKeys.StringApplication].ToString();
            infoMsg += MessageRejectCommon(args.messageOrigin, args.msg);
            InfoManager.PublishInfo(Enumeration.EInfo.Primary, infoMsg);
        }

        private string MessageRejectCommon(MessageOrigin messageOrigin,IMessage msg)
        {
            string infoMsg = "";
            switch (messageOrigin)
            {
                case MessageOrigin.Inbound:
                    infoMsg += " " + App.Current.Resources[ResourceKeys.StringInbound].ToString();
                    break;
                case MessageOrigin.Outbound:
                    infoMsg += " " + App.Current.Resources[ResourceKeys.StringOutbound].ToString();
                    break;
            }
            infoMsg += " " + App.Current.Resources[ResourceKeys.StringRejectMessage];
            infoMsg += "\n" + App.Current.Resources[ResourceKeys.StringRejectReason] + "\n";
            if (msg.IsSetGenericField(Tags.Text))
            {
                infoMsg += " " + msg.GetGenericField(Tags.Text);
            }
            else
            {
                infoMsg += "\n" + App.Current.Resources[ResourceKeys.StringNoReasonStated];
            }
            return infoMsg;
        }
    }
}
