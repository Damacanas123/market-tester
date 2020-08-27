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
using System.Deployment.Application;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Collections.Concurrent;
using BackOfficeEngine.Model;
using System.CodeDom.Compiler;

[assembly: InternalsVisibleTo("UnitTest")]
namespace BackOfficeEngine.Helper
{
    
    public class Util
    {
        public const string APP_VERSION = "1.0.0.1";

        public const string PRIMARY_STRING = "Primary";
        public const string SECONDARY_STRING = "Secondary";
        public const string DC1_STRING = "DC1";
        public const string DC2_STRING = "DC2";
        public const string RD_STRING = "RD";

        public const string FILE_PATH_DELIMITER = "\\";
        public static string APPLICATION_COMMON_DIR = "C:\\MATRIKS_OMS" + FILE_PATH_DELIMITER + "MarketTester" + FILE_PATH_DELIMITER;
        public static string APPLICATION_SEQ_NUM_DIR = APPLICATION_COMMON_DIR + "sequence_nums\\";
        public static string STATIC_DIR_PATH = "static" + FILE_PATH_DELIMITER;
        public static string APPLICATION_STATIC_DIR = APPLICATION_COMMON_DIR + STATIC_DIR_PATH;
        public static string EXCEPTIONLOG_FILE_PATH = APPLICATION_COMMON_DIR + "exception.log";
        public static string DEBUG_EXCEPTIONLOG_FILE_PATH = APPLICATION_COMMON_DIR + "debug_exception.log";
        public static string DEBUG_FILE_PATH = APPLICATION_COMMON_DIR + "debug.log";
        public static string APPLICATIONLOG_FILE_PATH = APPLICATION_COMMON_DIR + "application.log";
        public static string SPECIAL_DIR = APPLICATION_COMMON_DIR + "intsys" + FILE_PATH_DELIMITER;
        public static string DYNAMIC_SETTINGS_FILE_PATH = SPECIAL_DIR + "dyn_sets.cfg";
        public static string DateFormatMicrosecondPrecision = "yyyyMMdd-HH:mm:ss.ffffff";
        public static string DateFormatYearMonthDay = "yyyyMMdd";
        public static string APPLICATION_SAVE_DIR = APPLICATION_COMMON_DIR + "save" + FILE_PATH_DELIMITER;
        public static UTF8Encoding UTF8 = new UTF8Encoding(true);

        public static void LogError(Exception ex)
        {
            AppendStringToFile(EXCEPTIONLOG_FILE_PATH, $"Appl version({APP_VERSION}) - {DateTime.Now} {Environment.NewLine}Type : {ex.GetType().ToString()} {Environment.NewLine}Exception : {ex.ToString()}{Environment.NewLine}");
        }

        public static void Debug(string s)
        {
            AppendStringToFile(DEBUG_FILE_PATH, $"Appl version({APP_VERSION}) - {DateTime.Now} {Environment.NewLine}{s}{Environment.NewLine}");
        }

        public static void Log(string s)
        {
            AppendStringToFile(APPLICATIONLOG_FILE_PATH, $"Appl version({APP_VERSION}) - {DateTime.Now} {Environment.NewLine}{s}{Environment.NewLine}");
        }

