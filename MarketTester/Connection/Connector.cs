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
using MarketTester.ViewModel.Manager;
using BackOfficeEngine.MessageEnums;

using QuickFix.Fields;
using BackOfficeEngine.Model;
using BackOfficeEngine.Helper;
using System.IO;
using MarketTester.UI.Popup;
using System.Windows.Controls;
using FixHelper;
using MarketTester.Exceptions;
using System.Windows.Controls.Primitives;

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
            bool exceptionThrown = false;
            try
            {
                JsonConfig jsonConfig = JsonConfig.GetInstance();
            }
            catch(NotSupportedProtocolType ex)
            {
                exceptionThrown = true;
                Util.LogDebugError(ex);
                UserControlErrorPopup popup = new UserControlErrorPopup(ResourceKeys.StringUnsupportedProtocolType);
                popup.SetExtraText(ex.Data["data1"].ToString());
                PopupManager.OpenErrorPopup(popup);
            }
            catch(FileFormatException ex)
            {
                exceptionThrown = true;
                Util.LogDebugError(ex);
                UserControlErrorPopup popup = new UserControlErrorPopup(ResourceKeys.StringInvalidJsonFormat);
                PopupManager.OpenErrorPopup(popup);
            }
            catch(FileNotFoundException ex)
            {
                exceptionThrown = true;
                Util.LogDebugError(ex);
                UserControlErrorPopup popup = new UserControlErrorPopup(ResourceKeys.StringFileNotFound);
                popup.SetExtraText(JsonConfig.JSON_CONFIG_PATH);
                PopupManager.OpenErrorPopup(popup);
            }
            catch(Exception ex)
            {
                exceptionThrown = true;
                Util.LogError(ex);
                UserControlErrorPopup popup = new UserControlErrorPopup(ResourceKeys.StringUnknownErrorOccured);
                popup.SetExtraText(JsonConfig.JSON_CONFIG_PATH);
                PopupManager.OpenErrorPopup(popup);
                
            }
            finally
            {
                if(exceptionThrown)
                    Environment.Exit(0);
            }
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
            new Thread(() =>
            {
                try
                {
                    if (!File.Exists(channel.ConfigFilePath))
                    {
                        App.Invoke(() =>
                        {
                            PopupManager.OpenErrorPopup(new UserControlErrorPopup(ResourceKeys.StringCantFindConfigFile));
                        });
                        return;
                    }
                    channel.ConnectorName = engine.NewConnection(channel.ConfigFilePath, channel.ProtocolType);
                    try
                    {
                        if (channel.credentialParams == null)
                        {
                            engine.ConfigureConnection(channel.ConnectorName, channel.ConfigFilePath);
                        }
                        else
                        {
                            engine.ConfigureConnection(channel.ConnectorName, channel.ConfigFilePath, channel.credentialParams);
                        }
                    }
                    catch(Exception ex)
                    {
                        UserControlErrorPopup errorPopup = new UserControlErrorPopup(ResourceKeys.StringConfigurationError);
                        errorPopup.SetExtraText(ex.Message);
                        PopupManager.OpenErrorPopup(errorPopup);
                    }
                    try
                    {
                        channel.IsConfigured = true;
                        Connect(channel);
                    }
                    catch(Exception ex)
                    {
                        UserControlErrorPopup errorPopup = new UserControlErrorPopup(ResourceKeys.StringCantConnect);
                        errorPopup.SetExtraText(channel.Name);
                        PopupManager.OpenErrorPopup(errorPopup);
                    }
                }
                catch(Exception ex)
                {
                    Util.LogError(ex);
                }
                
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
                    ActiveChannels.Remove(ch);
                    InactiveChannels.Add(ch);
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
            if (args.msg.IsSetGenericField(Tags.RefTagID))
            {
                infoMsg += Environment.NewLine;
                string value = args.msg.GetGenericField(Tags.RefTagID);
                string explanation = GetValueExplanation(Tags.RefTagID, value);
                infoMsg += App.Current.Resources[ResourceKeys.StringReferenceTag] + " : " + (explanation != null ? explanation : "") + $" ({value})" + Environment.NewLine;
                if (args.msg.IsSetGenericField(Tags.RefMsgType))
                {
                    value = args.msg.GetGenericField(Tags.RefMsgType);
                    explanation = GetValueExplanation(Tags.MsgType, value);
                    infoMsg += App.Current.Resources[ResourceKeys.StringReferenceMsgType] + " : " + (explanation != null ? explanation : "") + $" ({value})" + Environment.NewLine;
                }
            }
            
            if (args.msg.IsSetGenericField(Tags.SessionRejectReason))
            {
                string value = args.msg.GetGenericField(Tags.SessionRejectReason);
                string explanation = GetValueExplanation(Tags.SessionRejectReason, value);
                infoMsg += App.Current.Resources[ResourceKeys.StringSessionRejectReason] + " : " + (explanation != null ? explanation : "") + $" ({value})";
            }
            InfoManager.PublishInfo(Enumeration.EInfo.Primary, infoMsg);
        }

        public string GetValueExplanation(int tag,string value)
        {
            AllFixTags allFixTags = AllFixTags.GetInstance();
            if (allFixTags.msgValueMap.TryGetValue(tag, out Dictionary<string, string> valueMap))
            {
                if (valueMap.TryGetValue(value, out string explanation))
                {
                    return explanation;
                }
            }
            return null;
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
            infoMsg += Environment.NewLine + App.Current.Resources[ResourceKeys.StringRejectReason] + Environment.NewLine;
            if (msg.IsSetGenericField(Tags.Text))
            {
                infoMsg += msg.GetGenericField(Tags.Text);
            }
            else
            {
                infoMsg += Environment.NewLine + App.Current.Resources[ResourceKeys.StringNoReasonStated];
            }
            return infoMsg;
        }

        public bool CheckChannelConnection(string channelName)
        {
            Channel channel = ActiveChannels.FirstOrDefault((o) => o.Name == channelName);
            if (channel == null)
            {
                return false;
            }
            return true;
        }
    }
}
