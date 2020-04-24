using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketTester.Base;
using MarketTester.Helper;
using MarketTester.Model.Sniffer;
using PcapDotNet.Core;

namespace MarketTester.ViewModel
{
    public class ViewModelFixSniffer : BaseNotifier
    {

        private string InvalidPort { get; set; }
        private string infoText;
        private bool IsRunning { get; set; }

        public string InfoText
        {
            get { return infoText; }
            set
            {
                infoText = value;
                NotifyPropertyChanged(nameof(InfoText));
            }
        }

        private string infoTextResourceKey;

        public string InfoTextResourceKey
        {
            get { return infoTextResourceKey; }
            set
            {
                if(value != null)
                {
                    infoTextResourceKey = value;
                    InfoText = App.Current.Resources[infoTextResourceKey].ToString();
                    NotifyPropertyChanged(nameof(InfoTextResourceKey));
                }
            }
        }

        private string textPorts;

        public string TextPorts
        {
            get { return textPorts; }
            set
            {
                textPorts = value;
                NotifyPropertyChanged(nameof(TextPorts));
            }
        }


        private void OnLanguageChanged()
        {
            if (InfoTextResourceKey != null)
            {
                InfoText = App.Current.Resources[InfoTextResourceKey].ToString();
                if(InfoTextResourceKey == ResourceKeys.StringInvalidPort)
                {
                    InfoText += InvalidPort;
                }
            }
            
            
        }

        private string textStartStop;

        public string TextStartStop
        {
            get { return textStartStop; }
            set
            {
                textStartStop = value;
                NotifyPropertyChanged(nameof(TextStartStop));
            }
        }

        private LivePacketDevice selectedDevice;

        public LivePacketDevice SelectedDevice
        {
            get { return selectedDevice; }
            set
            {
                selectedDevice = value;
                MarketTesterUtil.ConsoleDebug("Set selected device " + selectedDevice.Description);
                NotifyPropertyChanged(nameof(SelectedDevice));
            }
        }

        private int selectedDeviceIndex;

        public int SelectedDeviceIndex
        {
            get { return selectedDeviceIndex; }
            set
            {
                selectedDeviceIndex = value;
                SelectedDevice = Devices[selectedDeviceIndex];
                NotifyPropertyChanged(nameof(SelectedDeviceIndex));                
            }
        }


        public FixDelayHandler FixDelayHandler { get; } = new FixDelayHandler();

        public List<LivePacketDevice> Devices { get; set; }
        public List<string> DeviceNames { get; set; }
        public ViewModelFixSniffer()
        {
            CommandStartStop = new BaseCommand(CommandStartStopExecute, CommandStartStopCanExecute);
            Devices = LivePacketDevice.AllLocalMachine.ToList();
            DeviceNames = Devices.Select(item => item.Description).ToList();
            SelectedDeviceIndex = 0;
            Settings.GetInstance().LanguageChangedEventHandler += OnLanguageChanged;
            TextStartStop = App.Current.Resources[ResourceKeys.StringStart].ToString();
        }
        #region commands
        #region CommandStartStop
        public BaseCommand CommandStartStop { get; set; }
        public void CommandStartStopExecute(object param)
        {
            if (!IsRunning)
            {
                IsRunning = true;
                if (SelectedDevice == null)
                {
                    InfoTextResourceKey = ResourceKeys.StringPleaseSelectANetworkAdapter;
                    return;
                }
                if (string.IsNullOrWhiteSpace(TextPorts))
                {
                    InfoTextResourceKey = ResourceKeys.StringPleaseEnterPort;
                    return;
                }

                string[] ports = TextPorts.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                List<ushort> portsUshort = new List<ushort>();
                foreach (string port in ports)
                {
                    if (!ushort.TryParse(port, out ushort portUshort))
                    {
                        InvalidPort = port;
                        InfoTextResourceKey = ResourceKeys.StringInvalidPort;
                        InfoText += InvalidPort;
                        return;
                    }
                    else
                    {
                        portsUshort.Add(portUshort);
                    }
                }
                FixDelayHandler.SetDevice(SelectedDevice);
                FixDelayHandler.SetPorts(portsUshort);
                FixDelayHandler.Start();
                InfoTextResourceKey = ResourceKeys.StringStartedSniffing;
                TextStartStop = App.Current.Resources[ResourceKeys.StringStop].ToString();
            }
            else
            {
                IsRunning = false;
                FixDelayHandler.Stop();
                TextStartStop = App.Current.Resources[ResourceKeys.StringStart].ToString();
            }
            
        }
        public bool CommandStartStopCanExecute()
        {
            return true;
        }
        #endregion
        #endregion


        #region
        public void OnClose()
        {
            FixDelayHandler.Stop();
        }
        #endregion
    }
}
