using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MarketTester.Resource
{
    public static class ResourceLanguageManager
    {
        public enum Language
        {
            Turkish,English
        }
        public static void SetLanguage(Language language)
        {
            Application.Current.Resources.MergedDictionaries.RemoveAt(0);
            string uri = @"\Resource\";
            switch (language)
            {
                case Language.Turkish:
                    uri += "ResourceString_tur.xaml";
                    break;
                case Language.English:
                    uri += "ResourceString_eng.xaml";
                    break;
            }
            Application.Current.Resources.MergedDictionaries.Insert(0, (new ResourceDictionary { Source = new Uri(uri, UriKind.Relative) }));
        }
    }
}
