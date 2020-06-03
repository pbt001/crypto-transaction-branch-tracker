using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                value = $"{Constants.BranchKeys.BRANCH_IDENTIFIER}{Constants.VALUE_DELIMITER}{this.Identifier}";
                value += $"{Constants.PAIR_DELIMITER}{Constants.BranchKeys.BRANCH_NOTES}{Constants.VALUE_DELIMITER}{Globals.Compress(this.Notes)}";
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
                    Where(x => x.Key == Constants.BranchKeys.BRANCH_IDENTIFIER).FirstOrDefault();

                if (identifierPair.HasValue)
                    this.Identifier = new Guid(identifierPair.Value.Value);

                //Notes
                KeyValuePair<string, string>? notesPair = dictDelimitedValues.
                    Where(x => x.Key == Constants.BranchKeys.BRANCH_NOTES).FirstOrDefault();

                if (notesPair.HasValue)
                    this.Notes = Globals.Decompress(notesPair.Value.Value);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred populating branch values: {ex}");
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
