using System;
using System.Collections.Generic;
using XAP.Interface;

namespace XAP.Engine.MsTest
{
    public class TestConfiguration:IConfiguration
    {

        public List<AlertProperty> GetAlertProperties()
        {
            throw new NotImplementedException();
        }

        public SimpleTypeDefinition GetAssemblyLoaderType()
        {
            return new SimpleTypeDefinition
            {
                AssemblyName = "XAP.BuiltIn",
                TypeName = "XAP.BuiltIn.FileAssemblyLoader"
            };
        }

        public SimpleTypeDefinition GetAlertQueueType()
        {
            return new SimpleTypeDefinition
            {
                AssemblyName = "XAP.BuiltIn",
                TypeName = "XAP.BuiltIn.MemoryQueue"
            };
        }

        public SimpleTypeDefinition GetCheckpointManagerType()
        {
            return new SimpleTypeDefinition
            {
                AssemblyName = "XAP.BuiltIn",
                TypeName = "XAP.BuiltIn.MemoryCheckpointManager"
            }; ;
        }

        public string WorkerMode
        {
            get 
            {
                return "reporter, router";
            }
        }

        string IConfiguration.WorkerMode
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<SimpleTypeDefinition> GetActionTypes()
        {
            return new List<SimpleTypeDefinition>()
            {
                new SimpleTypeDefinition
                {
                    AssemblyName = "XAP.BuiltIn",
                    TypeName = "XAP.BuiltIn.WriteEventLogAction"
                }
            };
        }

        public IEnumerable<ReporterConfiguration> GetReporterTypes()
        {
            return new List<ReporterConfiguration>()
            {
                new ReporterConfiguration
                {
                    AssemblyName = "XAP.BuiltIn",
                    TypeName = "XAP.BuiltIn.TestableReporter"
                }
            };
        }


        public IEnumerable<Route> GetRouteManyRoutes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Route> GetRouteOneRoutes()
        {
            throw new NotImplementedException();
        }


        public SimpleTypeDefinition GetPersistenceManagerType()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ScheduledActionConfiguration> GetScheduledActions()
        {
            throw new NotImplementedException();
        }

        public void SetAlertProperties(List<AlertProperty> properties)
        {
            throw new NotImplementedException();
        }

        public void SetAssemblyLoaderType(SimpleTypeDefinition type)
        {
            throw new NotImplementedException();
        }

        public void SetPersistenceManagerType(SimpleTypeDefinition type)
        {
            throw new NotImplementedException();
        }

        public void SetAlertQueueType(SimpleTypeDefinition type)
        {
            throw new NotImplementedException();
        }

        public void SetCheckpointManagerType(SimpleTypeDefinition type)
        {
            throw new NotImplementedException();
        }

        public void SetActionTypes(IEnumerable<SimpleTypeDefinition> actionTypes)
        {
            throw new NotImplementedException();
        }

        public void SetReporterTypes(IEnumerable<ReporterConfiguration> reporters)
        {
            throw new NotImplementedException();
        }

        public void SetRouteManyRoutes(IEnumerable<Route> routes)
        {
            throw new NotImplementedException();
        }

        public void SetRouteOneRoutes(IEnumerable<Route> routes)
        {
            throw new NotImplementedException();
        }

        public void SetScheduledActions(IEnumerable<ScheduledActionConfiguration> actions)
        {
            throw new NotImplementedException();
        }
    }
}
