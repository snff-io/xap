using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XAP.Interface;

namespace XAP.Reporters
{
    class RandomReporter:IReporter
    {
        public Guid Id
        {
            get { return new Guid("56B6FDAD-7A64-45E0-90CB-6D1FEEAA2DAB"); }
        }

        public string FriendlyName
        {
            get
            {
                return "Random Reporter";
            }
        }

        public IEnumerable<AlertInstance> GetAlerts(string checkpointToken, out string newCheckpointToken)
        {
            newCheckpointToken = checkpointToken;

            return new AlertInstance[0];
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


        public ReporterType ReporterType
        {
            get { throw new NotImplementedException(); }
        }

        public void Dispose()
        {
            
        }

        public void SetParameters(Dictionary<string, string> parameters)
        {
   
        }
    }
}
