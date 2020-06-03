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
        public Guid Identifier = new Guid();

        public string Notes { get; set; } = "";

        public string GetDelimitedValue()
        {
            string value;

            try
            {
                value = $"{Strings.BranchKeys.BRANCH_IDENTIFIER}{Strings.VALUE_DELIMITER}{this.Identifier}";
                value += $"{Strings.PAIR_DELIMITER}{Strings.BranchKeys.BRANCH_NOTES}{Strings.VALUE_DELIMITER}{Globals.Compress(this.Notes)}";
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
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred populating branch values: {ex}");
            }
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
