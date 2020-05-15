using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackOfficeEngine.Helper;
using BackOfficeEngine.Model;
using FixLogAnalyzer;
using MarketTester.Base;
using MarketTester.Helper;
using MarketTester.Model.LogLoader;
using QuickFix;

namespace MarketTester.ViewModel
{
    public class ViewModelLogLoader : BaseNotifier
    {
        private IEnumerable<string> Lines { get; set; }

        public ObservableCollectionEx<string> MsgTypes { get; set; } = new ObservableCollectionEx<string>();
        public ObservableCollectionEx<string> TagsCollection { get; set; } = new ObservableCollectionEx<string>();
        public ObservableCollectionEx<string> Values { get; set; } = new ObservableCollectionEx<string>();
        //collection of lines which contains current selected tag-value
        public ObservableCollectionEx<string> UsedLines { get; set; } = new ObservableCollectionEx<string>();

        private Dictionary<string, MsgTypeTree> MsgTypeTrees { get; set; } = new Dictionary<string, MsgTypeTree>();

        private Dictionary<string, RequestResponses> ClOrdIDMap { get; set; } = new Dictionary<string, RequestResponses>();
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
                int count = 0;
                foreach(string line in Lines)
                {
                    count += 1;
                    try
                    {
                        ExtendedLogMessage logMessage = new ExtendedLogMessage(line,count);
                        string msgType = logMessage.GetField(QuickFix.Fields.Tags.MsgType);
                        if (string.IsNullOrWhiteSpace(msgType))
                        {
                            continue;
                        }
                        if(!MsgTypeTrees.TryGetValue(msgType,out MsgTypeTree msgTypeTree))
                        {
                            msgTypeTree = new MsgTypeTree();
                            MsgTypeTrees[msgType] = msgTypeTree;
                        }
                        msgTypeTree.AddMessage(logMessage);
                    }
                    catch (InvalidMessage)
                    {
                        continue;
                    }
                    catch(Exception ex)
                    {
                        Util.LogDebugError(ex);
                        continue;
                    }
                    
                    
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
