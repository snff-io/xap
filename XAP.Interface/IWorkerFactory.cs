using System.Collections.Generic;

namespace XAP.Interface
{
    public interface IWorkerFactory
    {
        IAlertAction CreateAlertActionInstance(string typeName, Dictionary<string, string> parameters);

        AlertInstance CreateAlertInstance(string reporterName);

        void FixupAlertInstance(AlertInstance alertInstance);

        void ParseFiredTime(AlertInstance alertInstance);

        T LoadInstanceFromAssembly<T>(SimpleTypeDefinition typeDef)
            where T : class;
    }
}
