namespace XAP.Interface
{
    public interface IReporterPull:IReporter
    {
        ReporterResult GetAlerts(string checkpointToken, IContext context);

        string Key { get; }
    }
}
