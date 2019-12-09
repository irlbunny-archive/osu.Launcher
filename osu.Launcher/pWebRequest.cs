using System;
using System.IO;
using System.Reflection;

namespace osu.Launcher
{
    class pWebRequest
    {
        public static Type _pWebRequest = null;

        public static MethodInfo pWebRequest_set_Url = null;
        public static MethodInfo pWebRequest_set_Url_patched = null;

        public void set_Url(string value)
        {
            Hosts.Load();

            FieldInfo pWebRequest_url = _pWebRequest.GetField(Symbols.GetSymbol("pWebRequest_url"), BindingFlags.Instance | BindingFlags.NonPublic);

            value = Hosts.Redirect(value);
            if (File.Exists($"{BuildConstants.LauncherDirectory}\\ForceHTTP"))
                value = value.Replace("https://", "http://");

            pWebRequest_url.SetValue(this, value);
        }

        public static MethodInfo pWebRequest_checkCertificate = null;
        public static MethodInfo pWebRequest_checkCertificate_patched = null;

        /// <summary>
        /// Stubbed.
        /// </summary>
        public void checkCertificate()
        { }
    }
}
