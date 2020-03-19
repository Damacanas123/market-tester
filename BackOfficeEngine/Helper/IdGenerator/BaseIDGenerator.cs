using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;


namespace BackOfficeEngine.Helper.IdGenerator
{
    internal abstract class BaseIDGenerator<T> where T : BaseIDGenerator<T>,new ()
    {
        protected abstract object fileLock { get; set; }
        protected abstract string filePath { get; set; }

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
            string clOrdID;
            lock (fileLock)
            {
                int seqnum = -1;
                using (FileStream fs = File.OpenRead(filePath))
                {
                    byte[] byteArray = new byte[20];

                    UTF8Encoding fileContent = new UTF8Encoding(true);
                    while (fs.Read(byteArray, 0, byteArray.Length) > 0)
                    {
                        seqnum = Int32.Parse(fileContent.GetString(byteArray), CultureInfo.CurrentCulture);
                    }
                }
                clOrdID = Util.GetTodayString() + seqnum;
                seqnum++;
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    sw.WriteLine(seqnum.ToString());
                }
            }
            return clOrdID;
        }
    }
}
