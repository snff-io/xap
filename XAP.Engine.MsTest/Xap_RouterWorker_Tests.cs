using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using XAP.Engine.Router;
using XAP.Interface;
using XAP.Interface.Fakes;

namespace XAP.Engine.MsTest
{
    [TestClass]
    public class Xap_RouterWorker_Tests
    {

        [TestMethod]
        public void Xap_RouteManager_RouteInstance()
        {
            var worker = new RouterWorker();


            int alertActionPerformAction = 0;
            StubIAlertAction alertAction1 = new StubIAlertAction
            {
                PerformActionAlertInstanceIContext = (alert, context) =>
                    {
                        alertActionPerformAction++;
                        return AlertActionResult.OkContinue;
                    }
            };

            StubIRouteManager routeManager = new StubIRouteManager
            {
                MatchAlertInstance = (alert) =>
                {
                    var matchResponse = new RouterResponse
                    {
                        RouteManyRoutes = new List<Route>
                        {
                            new Route
                            {
                                Name = "andRoute1",
                                Actions = new List<ActionConfiguration>
                                {
                                    new ActionConfiguration
                                    {
                                        Type = "alertAction",
                                        Params = null
                                    }
                                }
                            },
                            new Route
                            {
                                Name = "andRoute2",
                                Actions = new List<ActionConfiguration>
                                {
                                    new ActionConfiguration
                                    {
                                        Type = "alertAction",
                                        Params = null
                                    },

                                    new ActionConfiguration
                                    {
                                        Type = "alertAction",
                                        Params = null
                                    }
                                }
                            }
                        },

                        RouteOneRoute = new Route
                        {
                            Name = "orRoute1",
                            Actions = new List<ActionConfiguration>
                                {
                                    new ActionConfiguration
                                    {
                                        Type = "alertAction",
                                        Params = null
                                    },
                                    new ActionConfiguration
                                    {
                                        Type = "alertAction",
                                        Params = null
                                    },
                                    new ActionConfiguration
                                    {
                                        Type = "alertAction",
                                        Params = null
                                    },
                                }
                        }
                    };
                    return matchResponse;
                }
            };

            StubIWorkerFactory factory = new StubIWorkerFactory
            {
                CreateAlertActionInstanceStringDictionaryOfStringString = (type, param) =>
                    {
                        return alertAction1;
                    }
            };

            Context ctx = new Context
            {
                RouteManager = routeManager,
                Factory = factory
            };

            worker.Initialize(ctx);

            AlertInstance ai = new AlertInstance
            {
                XapId = Guid.Empty
            };

            worker.RouteInstance(ai);

            Assert.AreEqual(6, alertActionPerformAction);

        }

    }
}
