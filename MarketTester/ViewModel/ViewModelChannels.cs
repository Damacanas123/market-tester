using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using MarketTester.Base;
using MarketTester.Model;



namespace MarketTester.ViewModel
{
    public class ViewModelChannels : BaseNotifier
    {
        public ObservableCollection<Channel> Channels { get; set; }

        private Channel selectedChannel;
        public Channel SelectedChannel
        {
            get { return selectedChannel; }
            set
            {
                if (value != selectedChannel)
                {
                    selectedChannel = value;NotifyPropertyChanged(nameof(SelectedChannel));
                }
            }
        }
        public ObservableCollection<string> ActiveSessions { get; set; }
        public ObservableCollection<string> InactiveSessions { get; set; }
        public ViewModelChannels()
        {
            CommandSelectChannel = new BaseCommand(CommandSelectChannelExecute, CommandSelectChannelCanExecute);
            CommandConnectChannel = new BaseCommand(CommandConnectChannelExecute, CommandConnectChannelCanExecute);
            Channels = Connection.Connector.GetInstance().Channels;
            
        }
        public ICommand CommandSelectChannel { get; set; }
        public void CommandSelectChannelExecute(object sender)
        {
            SelectedChannel = (Channel)sender;
        }
        public bool CommandSelectChannelCanExecute()
        {
            return true;
        }


        public BaseCommand CommandConnectChannel { get; set; }
        public void CommandConnectChannelExecute(object executor)
        {
            Connection.Connector.GetInstance().StartConnection((Channel)executor);
        }

        public bool CommandConnectChannelCanExecute()
        {
            return true;
        }
    }
}
