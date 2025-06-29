using System;
using System.Globalization;
using System.Threading;
using System.Windows;

namespace NotSoEpicApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string preferredLanguage = NotSoEpicApp.Properties.Settings.Default.PreferredLanguage;
            if (!string.IsNullOrEmpty(preferredLanguage))
            {
                LanguageManager.LoadResourceDictionary(preferredLanguage);
            }
        }
    }
}
