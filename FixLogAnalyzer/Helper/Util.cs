using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FixLogAnalyzer
{
    internal class Util
    {

        internal const string outDateFormat = "yyyyMMdd-HH:mm:ss.ffffff";
        internal const string sendingTimeDateFormat = "yyyyMMdd-HH:mm:ss.fff";
        internal static string[] ReadLines(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new IOException("Given file path does not exist.");
            }
            FileStream inf = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            List<byte> bytes = new List<byte>(400);
            int a;
            List<string> lines = new List<string>(200);
            while ((a = inf.ReadByte()) != -1)
            {
                //newline
                if (a == 13)
                {
                    a = inf.ReadByte();
                    if (a == 10)
                    {
                        string asciiString = Encoding.ASCII.GetString(bytes.ToArray(), 0, bytes.Count);
                        lines.Add(asciiString);
                        bytes.Clear();
                    }
                }
                else if (a == 10)
                {
                    string asciiString = Encoding.ASCII.GetString(bytes.ToArray(), 0, bytes.Count);
                    lines.Add(asciiString);
                    bytes.Clear();
                }
                else
                {
                    bytes.Add((byte)a);
                }
            }
            inf.Close();
            inf.Dispose();
            return lines.ToArray();
        }

        internal static string ReadLine(Stream stream)
        {
            List<byte> bytes = new List<byte>(400);
            int a;
            while ((a = stream.ReadByte()) != -1)
            {
                //newline
                if (a == 13)
                {
                    a = stream.ReadByte();
                    if (a == 10)
                    {
                        return Encoding.ASCII.GetString(bytes.ToArray(), 0, bytes.Count); ;
                    }
                }
                else if (a == 10)
                {
                    return Encoding.ASCII.GetString(bytes.ToArray(), 0, bytes.Count);
                }
                else
                {
                    bytes.Add((byte)a);
                }
            }
            return Encoding.ASCII.GetString(bytes.ToArray(), 0, bytes.Count);
        }

        public static string GetTag(string msg,string tag)
        {
            string searchString = $"\u0001{tag}=";
            int startIndex = msg.IndexOf(searchString) + searchString.Length;
            int endIndex = msg.IndexOf("\u0001", startIndex);
            return msg.Substring(startIndex, endIndex - startIndex);
        }

        public static string GetSidePretty(string msg)
        {
            string searchString = $"\u000154=";
            int startIndex = msg.IndexOf(searchString) + searchString.Length;
            int endIndex = msg.IndexOf("\u0001", startIndex);
            string sideRaw = msg.Substring(startIndex, endIndex - startIndex);
            switch (sideRaw)
            {
                case "1":
                    return "Buy";
                case "2":
                    return "Sell";
                case "5":
                    return "Sell Short";
                default:
                    return $"Unknown({sideRaw})";
            }
        }

        public static decimal RoundToDecimalPlaces(decimal number, int decimalPlaces)
        {
            decimal power = (decimal)Math.Pow(10, decimalPlaces);
            decimal decimalPower = (decimal)Math.Pow(10, -(decimalPlaces + 1)) * 5;
            return Math.Floor((number + decimalPower) * power) / power;
        }
    }
}
