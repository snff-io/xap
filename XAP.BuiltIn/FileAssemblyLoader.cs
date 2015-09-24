using System;
using System.Linq;
using System.Reflection;
using XAP.Interface;

namespace XAP.BuiltIn
{
    public class FileAssemblyLoader:IAssemblyLoader
    {
        public Type GetTypeFromAssembly(SimpleTypeDefinition typeDefinition)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.Split(',').First() == typeDefinition.AssemblyName);

            if (assembly == null)
            {
                assembly = Assembly.Load(typeDefinition.AssemblyName);
            }

            var type = assembly.GetType(typeDefinition.TypeName);

            return type;
        }
    }
}
