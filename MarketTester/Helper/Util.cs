using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickFix;
using QuickFix.Fields;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
using FixHelper;
using System.Data;
using System.Windows.Navigation;

namespace MarketTester.Helper
{
    internal class Util
    {
        public const string FIX50SP2 = "Fix50sp2";
        public const string OUCH = "OUCH";

        public const string APP_VERSION = "1.0.0.1";

        public const string PRIMARY_STRING = "Primary";
        public const string SECONDARY_STRING = "Secondary";
        public const string DC1_STRING = "DC1";
        public const string DC2_STRING = "DC2";
        public const string RD_STRING = "RD";
        public const string APP_NAME = "MarketTester";

        public const string FILE_PATH_DELIMITER = "\\";
        public static string APPLICATION_COMMON_DIR = "C:\\MATRIKS_OMS" + FILE_PATH_DELIMITER + APP_NAME + FILE_PATH_DELIMITER;
        public static string APPLICATION_SEQ_NUM_DIR = APPLICATION_COMMON_DIR + "sequence_nums\\";
        public static string STATIC_DIR_PATH = "static" + FILE_PATH_DELIMITER;
        public static string APPLICATION_STATIC_DIR = APPLICATION_COMMON_DIR + STATIC_DIR_PATH;
        public static string APPLICATION_EXPORT_DIR = APPLICATION_COMMON_DIR + "exports" + FILE_PATH_DELIMITER;
        public static string APPLICATION_RESULTS_DIR = APPLICATION_COMMON_DIR + "results" + FILE_PATH_DELIMITER;
        public static string APPLICATION_SCHEDULESAVE_DIR = APPLICATION_COMMON_DIR + "schedule" + FILE_PATH_DELIMITER;
        public static string APPLICATION_FREEFORMATSCHEDULE_DIR = APPLICATION_SCHEDULESAVE_DIR + "freeformat" + FILE_PATH_DELIMITER;
        public static string APPLICATION_NONFREEFORMATSCHEDULE_DIR = APPLICATION_SCHEDULESAVE_DIR + "nonfreeformat" + FILE_PATH_DELIMITER;
        public static string EXCEPTIONLOG_FILE_PATH = APPLICATION_STATIC_DIR + "exception.log";
        public static string USERNAME = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Replace(Util.FILE_PATH_DELIMITER, "");
        public static string SCHEDULESAVE_DIR_PATH = APPLICATION_STATIC_DIR + "schedule_save" + FILE_PATH_DELIMITER;



        public static void Bootstrap()
        {
            Directory.CreateDirectory(APPLICATION_RESULTS_DIR);
            Directory.CreateDirectory(APPLICATION_EXPORT_DIR);
            Directory.CreateDirectory(APPLICATION_STATIC_DIR);
            Directory.CreateDirectory(APPLICATION_FREEFORMATSCHEDULE_DIR);
            Directory.CreateDirectory(APPLICATION_NONFREEFORMATSCHEDULE_DIR);
            CopyDirectoryAndSubDirectoriesToApplicationCommonPath(STATIC_DIR_PATH);
            Connection.Connector.GetInstance();
        }
        public static void LogError(Exception ex)
        {
            AppendStringToFile(EXCEPTIONLOG_FILE_PATH, $"Appl version({APP_VERSION}) {DateTime.Now} {ex.ToString()}\n");
        }

        public static int ReadSeqNum(string filePath)
        {
            try
            {
                int lastIndex = filePath.LastIndexOf(FILE_PATH_DELIMITER);
                string directory = filePath.Substring(0, lastIndex);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    FileStream fs = File.Create(filePath);
                    fs.Close();
                    using (StreamWriter outputFile = new StreamWriter(filePath))
                    {
                        outputFile.Write("1");
                    }
                    return 1;//seq num is going to be 1 in this case. no need to do further read on the same file.
                }
                if (!File.Exists(filePath))
                {
                    using (StreamWriter outputFile = new StreamWriter(filePath))
                    {
                        outputFile.Write("1");
                    }
                    return 1;//seq num is going to be 1 in this case. no need to do further read on the same file.
                }


                int seqNum = -1;
                using (FileStream fs = File.OpenRead(filePath))
                {
                    byte[] byteArray = new byte[20];

                    UTF8Encoding fileContent = new UTF8Encoding(true);
                    while (fs.Read(byteArray, 0, byteArray.Length) > 0)
                    {
                        seqNum = Int32.Parse(fileContent.GetString(byteArray), CultureInfo.CurrentCulture);
                    }
                }
                if (seqNum == -1)
                {
                    throw new Exception("Sequence num couldn't be read");
                }
                return seqNum;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return -1;
            }

        }

