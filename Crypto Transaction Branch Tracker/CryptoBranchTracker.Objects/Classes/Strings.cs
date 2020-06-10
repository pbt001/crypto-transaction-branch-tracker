using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoBranchTracker.Objects.Classes
{
    internal class Strings
    {
        public const string NULL_VALUE = "NULL";

        public const string PAIR_DELIMITER = "|";

        public const string VALUE_DELIMITER = ";";

        public struct JSONStrings
        {
            public const string FILE_NAME = "data";

            public const string DEFAULT_DATA = "{ \"Branches\": [	], }";

            public const string BRANCH_DATA = "DATA";

            public const string TRANSACTION_DATA = "TRANSACTION_DATA";

            public const string BRANCH_TRANSACTIONS = "TRANSACTIONS";

            public const string IDENTIFIER = "IDENTIFIER";
        }

        public struct SettingsNames
        {
            public const string DARK_MODE = "DARK_MODE";

            public const string SCHEME_A = "SCHEME_A";

            public const string SCHEME_R = "SCHEME_R";

            public const string SCHEME_G = "SCHEME_G";

            public const string SCHEME_B = "SCHEME_B";
        }

        public struct TransactionKeys
        {
            public const string TRANSACTION_SOURCE = "TRANSACTION_SOURCE";

            public const string TRANSACTION_DESTINATION = "TRANSACTION_DESTINATION";

            public const string TRANSACTION_IDENTIFIER = "TRANSACTION_IDENTIFIER";

            public const string TRANSACTION_BRANCH = "TRANSACTION_BRANCH";

            public const string TRANSACTION_DATE = "TRANSACTION_DATE";

            public const string TRANSACTION_TIME = "TRANSACTION_TIME";

            public const string TRANSACTION_TYPE = "TRANSACTION_TYPE";

            public const string TRANSACTION_FIAT = "TRANSACTION_FIAT";
        }

        public struct BranchKeys
        {
            public const string BRANCH_CRYPTO = "BRANCH_CRYPTO";

            public const string BRANCH_CREATED = "BRANCH_CREATED";

            public const string BRANCH_CREATED_TIME = "BRANCH_CREATED_TIME";

            public const string BRANCH_IDENTIFIER = "BRANCH_IDENTIFIER";
        }

        public struct RegistryLocations
        {
            public const string APPLICATION_LOCATION = @"Software\Crypto Branch Tracker";

            public const string TRANSACTION_LIST = "Transactions";

            public const string BRANCH_LIST = "Branches";

            public const string SETTINGS_VALUES = "Settings";
        }
    }
}
