using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XAP.Engine.Reporter;
using XAP.Interface;
using XAP.Interface.Fakes;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;

namespace XAP.Engine.MsTest
{
    [TestClass]
    public class Xap_Engine_Worker_Tests
    {
        [TestMethod]
        [DeploymentItem("..\\..\\..\\XAP.BuiltIn\\bin\\Debug")]
        public void Xap_Engine_Worker_Factory_Load()
        {
            IConfiguration config = new TestConfiguration();

            var factory = new WorkerFactory(config);
            var alertQueue = factory.CreateInstance<IAlertQueue>();
            var checkPointManager = factory.CreateInstance<ILockManager>();

            Assert.IsNotNull(alertQueue);
            Assert.IsNotNull(checkPointManager);

            Assert.IsNotNull(factory.CreateAlertActionInstance("XAP.BuiltIn.WriteEventLogAction", null));
            Assert.IsNotNull(factory.CreateReporterInstance("XAP.BuiltIn.TestableReporter"));
        }

        [TestMethod]
        public void Xap_Engine_ReporterWorker_Run()
        {

            int alertsQueued = 0;
            int pullAlertsCalled = 0;
            int pushAlertsCalled = 0;

            var queuedAlerts = new List<AlertInstance>();

            IAlertQueue queue = new StubIAlertQueue
            {
                EnqueueAlertInstance = (alert) => { alertsQueued++; queuedAlerts.Add(alert); },
                EnqueueIEnumerableOfAlertInstance = (alerts) => { alertsQueued += alerts.Count(); queuedAlerts.AddRange(alerts); }
            };

            var pullReporterLocked = false;
            var pullReporter = new StubIReporterPull
            {
                GetAlertsStringIContext = (cin, context) =>
                    {
                        pullAlertsCalled++;

                        var res = new ReporterResult
                        {
                            NewCheckpoint = cin
                        };

                        res.Alerts.Add(new AlertInstance { Reporter = "pull" });
                        return res;
                    }
            };

            Action<IEnumerable<AlertInstance>> pushAction = null;

            var pushReporter = new StubIReporterPush
            {
                InitializeActionOfIEnumerableOfAlertInstanceIContext = (push, context) =>
                {
                    pushAction = push;
                }
            };

            var reporters = new IReporter[] 
            {
                pullReporter,
                pushReporter
            };

            string checkpoint = "checkpoint";
           

            var persistenceManager = new StubIPersistenceManager
            {
                PersistAlertAlertInstance = (a) =>
                    {
                    }
            };

            var worker = new ReporterWorker();

            var ctx = new Context 
            {
                AlertQueue = queue,
               
                Reporters = reporters,
                Persistence = persistenceManager
                
            };

            Task.Factory.StartNew(() => { worker.Run(ctx); });

            while (pushAction == null)
            {
                Thread.Sleep(10);
            }

            for (pushAlertsCalled = 0; pushAlertsCalled < 5; pushAlertsCalled++)
            {
                pushAction(new[] { new AlertInstance { Reporter = "push" } });
            }

            while (pullAlertsCalled < 5)
            {
                Thread.Sleep(10);
            }

            worker.BlockingStopAll(TimeSpan.FromSeconds(10));

            Assert.AreEqual(alertsQueued, pushAlertsCalled + pullAlertsCalled, "Queued Alert Count");
            Assert.AreEqual(queuedAlerts.Count(x => x.Reporter == "push"), 5, "Pushed Alerts");
            Assert.AreEqual(queuedAlerts.Count(x => x.Reporter == "pull"), 5, "Pulled Alerts");
            Assert.AreEqual(pullAlertsCalled.ToString(), checkpoint, "Checkpoint Set");
        }

        [TestMethod]
        public void Test_TimeParser_expected()
        {
            var ctx = new Context
            {
                Factory = new WorkerFactory(new TestConfiguration())
            };

            var date = DateTime.Now.ToString(CultureInfo.InvariantCulture);

            var ai = new AlertInstance();
            ai.AddProperty("FiredTime", date);

            ctx.Factory.ParseFiredTime(ai);

            Assert.AreEqual(date, ai["FiredTime"].Value);
        }

        [TestMethod]
        public void Test_TimeParser()
        {
            var ctx = new Context
            {
                Factory = new WorkerFactory(new TestConfiguration())
            };

            var date = "baddate";

            var ai = new AlertInstance();
            ai.AddProperty("FiredTime", date);

            ctx.Factory.ParseFiredTime(ai);

            Assert.AreNotEqual(date, ai["FiredTime"].Value);
        }
    }
}
