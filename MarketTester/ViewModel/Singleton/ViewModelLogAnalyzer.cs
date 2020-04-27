using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MarketTester.Base;
using MarketTester.Model;
using MarketTester.UI.Popup;
using FixLogAnalyzer;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Documents.DocumentStructures;
using MarketTester.Helper;
using System.Threading;
using System.Runtime.CompilerServices;
using System.IO;
using BackOfficeEngine.Helper;

namespace MarketTester.ViewModel
{
    public class ViewModelLogAnalyzer : BaseNotifier
    {
        private static ViewModelLogAnalyzer instance;
        public static ViewModelLogAnalyzer Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new ViewModelLogAnalyzer();
                }
                return instance;
            }
        }
        public List<ILogAnalyzer> Analyzers { get; set; }
        private int selectedIndexFormat;
        public int SelectedIndexFormat
        {
            get { return selectedIndexFormat; }
            set
            {
                selectedIndexFormat = value;
                NotifyPropertyChanged(nameof(SelectedIndexFormat));

            }
        }
        private LineFormat selectedFormat;
        public LineFormat SelectedFormat
        {
            get { return selectedFormat; }
            set
            {
                if(value != null)
                {
                    selectedFormat = value;
                    NotifyPropertyChanged(nameof(SelectedFormat));
                    Name = new string(selectedFormat.Name.ToArray());
                    Format = new string(selectedFormat.Format.ToArray());
                }
            }
        }
        private ILogAnalyzer selectedAnalyzer;
        public ILogAnalyzer SelectedAnalyzer
        {
            get
            {
                return selectedAnalyzer;
            }
            set
            {
                if(value.GetType() == typeof(FixNewGatewayDmaStockLogMerger))
                {
                    SecondFilePathVisibility = Visibility.Visible;
                }
                else
                {
                    SecondFilePathVisibility = Visibility.Hidden;
                }
                selectedAnalyzer = value;
                NotifyPropertyChanged(nameof(SelectedAnalyzer));
            }
        }
        private string InfoTextResourceKey { get; set; }
        private string infoText;
        public string InfoText
        {
            get
            {
                return infoText;
            }
            set
            {
                infoText = value;
                NotifyPropertyChanged(nameof(InfoText));
            }
        }
        private void SetInfoText(string resourceKey)
        {
            InfoText = App.Current.Resources[resourceKey].ToString();
            InfoTextResourceKey = resourceKey;
        }
        private string inputFilePath1;
        public string InputFilePath1
        {
            get
            {
                return inputFilePath1;
            }
            set
            {
                inputFilePath1 = value;
                NotifyPropertyChanged(nameof(InputFilePath1));
            }
        }
        private string inputFilePath2;
        public string InputFilePath2
        {
            get
            {
                return inputFilePath2;
            }
            set
            {
                inputFilePath2 = value;
                NotifyPropertyChanged(nameof(InputFilePath2));
            }
        }
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                NotifyPropertyChanged(nameof(Name));
            }
        }
        private string format;
        public string Format
        {
            get
            {
                return format;
            }
            set
            {
                format = value;
                NotifyPropertyChanged(nameof(Format));
            }
        }
        private string outFilePath = MarketTesterUtil.APPLICATION_RESULTS_DIR + "analysis.csv";
        public string OutFilePath
        {
            get
            {
                return outFilePath;
            }
            set
            {
                outFilePath = value;
                NotifyPropertyChanged(nameof(OutFilePath));
            }
        }

        private Visibility secondFilePathVisibility;
        public Visibility SecondFilePathVisibility
        {
            get
            {
                return secondFilePathVisibility;
            }
            set
            {
                secondFilePathVisibility = value;
                NotifyPropertyChanged(nameof(SecondFilePathVisibility));
            }
        }
        private ViewModelLogAnalyzer()
        {
            LineFormat.LoadLineFormats();

            Analyzers = new List<ILogAnalyzer>();
            Analyzers.Add(new TimeDiffAnalyzer());
            Analyzers.Add(new ThrottlingAnalyzer());
            Analyzers.Add(new OrderStatusAnalyzer());
            Analyzers.Add(new FixNewGatewayDmaStockLogMerger());
            SelectedAnalyzer = Analyzers[0];

            CommandSaveFormats = new BaseCommand(CommandSaveFormatsExecute, CommandSaveFormatsCanExecute);
            CommandPickFile = new BaseCommand(CommandPickFileExecute, CommandPickFileCanExecute);
            CommandDeleteFormat = new BaseCommand(CommandDeleteFormatExecute, CommandDeleteFormatCanExecute);
            CommandStartAnalysis = new BaseCommand(CommandStartAnalysisExecute, CommandStartAnalysisCanExecute);

            Settings.GetInstance().LanguageChangedEventHandler += OnLanguageChanged;
        }

        #region event subscriptions
        private void OnLanguageChanged()
        {
            if(InfoTextResourceKey != null)
            {
                InfoText = App.Current.Resources[InfoTextResourceKey].ToString();
            }            
        }
        #endregion

        #region commands

        #region CommandSaveFormats
        public BaseCommand CommandSaveFormats { get; set; }
        public void CommandSaveFormatsExecute(object param)
        {
            if (!string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Format))
            {                
                LineFormat.Formats.Add(new LineFormat(Name,Format));
                LineFormat.SaveLineFormats();
            }
            else
            {
                SetInfoText("StringPleaseFillNameAndFormat");
            }
            
        }

        public bool CommandSaveFormatsCanExecute()
        {
            return true;
        }
        #endregion endregion

        #region CommandPickFile
        public BaseCommand CommandPickFile { get; set; }
        public void CommandPickFileExecute(object param)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            if (!string.IsNullOrWhiteSpace(openFileDialog.FileName))
            {
                if(param.ToString() == "1")
                {
                    InputFilePath1 = openFileDialog.FileName;
                }
                else
                {
                    InputFilePath2 = openFileDialog.FileName;
                }
                
            }
        }
        public bool CommandPickFileCanExecute()
        {
            return true;
        }

        #endregion
        #region CommandDeleteFormat
        public BaseCommand CommandDeleteFormat { get; set; }
        public void CommandDeleteFormatExecute(object param)
        {
             if(SelectedFormat != null)
             {
                 LineFormat.Formats.Remove(SelectedFormat);
                 LineFormat.SaveLineFormats();
             }
        }

        public bool CommandDeleteFormatCanExecute()
        {
            return true;
        }
        #endregion
        #region CommandStartAnalysis
        public BaseCommand CommandStartAnalysis { get; set; }
        public void CommandStartAnalysisExecute(object param)
        {
            SetInfoText("StringStartingAnalysis");
            if (SelectedAnalyzer.GetType() == typeof(FixNewGatewayDmaStockLogMerger))
            {
                FixNewGatewayDmaStockLogMerger analyzer = (FixNewGatewayDmaStockLogMerger)SelectedAnalyzer;
                analyzer.SetDmaPath(InputFilePath1);
                analyzer.SetStockPath(InputFilePath2);
            }
            else
            {
                SelectedAnalyzer.SetInFilePath(InputFilePath1);
            }
            SelectedAnalyzer.SetOutFilePath(OutFilePath);
            LineFormat format = new LineFormat(Name, Format);
            try
            {
                format.Configure();
            }
            catch(Exception ex)
            {
                Util.LogError(ex);
                SetInfoText("StringInvalidLineFormat");
            }
            
            SelectedAnalyzer.SetLogParser(format);
            new Thread(() =>
            {
                string errorMsgResourceKey = "";
                try
                {
                    SelectedAnalyzer.Start();
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        SetInfoText("StringFinishedAnalysis");
                    });
                }
                catch(IOException ex)
                {
                    errorMsgResourceKey = "StringFileIsBeingUsed";
                }
                catch(Exception ex)
                {
                    Util.LogError(ex);
                    errorMsgResourceKey = "StringFailedAnalysis";
                }
                finally
                {
                    if(errorMsgResourceKey != "")
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            UserControlErrorPopup popup = new UserControlErrorPopup();
                            popup.SetErrorText(errorMsgResourceKey);
                            PopupManager.OpenErrorPopup(popup);
                            SetInfoText("StringFailedAnalysis");
                        });
                    }
                    
                }
                
                
                
            }).Start();
        }

        public bool CommandStartAnalysisCanExecute()
        {
            return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Format) && !string.IsNullOrEmpty(OutFilePath)
                && !string.IsNullOrEmpty(InputFilePath1) && (SelectedAnalyzer.GetType() != typeof(FixNewGatewayDmaStockLogMerger) || !string.IsNullOrEmpty(InputFilePath2));
        }
        #endregion
        #endregion
    }
}
