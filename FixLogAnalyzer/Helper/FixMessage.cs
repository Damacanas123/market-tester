using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixLogAnalyzer
{
    internal class FixMessage
    {
        private Dictionary<string, string> tag_values = new Dictionary<string, string>();
        internal DateTime logtime;
        private string msgString;
        public const char delimiter = '\u0001';
        internal FixMessage(string msgString, DateTime logtime)
        {
            this.msgString = msgString;
            string[] tagValues = msgString.Split(delimiter);
            for (int i = 0; i < tagValues.Length - 1; i++)
            {
                string tagValue = tagValues[i];
                string[] temp = tagValue.Split('=');
                tag_values[temp[0]] = temp[1];
            }
            this.logtime = logtime;
        }
        internal string GetField(string field)
        {
            if (tag_values.TryGetValue(field, out string value))
            {

            }
            else
            {
                value = null;
            }
            return value;
        }

        internal string GetField(int field)
        {
            if (tag_values.TryGetValue(field.ToString(), out string value))
            {

            }
            else
            {
                value = null;
            }
            return value;
        }

        internal bool IsSet(int field)
        {
            return tag_values.ContainsKey(field.ToString());
        }

        internal int GetTotalMassQuoteNum()
        {
            if(GetField(35) != "i")
            {
                return 0;
            }
            else
            {
                int prevIndex = 0;
                int totalMassQuoteCount = 0;
                while (true)
                {
                    int noQuoteEntryIndex = msgString.IndexOf(delimiter + "295=",prevIndex + 1);
                    if(noQuoteEntryIndex == -1)
                    {
                        break;
                    }
                    else
                    {
                        try
                        {
                            int numEndIndex = msgString.IndexOf(delimiter, noQuoteEntryIndex + 1);
                            int temp = noQuoteEntryIndex + 5;
                            totalMassQuoteCount += (int.Parse(msgString.Substring(temp, numEndIndex - (temp)), CultureInfo.InvariantCulture)) * 2;
                        }
                        catch
                        {

                        }
                        finally
                        {
                            prevIndex = noQuoteEntryIndex;
                        }
                        
                        
                    }
                }
                return totalMassQuoteCount;
            }
        }
    }
}
