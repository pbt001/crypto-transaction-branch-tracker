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
    }
}
