using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackOfficeEngine.Helper;
using BackOfficeEngine.Model;
using FixHelper;
using FixLogAnalyzer;
using MarketTester.Base;
using MarketTester.Helper;
using MarketTester.Model.LogLoader;
using MarketTester.UI.Popup;
using Microsoft.Office.Interop.Excel;
using QuickFix;

namespace MarketTester.ViewModel
{
    public class ViewModelLogLoader : BaseNotifier
    {

        public ViewModelLogLoader()
        {
            CommandLoadFile = new BaseCommand(CommandLoadFileExecute, CommandLoadFileCanExecute);

        }
        private int tabControlSelectedIndex;

        public int TabControlSelectedIndex
        {
            get { return tabControlSelectedIndex; }
            set
            {
                tabControlSelectedIndex = value;
                IsEchoTagsSelected = value == 0;
                NotifyPropertyChanged(nameof(TabControlSelectedIndex));
            }
        }

        private bool IsEchoTagsSelected { get; set; }

        private string[] Lines { get; set; }


        public ObservableCollectionEx<MsgTypeUI> MsgTypes { get; set; } = new ObservableCollectionEx<MsgTypeUI>();
        private MsgTypeUI selectedMsgType;

        public MsgTypeUI SelectedMsgType
        {
            get { return selectedMsgType; }
            set
            {
                selectedMsgType = value;
                if(value != null)
                {
                    AllTagsTags.Clear();
                    AllTagsValues.Clear();
                    AllTagsUsedLines.Clear();
                    foreach (var item in MsgTypeTrees[selectedMsgType.MsgType].GetTagsAndOccurenceNums())
                    {
                        AllTagsTags.Add(new TagUI(item.Item1, item.Item2));
                    }
                    EchoBackResponseMsgTypes.Clear();
                    EchoBackTagValuePairs.Clear();
                    EchoBackUsedLines.Clear();
                    if (RequestResponses.EchoBackTags.TryGetValue(selectedMsgType.MsgType,
                        out Dictionary<string,List<EchoBackTagValuePair>> response))
                    {
                        foreach (string responseMsgType in response.Keys)
                        {
                            EchoBackResponseMsgTypes.Add(responseMsgType);
                        }
                    }
                }         
                NotifyPropertyChanged(nameof(SelectedMsgType));
            }
        }

        public ObservableCollectionEx<string> EchoBackResponseMsgTypes { get; set; } = new ObservableCollectionEx<string>();
        private string selectedEchoBackResponseMsgType;

        public string SelectedEchoBackResponseMsgType
        {
            get { return selectedEchoBackResponseMsgType; }
            set
            {
                selectedEchoBackResponseMsgType = value;
                if (value != null)
                {
                    EchoBackTagValuePairs.Clear();
                    if (RequestResponses.EchoBackTags.TryGetValue(selectedMsgType.MsgType,
                        out Dictionary<string, List<EchoBackTagValuePair>> response))
                    {
                        foreach (EchoBackTagValuePair pair in response[selectedEchoBackResponseMsgType].OrderBy((o) => o))
                        {
                            EchoBackTagValuePairs.Add(pair);
                        }
                    }
                }
                NotifyPropertyChanged(nameof(SelectedEchoBackResponseMsgType));
            }
        }

        public ObservableCollectionEx<EchoBackTagValuePair> EchoBackTagValuePairs { get; set; } = new ObservableCollectionEx<EchoBackTagValuePair>();

        private EchoBackTagValuePair selectedEchoBackTagValuePair;

        public EchoBackTagValuePair SelectedEchoBackTagValuePair
        {
            get { return selectedEchoBackTagValuePair; }
            set
            {
                selectedEchoBackTagValuePair = value;
                if(value != null)
                {
                    EchoBackUsedLines.Clear();
                    foreach ((int,int) item in selectedEchoBackTagValuePair.OccurenceLines)
                    {
                        EchoBackUsedLines.Add(item);
                    }
                }
                NotifyPropertyChanged(nameof(SelectedEchoBackTagValuePair));
            }
        }

        public ObservableCollectionEx<(int,int)> EchoBackUsedLines { get; set; } = new ObservableCollectionEx<(int, int)>();

        private (int,int) selectedEchoBackUsedLine;

        public (int,int) SelectedEchoBackUsedLine
        {
            get { return selectedEchoBackUsedLine; }
            set
            {
                selectedEchoBackUsedLine = value;
                int requestIndex = value.Item1;
                int responseIndex = value.Item2;
                if(requestIndex != -1)
                {
                    LogMessages.Clear();
                    LogMessages.Add(Lines[requestIndex]);
                    LogMessages.Add(Lines[responseIndex]);
                }
                NotifyPropertyChanged(nameof(SelectedEchoBackUsedLine));
            }
        }



