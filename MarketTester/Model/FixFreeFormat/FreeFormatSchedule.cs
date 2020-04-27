using BackOfficeEngine.GeneralBase;
using MarketTester.Helper;
using MarketTester.UI.Popup;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketTester.Model.FixFreeFormat
{
    public class FreeFormatSchedule : BaseNotifier
    {
        #region const
        private const char ValueDelimiter = '|';
        #endregion
        #region static
        public static FreeFormatSchedule Load(string s)
        {
            string[] lines = s.Split(new string[] { Environment.NewLine },StringSplitOptions.RemoveEmptyEntries);
            FreeFormatSchedule schedule = new FreeFormatSchedule() { Name = lines[0] };
            for(int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] values = line.Split(ValueDelimiter);
                
                FreeFormatScheduleItem item = new FreeFormatScheduleItem(int.Parse(values[1], CultureInfo.InvariantCulture), values[2], values[0]);
                schedule.Items.Add(item);
            }
            return schedule;
        }
        #endregion

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

        public string SaveString
        {
            get
            {
                string s = Name + Environment.NewLine;
                foreach(FreeFormatScheduleItem item in Items)
                {
                    s += item.Channel + ValueDelimiter + item.Delay + ValueDelimiter + item.Message + Environment.NewLine;
                }
                return s;
            }
        }
        public ObservableCollection<FreeFormatScheduleItem> Items { get; set; } = new ObservableCollection<FreeFormatScheduleItem>();
    }
}
