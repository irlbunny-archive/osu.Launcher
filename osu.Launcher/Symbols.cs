using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace osu.Launcher
{
    class Symbols
    {
        private static Dictionary<string, string> _symbols = new Dictionary<string, string>();

        #region Signatures

        // OsuMain_FullPath
        private static string[] fullPathSignature = {
            "ldsfld",
            "dup",
            "brtrue.s",
            "pop",
            "call",
            "dup",
            "stsfld",
            "ret"
        };

        // OsuMain_Filename
        private static string[] filenameSignature = {
            "call",
            "call",
            "ret"
        };

        // pWebRequest_set_Url
        private static string[] set_UrlSignature = {
            "ldarg.1",
            "ldc.i4",
            "call",
            "callvirt",
            "brtrue.s",
            "ldc.i4",
            "call",
            "ldarg.1",
            "ldc.i4",
            "call",
            "ldsfld",
            "callvirt",
            "call",
            "starg.s",
            "ldarg.0",
            "ldarg.1",
            "stfld",
            "ret"
        };

        // pWebRequest_checkCertificate
        private static string[] checkCertificateSignature = {
            "ldc.i4.1",
            "newarr",
            "stloc.0",
            "ldloc.0",
            "ldc.i4.0",
            "ldarg.0",
            "stelem.ref",
            "call",
            "call",
            "ldstr",
            "ldloc.0",
            "call",
            "ret"
        };

        #endregion

        /// <summary>
        /// Loads symbols.
        /// </summary>
        public static void Load()
        {
            if (File.Exists($"{BuildConstants.SymbolsDirectory}\\{Updater.OsuHash}"))
            {
                string[] lines = File.ReadAllLines($"{BuildConstants.SymbolsDirectory}\\{Updater.OsuHash}");
                for (int index = 0; index < lines.Length; index++)
                {
                    string line = lines[index].Trim();
                    string[] token = line.Split('|');
                    if (token[0].Length != 0)
                        _symbols[token[0]] = token[1];
                }
            }
            else
            {
                AssemblyDefinition osuAssembly = AssemblyDefinition.ReadAssembly(Program.OsuPath);

                int fullPathMatches = 0,
                    filenameMatches = 0,
                    set_UrlMatches = 0,
                    checkCertificateMatches = 0;

                foreach (TypeDefinition type in osuAssembly.MainModule.Types.Where(type => type.Name != "<Module>"))
                {
                    foreach (MethodDefinition method in type.Methods.Where(method => method.Body != null))
                    {
                        if (fullPathMatches < 2)
                        {
                            for (int index = 0; index < fullPathSignature.Length; index++)
                            {
                                if (method.Body.Instructions[index].OpCode.ToString() != fullPathSignature[index])
                                    goto fullPathExit;
                            }

                            _symbols["OsuMain"] = GetClassNameAndValueName(method.FullName)[0];

                            _symbols["OsuMain_FullPath"] = GetClassNameAndValueName(method.FullName)[1];

                            ++fullPathMatches;
                            continue;
                        }

                        fullPathExit:;

                        if (filenameMatches < 6)
                        {
                            for (int index = 0; index < filenameSignature.Length; index++)
                            {
                                if (method.Body.Instructions[index].OpCode.ToString() != filenameSignature[index])
                                    goto filenameExit;
                            }

                            string operand = method.Body.Instructions[1].Operand.ToString();
                            if (operand.Contains("GetFileName") && !operand.Contains("WithoutExtension"))
                            {
                                _symbols["OsuMain"] = GetClassNameAndValueName(method.FullName)[0];

                                _symbols["OsuMain_Filename"] = GetClassNameAndValueName(method.FullName)[1];
                            }

                            ++filenameMatches;
                            continue;
                        }

                        filenameExit:;

                        if (set_UrlMatches < 2)
                        {
                            for (int index = 0; index < set_UrlSignature.Length; index++)
                            {
                                if (method.Body.Instructions[index].OpCode.ToString() != set_UrlSignature[index])
                                    goto set_UrlExit;
                            }

                            _symbols["pWebRequest"] = GetClassNameAndValueName(method.FullName)[0];

                            string operand = method.Body.Instructions[16].Operand.ToString();

                            _symbols["pWebRequest_set_Url"] = GetClassNameAndValueName(method.FullName)[1];
                            _symbols["pWebRequest_url"] = GetClassNameAndValueName(operand)[1];

                            ++set_UrlMatches;
                            continue;
                        }

                        set_UrlExit:;

                        if (checkCertificateMatches < 2)
                        {
                            for (int index = 0; index < checkCertificateSignature.Length; index++)
                            {
                                if (method.Body.Instructions[index].OpCode.ToString() != checkCertificateSignature[index])
                                    goto checkCertificateExit;
                            }

                            _symbols["pWebRequest"] = GetClassNameAndValueName(method.FullName)[0];

                            _symbols["pWebRequest_checkCertificate"] = GetClassNameAndValueName(method.FullName)[1];

                            ++checkCertificateMatches;
                            continue;
                        }

                        checkCertificateExit:;
                    }
                }

                File.WriteAllLines($"{BuildConstants.SymbolsDirectory}\\{Updater.OsuHash}",
                    _symbols.Select(entry => $"{entry.Key}|{entry.Value}").ToArray());

                osuAssembly.Dispose();
            }
        }

        /// <summary>
        /// Gets a symbol for the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetSymbol(string key)
        {
            if (_symbols.TryGetValue(key, out string symbol))
                return symbol;
            else
                throw new Exception($"An error has occured while getting the symbol for \"{key}\", please yell at Cyuubi to fix this!");
        }

        private static string[] GetClassNameAndValueName(string value)
        {
            string[] splitValue = value.Split(' ')[1].Split(new string[] { "::" }, StringSplitOptions.None);

            int index = splitValue[splitValue.Length - 1].IndexOf("(");
            if (index > 0)
                splitValue[splitValue.Length - 1] = splitValue[splitValue.Length - 1].Substring(0, index);

            return splitValue;
        }
    }
}
