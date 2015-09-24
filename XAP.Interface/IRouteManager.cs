namespace XAP.Interface
{
    public interface IRouteManager
    {
        void Initialize(IContext context);
        RouterResponse Match(AlertInstance alert);
    }
}
