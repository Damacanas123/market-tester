using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketTester.Base;
using QuickFix.Fields;

namespace MarketTester.Model.FixFreeFormat
{
    public class FreeFormatScheduleItem : BaseNotifier
    {
		private int delay;

		public int Delay
		{
			get { return delay; }
			set 
			{ 
				delay = value;
				NotifyPropertyChanged(nameof(Delay));
			}
		}

		private string channel;

		public string Channel
		{
			get { return channel; }
			set
			{
				channel = value;
				NotifyPropertyChanged(nameof(Channel));
			}
		}

		private string message;

		public string Message
		{
			get { return message; }
			set
			{
				message = value;
				NotifyPropertyChanged(nameof(Message));
			}
		}
		
		public FreeFormatScheduleItem(int delay,string message,string channel)
		{
			Delay = delay;
			Message = message;
			Channel = channel;
		}

	}
}
