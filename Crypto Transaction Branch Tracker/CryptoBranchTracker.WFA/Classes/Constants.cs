using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoBranchTracker.WFA.Classes
{
    public class Constants
    {
        public const string NULL_VALUE = "NULL";

        public struct TransactionKeys
        {
            public const string TRANSACTION_BRANCH = "TRANSACTION_BRANCH";

            public const string TRANSACTION_DATE = "TRANSACTION_DATE";

            public const string TRANSACTION_TIME = "TRANSACTION_TIME";

            public const string TRANSACTION_TYPE = "TRANSACTION_TYPE";

            public const string TRANSACTION_FIAT = "TRANSACTION_FIAT";

            public const string TRANSACTION_NOTES = "TRANSACTION_NOTES";
        }

        public struct BranchKeys
        {
            public const string BRANCH_IDENTIFIER = "BRANCH_IDENTIFIER";

            public const string BRANCH_NOTES = "BRANCH_NOTES";
        }
    }
}
