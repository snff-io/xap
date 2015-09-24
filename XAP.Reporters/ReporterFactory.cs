using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XAP.Interface;


namespace XAP.Reporters
{
    public static class ReporterFactory
    {
        public static List<Type> ActiveReporterTypes
        {
            get;
            private set;
        }

        static ReporterFactory()
        {
            ActiveReporterTypes = new List<Type>();
            ActiveReporterTypes.Add(typeof(RandomReporter));
        }

        public static IEnumerable<IReporter> CreateReporters()
        {
            var reporters = new List<IReporter>();

            foreach (var type in ActiveReporterTypes)
            {
                var reporter = Assembly.GetCallingAssembly().CreateInstance(type.FullName) as IReporter;

                if (reporter != null)
                {
                    reporters.Add(reporter);
                }
            }

            return reporters;
        }
    }
}
