using BackOfficeEngine.GeneralBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketTester.Model.FixFreeFormat
{
    public class FreeFormatSchedule : BaseNotifier
    {
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
        public ObservableCollection<FreeFormatScheduleItem> Items { get; set; } = new ObservableCollection<FreeFormatScheduleItem>();

    }
}
