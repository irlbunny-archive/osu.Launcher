using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace osu.Launcher
{
    class Updater
    {
        /// <summary>
        /// Contains the directory to pending osu! updates.
        /// </summary>
        private static string _osuPendingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "_pending");

        /// <summary>
        /// Contains the osu! executable hash.
        /// </summary>
        public static string OsuHash = string.Empty;

        public static void Run()
        {
            if (Directory.Exists(_osuPendingDirectory))
            {
                List<string> osuConfiguration = new List<string>();
                if (File.Exists(Program.OsuConfigurationPath))
                    osuConfiguration = new List<string>(File.ReadAllLines(Program.OsuConfigurationPath));

                string[] pending = Directory.GetFiles(_osuPendingDirectory);
                foreach (string filePath in pending)
                {
                    string filename = Path.GetFileName(filePath);

                    File.Delete(filename);

                    bool hashExists = false;
                    for (int index = 0; index < osuConfiguration.Count; index++)
                    {
                        if (osuConfiguration[index].StartsWith($"h_{filename} = "))
                        {
                            osuConfiguration[index] = $"h_{filename} = {GetFileHash(filePath)}";
                            hashExists = true;
                        }
                    }

                    if (!hashExists)
                        osuConfiguration.Add($"h_{filename} = {GetFileHash(filePath)}");

                    while (true)
                    {
                        if (!File.Exists(Path.GetFileName(filePath)))
                        {
                            File.Move(filePath, filename);
                            break;
                        }
                    }
                }

                Directory.Delete(_osuPendingDirectory);

                File.WriteAllLines(Program.OsuConfigurationPath, osuConfiguration);
            }

            if (File.Exists(Program.OsuPath))
                OsuHash = GetFileHash(Program.OsuPath);
        }

        private static string GetFileHash(string filePath)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            md5.ComputeHash(fileStream);
            fileStream.Close();

            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < md5.Hash.Length; index++)
                stringBuilder.Append(md5.Hash[index].ToString("x2"));

            return stringBuilder.ToString().ToLowerInvariant();
        }
    }
}
