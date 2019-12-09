using System;
using System.Reflection;

namespace osu.Launcher
{
    class OsuMain
    {
        public static Type _OsuMain = null;

        public static MethodInfo OsuMain_FullPath = null;
        public static MethodInfo OsuMain_FullPath_patched = null;

        public static string FullPath() => Program.OsuPath;

        public static MethodInfo OsuMain_Filename = null;
        public static MethodInfo OsuMain_Filename_patched = null;

        public static string Filename() => "osu!.exe";
    }
}