        public static void LogDebugError(Exception ex,string s = null)
        {
            AppendStringToFile(DEBUG_EXCEPTIONLOG_FILE_PATH, (s != null ? s + Environment.NewLine : "") + $"Appl version({APP_VERSION}) - {DateTime.Now} {Environment.NewLine}Type : {ex.GetType().ToString()} {Environment.NewLine}Exception : {ex.ToString()}{Environment.NewLine}");
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

        public static string RemoveNonNumericKeepDotAndDash(string s)
        {
            string temp = "";
            bool isDotPresent = false;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                          
                if (c <= '9' && c >= '0')
                {
                    temp += c;
                }
                else if (c == '.' && !isDotPresent)
                {
                    temp += c;
                    isDotPresent = true;
                }
                else if (i == 0 && c == '-')
                {
                    temp += c;
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


        public static void OverwriteToFile(string filePath, string s)
        {
            lock (GetReferenceToLock(filePath))
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    sw.Write(s);
                }
            }
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


        public static string GetAutoCommentFromMessage(Message m)
        {
            string comment = "";
            if (m.Header.IsSetField(Tags.MsgType))
            {
                string msgType = m.Header.GetField(Tags.MsgType);
                if (FixValues.MsgTypeInbound.ContainsKey(msgType))
                {
                    comment += FixValues.MsgTypeInbound[msgType];
                }
                else if (FixValues.MsgTypeOutbound.ContainsKey(msgType))
                {
                    comment += FixValues.MsgTypeOutbound[msgType];
                }
            }

            if (m.IsSetField(Tags.Price))
            {
                comment += " Price : " + m.GetField(Tags.Price);
            }
            if (m.IsSetField(Tags.OrderQty))
            {
                comment += " OrdQty : " + m.GetField(Tags.OrderQty);
            }
            if (m.IsSetField(Tags.Side))
            {
                comment += " Side : " + m.GetField(Tags.Side);
            }
            if (m.IsSetField(Tags.Symbol))
            {
                comment += " Symbol : " + m.GetField(Tags.Symbol);
            }
            return comment;
        }

        public static string GetTodayString()
        {
            return DateTime.Now.ToString(DateFormatYearMonthDay, CultureInfo.InvariantCulture);
        }


        

        private static object fileLocksLock {get;set;} = new object();
        private static Dictionary<string, object> fileLocks { get; set; } = new Dictionary<string, object>();
        private static object GetReferenceToLock(string filePath)
        {
            lock (fileLocksLock)
            {
                if (fileLocks.TryGetValue(filePath, out object Lock))
                {
                    return Lock;
                }
                else
                {
                    Lock = new object();
                    fileLocks[filePath] = Lock;
                }
                return Lock;
            }
            
        }
        public static void AppendStringToFile(string filePath, string content)
        {
            FileWriteQueue.Enqueue((filePath, content));
        }


        public static void DeleteFile(string filePath)
        {
            lock (GetReferenceToLock(filePath))
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        public static void ClearFileLocks()
        {
            lock (fileLocksLock)
            {
                fileLocks.Clear();
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


        static string pattern = @"(?<=\x01).*?(?==)";
        static Regex tagPatternRgx = new Regex(pattern);
        public static List<int> GetAllTagsOfAMessage(Message m)
        {

            return GetAllTagsOfAMessage(m.ToString());
        }

        public static List<int> GetAllTagsOfAMessage(string msg)
        {
            List<int> tags = new List<int>();
            foreach (Match match in tagPatternRgx.Matches(msg.ToString()))
            {
                try
                {
                    tags.Add(Int32.Parse(match.Value, CultureInfo.CurrentCulture));
                }
                catch
                {
                    Util.Debug($"Couldn't parse tag {match.Value} in {Environment.NewLine}{msg}");
                }
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
                    s += Environment.NewLine;
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

        public static string GetDirFromFullPath(string fullPath)
        {
            if(fullPath == null)
            {
                return null;
            }
            int lastIndex = fullPath.LastIndexOf(FILE_PATH_DELIMITER);
            if(lastIndex == -1)
            {
                return null;
            }
            else
            {
                return fullPath.Substring(0, lastIndex);
            }

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

        

        public static Dictionary<int, string> GetTagValuePairs(Message m)
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


        public static string GetRandomString(int length)
        {
            string s = "";
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                s += (char)('A' + random.Next(0, 25));
            }
            return s;
        }

        public static string ReadFile(string filePath)
        {
            lock (GetReferenceToLock(filePath))
            {
                if (File.Exists(filePath))
                {
                    string content = File.ReadAllText(filePath);
                    return content;
                }
                else
                {
                    string directories = filePath.Substring(0, filePath.LastIndexOf(FILE_PATH_DELIMITER));
                    Directory.CreateDirectory(directories);
                    File.Create(filePath);
                    return "";
                }
            }
        }

        public static string[] ReadLines(string filePath)
        {
            string content = BackOfficeEngine.Helper.Util.ReadFile(filePath);
            if (!string.IsNullOrEmpty(content))
            {
                return content.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                return new string[] { };
            }
        }

        public delegate void ThreadParameterlessFunction();
        public static void ThreadStart(ThreadParameterlessFunction func)
        {
            Thread t = new Thread(() =>
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
                func();
            });
            t.Start();
        }

        static ConcurrentQueue<(string, string)> FileWriteQueue { get; set; } = new ConcurrentQueue<(string, string)>();
        static bool FileWriteThreadStarted = false;
        public static void StartFileWriteThread()
        {
            if (FileWriteThreadStarted)
            {
                return;
            }
            FileWriteThreadStarted = true;
            ThreadStart(() =>
            {
                while (true)
                {
                    if (FileWriteQueue.TryDequeue(out var item))
                    {
                        string filePath = item.Item1;
                        string content = item.Item2;
                        lock (GetReferenceToLock(filePath))
                        {
                            string dirPath = GetDirFromFullPath(filePath);
                            if(dirPath != null)
                            {
                                Directory.CreateDirectory(dirPath);                        
                            }
                            using (StreamWriter sw = File.AppendText(filePath))
                            {
                                sw.WriteLine(content);
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(2000);
                    }
                }
            });
        }


        public static string GetLineRepr(IMessage msg)
        {
            return msg.ToString() + "|" + msg.TimeStamp.ToString(Util.DateFormatMicrosecondPrecision, CultureInfo.InvariantCulture);
        }

        public static IMessage ParseLineRepr(string line)
        {
            int pipeIndex = line.IndexOf("|");
            IMessage imsg;
            if (pipeIndex != -1)
            {
                string msg = line.Substring(0, pipeIndex);
                string date = line.Substring(pipeIndex + 1, line.Length - (pipeIndex + 1));
                imsg = new QuickFixMessage(msg);
                imsg.TimeStamp = DateTime.ParseExact(date, Util.DateFormatMicrosecondPrecision, CultureInfo.InvariantCulture);
            }
            else
            {
                imsg = new QuickFixMessage(line);
            }
            return imsg;
        }

        public static string RemovePathInvalidChars(string filePath)
        {
            string temp = "";
            char [] invalidChars = Path.GetInvalidPathChars();
            foreach(char c in filePath)
            {
                if (!invalidChars.Contains(c))
                {
                    temp += c;
                }
            }
            return temp;
        }
        public static string RemoveFileNameInvalidChars(string filePath)
        {
            string temp = "";
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in filePath)
            {
                if (!invalidChars.Contains(c))
                {
                    temp += c;
                }
            }
            return temp;
        }

        //public static void ReadBasicConfigFile(string filePath,out Dictionary<string,string> keyValueMap)
        //{
        //    if (File.Exists(filePath))
        //    {
        //        foreach(string line in File.ReadLines(filePath))
        //        {
        //            if (line.Contains("="))
        //            {
        //                string[] keyValue = line.Split('=');

        //            }
        //        }
        //    }
        //}

    }
}
