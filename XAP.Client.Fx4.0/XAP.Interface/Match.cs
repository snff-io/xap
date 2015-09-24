using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAP.Interface
{
    public class Match
    {
        public string Property { get; set; }

        public string RegEx { get; set; }

        public bool Inverse { get; set; }

    }
}
