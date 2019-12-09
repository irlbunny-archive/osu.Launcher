using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace osu.Launcher
{
    class Hosts
    {
        private static Dictionary<string, string> _hosts = new Dictionary<string, string>();

        /// <summary>
        /// Loads hosts redirections.
        /// </summary>
        public static void Load()
        {
            // Default hosts redirections.
            _hosts["osu.ppy.sh"] = "ripple.moe";
            _hosts["c.ppy.sh"] = "c.ripple.moe";
            _hosts["ce.ppy.sh"] = "c.ripple.moe";

            for (int index = 1; index < 10; index++)
            {
                if (index != 2)
                    _hosts[$"c{index}.ppy.sh"] = "c.ripple.moe";
            }

            _hosts["a.ppy.sh"] = "a.ripple.moe";
            _hosts["i.ppy.sh"] = "i.ripple.moe";

            if (File.Exists($"{BuildConstants.LauncherDirectory}\\hosts"))
            {
                string[] lines = File.ReadAllLines($"{BuildConstants.LauncherDirectory}\\hosts");
                for (int index = 0; index < lines.Length; index++)
                {
                    string line = lines[index].Trim();
                    if (line.StartsWith("#"))
                        continue;
                    if (line.Contains("#"))
                        line = Regex.Split(line, "#")[0];

                    string[] token = line.Split(' ');
                    if (token[0].Length != 0)
                        _hosts[token[1]] = token[0];
                }
            }
        }

        /// <summary>
        /// Redirects URLs to a different host (if specified in hosts redirections).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Redirect(string value)
        {
            UriBuilder builder = new UriBuilder(value);
            if (_hosts.TryGetValue(builder.Host, out string host))
            {
                string[] token = host.Split(':');

                builder.Host = token[0];
                if (token.Length > 1)
                    builder.Port = Convert.ToInt32(token[1]);
            }

            return builder.Uri.ToString(); // TODO: This may cause issues?
        }
    }
}
