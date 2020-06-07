using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoBranchTracker.Objects.Classes
{
    public class Transaction
    {
        public Guid Identifier { get; set; } = new Guid();

        public Guid BranchIdentifier { get; set; } = new Guid();

        public DateTime? DateProcessed { get; set; }

        public TimeSpan? TimeProcessed { get; set; }

        public double FiatDifference { get; set; } = 0;

        public TransactionTypes TransactionType { get; set; } = TransactionTypes.DEPOSIT;

        public LocationTypes Source { get; set; } = LocationTypes.FIAT;

        public LocationTypes Destination { get; set; } = LocationTypes.CRYPTO;

        public enum LocationTypes
        {
            FIAT,
            BANK,
            CRYPTO,
            WALLET
        }

        public enum TransactionTypes
        {
            DEPOSIT,
            WITHDRAWAL,
            TRADE,
            TRANSFER
        }

        //Get the text to be displayed for this transaction based on its type and value
        public string GetBasicDisplayText()
        {
            try
            {
                return $"{this.Source.ToString().FirstCharToUpper()} -> {this.Destination.ToString().FirstCharToUpper()}";
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred getting display text: {ex}");
            }
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

                //Transaction Type
                KeyValuePair<string, string>? typePair = dictDelimitedValues.
                    Where(x => x.Key == Strings.TransactionKeys.TRANSACTION_TYPE).FirstOrDefault();

                if (typePair.HasValue)
                    this.TransactionType = (TransactionTypes)Enum.Parse(typeof(TransactionTypes), typePair.Value.Value);

                //Source
                KeyValuePair<string, string>? sourcePair = dictDelimitedValues.
                    Where(x => x.Key == Strings.TransactionKeys.TRANSACTION_SOURCE).FirstOrDefault();

                if (sourcePair.HasValue)
                    this.Source = (LocationTypes)Enum.Parse(typeof(LocationTypes), sourcePair.Value.Value);

                //Destination
                KeyValuePair<string, string>? destinationPair = dictDelimitedValues.
                    Where(x => x.Key == Strings.TransactionKeys.TRANSACTION_DESTINATION).FirstOrDefault();

                if (destinationPair.HasValue)
                    this.Destination = (LocationTypes)Enum.Parse(typeof(LocationTypes), destinationPair.Value.Value);
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
                Dictionary<string, object> dictPairs = new Dictionary<string, object>()
                {
                    { Strings.TransactionKeys.TRANSACTION_BRANCH, this.BranchIdentifier },
                    { Strings.TransactionKeys.TRANSACTION_DATE, this.DateProcessed.HasValue ? this.DateProcessed.Value.Ticks : (object)Strings.NULL_VALUE },
                    { Strings.TransactionKeys.TRANSACTION_TIME, this.TimeProcessed.HasValue ? this.TimeProcessed.Value.Ticks : (object)Strings.NULL_VALUE },
                    { Strings.TransactionKeys.TRANSACTION_FIAT, this.FiatDifference },
                    { Strings.TransactionKeys.TRANSACTION_TYPE, this.TransactionType },
                    { Strings.TransactionKeys.TRANSACTION_SOURCE, this.Source },
                    { Strings.TransactionKeys.TRANSACTION_DESTINATION, this.Destination }
                };

                //Initial value
                value = $"{Strings.TransactionKeys.TRANSACTION_IDENTIFIER}{Strings.VALUE_DELIMITER}{this.Identifier}";

                foreach (KeyValuePair<string, object> kvPair in dictPairs)
                    value += $"{Strings.PAIR_DELIMITER}{kvPair.Key}{Strings.VALUE_DELIMITER}{kvPair.Value}";
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
