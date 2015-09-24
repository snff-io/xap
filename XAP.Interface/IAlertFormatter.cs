namespace XAP.Interface
{
    public interface IAlertFormatter
    {
        void Format(AlertInstance alert, string serviceInventoryUriBase);
    }
}
