using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using XAP.Engine.Router;
using XAP.Interface;
using XAP.Interface.Fakes;

namespace XAP.Engine.MsTest
{
    [TestClass]
    public class Xap_RouteManager_Tests
    {

        [TestMethod]
        public void Xap_RouteManager_Match()
        {

            var testAi = new AlertInstance();

            testAi.Properties.Add(new AlertProperty
            {
                Name = "Description",
                Value = "This is a test Alert"
            });

            testAi.Properties.Add(new AlertProperty
            {
                Name = "Priority",
                Value = "3"
            });

            var routeMangaer = new RouteManager();

            StubIConfiguration config = new StubIConfiguration
            {
                GetRouteManyRoutes = () =>
                    {
                        return new Route[] {
                            new Route
                            {
                                Name = "Match - Exact Desc, Range Pri",
                                Matches = new Match[] 
                                {
                                    new Match
                                    {
                                        Property = "Description",
                                        Inverse = false,
                                        RegEx = "^This is a test Alert$"
                                    },
                                    new Match
                                    {
                                        Property = "Priority",
                                        Inverse = false,
                                        RegEx = "[1-3]"
                                    }
                                }
                            },
                            new Route
                            {
                                Name = "Match - Exact Pri",
                                Matches = new Match[] 
                                {
                                    new Match
                                    {
                                        Property = "Priority",
                                        Inverse = false,
                                        RegEx = "3"
                                    }
                                }
                            },
                            new Route
                            {
                                Name = "NoMatch - Priority",
                                Matches = new Match[] 
                                {
                                    new Match
                                    {
                                        Property = "Description",
                                        Inverse = false,
                                        RegEx = "^This is a test Alert$"
                                    },
                                    new Match
                                    {
                                        Property = "Priority",
                                        Inverse = false,
                                        RegEx = "4"
                                    }
                                }
                            },
                            new Route
                            {
                                Name = "NoMatch - Inverse Match Description",
                                Matches = new Match[] 
                                {
                                    new Match
                                    {
                                        Property = "Description",
                                        Inverse = true,
                                        RegEx = "^This is a test Alert$"
                                    }
                                }
                            },
                            new Route
                            {
                                Name = "Match - Inverse Match Description",
                                Matches = new Match[] 
                                {
                                    new Match
                                    {
                                        Property = "Description",
                                        Inverse = true,
                                        RegEx = "^Aint This$"
                                    }
                                }
                            }

                        };
                    },
                GetRouteOneRoutes = () =>
                    {
                     return new Route[] {
                            new Route
                            {
                                Name = "Match - Exact Desc, Range Pri",
                                Matches = new Match[] 
                                {
                                    new Match
                                    {
                                        Property = "Description",
                                        Inverse = false,
                                        RegEx = "^This is a test Alert$"
                                    },
                                    new Match
                                    {
                                        Property = "Priority",
                                        Inverse = false,
                                        RegEx = "[1-3]"
                                    }
                                }
                            },
                            new Route
                            {
                                Name = "NoMatch - Priority Only",
                                Matches = new Match[] 
                                {
                                    new Match
                                    {
                                        Property = "Priority",
                                        Inverse = false,
                                        RegEx = "3"
                                    }
                                }
                            },
                            new Route
                            {
                                Name = "NoMatch - Priority",
                                Matches = new Match[] 
                                {
                                    new Match
                                    {
                                        Property = "Description",
                                        Inverse = false,
                                        RegEx = "^This is a test Alert$"
                                    },
                                    new Match
                                    {
                                        Property = "Priority",
                                        Inverse = false,
                                        RegEx = "4"
                                    }
                                }
                            }
                        };
                    }
            };

            var context = new Context
            {
                Configuration = config
            };

            routeMangaer.Initialize(context);

            var routerResponse = routeMangaer.Match(testAi);

            Assert.IsTrue(routerResponse.RouteOneRoute.Name == "Match - Exact Desc, Range Pri");
            Assert.IsTrue(routerResponse.RouteManyRoutes.Any(x => x.Name == "Match - Exact Desc, Range Pri"));
            Assert.IsTrue(routerResponse.RouteManyRoutes.Any(x => x.Name == "Match - Exact Pri"));
            Assert.IsTrue(routerResponse.RouteManyRoutes.Any(x => x.Name == "Match - Inverse Match Description"));
            Assert.IsFalse(routerResponse.RouteManyRoutes.Any(x => x.Name == "NoMatch - Priority"));
            Assert.IsFalse(routerResponse.RouteManyRoutes.Any(x => x.Name == "NoMatch - Inverse Match Description"));
            
            Assert.AreEqual(3, routerResponse.RouteManyRoutes.Count());
            

        }
    }
}
