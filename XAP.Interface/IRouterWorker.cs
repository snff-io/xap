namespace XAP.Interface
{
    public interface IRouterWorker
    {
        void Initialize(IContext context);
        void Run();
        void Stop();
    }
}
