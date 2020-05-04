using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BackOfficeEngine;
using BackOfficeEngine.ParamPacker;

using MarketTester.Model;
using MarketTester.ViewModel.Manager;
using MarketTester.Exceptions;

namespace MarketTester.Helper
{
    
    public class JsonConfig
    {
        public static string JSON_CONFIG_PATH = MarketTesterUtil.APPLICATION_STATIC_DIR + "config.json";

        private const string CONFIG_FILES = "ConfigFiles";
        private const string FILE_PATH = "FilePath";
        private const string PROTOCOL_TYPE = "ProtocolType";
        private const string USERNAME = "Username";
        private const string PASSWORD = "Password";

        private static JsonConfig instance = null;

        #region properties
        public List<ConfigFile> ConfigFiles { get; set; } = new List<ConfigFile>();
        #endregion
        public static JsonConfig GetInstance()
        {
            if(instance == null)
            {
                instance = new JsonConfig();
            }
            return instance;
        }
        private JsonConfig()
        {
            if (File.Exists(JSON_CONFIG_PATH))
            {
                string configContent = "";
                using (StreamReader file = new StreamReader(JSON_CONFIG_PATH))
                {
                    string ln;

                    while ((ln = file.ReadLine()) != null)
                    {
                        if (ln.Length > 0 && ln.Substring(0, 1) != "#")
                        {
                            configContent += ln;
                        }
                    }
                }
                Dictionary<string, object> jsonDeserialized;
                try
                {
                    jsonDeserialized = JsonConvert.DeserializeObject<Dictionary<string, object>>(configContent);
                }
                catch
                {
                    throw new FileFormatException();
                }
                
                //Check mandatory fields
                CheckJsonKey(CONFIG_FILES, jsonDeserialized);
                if (jsonDeserialized.TryGetValue(CONFIG_FILES, out object configFiles))
                {
                    foreach (JObject configFile in (JArray)configFiles)
                    {
                        BISTCredentialParams credentialParams = null;
                        if (configFile.ContainsKey(USERNAME))
                        {
                            if (!configFile.ContainsKey(PASSWORD))
                            {
                                InfoManager.PublishInfo(Enumeration.EInfo.Primary, App.Current.Resources[ResourceKeys.StringConfigPasswordNotSetWarning].ToString() + " " + configFile[FILE_PATH]);
                            }
                            else
                            {
                                credentialParams = new BISTCredentialParams((string)configFile[USERNAME], (string)configFile[PASSWORD]);
                            }
                        }
                        ConfigFile cfgFile = new ConfigFile((string)configFile[FILE_PATH], ConvertProtocolType((string)configFile[PROTOCOL_TYPE]));
                        
                        if (credentialParams != null)
                        {
                            cfgFile.CredentialParams = credentialParams;
                        }
                        ConfigFiles.Add(cfgFile);

                    }
                }
            }
            else
            {
                throw new FileNotFoundException($"{JSON_CONFIG_PATH} can't be found");
            }

        }
        private ProtocolType ConvertProtocolType(string protocolTypeString)
        {
            switch (protocolTypeString)
            {
                case MarketTesterUtil.FIX50SP2:
                    return ProtocolType.Fix50sp2;
                case MarketTesterUtil.FIX40:
                    return ProtocolType.Fix40;
                case MarketTesterUtil.FIX41:
                    return ProtocolType.Fix41;
                case MarketTesterUtil.FIX42:
                    return ProtocolType.Fix42;
                case MarketTesterUtil.FIX43:
                    return ProtocolType.Fix43;
                case MarketTesterUtil.FIX44:
                    return ProtocolType.Fix44;
                case MarketTesterUtil.FIX50:
                    return ProtocolType.Fix50;
                default:
                    NotSupportedProtocolType ex = new NotSupportedProtocolType("Unsupported protocol type in config File");
                    ex.Data.Add("data1", protocolTypeString);
                    throw ex;
            }
        }

        private void CheckJsonKey(string key,Dictionary<string,object> jsonDeserialized)
        {
            if (!jsonDeserialized.ContainsKey(key))
            {
                throw new FileFormatException($"Key {key} is not set at the root level of configurations json.");
            }
        }
    }
}
