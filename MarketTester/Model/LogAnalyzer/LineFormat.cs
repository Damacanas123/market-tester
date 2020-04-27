using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

using MarketTester.Base;
using MarketTester.Helper;
using FixLogAnalyzer;
using BackOfficeEngine.Helper;

namespace MarketTester.Model
{
    public class LineFormat : BaseNotifier,IFixLineParser
    {
        public const char WildCard = '%';
        public const string DateFormat = "yyyyMMdd-HH:mm:ss.fff";
        public const char YearCard = 'y';
        public const char MonthCard = 'M';
        public const char DayCard = 'd';
        public const char HourCard = 'H';
        public const char MinuteCard = 'm';
        public const char SecondCard = 's';
        public const char MillisecondCard = 'f';
        public const char MicrosecondCard = 'u';
        public const string MessageCard = "{message}";
        private const string SaveDelimiter = "dfbdfbdbd";

        public static string SaveFilePath = MarketTesterUtil.APPLICATION_STATIC_DIR + "lineformats.save";
        public static ObservableCollection<LineFormat> Formats { get; set; } = new ObservableCollection<LineFormat>();
        public static void LoadLineFormats()
        {
            string[] formats = MarketTesterUtil.ReadLines(SaveFilePath);
            Formats.Clear();
            for(int i = 0; i < formats.Length - 1; i++)
            {
                string format = formats[i];
                int nameEnd = format.IndexOf(SaveDelimiter);
                string name = format.Substring(0, nameEnd);
                int start = nameEnd + SaveDelimiter.Length;
                string onlyFormat = format.Substring(start);
                Formats.Add(new LineFormat(name,onlyFormat));
            }
        }

        public static void SaveLineFormats()
        {
            string content = "";
            foreach (LineFormat format in Formats)
            {
                content += format.ToString() + Environment.NewLine;
            }
            MarketTesterUtil.OverwriteToFile(SaveFilePath,content);
        }

        public override string ToString()
        {
            return Name + SaveDelimiter + Format;
        }

        public LineFormat() { }

        public LineFormat(string name,string format)
        {
            Name = name;
            Format = format;
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
        private string dateFormat;
        private int dateStartIndex;
        private int dateEndIndex;
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

        public void Configure()
        {
            int messageStartIndex = Format.IndexOf(MessageCard);
            string formatWithoutMessage = Format.Substring(0, messageStartIndex);
            bool dateStarted = false;
            for (int i = 0; i < formatWithoutMessage.Length; i++)
            {
                char c = formatWithoutMessage[i];
                if (c == WildCard)
                {
                    if (dateStarted)
                    {
                        dateEndIndex = i;
                        dateStarted = false;
                    }
                }
                else
                {
                    if (!dateStarted)
                    {
                        dateStarted = true;
                        dateStartIndex = i;
                    }
                    if (dateStarted)
                    {
                        dateFormat += c;
                    }
                }
            }
        }
        

        //logtime,message string
        public (DateTime,string) Parse(string line)
        {
            int messageStartIndex = Format.IndexOf(MessageCard);
            if (dateFormat.Length != 0)
            {
                string datePart = line.Substring(dateStartIndex, dateEndIndex - dateStartIndex);
                DateTime logtime;
                try
                {
                    logtime = DateTime.ParseExact(datePart, dateFormat, CultureInfo.InvariantCulture);
                }
                catch(Exception ex)
                {
                    Util.LogError(ex);
                    throw new Exception("Date format does not match log date format");
                }
                
                string msg = line.Substring(messageStartIndex, line.Length - messageStartIndex);
                return (logtime, msg);
            }
            else
            {
                throw new Exception("Line format does not contain logtime.");
            }
        }
    }
}
