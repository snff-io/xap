using System;
using System.Collections.Generic;
using XAP.Interface;

namespace XAP.BuiltIn
{
    public class TestableReporter : IReporterPull
    {
        public static List<TestableReporter> Instances
        {
            get;
            set;
        }


        public static Func<string, IContext, ReporterResult> GetAlertFunc
        {
            get;
            set;
        }

        public string Key { get { return "TestableReporter:00001"; } }

        static TestableReporter()
        {
            Instances = new List<TestableReporter>();
            GetAlertFunc = (checkpointToken, context) =>
                {
                    int cp = 0;

                    if (checkpointToken == null || !int.TryParse(checkpointToken, out cp))
                    {
                        cp = 0;
                    }

                    cp++;

                    var alertBucket = new List<AlertInstance>();

                    AlertInstance alertInstance = context.Factory.CreateAlertInstance("TestableReporter");
                    alertInstance.Reporter = "TesableReporter";
                    alertInstance["priority"].Value = "3";
                    alertInstance["title"].Value = "Test Alert " + cp.ToString();
                    alertInstance["description"].Value = cp.ToString();

                    alertBucket.Add(alertInstance);

                    var result = new ReporterResult
                    {
                        NewCheckpoint = cp.ToString(),
                        Alerts = alertBucket
                    };

                    return result;
                };
        }

        public TestableReporter()
        {
            Instances.Add(this);
        }

        public void SetParameters(Dictionary<string, string> parameters)
        {

        }


        public ReporterResult GetAlerts(string checkpointToken, IContext context)
        {
            return GetAlertFunc(checkpointToken, context);
        }

        public Guid Id
        {
            get { return Guid.Empty; }
        }

        public string FriendlyName
        {
            get { return "TestableReporter"; }
        }

        public ReporterType ReporterType
        {
            get { return ReporterType.Pull; }
        }

        public IEnumerable<AlertInstance> GetAllAlertDefinitions()
        {
            throw new NotImplementedException();
        }

        public AlertInstance GetAlertDefinition(Guid definitionId)
        {
            throw new NotImplementedException();
        }

        public bool IsAlertActive(AlertInstance alert)
        {
            throw new NotImplementedException();
        }

        public bool IsHealthy()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
