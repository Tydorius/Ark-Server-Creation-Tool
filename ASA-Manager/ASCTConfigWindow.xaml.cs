using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MessageBox = System.Windows.Forms.MessageBox;

namespace ARKServerCreationTool
{
    /// <summary>
    /// Interaction logic for ASCTConfigWindow.xaml
    /// </summary>
    public partial class ASCTConfigWindow : Window
    {
        private bool firstLaunch = false;

        public ASCTConfigWindow(bool firstLaunch = false)
        {
            InitializeComponent();

            txt_InstallPath.Text = ASCTGlobalConfig.Instance.ServersInstallationPath;
            txt_defaultPort.Text = ASCTGlobalConfig.Instance.StartingGamePort.ToString();
            txt_portIncrement.Text = ASCTGlobalConfig.Instance.PortIncrement.ToString();
            chk_autoFirewallRules.IsChecked = ASCTGlobalConfig.Instance.AutomaticallyCreateFirewallRule;
            chk_AllowAutoLaunch.IsChecked = ASCTGlobalConfig.Instance.AllowAutomaticStart;
            chk_PromptStartAllServers.IsChecked = ASCTGlobalConfig.Instance.PromptStartAllServersInCluster;
            chk_AseLocalPlay.IsChecked = ASCTGlobalConfig.Instance.EnableAseLocalPlay;
            txt_clusterDir.Text = ASCTGlobalConfig.Instance.GlobalClusterDir;

            this.firstLaunch = firstLaunch;
        }

        public ASCTConfigWindow() : this(false)
        {
        }

        private void btn_saveConfig_Click(object sender, RoutedEventArgs e)
        {
            ASCTGlobalConfig.Instance.ServersInstallationPath = txt_InstallPath.Text.Trim();
            ASCTGlobalConfig.Instance.StartingGamePort = ushort.Parse(txt_defaultPort.Text.Trim());
            ASCTGlobalConfig.Instance.PortIncrement = ushort.Parse(txt_portIncrement.Text.Trim());
            ASCTGlobalConfig.Instance.AutomaticallyCreateFirewallRule = chk_autoFirewallRules.IsChecked.Value;
            ASCTGlobalConfig.Instance.GlobalClusterDir = txt_clusterDir.Text;
            ASCTGlobalConfig.Instance.AllowAutomaticStart = chk_AllowAutoLaunch.IsChecked.Value;
            ASCTGlobalConfig.Instance.PromptStartAllServersInCluster = chk_PromptStartAllServers.IsChecked.Value;
            ASCTGlobalConfig.Instance.EnableAseLocalPlay = chk_AseLocalPlay.IsChecked.Value;
            ASCTGlobalConfig.Instance.Save();

            ASCTTools.FindOrCreateWindow<ServerList>();

            if (firstLaunch)
            {
                //UpdaterWindow update = new UpdaterWindow(true);

                //update.Show();
            }

            firstLaunch = false;

            this.Close();
        }

        private void WindowClose(object sender, CancelEventArgs e)
        {
            if (firstLaunch)
            {
                System.Windows.Forms.DialogResult result = MessageBox.Show(
                    "Are you sure you wish to exit, no config has been created?", "Are you sure?",
                    System.Windows.Forms.MessageBoxButtons.YesNo);

                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    Environment.Exit(0);
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }


        private void btn_gameDirBrowse_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.InitialDirectory = txt_InstallPath.Text;

                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    txt_InstallPath.Text = dialog.SelectedPath;
                }
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[0-9]+");

            e.Handled = !regex.IsMatch(e.Text);
        }

        private void btn_clusterDirBrowse_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.InitialDirectory = txt_clusterDir.Text;

                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    txt_clusterDir.Text = dialog.SelectedPath;
                }
            }
        }

        private void chk_AseLocalPlay_Changed(object sender, RoutedEventArgs e)
        {
            if (chk_AseLocalPlay.IsChecked == true)
            {
                InterfaceMetricHelper.ClearCachedIP();
                string detectedIP = InterfaceMetricHelper.GetAutoDetectedLanIP();

                if (string.IsNullOrEmpty(detectedIP))
                {
                    MessageBox.Show(
                        "No suitable network adapter was detected. Local Play requires an active Wi-Fi or LAN adapter with an IPv4 address.",
                        "Local Play Unavailable");
                    chk_AseLocalPlay.IsChecked = false;
                    return;
                }

                string adapterType = "adapter";
                foreach (var ni in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.GetIPProperties().UnicastAddresses.Any(ua => ua.Address.ToString() == detectedIP))
                    {
                        adapterType = ni.NetworkInterfaceType == System.Net.NetworkInformation.NetworkInterfaceType.Wireless80211
                            ? "Wi-Fi adapter" : "network adapter";
                        break;
                    }
                }

                MessageBox.Show(
                    $"Local Play enabled. Detected IP: {detectedIP} ({adapterType}).\n\n" +
                    "When an Ark: Survival Evolved server is started, ASCT will:\n" +
                    "• Automatically enable Multihome with this IP\n" +
                    "• Temporarily lower the adapter's interface metric to priority 1\n\n" +
                    "The original metric will be restored when the server stops.",
                    "Local Play Configured");
            }
        }

        private void btn_localPlayHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Local Play for Ark: Survival Evolved (ASE)\n" +
                "================================\n\n" +
                "When hosting an ASE dedicated server on the same machine you play on, " +
                "the server may bind to the wrong network adapter (e.g., an unused ethernet port) " +
                "and advertise a 169.254.x.x APIPA address instead of your real LAN IP.\n\n" +
                "This prevents you from connecting via:\n" +
                "  • 'open 127.0.0.1' (server doesn't listen on loopback)\n" +
                "  • LAN server browser (UDP broadcasts don't loop back)\n\n" +
                "Local Play fixes this by:\n" +
                "  1. Auto-enabling Multihome with your Wi-Fi/LAN IP\n" +
                "  2. Temporarily setting your adapter's interface metric to 1\n" +
                "     (this makes Windows prioritize it over unused adapters)\n\n" +
                "The original metric is restored when the server stops.\n\n" +
                "This is only needed for ASE + Wi-Fi. ASA works without this.\n" +
                "If you use ethernet, you likely don't need this either.",
                "Local Play Help");
        }
    }
}