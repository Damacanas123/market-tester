using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackOfficeEngine;
using BackOfficeEngine.ParamPacker;

namespace MarketTester.Model
{
    public class ConfigFile
    {
        public string FilePath { get; set; }
        public ProtocolType ProtocolType { get; set; }
        public BISTCredentialParams CredentialParams { get; set; }
        public ConfigFile(string filePath, ProtocolType protocolType)
        {
            FilePath = filePath;
            ProtocolType = protocolType;
        }

        public ConfigFile(string filePath, ProtocolType protocolType,BISTCredentialParams credentialParams)
        {
            FilePath = filePath;
            ProtocolType = protocolType;
        }
    }
}
