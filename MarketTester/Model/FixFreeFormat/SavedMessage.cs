using BackOfficeEngine.GeneralBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MarketTester.Helper;

namespace MarketTester.Model.FixFreeFormat
{
    public class SavedMessage : BaseNotifier
    {
        private static string SaveFilePath= MarketTesterUtil.APPLICATION_SAVEDMESSAGES_DIR + "saved_messages";
        private const string TagValueSeparator = "=";
        private const string Pipe = "|";
        public static ObservableCollection<SavedMessage> SavedMessages { get; set; } = new ObservableCollection<SavedMessage>();
        private List<TagValuePair> TagValuePairs = new List<TagValuePair>();

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged(nameof(Name));
            }
        }



        public static void Save()
        {
            string content = "";
            foreach(SavedMessage savedMessage in SavedMessages)
            {
                content += savedMessage.Name;
                foreach(TagValuePair pair in savedMessage.TagValuePairs)
                {
                    content += Pipe + pair.Tag + TagValueSeparator + pair.Value + TagValueSeparator + pair.IsSelected;
                }
                content += Environment.NewLine;
            }
            BackOfficeEngine.Helper.Util.OverwriteToFile(SaveFilePath, content);
        }

        public static void Load()
        {
            string[] lines = BackOfficeEngine.Helper.Util.ReadLines(SaveFilePath);
            foreach(string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                string [] values = line.Split(new string[] { Pipe },StringSplitOptions.RemoveEmptyEntries);
                SavedMessage m = new SavedMessage();
                m.Name = values[0];
                for(int i = 1; i < values.Length; i++)
                {
                    string[] tagDetails = values[i].Split(new string[] { TagValueSeparator }, StringSplitOptions.RemoveEmptyEntries);
                    bool isSelected = tagDetails[2] == "True" ? true : false;
                    m.TagValuePairs.Add(new TagValuePair(tagDetails[0], tagDetails[1], isSelected));
                }
                SavedMessages.Add(m);
            }
        }


        public void AddTagValuePair(TagValuePair pair)
        {
            TagValuePairs.Add(pair);
        }

        public List<TagValuePair> GetTagValuePairs()
        {
            return TagValuePairs;
        }

        public void ClearTagValuePairs()
        {
            TagValuePairs.Clear();
        }
    }
}
