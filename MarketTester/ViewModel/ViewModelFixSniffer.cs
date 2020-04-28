using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BackOfficeEngine.Helper;
using MarketTester.Base;
using MarketTester.Helper;
using MarketTester.Model.Sniffer;
using MarketTester.UI.Popup;
using PcapDotNet.Core;

namespace MarketTester.ViewModel
{
    public class ViewModelFixSniffer : BaseNotifier
    {

        private string InvalidPort { get; set; }
        private string infoText;
        private bool isLocalActive;

        public bool IsLocalActive
        {
            get { return isLocalActive; }
            set
            {
                isLocalActive = value;
                NotifyPropertyChanged(nameof(IsLocalActive));
            }
        }

        private bool isRemoteActive;

        public bool IsRemoteActive
        {
            get { return isRemoteActive; }
            set
            {
                isRemoteActive = value;
                NotifyPropertyChanged(nameof(IsRemoteActive));
            }
        }


        private bool localSnifferActive;

        public bool LocalSnifferActive
        {
            get { return localSnifferActive; }
            set
            {
                localSnifferActive = value;
                NotifyPropertyChanged(nameof(LocalSnifferActive));
            }
        }

        private bool remoteSnifferActive;

        public bool RemoteSnifferActive
        {
            get { return remoteSnifferActive; }
            set
            {
                remoteSnifferActive = value;
                NotifyPropertyChanged(nameof(RemoteSnifferActive));
            }
        }


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


        #region remote sinffer properties
        private string textRemoteSnifferHost;

        public string TextRemoteSnifferHost
        {
            get { return textRemoteSnifferHost; }
            set
            {
                textRemoteSnifferHost = value;
                NotifyPropertyChanged(nameof(TextRemoteSnifferHost));
            }
        }

        private string textRemoteSnifferPort;

        public string TextRemoteSnifferPort
        {
            get { return textRemoteSnifferPort; }
            set
            {
                textRemoteSnifferPort = value;
                NotifyPropertyChanged(nameof(TextRemoteSnifferPort));
            }
        }

        


        

        #endregion


        public FixDelayHandler FixDelayHandler { get; } = new FixDelayHandler();

        public List<LivePacketDevice> Devices { get; set; }
        public List<string> DeviceNames { get; set; }
        public ViewModelFixSniffer()
        {
            CommandStartStop = new BaseCommand(CommandStartStopExecute, CommandStartStopCanExecute);
            CommandRemoteStartStop = new BaseCommand(CommandRemoteStartStopExecute, CommandRemoteStartStopCanExecute);
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
            if (!IsLocalActive)
            {
                IsLocalActive = true;
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
                IsLocalActive = false;
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

        private void OnRemoteFinish()
        {
            App.Invoke(() =>
            {
                IsRemoteActive = false;
                FixDelayHandler.StopRemote();
                TextStartStop = App.Current.Resources[ResourceKeys.StringStart].ToString();
                InfoTextResourceKey = ResourceKeys.StringRemoteSnifferHasStopped;
            });
        }

        #region remote sniffer commands

        #region CommandRemoteStartStop
        public BaseCommand CommandRemoteStartStop { get; set; }
        public void CommandRemoteStartStopExecute(object param)
        {
            if (!IsRemoteActive)
            {
                if (!MarketTesterUtil.CheckIPv4AddressValidity(TextRemoteSnifferHost))
                {
                    InfoTextResourceKey = ResourceKeys.StringHostAddressInvalid;
                    return;
                }

                IPAddress.TryParse(TextRemoteSnifferHost, out IPAddress ipAddress);
                
                if (ushort.TryParse(TextRemoteSnifferPort, out ushort port))
                {

                }
                else
                {
                    InvalidPort = TextRemoteSnifferPort;
                    InfoTextResourceKey = ResourceKeys.StringInvalidPort;
                    InfoText += InvalidPort;
                    return;
                }
                try
                {
                    FixDelayHandler.SetParameters(ipAddress, port);
                }
                catch (Exception ex)
                {
                    Util.LogError(ex);
                    PopupManager.OpenErrorPopup(new UserControlErrorPopup(ResourceKeys.StringUnknownErrorOccured));
                    return;
                }

                bool result; string errorKey;
                
                (result, errorKey) = FixDelayHandler.StartRemote(OnRemoteFinish);
                if (!result)
                {
                    PopupManager.OpenErrorPopup(new UserControlErrorPopup(errorKey));
                    return;
                }
                else
                {

                }
                IsRemoteActive = true;
                InfoTextResourceKey = ResourceKeys.StringStartedSniffing;
                TextStartStop = App.Current.Resources[ResourceKeys.StringStop].ToString();
            }
            else
            {
                IsRemoteActive = false;
                FixDelayHandler.StopRemote();
                TextStartStop = App.Current.Resources[ResourceKeys.StringStart].ToString();
            }
            

        }
        public bool CommandRemoteStartStopCanExecute()
        {
            return true;
        }
        #endregion
        #endregion


        public void OnClose()
        {
            FixDelayHandler.Stop();
        }
    }
}