        public static int IncrementSequenceNumber(string filePath)
        {
            int seqNum = ReadSeqNum(filePath) + 1;
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine(seqNum);
            }
            return seqNum;
        }
        public static string RemoveNonNumericKeepDot(string s)
        {
            string temp = "";
            bool isDotPresent = false;
            foreach (char c in s)
            {
                if (c <= '9' && c >= '0')
                {
                    temp += c;
                }
                if (c == '.' && !isDotPresent)
                {
                    temp += c;
                    isDotPresent = true;
                }
            }
            return temp;
        }

        public static void CopyDirectoryAndSubDirectoriesToApplicationCommonPath(string dir)
        {
            if (!Directory.Exists(Util.APPLICATION_COMMON_DIR + dir))
            {
                Directory.CreateDirectory(Util.APPLICATION_COMMON_DIR + dir);
            }
            string[] filePaths = Directory.GetFiles(dir);
            string[] dirPaths = Directory.GetDirectories(dir);
            foreach (string filePath in filePaths)
            {
                if (!File.Exists(APPLICATION_COMMON_DIR + filePath))
                {
                    File.Copy(filePath, APPLICATION_COMMON_DIR + filePath);
                }
            }
            foreach (string dirPath in dirPaths)
            {
                CopyDirectoryAndSubDirectoriesToApplicationCommonPath(dirPath);
            }

        }




        public static void ChangeConfigFixLogPath(string configPath)
        {
            string fileContent = "";
            using (StreamReader reader = new StreamReader(configPath))
            {
                string session = null;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!(line.Contains("FileStorePath") || line.Contains("FileLogPath")))
                    {
                        fileContent += line + "\n";
                    }
                    if (line.Contains("SessionQualifier"))
                    {
                        int startIndex = line.IndexOf("=") + 1;
                        session = line.Substring(startIndex, line.Length - startIndex);
                        if (line.Contains("#"))
                        {
                            fileContent += "#FileStorePath=" + APPLICATION_COMMON_DIR + "FIXLog\\" + session + "\n";
                            fileContent += "#FileLogPath=" + APPLICATION_COMMON_DIR + "FIXLog\\" + session + "\n";
                        }
                        else
                        {
                            fileContent += "FileStorePath=" + APPLICATION_COMMON_DIR + "FIXLog\\" + session + "\n";
                            fileContent += "FileLogPath=" + APPLICATION_COMMON_DIR + "FIXLog\\" + session + "\n";
                        }
                    }
                }
            }
            using (StreamWriter outputFile = new StreamWriter(configPath))
            {
                outputFile.Write(fileContent);
            }
            Console.WriteLine(fileContent);
        }


        public static string RemoveNonNumeric(string s)
        {
            string temp = "";
            foreach (char c in s)
            {
                if (c <= '9' && c >= '0')
                {
                    temp += c;
                }
            }
            return temp;
        }


        

        public static string GetTodayString()
        {
            DateTime today = DateTime.Today;
            string year = today.Year.ToString(CultureInfo.CurrentCulture);
            string month = today.Month.ToString(CultureInfo.CurrentCulture);
            string day = today.Day.ToString(CultureInfo.CurrentCulture);
            for (int i = 0; i < 4 - year.Length; i++)
            {
                year = "0" + year;
            }
            for (int i = 0; i < 2 - month.Length; i++)
            {
                month = "0" + month;
            }
            for (int i = 0; i < 2 - day.Length; i++)
            {
                day = "0" + day;
            }
            return year + month + day;
        }

        public static string ReadFile(string filePath)
        {
            lock (GetReferenceToLock(filePath))
            {
                if (File.Exists(filePath))
                {
                    string content = File.ReadAllText(filePath);
                    content = content.Replace("\r", "");
                    return content;
                }
                else
                {
                    string directories = filePath.Substring(0,filePath.LastIndexOf(FILE_PATH_DELIMITER));
                    Directory.CreateDirectory(directories);
                    File.Create(filePath);
                    return "";
                }
            }
        }

        public static string[] ReadLines(string filePath)
        {
            string content = ReadFile(filePath);
            if (!string.IsNullOrEmpty(content))
            {
                return content.Split(new char[] { '\n' });
            }
            else
            {
                return new string[] { };
            }
        }

        public static void OverwriteToFile(string filePath,string s)
        {
            lock (GetReferenceToLock(filePath))
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    sw.Write(s);
                }
            }
        }

        

        private static Dictionary<string, object> locks = new Dictionary<string, object>();
        private static object GetReferenceToLock(string filePath)
        {
            if(locks.TryGetValue(filePath,out object Lock))
            {
                return Lock;
            }
            else
            {
                Lock = new object();
                locks[filePath] = Lock;
            }
            return Lock;
        }
        public static void AppendStringToFile(string filePath, string content)
        {
            lock (GetReferenceToLock(filePath))
            {
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLine(content);
                }
            }
            
        }

        public static string GetNowString()
        {
            DateTime today = DateTime.Now;
            string year = today.Year.ToString();
            string month = today.Month.ToString();
            string day = today.Day.ToString();
            string hour = today.Hour.ToString();
            string minute = today.Minute.ToString();
            string second = today.Second.ToString();
            string milisecond = today.Millisecond.ToString();
            for (int i = 0; i < 4 - year.Length; i++)
            {
                year = "0" + year;
            }
            for (int i = 0; i < 2 - month.Length; i++)
            {
                month = "0" + month;
            }
            for (int i = 0; i < 2 - day.Length; i++)
            {
                day = "0" + day;
            }
            for (int i = 0; i < 2 - hour.Length; i++)
            {
                hour = "0" + hour;
            }
            for (int i = 0; i < 2 - minute.Length; i++)
            {
                minute = "0" + minute;
            }
            for (int i = 0; i < 2 - second.Length; i++)
            {
                second = "0" + second;
            }
            for (int i = 0; i < 3 - milisecond.Length; i++)
            {
                milisecond = "0" + milisecond;
            }
            return year + month + day + "-" + hour + ":" + minute + ":" + second + "." + milisecond;
        }



        public static List<int> GetAllTagsOfAMessage(Message m)
        {
            string pattern = @"(?<=\x01).*?(?==)";
            Regex rgx = new Regex(pattern);
            List<int> tags = new List<int>();
            foreach (Match match in rgx.Matches(m.ToString()))
            {
                tags.Add(Int32.Parse(match.Value, CultureInfo.CurrentCulture));
            }
            return tags;
        }

        public static string GetPrettyStringOfMessage(Message m)
        {
            string s = "";
            List<int> tags = GetAllTagsOfAMessage(m);
            string messageType = m.Header.GetField(Tags.MsgType);
            AllFixTags allFixTags = AllFixTags.GetInstance();
            foreach (int tag in tags)
            {
                if (tag != 9)
                {
                    if (m.Header.IsSetField(tag))
                    {
                        s += tag.ToString() + "(" + allFixTags.headerTagToObjectMap[tag].Name + ") : ";
                        s += m.Header.GetField(tag);
                        if (tag == Tags.MsgType)
                        {
                            if (FixValues.MsgTypes.ContainsKey(m.Header.GetField(tag)))
                            {
                                s += " (" + FixValues.MsgTypes[m.Header.GetField(tag)] + ")";
                            }
                        }
                    }

                    else if (m.IsSetField(tag))
                    {
                        s += tag.ToString();
                        if (allFixTags.tagToObjectMap.ContainsKey(tag))
                        {
                            AllFixTags.Tag tagStruct = allFixTags.tagToObjectMap[tag];
                            s += " (" + tagStruct.Name + ")";
                        }
                        s += " : " + m.GetField(tag);
                    }
                    s += "\n";
                }
            }
            s = s.Substring(0, s.Length - 1);
            return s;
        }

        public static string GetPrettyStringOfMessageWithoutNewLines(Message m, string channel)
        {
            string s = "";
            List<int> tags = GetAllTagsOfAMessage(m);
            string messageType = m.Header.GetField(Tags.MsgType);
            AllFixTags allFixTags = AllFixTags.GetInstance();
            foreach (int tag in tags)
            {
                if (tag != 9)
                {
                    if (m.Header.IsSetField(tag))
                    {
                        s += tag.ToString() + " (" + allFixTags.headerTagToObjectMap[tag].Name + ") : ";
                        s += m.Header.GetField(tag);
                        if (tag == Tags.MsgType)
                        {
                            if (FixValues.MsgTypes.ContainsKey(m.Header.GetField(tag)))
                            {
                                s += " (" + FixValues.MsgTypes[m.Header.GetField(tag)] + ")";
                            }
                        }
                    }
                    else if (m.IsSetField(tag))
                    {
                        s += tag.ToString();
                        if (allFixTags.tagToObjectMap.ContainsKey(tag))
                        {
                            AllFixTags.Tag tagStruct = allFixTags.tagToObjectMap[tag];
                            s += " (" + tagStruct.Name + ")";
                        }
                        s += " : " + m.GetField(tag);
                    }
                    s += "\t";
                }
            }
            s = s.Substring(0, s.Length - 1);
            return s;
        }

        public static string GetLocalMktDateFromDate(DateTime dateTime)
        {
            string year = dateTime.Year.ToString();
            string month = dateTime.Month.ToString();
            string day = dateTime.Day.ToString();
            for (int i = 0; i < 4 - year.Length; i++)
            {
                year = "0" + year;
            }
            for (int i = 0; i < 2 - month.Length; i++)
            {
                month = "0" + month;
            }
            for (int i = 0; i < 2 - day.Length; i++)
            {
                day = "0" + day;
            }
            return year + month + day;
        }

        public static string GetFileNameWithoutExtensionFromFullPath(string fullPath)
        {
            int lastDelimiterIndex = fullPath.LastIndexOf(Util.FILE_PATH_DELIMITER) + 1;
            int dotIndex = fullPath.LastIndexOf(".");
            if (lastDelimiterIndex == -1)
            {
                lastDelimiterIndex = 0;
            }
            if (dotIndex == -1)
            {
                dotIndex = fullPath.Length;
            }
            return fullPath.Substring(lastDelimiterIndex, dotIndex - lastDelimiterIndex);
        }

        public static string GetHeaderField(QuickFix.Message m, int tag)
        {
            return m.Header.IsSetField(tag) ? m.Header.GetField(tag) : "";
        }

        public static string GetField(QuickFix.Message m, int tag)
        {
            return m.IsSetField(tag) ? m.GetField(tag) : "";
        }

        public static void DeleteFile(string filePath, object fileLock)
        {
            lock (fileLock)
            {
                File.Delete(filePath);
            }
        }
        public static void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        public static Dictionary<int, string> GetTagValuePairs(Message m)
        {
            return GetTagValuePairs(m.ToString());
        }
        public static Dictionary<int, string> GetTagValuePairs(string m)
        {
            Dictionary<int, string> pairs = new Dictionary<int, string>();
            string mString = m.ToString();
            string[] keyValues = mString.Split((char)1);
            foreach (string keyValue in keyValues)
            {
                int index = keyValue.IndexOf('=');
                if (index != -1)
                {
                    int key = Int32.Parse(keyValue.Substring(0, index), CultureInfo.CurrentCulture);
                    string value = keyValue.Substring(index + 1, keyValue.Length - (index + 1));
                    pairs[key] = value;
                }
            }
            return pairs;
        }

        public static decimal RoundToDecimalPlaces(decimal number, int decimalPlaces)
        {
            decimal power = (decimal)Math.Pow(10, decimalPlaces);
            decimal decimalPower = (decimal)Math.Pow(10, -(decimalPlaces + 1)) * 5;
            return Math.Floor((number + decimalPower) * power) / power;
        }

        

    }
}
