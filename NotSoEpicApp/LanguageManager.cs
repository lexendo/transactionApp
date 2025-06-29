using System;
using System.Globalization;
using System.Threading;
using System.Windows;

namespace NotSoEpicApp
{
    internal class LanguageManager
    {
        public static void LoadResourceDictionary(string cultureCode)
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (cultureCode)
            {
                case "sk":
                    dict.Source = new Uri("/Properties/StringResources.sk.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("/Properties/StringResources.xaml", UriKind.Relative);
                    break;
            }

            Application.Current.Resources.MergedDictionaries.Add(dict);
        }

    }
}