        public ObservableCollectionEx<TagUI> AllTagsTags { get; set; } = new ObservableCollectionEx<TagUI>();
        private TagUI selectedAllTagsTag;

        public TagUI SelectedAllTagsTag
        {
            get { return selectedAllTagsTag; }
            set
            {
                selectedAllTagsTag = value;
                if(value != null)
                {
                    AllTagsValues.Clear();
                    foreach (var item in MsgTypeTrees[SelectedMsgType.MsgType].TagTrees[selectedAllTagsTag.Tag].GetValuesAndOccurenceNums())
                    {
                        AllTagsValues.Add(new ValueUI(selectedAllTagsTag.Tag, item.Item1, item.Item2));
                    }
                }
                
                NotifyPropertyChanged(nameof(SelectedAllTagsTag));
            }
        }

        public ObservableCollectionEx<ValueUI> AllTagsValues { get; set; } = new ObservableCollectionEx<ValueUI>();

        private ValueUI selectedAllTagsValue;

        public ValueUI SelectedAllTagsValue
        {
            get { return selectedAllTagsValue; }
            set
            {
                selectedAllTagsValue = value;
                if(value != null)
                {
                    AllTagsUsedLines.Clear();
                    foreach (int line in MsgTypeTrees[SelectedMsgType.MsgType].
                        TagTrees[SelectedAllTagsTag.Tag].ValueTrees[selectedAllTagsValue.Value].LineNumbers)
                    {
                        AllTagsUsedLines.Add(line);
                    }
                }
                NotifyPropertyChanged(nameof(SelectedAllTagsValue));
            }
        }

        //collection of lines which contains current selected tag-value
        public ObservableCollectionEx<int> AllTagsUsedLines { get; set; } = new ObservableCollectionEx<int>();

        private int selectedAllTagsUsedLine;

        public int SelectedAllTagsUsedLine
        {
            get { return selectedAllTagsUsedLine; }
            set
            {
                selectedAllTagsUsedLine = value;
                if(value != -1)
                {
                    LogMessages.Clear();
                    LogMessages.Add(Lines[value - 1]);
                }
                NotifyPropertyChanged(nameof(SelectedAllTagsUsedLine));
            }
        }

        public ObservableCollection<string> LogMessages { get; set; } = new ObservableCollection<string>();

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

        private int invalidMessageCount;

        public int InvalidMessageCount
        {
            get { return invalidMessageCount; }
            set
            {
                invalidMessageCount = value;
                NotifyPropertyChanged(nameof(InvalidMessageCount));
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
                InfoTextResourceKey = ResourceKeys.StringStartedReadingFixLog;

                Util.ThreadStart(() =>
                {
                    RequestResponses.EchoBackTags.Clear();
                    MsgTypeTrees.Clear();
                    try
                    {
                        Lines = File.ReadAllLines(filePath);
                        int count = 0;
                        foreach (string line in Lines)
                        {
                            count += 1;
                            try
                            {
                                ExtendedLogMessage logMessage = new ExtendedLogMessage(line, count);
                                string msgType = logMessage.GetField(QuickFix.Fields.Tags.MsgType);
                                if (string.IsNullOrWhiteSpace(msgType))
                                {
                                    continue;
                                }
                                if (!MsgTypeTrees.TryGetValue(msgType, out MsgTypeTree msgTypeTree))
                                {
                                    msgTypeTree = new MsgTypeTree();
                                    MsgTypeTrees[msgType] = msgTypeTree;
                                }
                                msgTypeTree.AddMessage(logMessage);
                                string clOrdID = logMessage.GetField(QuickFix.Fields.Tags.ClOrdID);
                                if (!string.IsNullOrWhiteSpace(clOrdID))
                                {
                                    if (!ClOrdIDMap.TryGetValue(clOrdID, out RequestResponses rr))
                                    {
                                        rr = new RequestResponses();
                                        ClOrdIDMap[clOrdID] = rr;
                                    }
                                    rr.Add(logMessage);
                                }
                            }
                            catch (InvalidMessage)
                            {
                                continue;
                            }
                        }
                        App.Invoke(() =>
                        {
                            AllFixTags allFixTags = AllFixTags.GetInstance();
                            MsgTypes.Clear();
                            foreach(string msgType in MsgTypeTrees.Keys)
                            {
                                string explanation = allFixTags.GetValueExplanation(Tags.MsgType, msgType);
                                MsgTypes.Add(new MsgTypeUI(msgType));
                            }
                            InfoTextResourceKey = ResourceKeys.StringFinishedAnalysis;

                        });
                    }
                    catch(Exception ex)
                    {
                        Util.LogDebugError(ex);
                        App.Invoke(() =>
                        {
                            UserControlErrorPopup errorPopup = new UserControlErrorPopup(ex.ToString());
                            PopupManager.OpenErrorPopup(errorPopup);
                        });
                    }
                    
                });
                
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
