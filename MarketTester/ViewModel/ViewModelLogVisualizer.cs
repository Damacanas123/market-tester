using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackOfficeEngine.Helper;
using BackOfficeEngine.Model;
using MarketTester.Base;
using MarketTester.Helper;
using QuickFix;

namespace MarketTester.ViewModel
{
    public class ViewModelLogVisualizer : BaseNotifier
    {
        private IEnumerable<string> Lines { get; set; }

        public ObservableCollectionEx<string> MsgTypes { get; set; } = new ObservableCollectionEx<string>();
        public ObservableCollectionEx<string> Tags { get; set; } = new ObservableCollectionEx<string>();
        public ObservableCollectionEx<string> Values { get; set; } = new ObservableCollectionEx<string>();
        //collection of lines which contains current selected tag-value
        public ObservableCollectionEx<string> UsedLines { get; set; } = new ObservableCollectionEx<string>();

        private Dictionary<string, List<int>> MsgTypeToTag { get; set; } = new Dictionary<string, List<int>>();
        private Dictionary<int, string> TagToValue { get; set; } = new Dictionary<int, string>();
        private Dictionary<string, string> ValueToUsedLine { get; set; } = new Dictionary<string, string>();
        private string infoText;

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
                if (App.Current.Resources.Contains(value))
                {

                    infoTextResourceKey = value;
                    InfoText = App.Current.Resources[value].ToString();
                    NotifyPropertyChanged(nameof(InfoTextResourceKey));
                }
            }
        }
        #region commands

        #region CommandLoadFile
        public BaseCommand CommandLoadFile { get; set; }
        public void CommandLoadFileExecute(object param)
        {
            string filePath = UIUtil.OpenFileDialog(new string[] { });
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                if (!File.Exists(filePath))
                {
                    InfoTextResourceKey = ResourceKeys.StringFileNotFound;
                    return;
                }
                Lines = File.ReadLines(filePath);
                foreach(string line in Lines)
                {
                    string msg = Fix.ExtractFixMessageFromALine(line);
                    if (string.IsNullOrWhiteSpace(msg))
                    {
                        continue;
                    }
                    Dictionary<int,string> tagValuePairs = MarketTesterUtil.GetTagValuePairs(msg);
                    if(!tagValuePairs.TryGetValue(QuickFix.Fields.Tags.MsgType,out string msgType))
                    {
                        continue;
                    }
                    //if(MsgTypeToTag.TryGetValue(msgType))
                }
                
            }
        }
        public bool CommandLoadFileCanExecute()
        {
            return true;
        }
        #endregion
        #endregion
    }
}
