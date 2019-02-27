using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Markup;

namespace AgentAssignment4
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("da-DK"); 
            //Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("da-DK");
            //CultureInfo ci1 = CultureInfo.CurrentCulture;
            //CultureInfo ci2 = Thread.CurrentThread.CurrentCulture;
            //CultureInfo ci3 = Thread.CurrentThread.CurrentUICulture;

            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            //MessageBox.Show(Thread.CurrentThread.CurrentUICulture.Name);

            // Uncomment the statement below to fix the UI culture for Xaml.
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
              new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.Name)));

            base.OnStartup(e);

        }
    }
}
