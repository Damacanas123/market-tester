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

namespace MarketTester.Helper
{
    
    public class JsonConfig
    {
        private static string SETTINGS_FILEPATH = MarketTesterUtil.APPLICATION_STATIC_DIR + "config.json";

        private const string CONFIG_FILES = "ConfigFiles";
        private const string DATA_REFRESH_RATE = "DataRefreshRate";
        private const string FILE_PATH = "FilePath";
        private const string PROTOCOL_TYPE = "ProtocolType";
        private const string USERNAME = "Username";
        private const string PASSWORD = "Password";

        private static JsonConfig instance = null;

        #region properties
        public List<ConfigFile> ConfigFiles { get; set; } = new List<ConfigFile>();
        public int DataRefreshRate { get; set; }
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
            if (File.Exists(SETTINGS_FILEPATH))
            {
                string configContent = "";
                using (StreamReader file = new StreamReader(SETTINGS_FILEPATH))
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
                Dictionary<string, object> jsonDeserialized = JsonConvert.DeserializeObject<Dictionary<string, object>>(configContent);
                //Check mandatory fields
                CheckJsonKey(CONFIG_FILES, jsonDeserialized);
                CheckJsonKey(DATA_REFRESH_RATE, jsonDeserialized);
                if (jsonDeserialized.TryGetValue(CONFIG_FILES, out object configFiles))
                {
                    foreach (JObject configFile in (JArray)configFiles)
                    {
                        BISTCredentialParams credentialParams = null;
                        if (configFile.ContainsKey(USERNAME))
                        {
                            if (!configFile.ContainsKey(PASSWORD))
                            {
                                InfoManager.PublishInfo(Enumeration.EInfo.Primary, App.Current.Resources["StringConfigPasswordNotSetWarning"].ToString() + " " + configFile[FILE_PATH]);
                            }
                            else
                            {
                                credentialParams = new BISTCredentialParams((string)configFile[USERNAME], (string)configFile[PASSWORD]);
                            }
                        }
                        if(credentialParams != null)
                        {
                            ConfigFiles.Add(new ConfigFile((string)configFile[FILE_PATH], ConvertProtocolType((string)configFile[PROTOCOL_TYPE]),
                            credentialParams));
                        }
                        else
                        {
                            ConfigFiles.Add(new ConfigFile((string)configFile[FILE_PATH], ConvertProtocolType((string)configFile[PROTOCOL_TYPE])));
                        }
                        
                    }
                }
                DataRefreshRate = int.Parse(jsonDeserialized[DATA_REFRESH_RATE].ToString(), CultureInfo.CurrentCulture);
            }
            else
            {
                throw new FileNotFoundException($"{SETTINGS_FILEPATH} can't be found");
            }

        }
        private ProtocolType ConvertProtocolType(string protocolTypeString)
        {
            switch (protocolTypeString)
            {
                case MarketTesterUtil.FIX50SP2:
                    return ProtocolType.Fix50sp2;
                case MarketTesterUtil.OUCH:
                    return ProtocolType.OUCH;
                default:
                    throw new Exception("Unsopprted protocol type in config File");
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
