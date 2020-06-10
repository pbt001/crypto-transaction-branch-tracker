using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media.Animation;

namespace CryptoBranchTracker.Objects.Classes
{
    public class Branch
    {
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

        public DateTime? DateCreated { get; set; }

        public TimeSpan? TimeCreated { get; set; }

        public Guid Identifier = new Guid();

        public string Cryptocurrency { get; set; } = "";

        public string GetDelimitedValue()
        {
            string value;

            try
            {
                Dictionary<string, object> dictPairs = new Dictionary<string, object>()
                {
                    { Strings.BranchKeys.BRANCH_CREATED, this.DateCreated.HasValue ? this.DateCreated.Value.Ticks : (object)Strings.NULL_VALUE },
                    { Strings.BranchKeys.BRANCH_CRYPTO, this.Cryptocurrency },
                    { Strings.BranchKeys.BRANCH_CREATED_TIME, this.TimeCreated.HasValue ? this.TimeCreated.Value.Ticks : (object)Strings.NULL_VALUE }
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

                //Crypto
                KeyValuePair<string, string>? cryptoPair = dictDelimitedValues.
                    Where(x => x.Key == Strings.BranchKeys.BRANCH_CRYPTO).FirstOrDefault();

                if (cryptoPair.HasValue)
                    this.Cryptocurrency = cryptoPair.Value.Value;

                //Date Created
                KeyValuePair<string, string>? createdPair = dictDelimitedValues.
                    Where(x => x.Key == Strings.BranchKeys.BRANCH_CREATED).FirstOrDefault();

                if (createdPair.HasValue)
                {
                    string createdValue = createdPair.Value.Value;

                    if (createdValue != Strings.NULL_VALUE)
                        this.DateCreated = new DateTime(Convert.ToInt64(createdValue));
                }

                //Time Created
                KeyValuePair<string, string>? timePair = dictDelimitedValues.
                    Where(x => x.Key == Strings.BranchKeys.BRANCH_CREATED_TIME).FirstOrDefault();

                if (timePair.HasValue)
                {
                    string timeValue = timePair.Value.Value;

                    if (timeValue != Strings.NULL_VALUE)
                        this.TimeCreated = new TimeSpan(Convert.ToInt64(timeValue));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred populating branch values: {ex}");
            }
        }

        public void ReloadTransactions()
        {
            try
            {
                this.Transactions.Clear();
                JObject objBranch = Globals.GetRawBranchData(this.Identifier);

                if (objBranch != null)
                {
                    JEnumerable<JObject> enTransactions = Globals.GetTransactionArray(objBranch).Children<JObject>();

                    foreach (JObject transactionData in enTransactions)
                    {
                        JProperty propData = transactionData.Children<JProperty>().
                            Where(x => x.Name == Strings.JSONStrings.TRANSACTION_DATA).FirstOrDefault();

                        if (propData != null)
                            this.Transactions.Add(new Transaction(propData.Value.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred reloading transactions: {ex}");
            }
        }

        public static List<Branch> GetAllLocalBranches()
        {
            List<Branch> lstBranches = new List<Branch>();

            try
            {
                Globals.FixJSONFile();

                try
                {
                    JEnumerable<JObject> enBranches = Globals.GetBranchArray().Children<JObject>();

                    foreach (JObject branchData in enBranches)
                    {
                        JProperty branchCode = branchData.Children<JProperty>().
                            Where(x => x.Name == Strings.JSONStrings.BRANCH_DATA).FirstOrDefault();

                        if (branchCode != null)
                        {
                            Branch branch = new Branch(branchCode.Value.ToString());

                            JEnumerable<JObject> enTransactions = Globals.GetTransactionArray(branchData).Children<JObject>();

                            foreach (JObject transactionData in enTransactions)
                            {
                                JProperty propData = transactionData.Children<JProperty>().
                                    Where(x => x.Name == Strings.JSONStrings.TRANSACTION_DATA).FirstOrDefault();
                                    
                                if (propData != null)
                                    branch.Transactions.Add(new Transaction(propData.Value.ToString()));
                            }

                            lstBranches.Add(branch);
                        }
                    }
                }
                catch (Exception) { }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred getting local branches: {ex}");
            }

            return lstBranches;
        }

        public void Delete()
        {
            try
            {
                Globals.FixJSONFile();

                JObject tarBranch = Globals.GetRawBranchData(this.Identifier);

                if (tarBranch != null)
                {
                    JArray arrParent = tarBranch.Parent as JArray;
                    arrParent.Remove(tarBranch);

                    Globals.UpdateDataFile(arrParent.Root.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred deleting branch: {ex}");
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

                Globals.FixJSONFile();

                JObject objBranch = Globals.GetRawBranchData(this.Identifier);

                if (objBranch != null)
                {
                    JProperty propData = objBranch.Children<JProperty>().
                        Where(x => x.Name == Strings.JSONStrings.BRANCH_DATA).FirstOrDefault();

                    if (propData != null)
                    {
                        propData.Value = saveValue;
                        Globals.UpdateDataFile(propData.Root.ToString());
                    }
                }
                else
                {
                    JArray arrBranches = Globals.GetBranchArray();

                    JObject objNewBranch = new JObject
                    {
                        new JProperty(Strings.JSONStrings.IDENTIFIER, this.Identifier.ToString()),
                        new JProperty(Strings.JSONStrings.BRANCH_DATA, Globals.Compress(this.GetDelimitedValue())),
                        new JProperty(Strings.JSONStrings.BRANCH_TRANSACTIONS, new JArray())
                    };

                    arrBranches.Add(objNewBranch);

                    Globals.UpdateDataFile(objNewBranch.Root.ToString());
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
