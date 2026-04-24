using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace ARKServerCreationTool
{
    public static class InterfaceMetricHelper
    {
        private static string cachedLanIP;

        public static string GetAutoDetectedLanIP()
        {
            if (cachedLanIP != null) return cachedLanIP;

            var wifiInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211
                             && ni.OperationalStatus == OperationalStatus.Up
                             && ni.GetIPProperties().UnicastAddresses
                                 .Any(ua => ua.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                                            && !IPAddress.IsLoopback(ua.Address)));

            var target = wifiInterfaces.FirstOrDefault();

            if (target == null)
            {
                target = NetworkInterface.GetAllNetworkInterfaces()
                    .FirstOrDefault(ni => ni.NetworkInterfaceType != NetworkInterfaceType.Loopback
                                          && ni.OperationalStatus == OperationalStatus.Up
                                          && ni.GetIPProperties().UnicastAddresses
                                              .Any(ua => ua.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                                                         && !IPAddress.IsLoopback(ua.Address)));
            }

            if (target == null) return cachedLanIP = string.Empty;

            var ip = target.GetIPProperties().UnicastAddresses
                .First(ua => ua.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                             && !IPAddress.IsLoopback(ua.Address))
                .Address.ToString();

            return cachedLanIP = ip;
        }

        public static void ClearCachedIP()
        {
            cachedLanIP = null;
        }

        public static int? FindInterfaceIndexForIP(string ipAddress)
        {
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                var ipProps = ni.GetIPProperties();
                if (ipProps.UnicastAddresses.Any(ua => ua.Address.ToString() == ipAddress))
                {
                    return ipProps.GetIPv4Properties()?.Index;
                }
            }
            return null;
        }

        public static int GetCurrentMetric(int interfaceIndex)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -Command \"(Get-NetIPInterface -InterfaceIndex {interfaceIndex} -AddressFamily IPv4).InterfaceMetric\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using (var process = Process.Start(startInfo))
            {
                string output = process.StandardOutput.ReadToEnd().Trim();
                process.WaitForExit();
                return int.TryParse(output, out int metric) ? metric : -1;
            }
        }

        public static bool SetMetric(int interfaceIndex, int metric)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -Command \"Set-NetIPInterface -InterfaceIndex {interfaceIndex} -InterfaceMetric {metric}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                    return process.ExitCode == 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool ApplyLocalPlayMetric(string ipAddress)
        {
            int? ifaceIndex = FindInterfaceIndexForIP(ipAddress);
            if (ifaceIndex == null) return false;

            int currentMetric = GetCurrentMetric(ifaceIndex.Value);
            if (currentMetric < 0) return false;

            var config = ASCTGlobalConfig.Instance;
            config.SavedInterfaceMetric = currentMetric;
            config.SavedInterfaceIndex = ifaceIndex.Value;
            config.Save();

            return SetMetric(ifaceIndex.Value, 1);
        }

        public static bool RestoreMetric()
        {
            var config = ASCTGlobalConfig.Instance;
            if (config.SavedInterfaceIndex == null || config.SavedInterfaceMetric == null)
                return true;

            bool success = SetMetric(config.SavedInterfaceIndex.Value, config.SavedInterfaceMetric.Value);

            if (success)
            {
                config.SavedInterfaceMetric = null;
                config.SavedInterfaceIndex = null;
                config.Save();
            }

            return success;
        }
    }
}
