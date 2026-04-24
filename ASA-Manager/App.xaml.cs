using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ARKServerCreationTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            bool newConfig = false;

            if (newConfig = (ASCTGlobalConfig.Instance == null))
            {
                ASCTGlobalConfig.NewConfig();
                ASCTConfiguration legacyConfig = ASCTConfiguration.LoadConfig();
                if (legacyConfig != null)
                {
                    ASCTGlobalConfig.Instance.AutomaticallyCreateFirewallRule = legacyConfig.AutomaticallyCreateFirewallRule;

                    ASCTServerConfig legacyServer = new ASCTServerConfig(ASCTGlobalConfig.Instance.NextAvailableID(), ASCTGlobalConfig.Instance.NextAvailablePort());
                    legacyServer.IPAddress = legacyConfig.IPAddress;
                    legacyServer.GamePort = legacyConfig.GamePort;
                    legacyServer.GameDirectory = legacyConfig.GameDirectory;
                    legacyServer.customLaunchArgs = legacyConfig.customLaunchArgs;
                    legacyServer.useCustomLaunchArgs = legacyConfig.useCustomLaunchArgs;
                    legacyServer.UseMultihome = legacyConfig.UseMultihome;

                    ASCTGlobalConfig.Instance.Servers.Add(legacyServer);

                    MessageBox.Show("Existing settings have been imported to the new format successfully");
                }
            }

            if (newConfig)
            {
                ASCTConfigWindow configWindow = new ASCTConfigWindow(true);
                configWindow.Show();
            }
            else
            {
                ServerList list = new ServerList();

                list.Show();

                if (ASCTGlobalConfig.Instance.SavedInterfaceIndex != null && ASCTGlobalConfig.Instance.SavedInterfaceMetric != null)
                {
                    var result = System.Windows.MessageBox.Show(
                        "ASCT detected that a previous server session did not clean up network adapter settings.\n\n" +
                        "Restore original adapter metrics?",
                        "Network Settings Recovery",
                        System.Windows.MessageBoxButton.YesNo);

                    if (result == System.Windows.MessageBoxResult.Yes)
                    {
                        InterfaceMetricHelper.RestoreMetric();
                    }
                }

                if (ASCTGlobalConfig.Instance.AllowAutomaticStart)
                {
                    foreach (ASCTServerConfig server in ASCTGlobalConfig.Instance.Servers.Where(s => s.StartAutomatically))
                    {
                        server.ProcessManager.Start();
                    }
                }
            }
        }
    }
}
