using MarketTester.Base;
using System.Windows;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Globalization;
using System;
using System.Windows.Controls;

using BackOfficeEngine.ParamPacker;
using BackOfficeEngine.MessageEnums;
using BackOfficeEngine.Model;

using MarketTester.Model;
using MarketTester.Helper;
using MarketTester.Connection;

namespace MarketTester.ViewModel
{
    public class ViewModelOrderEntry : BaseNotifier
    {
        #region UI properties
        private string accountText;
        public string AccountText { get => accountText; set { accountText = value;NotifyPropertyChanged(nameof(AccountText)); } }

        private string priceText;
        public string PriceText 
        { 
            get => priceText; 
            set 
            { 
                priceText = value;
                priceText = Util.RemoveNonNumericKeepDot(priceText);
                NotifyPropertyChanged(nameof(PriceText)); 
            } 
        }

        private string symbolText;
        public string SymbolText { get => symbolText; set { symbolText = value; NotifyPropertyChanged(nameof(SymbolText)); } }

        private string quantityText;
        public string QuantityText { get => quantityText; set { quantityText = value; NotifyPropertyChanged(nameof(QuantityText)); } }

        private string stepPriceText;
        public string StepPriceText { get => stepPriceText; set { stepPriceText = value; NotifyPropertyChanged(nameof(StepPriceText)); } }

        private Brush sideColor;
        public Brush SideColor { get => sideColor; set { sideColor = value; NotifyPropertyChanged(nameof(SideColor)); } }

        private Side side;
        public Side Side { 
            get => side; 
            set 
            { 
                side = value;
                NotifyPropertyChanged(nameof(Side));
                switch (side)
                {
                    case Side.Buy:
                        SideColor = (SolidColorBrush)Application.Current.Resources[Const.ResourceColorBuy];
                        break;
                    case Side.Sell:
                        SideColor = (SolidColorBrush)Application.Current.Resources[Const.ResourceColorSell];
                        break;
                }                    
            } 
        }

        private TimeInForce timeInForce;
        public TimeInForce TimeInForce { get => timeInForce;set { timeInForce = value;NotifyPropertyChanged(nameof(TimeInForce)); } }

        private OrdType ordType;
        public OrdType OrdType { get => ordType; set { ordType = value;NotifyPropertyChanged(nameof(OrdType)); } }

        private Channel channel;
        public Channel Channel { get => channel; set { channel = value; NotifyPropertyChanged(nameof(Channel)); } }

        private Order order;
        public Order Order
        {
            get
            {
                return order;
            }
            set
            {
                IsNewOrderWindow = false;
                order = value;
                AccountText = order.Account.ToString();
                PriceText = order.Price.ToString(CultureInfo.InvariantCulture);
                SymbolText = order.Symbol;
                QuantityText = order.OrderQty.ToString(CultureInfo.InvariantCulture);
                Side = order.Side;
                TimeInForce = order.TimeInForce;
                OrdType = order.OrdType;
                Channel = Connector.ActiveChannels.FirstOrDefault((o) => o.ConnectorName == order.ConnectorName);
                if(Channel == null)
                {
                    InfoTextResourceKey = "StringChannelNotConnectedWarning";
                }
            }
        }

        private string infoTextResourceKey;
        private string InfoTextResourceKey
        {
            get
            {
                return infoTextResourceKey;
            }
            set
            {
                infoTextResourceKey = value;
                if (App.Current.Resources.Contains(value))
                {
                    InfoText = App.Current.Resources[value] + " (" + order.ConnectorName + ")";
                }
                
            }
        }

        private void OnLanguageChanged()
        {
            if(InfoTextResourceKey != null)
            {
                InfoText = App.Current.Resources[InfoTextResourceKey].ToString() + " (" + order.ConnectorName + ")";
            }
            
        }

        private string infoText;
        public string InfoText { get { return infoText; } set { infoText = value; NotifyPropertyChanged(nameof(InfoText)); } }

        
        public ObservableCollection<Channel> Channels { get; set; } = new ObservableCollection<Channel>();

        #endregion

