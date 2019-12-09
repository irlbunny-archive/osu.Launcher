using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.IO;
using File = System.IO.File;

namespace osu.Launcher.Setup
{
    class Program
    {
        static void Main(string[] args)
        {
            string osuPath = string.Empty;

            try
            {
                osuPath = GetOsuPath();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.ReadKey();
                return;
            }

            if (!File.Exists("osu.Launcher.exe"))
            {
                Console.WriteLine("The osu!Launcher executable was not found, please make sure it exists in the working directory!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Setting up osu!Launcher...");

            Directory.CreateDirectory($"{osuPath}\\osu.Launcher-Data");

            if (!File.Exists($"{osuPath}\\osu.Launcher-Data\\hosts"))
                File.Create($"{osuPath}\\osu.Launcher-Data\\hosts").Dispose();

            File.Delete($"{osuPath}\\osu.Launcher.exe");
            File.Move("osu.Launcher.exe", $"{osuPath}\\osu.Launcher.exe");

            CreateDesktopShortcut("Launch osu!", "-go",
                $"{osuPath}\\osu.Launcher.exe", "Launch osu! using osu!Launcher.",
                osuPath, $"{osuPath}\\osu.Launcher.exe");

            CreateDesktopShortcut("Edit osu!Launcher's hosts", $"{osuPath}\\osu.Launcher-Data\\hosts",
                $"{Environment.SystemDirectory}\\notepad.exe", "Edit osu!Launcher's hosts using Notepad.",
                $"{osuPath}\\osu.Launcher-Data", $"{Environment.SystemDirectory}\\notepad.exe");

            Console.WriteLine("Done! You can now run the \"Launch osu!\" shortcut on your desktop to launch osu! using osu!Launcher or");
            Console.WriteLine("you can edit osu!Launcher's hosts file by running the \"Edit osu!Launcher's hosts\" shortcut on your desktop!");

            Console.ReadKey();
        }

        private static string GetOsuPath()
        {
            using (RegistryKey osuRegistry = Registry.ClassesRoot.OpenSubKey("osu\\DefaultIcon"))
            {
                if (osuRegistry != null)
                {
                    string osuKey = osuRegistry.GetValue(null).ToString();
                    string osuPath = string.Empty;

                    osuPath = osuKey.Remove(0, 1);
                    osuPath = osuPath.Remove(osuPath.Length - 12);

                    return osuPath;
                }
            }

            throw new Exception("Could not obtain path to osu! installation, please make sure that you have run osu! at least once.");
        }

        public static void CreateDesktopShortcut(string shortcutName, string arguments,
            string targetPath, string description,
            string workingDirectory, string iconLocation)
        {
            WshShell wsh = new WshShell();
            IWshShortcut shortcut = wsh.CreateShortcut($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{shortcutName}.lnk") as IWshShortcut;

            shortcut.Arguments = arguments;
            shortcut.TargetPath = targetPath;
            shortcut.WindowStyle = 1;
            shortcut.Description = description;
            shortcut.WorkingDirectory = workingDirectory;
            shortcut.IconLocation = iconLocation;

            shortcut.Save();
        }
    }
}
