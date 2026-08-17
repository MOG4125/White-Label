using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace WhiteLabelLauncher
{
    internal static class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool SetWindowText(IntPtr hWnd, string text);

        [DllImport("user32.dll")]
        private static extern bool IsWindow(IntPtr hWnd);

        private static int Main(string[] args)
        {
            var folder = AppDomain.CurrentDomain.BaseDirectory;
            var config = LauncherConfig.Load(Path.Combine(folder, "WhiteLabelLauncher.ini"));
            var playnitePath = Path.Combine(folder, "Playnite.DesktopApp.exe");
            if (!File.Exists(playnitePath)) playnitePath = Path.Combine(folder, "Playnite.exe");

            if (!File.Exists(playnitePath))
            {
                Console.Error.WriteLine("Playnite executable was not found beside WhiteLabelLauncher.exe.");
                return 2;
            }

            var forwarded = string.Join(" ", args ?? new string[0]);
            if (string.IsNullOrWhiteSpace(forwarded)) forwarded = config.StartFullscreen ? "--startfullscreen" : "--startdesktop";
            var psi = new ProcessStartInfo(playnitePath, forwarded)
            {
                WorkingDirectory = folder,
                UseShellExecute = false
            };

            using (var process = Process.Start(psi))
            {
                if (process == null) return 3;
                while (!process.HasExited)
                {
                    try
                    {
                        process.Refresh();
                        if (process.MainWindowHandle != IntPtr.Zero && IsWindow(process.MainWindowHandle))
                        {
                            SetWindowText(process.MainWindowHandle, config.LauncherTitle);
                        }
                    }
                    catch (InvalidOperationException) { break; }
                    catch (Exception ex) { Console.Error.WriteLine(ex.Message); }
                    Thread.Sleep(config.PollMilliseconds);
                }
                return process.ExitCode;
            }
        }
    }

    internal sealed class LauncherConfig
    {
        public string LauncherTitle = "My Launcher";
        public bool StartFullscreen;
        public int PollMilliseconds = 500;

        public static LauncherConfig Load(string path)
        {
            var result = new LauncherConfig();
            if (!File.Exists(path)) return result;
            foreach (var raw in File.ReadAllLines(path))
            {
                var line = raw.Trim();
                if (line.Length == 0 || line.StartsWith("#") || !line.Contains("=")) continue;
                var pair = line.Split(new[] { '=' }, 2);
                var key = pair[0].Trim();
                var value = pair[1].Trim();
                if (key.Equals("LauncherTitle", StringComparison.OrdinalIgnoreCase)) result.LauncherTitle = value;
                else if (key.Equals("StartFullscreen", StringComparison.OrdinalIgnoreCase)) bool.TryParse(value, out result.StartFullscreen);
                else if (key.Equals("PollMilliseconds", StringComparison.OrdinalIgnoreCase)) int.TryParse(value, out result.PollMilliseconds);
            }
            if (string.IsNullOrWhiteSpace(result.LauncherTitle)) result.LauncherTitle = "My Launcher";
            if (result.PollMilliseconds < 100) result.PollMilliseconds = 100;
            return result;
        }
    }
}
