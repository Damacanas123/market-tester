using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.Threading;

namespace BackOfficeEngine.Helper.IdGenerator
{
    public abstract class BaseIDGenerator<T> where T : BaseIDGenerator<T>, new()
    {
        protected abstract object fileLock { get; set; }
        protected abstract string filePath { get; set; }
        protected abstract object counterLock { get; set; }
        private int LastDumpedSeqNum {get;set;}
        private int CurrentSeqNum { get; set; }
        private bool IsInitialized { get; set; } = false;
        private static UTF8Encoding encoder = Util.UTF8;
        private string CurrentWorkingDay = Util.GetTodayString();

        private static readonly T _instance = new T();
        public static T Instance
        {
            get
            {
                return _instance;
            }
        }

        public string GetNextId()
        {
            lock (fileLock)
            {
                if (!IsInitialized)
                {
                    Initialize();
                }
            }
            string clOrdID;
            lock (counterLock)
            {
                clOrdID = Util.GetRandomString(2) + Util.GetTodayString() + CurrentSeqNum++;
            }            
            return clOrdID;
        }

        private void Initialize()
        {
            string date = CurrentWorkingDay;
            string seqNum = "1";
            using (FileStream fs = File.OpenRead(filePath))
            {
                byte[] byteArray = new byte[50];                
                if (fs.Read(byteArray, 0, byteArray.Length) > 0)
                {
                    string content = encoder.GetString(byteArray);
                    content = content.Replace("\0", "");
                    if (content.Contains(Environment.NewLine))
                    {
                        string[] arr = content.Split(new string[] { Environment.NewLine },StringSplitOptions.RemoveEmptyEntries);
                        if(arr.Length == 2)
                        {
                            if (string.Compare(arr[0],CurrentWorkingDay, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                date = arr[0];
                                seqNum = arr[1];
                            }
                        }
                        
                    }
                }                
            }
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine(date);
                sw.WriteLine(seqNum);
            }
            LastDumpedSeqNum = Int32.Parse(seqNum, CultureInfo.InvariantCulture);
            CurrentSeqNum = LastDumpedSeqNum;
            Util.ThreadStart(() =>
            {
                while (true)
                {
                    if(LastDumpedSeqNum < CurrentSeqNum)
                    {
                        using (StreamWriter sw = new StreamWriter(filePath))
                        {   
                            sw.WriteLine(CurrentWorkingDay);
                            sw.WriteLine(CurrentSeqNum.ToString(CultureInfo.InvariantCulture));
                        }
                        LastDumpedSeqNum = CurrentSeqNum;
                        string todayAgain = Util.GetTodayString();
                        if (string.Compare(todayAgain, CurrentWorkingDay, StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            CurrentWorkingDay = todayAgain;
                            CurrentSeqNum = 1;
                            LastDumpedSeqNum = 1;
                        }
                    }
                    else
                    {
                        Thread.Sleep(2000);
                    }
                }
            });
            IsInitialized = true;
        }
    }
}
