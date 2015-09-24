using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAP.Interface
{
    public interface IAssemblyLoader
    {
        Type GetTypeFromAssembly(SimpleTypeDefinition typeDefinition);

    }

    public class SimpleTypeDefinition
    {
        public string AssemblyName { get; set; }
        public string TypeName { get; set; }
    }
}
