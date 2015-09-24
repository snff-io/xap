using System;

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
