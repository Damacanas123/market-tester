using MarketTester.Base;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;

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

        private bool isBuy;
        public bool IsBuy { get => isBuy; set { isBuy = value;NotifyPropertyChanged(nameof(IsBuy)); } }

        private Channel channel;
        public Channel Channel { get => channel; set { channel = value; NotifyPropertyChanged(nameof(Channel)); } }

        private object channelsLock = new object();
        public ObservableCollection<Channel> Channels { get; set; } = new ObservableCollection<Channel>();

        #endregion

        #region private fields
        private bool isReplacingWindow;
        #endregion

        #region Commands

        public ICommand CommandSwitchSide { get; set; }
        private void CommandSwitchSideExecute(object param)
        {
            if(IsBuy)
            {
                SideColor = (SolidColorBrush)Application.Current.Resources[Const.ResourceColorSell];
                IsBuy = false;
            }
            else
            {
                SideColor = (SolidColorBrush)Application.Current.Resources[Const.ResourceColorBuy];
                IsBuy = true;
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
            SideColor = (SolidColorBrush)Application.Current.Resources[Const.ResourceColorSell];
            UpdateChannelsCollection();
            Connection.Connector.GetInstance().ActiveChannels.CollectionChanged += OnChannelsCollectionChanged;
            
        }


        #endregion

        private void OnChannelsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateChannelsCollection();
        }

        private void UpdateChannelsCollection()
        {
            lock (channelsLock)
            {
                Channels.Clear();
                foreach (Channel channel in Connection.Connector.GetInstance().ActiveChannels)
                {
                    if (channel.IsConnected)
                    {
                        Channels.Add(channel);
                    }
                }
            }
        }
    }
}