        #region private fields
        private bool isNewOrderWindow = true;
        public bool IsNewOrderWindow { get { return isNewOrderWindow; } set { isNewOrderWindow = value;NotifyPropertyChanged(nameof(IsNewOrderWindow)); } } 
        #endregion

        #region Commands

        public ICommand CommandSwitchSide { get; set; }
        private void CommandSwitchSideExecute(object param)
        {
            if(Side == Side.Buy)
            {
                Side = Side.Sell;
            }
            else
            {
                Side = Side.Buy;
            }  
        }
        private bool CommandSwitchSideCanExecute()
        {
            return IsNewOrderWindow;
        }

        public ICommand CommandSendOrder { get; set; }
        private void CommandSendOrderExecute(object param)
        {
            if(Channel != null && Channel.IsConfigured)
            {
                if (IsNewOrderWindow)
                {
                    NewMessageParameters prms = new NewMessageParameters(
                       Channel.ProtocolType,
                       AccountText,
                       SymbolText,
                       decimal.Parse(QuantityText, CultureInfo.InvariantCulture),
                       Side,
                       TimeInForce,
                       OrdType,
                       decimal.Parse(PriceText, CultureInfo.InvariantCulture));
                    Connection.Connector.GetInstance().SendMessageNew(Channel, prms);
                    
                }
                else
                {
                    ReplaceMessageParameters prms = new ReplaceMessageParameters(
                         order.NonProtocolID,
                         decimal.Parse(QuantityText, CultureInfo.InvariantCulture),
                         decimal.Parse(PriceText, CultureInfo.InvariantCulture));
                    Connection.Connector.GetInstance().SendMessageReplace(order.ConnectorName, prms);
                }
            }
        }
        private bool CommandSendOrderCanExecute()
        {
            return true;
        }
        #endregion

        #region Constructor
        public ViewModelOrderEntry()
        {
            CommandSwitchSide = new BaseCommand(CommandSwitchSideExecute, CommandSwitchSideCanExecute);
            CommandSendOrder = new BaseCommand(CommandSendOrderExecute, CommandSendOrderCanExecute);
            CommandRadioButtonOrdType = new BaseCommand(CommandRadioButtonOrdTypeExecute, CommandRadioButtonOrdTypeCanExecute);
            TimeInForce = TimeInForce.Day;
            SideColor = (SolidColorBrush)Application.Current.Resources[Const.ResourceColorSell];
            Side = Side.Sell;
            UpdateChannelsCollection();
            Connection.Connector.ActiveChannels.CollectionChanged += OnActiveChannelsCollectionChanged;
            SetDefault();
            Settings.GetInstance().LanguageChangedEventHandler += OnLanguageChanged;
        }

        private void SetDefault()
        {
            PriceText = "10";
            AccountText = "DE-1";
            SymbolText = "F_GARAN1220";
            QuantityText = "10";
            OrdType = OrdType.Limit;
        }

        public ICommand CommandRadioButtonOrdType { get; set; }
        public void CommandRadioButtonOrdTypeExecute (object executor)
        {
            RadioButton sender = (RadioButton)executor;
            if (sender.Name == "RadioButtonLimit")
            {
                OrdType = OrdType.Limit;
            }
            else if (sender.Name == "RadioButtonMarketToLimit")
            {
                OrdType = OrdType.MarketToLimit;
            }
            else
            {
                OrdType = OrdType.Market;
            }
        }

        public bool CommandRadioButtonOrdTypeCanExecute()
        {
            return IsNewOrderWindow;
        }


        #endregion

        private void OnActiveChannelsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (IsNewOrderWindow)
            {
                UpdateChannelsCollection();
            }
        }

        private void UpdateChannelsCollection()
        {            
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    lock (Channels)
                    {
                        Channels.Clear();
                        foreach (Channel channel in Connection.Connector.ActiveChannels)
                        {
                            if (channel.IsConnected)
                            {
                                if (!Channels.Contains(channel))
                                    Channels.Add(channel);
                            }
                        }
                    }
                });
            }   
        }


        
    }
}
