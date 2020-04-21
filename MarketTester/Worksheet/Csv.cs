using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MarketTester.Model.OrderHistoryFix;
using MarketTester.Helper;

namespace MarketTester.Worksheet
{
    public class Csv
    {
        public static void CreateCsvFromHistoryItems(string filePath,List<HistoryItem> historyItems)
        {
            string csv = "";
            foreach (string column in HistoryItem.ColumnNames)
            {
                csv += column.ToString() + ",";
            }
            csv = csv.Substring(0, csv.Length - 1) + "\n";
            foreach (HistoryItem historyItem in historyItems)
            {
                foreach (string column in historyItem.Columns)
                {
                    csv += "\"" + column + "\",";
                }
                csv = csv.Substring(0, csv.Length - 1) + "\n";
            }
            MarketTesterUtil.OverwriteToFile(filePath, csv);
        }
        public static void AppendCsvFromHistoryItems(string filePath, List<HistoryItem> historyItems)
        {
            string csv = "";
            
            foreach (HistoryItem historyItem in historyItems)
            {
                foreach (string column in historyItem.Columns)
                {
                    csv += "\"" + column + "\",";
                }
                csv = csv.Substring(0, csv.Length - 1) + "\n";
            }
            MarketTesterUtil.AppendStringToFile(filePath, csv);
        }
    }
}
