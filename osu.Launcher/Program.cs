using System;
using System.IO;

/*
 * TODOs:
 * - Implement symbols saving.
 */

namespace osu.Launcher
{
    class Program
    {
        /// <summary>
        /// Contains the path to the osu! executable.
        /// </summary>
        public static string OsuPath = Path.Combine(Directory.GetCurrentDirectory(), "osu!.exe");

        /// <summary>
        /// Contains the path to the osu! configuration.
        /// </summary>
        public static string OsuConfigurationPath = Path.Combine(Directory.GetCurrentDirectory(), "osu!.cfg");

        [STAThread]
        static void Main(string[] args)
        {
            // Create directories if they do not exist already.
            if (!Directory.Exists(BuildConstants.LauncherDirectory))
                Directory.CreateDirectory(BuildConstants.LauncherDirectory);
            if (!Directory.Exists(BuildConstants.SymbolsDirectory))
                Directory.CreateDirectory(BuildConstants.SymbolsDirectory);

            Updater.Run(); // Check if there are any pending osu! updates.

            // Check if osu! executable exists.
            if (!File.Exists(OsuPath))
            {
                Console.WriteLine("osu!Launcher could not find \"osu!.exe\", please make sure it exists in your working directory!");
                Console.ReadKey();
                return;
            }

            try
            {
                Symbols.Load();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"It looks like your osu! version is unsupported by osu!Launcher, sorry! Message = {exception.Message}");
                Console.ReadKey();
                return;
            }

            Patcher.Load();
            Patcher.Start();
        }
    }
}
