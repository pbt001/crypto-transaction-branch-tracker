using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CryptoBranchTracker.WFA.Classes
{
    public class Branch
    {
        public DateTime? DateCreated { get; set; }

        public Guid Identifier = new Guid();

        public string Notes { get; set; } = "";

        public string GetDelimitedValue()
        {
            string value;

            try
            {
                Dictionary<string, object> dictPairs = new Dictionary<string, object>()
                {
                    { Strings.BranchKeys.BRANCH_NOTES, Globals.Compress(this.Notes) },
                    { Strings.BranchKeys.BRANCH_CREATED, this.DateCreated.HasValue ? this.DateCreated.Value.Ticks : (object)Strings.NULL_VALUE },
                };

                //Initial value
                value = $"{Strings.BranchKeys.BRANCH_IDENTIFIER}{Strings.VALUE_DELIMITER}{this.Identifier}";

                foreach (KeyValuePair<string, object> kvPair in dictPairs)
                    value += $"{Strings.PAIR_DELIMITER}{kvPair.Key}{Strings.VALUE_DELIMITER}{kvPair.Value}";
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred retrieving the delimited value for the branch: {ex}");
            }

            return value;
        }

        private void PopulateProperties(string base64Value)
        {
            try
            {
                string decompressedString = Globals.Decompress(base64Value);

                Dictionary<string, string> dictDelimitedValues = decompressedString.Split('|').
                    Select(pair => pair.Split(';')).ToDictionary(key => key[0], value => value[1]);

                //Identifier
                KeyValuePair<string, string>? identifierPair = dictDelimitedValues.
                    Where(x => x.Key == Strings.BranchKeys.BRANCH_IDENTIFIER).FirstOrDefault();

                if (identifierPair.HasValue)
                    this.Identifier = new Guid(identifierPair.Value.Value);

                //Notes
                KeyValuePair<string, string>? notesPair = dictDelimitedValues.
                    Where(x => x.Key == Strings.BranchKeys.BRANCH_NOTES).FirstOrDefault();

                if (notesPair.HasValue)
                    this.Notes = Globals.Decompress(notesPair.Value.Value);

                //Date Created
                KeyValuePair<string, string>? createdPair = dictDelimitedValues.
                    Where(x => x.Key == Strings.BranchKeys.BRANCH_CREATED).FirstOrDefault();

                if (createdPair.HasValue)
                {
                    string createdValue = createdPair.Value.Value;

                    if (createdValue != Strings.NULL_VALUE)
                        this.DateCreated = new DateTime(Convert.ToInt64(createdValue));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred populating branch values: {ex}");
            }
        }

        public static List<Branch> GetAllLocalBranches()
        {
            List<Branch> lstBranches = new List<Branch>();

            try
            {
                Globals.FixRegistry();

                RegistryView platformView = Environment.Is64BitOperatingSystem
                    ? RegistryView.Registry64
                    : RegistryView.Registry32;

                using (RegistryKey registryBase = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, platformView))
                {
                    if (registryBase != null)
                    {
                        using (RegistryKey applicationKey = registryBase.CreateSubKey(Strings.RegistryLocations.APPLICATION_LOCATION))
                        {
                            using (RegistryKey branchList = applicationKey.CreateSubKey(Strings.RegistryLocations.BRANCH_LIST))
                            {
                                lstBranches = branchList.GetValueNames().
                                    Select(valueName => new Branch(branchList.GetValue(valueName).ToString())).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred getting local branches: {ex}");
            }

            return lstBranches;
        }

        public void Save()
        {
            try
            {
                //If this is the first time the branch is being saved, create a new identifier for it
                this.Identifier = this.Identifier == Guid.Empty
                    ? Guid.NewGuid()
                    : this.Identifier;

                string saveValue = Globals.Compress(this.GetDelimitedValue());

                Globals.FixRegistry();

                RegistryView platformView = Environment.Is64BitOperatingSystem
                    ? RegistryView.Registry64
                    : RegistryView.Registry32;

                using (RegistryKey registryBase = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, platformView))
                {
                    if (registryBase != null)
                    {
                        using (RegistryKey applicationKey = registryBase.CreateSubKey(Strings.RegistryLocations.APPLICATION_LOCATION))
                        {
                            using (RegistryKey branchList = applicationKey.CreateSubKey(Strings.RegistryLocations.BRANCH_LIST))
                                branchList.SetValue($"{this.Identifier}", saveValue, RegistryValueKind.String);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred saving the branch: {ex}");
            }
        }

        public Branch() { }

        public Branch(string base64Value)
        {
            try
            {
                this.PopulateProperties(base64Value);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred initializing the branch: {ex}");
            }
        }
    }
}
