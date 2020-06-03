﻿using System;
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

                //Branch Identifier
                KeyValuePair<string, string>? branchIdentifierPair = dictDelimitedValues.
                    Where(x => x.Key == Constants.TransactionKeys.TRANSACTION_BRANCH).FirstOrDefault();

                if (branchIdentifierPair.HasValue)
                    this.BranchIdentifier = new Guid(branchIdentifierPair.Value.Value);

                //Date Processed
                KeyValuePair<string, string>? dateProcessedPair = dictDelimitedValues.
                    Where(x => x.Key == Constants.TransactionKeys.TRANSACTION_DATE).FirstOrDefault();

                if (dateProcessedPair.HasValue)
                {
                    string dateValue = dateProcessedPair.Value.Value;

                    if (dateValue != Constants.NULL_VALUE)
                        this.DateProcessed = new DateTime(Convert.ToInt64(dateValue));
                }

                //Time Processed
                KeyValuePair<string, string>? timeProcessedPair = dictDelimitedValues.
                    Where(x => x.Key == Constants.TransactionKeys.TRANSACTION_TIME).FirstOrDefault();

                if (timeProcessedPair.HasValue)
                {
                    string timeValue = timeProcessedPair.Value.Value;

                    if (timeValue != Constants.NULL_VALUE)
                        this.TimeProcessed = new TimeSpan(Convert.ToInt64(timeValue));
                }

                //Fiat Difference
                KeyValuePair<string, string>? fiatDifferencePair = dictDelimitedValues.
                    Where(x => x.Key == Constants.TransactionKeys.TRANSACTION_FIAT).FirstOrDefault();

                if (fiatDifferencePair.HasValue)
                    this.FiatDifference = Convert.ToDouble(fiatDifferencePair.Value.Value);

                //Notes
                KeyValuePair<string, string>? notesPair = dictDelimitedValues.
                    Where(x => x.Key == Constants.TransactionKeys.TRANSACTION_NOTES).FirstOrDefault();

                if (notesPair.HasValue)
                    this.Notes = Globals.Decompress(notesPair.Value.Value);

                //Transaction Type
                KeyValuePair<string, string>? typePair = dictDelimitedValues.
                    Where(x => x.Key == Constants.TransactionKeys.TRANSACTION_TYPE).FirstOrDefault();

                if (typePair.HasValue)
                    this.TransactionType = (TransactionTypes)Enum.Parse(typeof(TransactionTypes), typePair.Value.Value);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred populating the transaction data with the base64 encoded date: {ex}");
            }
        }

        public string GetDelimitedValue()
        {
            string value;

            try
            {
                //TODO: Format this better, it looks horrible

                value = $"{Constants.TransactionKeys.TRANSACTION_BRANCH}{Constants.VALUE_DELIMITER}{this.BranchIdentifier}";
                value += $"{Constants.PAIR_DELIMITER}{Constants.TransactionKeys.TRANSACTION_FIAT}{Constants.VALUE_DELIMITER}{this.FiatDifference}";
                value += $"{Constants.PAIR_DELIMITER}{Constants.TransactionKeys.TRANSACTION_TYPE}{Constants.VALUE_DELIMITER}{this.TransactionType}";
                value += $"{Constants.PAIR_DELIMITER}{Constants.TransactionKeys.TRANSACTION_NOTES}{Constants.VALUE_DELIMITER}{Globals.Compress(this.Notes)}";

                value += this.DateProcessed.HasValue
                    ? $"{Constants.PAIR_DELIMITER}{Constants.TransactionKeys.TRANSACTION_DATE}{Constants.VALUE_DELIMITER}{this.DateProcessed.Value.Ticks}"
                    : $"{Constants.PAIR_DELIMITER}{Constants.TransactionKeys.TRANSACTION_DATE}{Constants.VALUE_DELIMITER}{Constants.NULL_VALUE}";

                value += this.TimeProcessed.HasValue
                    ? $"{Constants.PAIR_DELIMITER}{Constants.TransactionKeys.TRANSACTION_TIME}{Constants.VALUE_DELIMITER}{this.TimeProcessed.Value.Ticks}"
                    : $"{Constants.PAIR_DELIMITER}{Constants.TransactionKeys.TRANSACTION_TIME}{Constants.VALUE_DELIMITER}{Constants.NULL_VALUE}";
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred getting the transaction delimited value: {ex}");
            }

            return value;
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

