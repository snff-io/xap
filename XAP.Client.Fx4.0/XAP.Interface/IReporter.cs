using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAP.Interface
{
    public interface IReporter:IDisposable
    {
        /// <summary>
        /// alert Reporter Identifier
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Name of this reporter.
        /// </summary>
        string FriendlyName { get; }

        /// <summary>
        /// Type (push/pull) of this reporter
        /// </summary>
        ReporterType ReporterType { get; }

        /// <summary>
        /// Get all defined alerts in reporting system.
        /// </summary>
        /// <returns>A list of alert definitions.</returns>
        IEnumerable<AlertInstance> GetAllAlertDefinitions();
       
        /// <summary>
        /// Get a single defined alert in reporting system.
        /// </summary>
        /// <param name="definitionId"></param>
        /// <returns>An alert definition</returns>
        AlertInstance GetAlertDefinition(Guid definitionId);

        /// <summary>
        /// Is the alert still active.
        /// </summary>
        /// <param name="definitionId"></param>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        bool IsAlertActive(AlertInstance alert);

        /// <summary>
        /// Check the health of this alert reporter
        /// </summary>
        /// <returns></returns>
        bool IsHealthy();
    }

    public enum ReporterType
    {
        Push,
        Pull
    }
}
