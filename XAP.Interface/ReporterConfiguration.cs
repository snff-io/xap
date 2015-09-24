using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAP.Interface
{
    public class ReporterConfiguration
    {
        public string AssemblyName { get; set; }
        public string TypeName { get; set; }
        public Dictionary<string, string> Params { get; set; }

    }
}
