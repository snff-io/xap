using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAP.Interface
{
    public interface IWorkerFactory
    {
        IAlertAction CreateAlertActionInstance(string typeName, Dictionary<string, string> parameters);

        AlertInstance CreateAlertInstance(string reporterName);

        void FixupAlertInstance(AlertInstance alertInstance);

        T LoadInstanceFromAssembly<T>(SimpleTypeDefinition typeDef)
            where T : class;
    }
}
