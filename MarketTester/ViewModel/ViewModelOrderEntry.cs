using MarketTester.Base;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Globalization;
using System;
using System.Windows.Controls;

using BackOfficeEngine.ParamPacker;
using BackOfficeEngine.MessageEnums;

using MarketTester.Model;


namespace MarketTester.ViewModel
{
    public class ViewModelOrderEntry : BaseNotifier
    {
        #region UI properties
        private string accountText;
        public string AccountText { get => accountText; set { accountText = value;NotifyPropertyChanged(nameof(AccountText)); } }

        private string priceText;
        public string PriceText { get => priceText; set { priceText = value; NotifyPropertyChanged(nameof(PriceText)); } }

        private string symbolText;
        public string SymbolText { get => symbolText; set { symbolText = value; NotifyPropertyChanged(nameof(SymbolText)); } }

        private string quantityText;
        public string QuantityText { get => quantityText; set { quantityText = value; NotifyPropertyChanged(nameof(QuantityText)); } }

        private string sideText;
        public string SideText { get => sideText; set { sideText = value; NotifyPropertyChanged(nameof(SideText)); } }

        private string stepPriceText;
        public string StepPriceText { get => stepPriceText; set { stepPriceText = value; NotifyPropertyChanged(nameof(StepPriceText)); } }

        private Brush sideColor;
        public Brush SideColor { get => sideColor; set { sideColor = value; NotifyPropertyChanged(nameof(SideColor)); } }

        private Side side;
        public Side Side { get => side; set { side = value;NotifyPropertyChanged(nameof(Side)); } }

        private TimeInForce timeInForce;
        public TimeInForce TimeInForce { get => timeInForce;set { timeInForce = value;NotifyPropertyChanged(nameof(TimeInForce)); } }

        private OrdType ordType;
        public OrdType OrdType { get => ordType; set { ordType = value;NotifyPropertyChanged(nameof(OrdType)); } }

        private Channel channel;
        public Channel Channel { get => channel; set { channel = value; NotifyPropertyChanged(nameof(Channel)); } }

        
        public ObservableCollection<Channel> Channels { get; set; } = new ObservableCollection<Channel>();

        #endregion

        #region private fields
        private bool isReplacingWindow;
        #endregion

        #region Commands

        public ICommand CommandSwitchSide { get; set; }
        private void CommandSwitchSideExecute(object param)
        {
            if(Side == Side.Buy)
            {
                SideColor = (SolidColorBrush)Application.Current.Resources[Const.ResourceColorSell];
                Side = Side.Sell;
            }
            else
            {
                SideColor = (SolidColorBrush)Application.Current.Resources[Const.ResourceColorBuy];
                Side = Side.Buy;
            }  
        }
        private bool CommandSwitchSideCanExecute()
        {
            return !isReplacingWindow;
        }

        public ICommand CommandSendOrder { get; set; }
        private void CommandSendOrderExecute(object param)
        {
            if(Channel != null)
            {
                if (isReplacingWindow)
                {

                }
                else
                {
                    NewMessageParameters prms = new NewMessageParameters(
                        Channel.ProtocolType,
                        AccountText,
                        SymbolText,
                        decimal.Parse(QuantityText, CultureInfo.InvariantCulture),
                        Side,
                        TimeInForce,
                        OrdType,
                        decimal.Parse(PriceText,CultureInfo.InvariantCulture));
                    Connection.Connector.GetInstance().SendMessageNew(Channel, prms);
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
            Connection.Connector.GetInstance().ActiveChannels.CollectionChanged += OnChannelsCollectionChanged;
            SetDefault();
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
            return !isReplacingWindow;
        }


        #endregion

        private void OnChannelsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateChannelsCollection();
        }

        private void UpdateChannelsCollection()
        {           
            if(Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    Channels.Clear();
                    foreach (Channel channel in Connection.Connector.GetInstance().ActiveChannels)
                    {
                        if (channel.IsConnected)
                        {
                            if (!Channels.Contains(channel))
                                Channels.Add(channel);
                        }
                    }
                });
            }
            
        }
    }
}
