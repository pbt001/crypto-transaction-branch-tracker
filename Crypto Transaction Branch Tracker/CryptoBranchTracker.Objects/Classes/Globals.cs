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

        /// <summary>
        /// Get the list of transactions in a branch that can be edited
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        public static JArray GetTransactionArray(JObject branch)
        {
            JArray arrTransactions = null;

            try
            {
                if (branch != null)
                {
                    JProperty propTransactions = branch.Children<JProperty>().
                        Where(x => x.Name == Strings.JSONStrings.BRANCH_TRANSACTIONS).FirstOrDefault();

                    if (propTransactions != null)
                        arrTransactions = propTransactions.Children<JArray>().FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred retrieving transaction array: {ex}");
            }

            return arrTransactions;
        }

        /// <summary>
        /// Dynamically generate the default JSON data based on stored constant values
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultJSONData()
        {
            string data;

            try
            {
                //Main data wrap
                JObject objDataWrap = new JObject();

                //Branches
                JProperty propBranches = new JProperty(Strings.JSONStrings.BRANCHES_OBJECT, new JArray());

                //Settings
                JObject objSettings = new JObject()
                {
                    new JProperty(Strings.SettingsNames.DARK_MODE, false),
                    new JProperty(Strings.SettingsNames.AUTO_MAXIMIZE, false),
                    new JProperty(Strings.SettingsNames.SCHEME_A, Constants.DEFAULT_SCHEME_A),
                    new JProperty(Strings.SettingsNames.SCHEME_R, Constants.DEFAULT_SCHEME_R),
                    new JProperty(Strings.SettingsNames.SCHEME_G, Constants.DEFAULT_SCHEME_G),
                    new JProperty(Strings.SettingsNames.SCHEME_B, Constants.DEFAULT_SCHEME_B)
                };

                JProperty propSettings = new JProperty(Strings.JSONStrings.SETTINGS_OBJECT, objSettings);

                objDataWrap.Add(propBranches);
                objDataWrap.Add(propSettings);

                data = objDataWrap.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred getting default JSON data: {ex}");
            }

            return data;
        }

        /// <summary>
        /// Get the JSON data associated with a specific transaction
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="branchIdentifier"></param>
        /// <returns></returns>
        public static JObject GetRawTransactionData(Guid identifier, Guid branchIdentifier)
        {
            JObject obj = null;

            try
            {
                JObject tarBranch = Globals.GetRawBranchData(branchIdentifier);

                if (tarBranch != null)
                {
                    JEnumerable<JObject> enTransactions = Globals.GetTransactionArray(tarBranch).Children<JObject>();

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

        /// <summary>
        /// Get the JSON data associated with a specific branch
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public static JObject GetRawBranchData(Guid identifier)
        {
            JObject obj = null;

            try
            {
                JEnumerable<JObject> enBranches = GetBranchArray().Children<JObject>();

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

        /// <summary>
        /// Get the list of branches in the base data file that can be edited
        /// </summary>
        /// <returns></returns>
        public static JArray GetBranchArray()
        {
            JArray arrBranches = null;

            try
            {
                using (StreamReader reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), $"{Strings.JSONStrings.FILE_NAME}.json")))
                {
                    JObject mainData = (JObject)JToken.ReadFrom(new JsonTextReader(reader));

                    JProperty propBranches = mainData.Children<JProperty>().
                        Where(x => x.Name == Strings.JSONStrings.BRANCHES_OBJECT).FirstOrDefault();

                    if (propBranches != null)
                        arrBranches = propBranches.Values<JArray>().FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred getting branch array: {ex}");
            }

            return arrBranches;
        }

        public static JObject GetRawSettingsData()
        {
            JObject objSettings = null;

            try
            {
                using (StreamReader reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), $"{Strings.JSONStrings.FILE_NAME}.json")))
                {
                    JObject mainData = (JObject)JToken.ReadFrom(new JsonTextReader(reader));

                    JProperty propSettings = mainData.Children<JProperty>().
                        Where(x => x.Name == Strings.JSONStrings.SETTINGS_OBJECT).FirstOrDefault();

                    if (propSettings != null)
                        objSettings = propSettings.Values<JObject>().FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred getting raw settings data: {ex}");
            }

            return objSettings;
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
                        byte[] arrData = ASCIIEncoding.ASCII.GetBytes(Globals.GetDefaultJSONData());
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
