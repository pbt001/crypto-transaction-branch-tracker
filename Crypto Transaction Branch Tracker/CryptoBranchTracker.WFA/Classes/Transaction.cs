using Microsoft.Win32;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CryptoBranchTracker.WFA.Classes
{
    public class Transaction
    {
        public Guid Identifier { get; set; } = new Guid();

        public Guid BranchIdentifier { get; set; } = new Guid();

        public DateTime? DateProcessed { get; set; }

        public TimeSpan? TimeProcessed { get; set; }

        public double FiatDifference { get; set; } = 0;

        public string Notes { get; set; } = "";

        public TransactionTypes TransactionType { get; set; } = TransactionTypes.BUY;

        public enum TransactionTypes
        {
            BUY,
            SELL
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
                    Where(x => x.Key == Strings.TransactionKeys.TRANSACTION_IDENTIFIER).FirstOrDefault();

                if (identifierPair.HasValue)
                    this.Identifier = new Guid(identifierPair.Value.Value);

                //Branch Identifier
                KeyValuePair<string, string>? branchIdentifierPair = dictDelimitedValues.
                    Where(x => x.Key == Strings.TransactionKeys.TRANSACTION_BRANCH).FirstOrDefault();

                if (branchIdentifierPair.HasValue)
                    this.BranchIdentifier = new Guid(branchIdentifierPair.Value.Value);

                //Date Processed
                KeyValuePair<string, string>? dateProcessedPair = dictDelimitedValues.
                    Where(x => x.Key == Strings.TransactionKeys.TRANSACTION_DATE).FirstOrDefault();

                if (dateProcessedPair.HasValue)
                {
                    string dateValue = dateProcessedPair.Value.Value;

                    if (dateValue != Strings.NULL_VALUE)
                        this.DateProcessed = new DateTime(Convert.ToInt64(dateValue));
                }

                //Time Processed
                KeyValuePair<string, string>? timeProcessedPair = dictDelimitedValues.
                    Where(x => x.Key == Strings.TransactionKeys.TRANSACTION_TIME).FirstOrDefault();

                if (timeProcessedPair.HasValue)
                {
                    string timeValue = timeProcessedPair.Value.Value;

                    if (timeValue != Strings.NULL_VALUE)
                        this.TimeProcessed = new TimeSpan(Convert.ToInt64(timeValue));
                }

                //Fiat Difference
                KeyValuePair<string, string>? fiatDifferencePair = dictDelimitedValues.
                    Where(x => x.Key == Strings.TransactionKeys.TRANSACTION_FIAT).FirstOrDefault();

                if (fiatDifferencePair.HasValue)
                    this.FiatDifference = Convert.ToDouble(fiatDifferencePair.Value.Value);

                //Notes
                KeyValuePair<string, string>? notesPair = dictDelimitedValues.
                    Where(x => x.Key == Strings.TransactionKeys.TRANSACTION_NOTES).FirstOrDefault();

                if (notesPair.HasValue)
                    this.Notes = Globals.Decompress(notesPair.Value.Value);

                //Transaction Type
                KeyValuePair<string, string>? typePair = dictDelimitedValues.
                    Where(x => x.Key == Strings.TransactionKeys.TRANSACTION_TYPE).FirstOrDefault();

                if (typePair.HasValue)
                    this.TransactionType = (TransactionTypes)Enum.Parse(typeof(TransactionTypes), typePair.Value.Value);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred populating the transaction data with the base64 encoded date: {ex}");
            }
        }

        public void Save()
        {
            try
            {
                //If this is the first time the transaction is being saved, create a new identifier for it
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
                            using (RegistryKey branchList = applicationKey.CreateSubKey(Strings.RegistryLocations.TRANSACTION_LIST))
                                branchList.SetValue($"{this.Identifier}", saveValue, RegistryValueKind.String);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred saving transaction: {ex}");
            }
        }

        public string GetDelimitedValue()
        {
            string value;

            try
            {
                //TODO: Format this better, it looks horrible

                value = $"{Strings.TransactionKeys.TRANSACTION_BRANCH}{Strings.VALUE_DELIMITER}{this.BranchIdentifier}";
                value += $"{Strings.PAIR_DELIMITER}{Strings.TransactionKeys.TRANSACTION_IDENTIFIER}{Strings.VALUE_DELIMITER}{this.Identifier}";
                value += $"{Strings.PAIR_DELIMITER}{Strings.TransactionKeys.TRANSACTION_FIAT}{Strings.VALUE_DELIMITER}{this.FiatDifference}";
                value += $"{Strings.PAIR_DELIMITER}{Strings.TransactionKeys.TRANSACTION_TYPE}{Strings.VALUE_DELIMITER}{this.TransactionType}";
                value += $"{Strings.PAIR_DELIMITER}{Strings.TransactionKeys.TRANSACTION_NOTES}{Strings.VALUE_DELIMITER}{Globals.Compress(this.Notes)}";

                value += this.DateProcessed.HasValue
                    ? $"{Strings.PAIR_DELIMITER}{Strings.TransactionKeys.TRANSACTION_DATE}{Strings.VALUE_DELIMITER}{this.DateProcessed.Value.Ticks}"
                    : $"{Strings.PAIR_DELIMITER}{Strings.TransactionKeys.TRANSACTION_DATE}{Strings.VALUE_DELIMITER}{Strings.NULL_VALUE}";

                value += this.TimeProcessed.HasValue
                    ? $"{Strings.PAIR_DELIMITER}{Strings.TransactionKeys.TRANSACTION_TIME}{Strings.VALUE_DELIMITER}{this.TimeProcessed.Value.Ticks}"
                    : $"{Strings.PAIR_DELIMITER}{Strings.TransactionKeys.TRANSACTION_TIME}{Strings.VALUE_DELIMITER}{Strings.NULL_VALUE}";
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred getting the transaction delimited value: {ex}");
            }

            return value;
        }

        public static List<Transaction> GetAllLocalTransactions()
        {
            List<Transaction> lstTransactions = new List<Transaction>();

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
                            using (RegistryKey transactionList = applicationKey.CreateSubKey(Strings.RegistryLocations.TRANSACTION_LIST))
                            {
                                lstTransactions = transactionList.GetValueNames().
                                    Select(valueName => new Transaction(transactionList.GetValue(valueName).ToString())).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred getting all local transactions: {ex}");
            }

            return lstTransactions;
        }

        public Transaction() { }

        public Transaction(string base64Value)
        {
            try
            {
                this.PopulateProperties(base64Value);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred creating transaction instance: {ex}");
            }
        }
    }
}

