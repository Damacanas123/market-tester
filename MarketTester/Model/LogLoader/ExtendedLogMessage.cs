using BackOfficeEngine.Helper;
using MarketTester.Helper;
using QuickFix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketTester.Model.LogLoader
{
    public class ExtendedLogMessage
    {
        public Dictionary<int, string> TagValuePairs { get; set; } = new Dictionary<int, string>();
        public string Raw { get; set; }
        public int LineNum { get; set; }
        /// <summary>
        /// Initialize log message from a string that contains a valid fix message
        /// </summary>
        /// <param name="line"></param>
        public ExtendedLogMessage(string line,int lineNum)
        {
            string msg = Fix.ExtractFixMessageFromALine(line);
            if (string.IsNullOrWhiteSpace(msg))
            {
                throw new InvalidMessage();
            }
            TagValuePairs = MarketTesterUtil.GetTagValuePairs(msg);
            LineNum = lineNum;
        }

        public string GetField(int field)
        {
            TagValuePairs.TryGetValue(field, out string value);
            return value;
        }
    }
}
