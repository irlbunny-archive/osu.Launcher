using System;
using System.Reflection;
using System.Security.Permissions;

namespace osu.Launcher
{
    class Patcher
    {
        /// <summary>
        /// Contains the osu! assembly.
        /// </summary>
        public static Assembly OsuAssembly;

        /// <summary>
        /// Loads and patches osu!.
        /// </summary>
        public static void Load()
        {
            OsuAssembly = Assembly.LoadFile(Program.OsuPath);

            try
            {
                // Fetch classes
                OsuMain._OsuMain = OsuAssembly.GetType(Symbols.GetSymbol("OsuMain"));
                pWebRequest._pWebRequest = OsuAssembly.GetType(Symbols.GetSymbol("pWebRequest"));

                // Fetch OsuMain methods
                OsuMain.OsuMain_FullPath = OsuMain._OsuMain.GetMethod(Symbols.GetSymbol("OsuMain_FullPath"), BindingFlags.Static | BindingFlags.NonPublic);
                OsuMain.OsuMain_FullPath_patched = typeof(OsuMain).GetMethod("FullPath");

                OsuMain.OsuMain_Filename = OsuMain._OsuMain.GetMethod(Symbols.GetSymbol("OsuMain_Filename"), BindingFlags.Static | BindingFlags.NonPublic);
                OsuMain.OsuMain_Filename_patched = typeof(OsuMain).GetMethod("Filename");

                // Fetch pWebRequest methods
                pWebRequest.pWebRequest_set_Url = pWebRequest._pWebRequest.GetMethod(Symbols.GetSymbol("pWebRequest_set_Url"), BindingFlags.Instance | BindingFlags.NonPublic);
                pWebRequest.pWebRequest_set_Url_patched = typeof(pWebRequest).GetMethod("set_Url");

                pWebRequest.pWebRequest_checkCertificate = pWebRequest._pWebRequest.GetMethod(Symbols.GetSymbol("pWebRequest_checkCertificate"), BindingFlags.Instance | BindingFlags.NonPublic);
                pWebRequest.pWebRequest_checkCertificate_patched = typeof(pWebRequest).GetMethod("checkCertificate");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"It looks like your osu! version is unsupported by osu!Launcher, sorry! Message = {exception.Message}");
                Console.ReadKey();
                return;
            }

            // Unsafe because it requires some pointer hacks.
            unsafe
            {
                try
                {
                    // Patch executable checks.
                    Console.Write("Patching executable checks... ");

                    int* p_OsuMain_FullPath = (int*)OsuMain.OsuMain_FullPath.MethodHandle.Value.ToPointer() + 2;
                    int* p_OsuMain_FullPath_patched = (int*)OsuMain.OsuMain_FullPath_patched.MethodHandle.Value.ToPointer() + 2;

                    *p_OsuMain_FullPath = *p_OsuMain_FullPath_patched;

                    int* p_OsuMain_Filename = (int*)OsuMain.OsuMain_Filename.MethodHandle.Value.ToPointer() + 2;
                    int* p_OsuMain_Filename_patched = (int*)OsuMain.OsuMain_Filename_patched.MethodHandle.Value.ToPointer() + 2;

                    *p_OsuMain_Filename = *p_OsuMain_Filename_patched;

                    Console.WriteLine("OK!");

                    // Patch URL & certificate checks.
                    Console.Write("Patching URL & certificate checks... ");

                    int* p_pWebRequest_set_Url = (int*)pWebRequest.pWebRequest_set_Url.MethodHandle.Value.ToPointer() + 2;
                    int* p_pWebRequest_set_Url_patched = (int*)pWebRequest.pWebRequest_set_Url_patched.MethodHandle.Value.ToPointer() + 2;

                    *p_pWebRequest_set_Url = *p_pWebRequest_set_Url_patched;

                    int* p_pWebRequest_checkCertificate = (int*)pWebRequest.pWebRequest_checkCertificate.MethodHandle.Value.ToPointer() + 2;
                    int* p_pWebRequest_checkCertificate_patched = (int*)pWebRequest.pWebRequest_checkCertificate_patched.MethodHandle.Value.ToPointer() + 2;

                    *p_pWebRequest_checkCertificate = *p_pWebRequest_checkCertificate_patched;

                    Console.WriteLine("OK!");
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"\nIt looks like your osu! version is unsupported by osu!Launcher, sorry! Message = {exception.Message}");
                    Console.ReadKey();
                    return;
                }
            }
        }

        /// <summary>
        /// Starts osu!.
        /// </summary>
        public static void Start()
        {
            new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess).Assert();
            OsuAssembly.EntryPoint.Invoke(null, null);
        }
    }
}
