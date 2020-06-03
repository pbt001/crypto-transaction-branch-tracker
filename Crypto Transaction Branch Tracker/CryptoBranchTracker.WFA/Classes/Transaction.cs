using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
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

        public void PopulateProperties(string base64Value)
        {
            try
            {
                string decompressedString = Globals.Decompress(base64Value);
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
                value = $"{Constants.TransactionKeys.TRANSACTION_BRANCH};{this.BranchIdentifier}";
                value += $"|{Constants.TransactionKeys.TRANSACTION_FIAT};{this.FiatDifference}";
                value += $"|{Constants.TransactionKeys.TRANSACTION_TYPE};{this.TransactionType}";
                value += $"|{Constants.TransactionKeys.TRANSACTION_NOTES};{Globals.Compress(this.Notes)}";

                value += this.DateProcessed.HasValue
                    ? $"|{Constants.TransactionKeys.TRANSACTION_DATE};{this.DateProcessed.Value.Ticks}"
                    : $"|{Constants.TransactionKeys.TRANSACTION_DATE};NULL";

                value += this.TimeProcessed.HasValue
                    ? $"|{Constants.TransactionKeys.TRANSACTION_TIME};{this.TimeProcessed.Value.Ticks}"
                    : $"|{Constants.TransactionKeys.TRANSACTION_TIME};NULL";
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred getting the transaction delimited value: {ex}");
            }

            return value;
        }

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
