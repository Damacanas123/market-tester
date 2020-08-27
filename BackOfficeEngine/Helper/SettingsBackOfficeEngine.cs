using BackOfficeEngine.MessageEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BackOfficeEngine.Helper
{
    public class SettingsBackOfficeEngine
    {
        private string SaveFilePath = Util.APPLICATION_SAVE_DIR + "app_settings";
        private static SettingsBackOfficeEngine instance;
        public static SettingsBackOfficeEngine Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new SettingsBackOfficeEngine();
                    instance.LoadSettings();
                }
                return instance;
            }
        }
        private SymbolISIN symbolISINSetting;
        public SymbolISIN SymbolISINSetting
        {
            get
            {
                return symbolISINSetting;
            }
            set
            {
                symbolISINSetting = value;
                SaveSettings();
            }
        }
        

        private void SaveSettings()
        {
            Dictionary<string, string> settingsDic = new Dictionary<string, string>();
            settingsDic[nameof(SymbolISINSetting)] = SymbolISINSetting.ToString();
            string settingsString = "";
            foreach(KeyValuePair<string,string> pair in settingsDic)
            {
                settingsString += pair.Key + "=" + pair.Value + Environment.NewLine;
            }
            Util.OverwriteToFile(SaveFilePath, settingsString);
        }

        private void LoadSettings()
        {
            if (File.Exists(SaveFilePath))
            {
                string[] lines = Util.ReadLines(SaveFilePath);
                
                foreach(string line in lines)
                {
                    string [] splitted = line.Split('=');
                    string key = splitted[0];
                    string value = splitted[1];
                    if (key == nameof(SymbolISINSetting))
                    {
                        try
                        {
                            SymbolISINSetting = (SymbolISIN)Enum.Parse(typeof(SymbolISIN), value);
                        }
                        catch
                        {
                            string logMsg = $"Couldn't parse value({value}) of {key}. Valid values are ";
                            foreach (var enumValue in Enum.GetValues(typeof(SymbolISIN)))
                            {
                                logMsg += enumValue.ToString() + ",";
                            }
                            Util.Log(logMsg);
                        }
                        
                    }
                }
            }
            
        }

    }
}
