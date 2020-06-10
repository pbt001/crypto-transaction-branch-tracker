using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptoBranchTracker.Objects.Classes
{
    internal class Globals
    {
        [Obsolete]
        /// <summary>
        /// Sort out the registry directory, fix any tampering that may have been done
        /// </summary>
        public static void FixRegistry()
        {
            try
            {
                RegistryView platformView = Environment.Is64BitOperatingSystem
                    ? RegistryView.Registry64
                    : RegistryView.Registry32;

                using (RegistryKey registryBase = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, platformView))
                {
                    if (registryBase != null)
                    {
                        using (RegistryKey applicationKey = registryBase.CreateSubKey(Strings.RegistryLocations.APPLICATION_LOCATION))
                        {
                            applicationKey.CreateSubKey(Strings.RegistryLocations.BRANCH_LIST).Close();
                            applicationKey.CreateSubKey(Strings.RegistryLocations.TRANSACTION_LIST).Close();
                            applicationKey.CreateSubKey(Strings.RegistryLocations.SETTINGS_VALUES).Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred fixing the registry: {ex}");
            }
        }

        //I know it's usually bad to have a method with one line, but this actually saves a bunch of time
        public static void UpdateDataFile(string data)
        {
            try
            {
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), $"{Strings.JSONStrings.FILE_NAME}.json"), data);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred updating data file: {ex}");
            }
        }

        public static JEnumerable<JObject> GetTransactionList(JObject branch)
        {
            JEnumerable<JObject> enTransactions = new JEnumerable<JObject>();

            try
            {
                if (branch != null)
                {
                    JProperty propTransactions = branch.Children<JProperty>().
                        Where(x => x.Name == Strings.JSONStrings.BRANCH_TRANSACTIONS).FirstOrDefault();

                    if (propTransactions != null)
                    {
                        JArray arrTransactions = propTransactions.Children<JArray>().FirstOrDefault();

                        if (arrTransactions != null)
                        {
                            enTransactions = arrTransactions.Children<JObject>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred getting transaction list: {ex}");
            }

            return enTransactions;
        }

        public static JObject GetRawTransactionData(Guid identifier, Guid branchIdentifier)
        {
            JObject obj = null;

            try
            {
                JObject tarBranch = Globals.GetRawBranchData(branchIdentifier);

                if (tarBranch != null)
                {
                    JEnumerable<JObject> enTransactions = Globals.GetTransactionList(tarBranch);

                    foreach (JObject transaction in enTransactions)
                    {
                        JProperty propIdentifier = transaction.Children<JProperty>().
                            Where(x => x.Name == Strings.JSONStrings.IDENTIFIER).FirstOrDefault();

                        if (propIdentifier != null && propIdentifier.Value.ToString() == identifier.ToString())
                            return transaction;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred getting raw transaction data: {ex}");
            }

            return obj;
        }

        public static JObject GetRawBranchData(Guid identifier)
        {
            JObject obj = null;

            try
            {
                JEnumerable<JObject> enBranches = Globals.GetBranchList();

                foreach (JObject branch in enBranches)
                {
                    JProperty propIdentifier = branch.Children<JProperty>().
                        Where(x => x.Name == Strings.JSONStrings.IDENTIFIER).FirstOrDefault();

                    if (propIdentifier != null && propIdentifier.Value.ToString() == identifier.ToString())
                        return branch;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred getting raw branch data: {ex}");
            }

            return obj;
        }

        public static JEnumerable<JObject> GetBranchList()
        {
            JEnumerable<JObject> enBranches = new JEnumerable<JObject>();

            try
            {
                using (StreamReader reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), $"{Strings.JSONStrings.FILE_NAME}.json")))
                {
                    JObject mainData = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
                    JProperty propBranches = mainData.Children<JProperty>().FirstOrDefault();

                    if (propBranches != null)
                    {
                        JArray arrBranches = propBranches.Values<JArray>().FirstOrDefault();

                        if (arrBranches != null)
                            enBranches = arrBranches.Children<JObject>();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred getting branches: {ex}");
            }

            return enBranches;
        }

        public static void FixJSONFile()
        {
            try
            {
                string strExpectedLocation = Path.Combine(Directory.GetCurrentDirectory(), $"{Strings.JSONStrings.FILE_NAME}.json");

                if (!File.Exists(strExpectedLocation))
                {
                    using (FileStream stream = File.Create(strExpectedLocation))
                    {
                        byte[] arrData = ASCIIEncoding.ASCII.GetBytes(Strings.JSONStrings.DEFAULT_DATA);
                        stream.Write(arrData, 0, arrData.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred fixing JSON file: {ex}");
            }
        }

        /// <summary>
        /// Compress a string down to a base64 format
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Compress(string text)
        {
            string compressedString;

            try
            {
                byte[] compressedBytes;

                using (var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
                {
                    using (var compressedStream = new MemoryStream())
                    {
                        using (var compressorStream = new DeflateStream(compressedStream, CompressionLevel.Fastest, true))
                            uncompressedStream.CopyTo(compressorStream);

                        compressedBytes = compressedStream.ToArray();
                    }
                }

                compressedString = Convert.ToBase64String(compressedBytes);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred compressing string: {ex}");
            }

            return compressedString;
        }

        /// <summary>
        /// Decompress a string from the Compress method back to the readable string
        /// </summary>
        /// <param name="compressedString"></param>
        /// <returns></returns>
        public static string Decompress(string compressedString)
        {
            string decompressedString;

            try
            {
                byte[] decompressedBytes;

                var compressedStream = new MemoryStream(Convert.FromBase64String(compressedString));

                using (var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
                {
                    using (var decompressedStream = new MemoryStream())
                    {
                        decompressorStream.CopyTo(decompressedStream);

                        decompressedBytes = decompressedStream.ToArray();
                    }
                }

                decompressedString = Encoding.UTF8.GetString(decompressedBytes);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred decompressing string: {ex}");
            }

            return decompressedString;
        }
    }
}
