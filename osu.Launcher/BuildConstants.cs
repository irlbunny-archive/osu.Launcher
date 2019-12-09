namespace osu.Launcher
{
    public class BuildConstants
    {
        /// <summary>
        /// Used to store launcher specific data. (e.g. Symbols, Configurations)
        /// </summary>
        internal const string LauncherDirectory = "osu.Launcher-Data";
        /// <summary>
        /// Used to store precompiled symbols so that the launcher does not need to scan for symbols at every launch.
        /// </summary>
        internal const string SymbolsDirectory = LauncherDirectory + "\\Symbols";
    }
}
