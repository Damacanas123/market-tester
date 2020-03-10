using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackOfficeEngine;

namespace MarketTester.Model
{
    public class ConfigFile
    {
        public string FilePath;
        public ProtocolType ProtocolType;
        public ConfigFile(string filePath, ProtocolType protocolType)
        {
            FilePath = filePath;
            ProtocolType = protocolType;
        }
    }
}
