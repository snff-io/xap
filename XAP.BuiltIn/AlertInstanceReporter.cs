using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using XAP.Interface;

namespace XAP.BuiltIn
{
    [ServiceContract]
    public interface IAlertInstanceReporter
    {
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Xml)]
        AlertInstance Post(AlertInstance alert);
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class AlertInstanceReporter : IReporterPush, IAlertInstanceReporter
    {
        Action<IEnumerable<AlertInstance>> _pushAlertMethod;
        private WebServiceHost _host;
        IContext _context;

        public void Initialize(Action<IEnumerable<AlertInstance>> pushAlertMethod, IContext context)
        {
            _context = context;
            _pushAlertMethod = pushAlertMethod;
            StartHost();
        }

        public void SetParameters(Dictionary<string,string> parameters)
        {

        }

        public ServiceHost StartHost()
        {
            if (_host != null)
            {
                _host.Close();
            }

            var serviceUri = new Uri("http://localhost:80/xap/");

            _host = new WebServiceHost(this, serviceUri);
            _host.AddServiceEndpoint(typeof(IAlertInstanceReporter), new WebHttpBinding(WebHttpSecurityMode.None), "");
            _host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            _host.Description.Behaviors.Find<ServiceDebugBehavior>().IncludeExceptionDetailInFaults = true;
            _host.Description.Behaviors.Find<ServiceDebugBehavior>().HttpHelpPageUrl = serviceUri;

            _host.Open();

            return _host;
        }

        public void StopHost()
        {
            _host.Close();
        }

        public AlertInstance Post(AlertInstance alert)
        {
            if (string.IsNullOrEmpty(alert.Reporter))
            {
                alert.Reporter = "XAPWAPI";
            }

            _context.Factory.FixupAlertInstance(alert);
            _pushAlertMethod(new[] { alert });
            return alert;
        }

        public Guid Id
        {
            get { return new Guid("C355037C-5F01-4C24-AA45-BA37DB067C82"); }
        }

        public string FriendlyName
        {
            get { return "Alert Instance Reporter"; }
        }

        public ReporterType ReporterType
        {
            get { return ReporterType.Push; }
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
                _host.Close();
        }
    }
}
