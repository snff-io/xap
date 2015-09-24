using System;
using System.Collections.Generic;
using System.Linq;
using XAP.Interface;

namespace XAP.BuiltIn
{

    public sealed class FileConfiguration : IConfiguration
    {
        private readonly XapSection _config;

        public FileConfiguration()
        {
            _config = ((XapSection)(global::System.Configuration.ConfigurationManager.GetSection("xap")));
        }

        public List<AlertProperty> GetAlertProperties()
        {
            var alertProperties = new List<AlertProperty>();

            foreach (XapSection.AlertPropertiesElementCollection.AddElement configProperty in _config.AlertProperties)
            {
                alertProperties.Add(new AlertProperty
                {
                    Name = configProperty.Name,
                    Required = configProperty.Required,
                    Validation = configProperty.Validation,
                    Value = configProperty.Value ?? String.Empty
                });
            }

            return alertProperties;
        }

        public SimpleTypeDefinition GetAssemblyLoaderType()
        {
            var typeDef = new SimpleTypeDefinition
            {
                AssemblyName = _config.AssemblyLoader.Assembly,
                TypeName = _config.AssemblyLoader.Type
            };

            return typeDef;
        }


        public SimpleTypeDefinition GetAlertQueueType()
        {
            var typeDef = new SimpleTypeDefinition
            {
                AssemblyName = _config.Queue.Assembly,
                TypeName = _config.Queue.Type
            };

            return typeDef;
        }
        public SimpleTypeDefinition GetCheckpointManagerType()
        {
            var typeDef = new SimpleTypeDefinition
            {
                AssemblyName = _config.CheckpointManager.Assembly,
                TypeName = _config.CheckpointManager.Type
            };

            return typeDef;
        }

        public SimpleTypeDefinition GetPersistenceManagerType()
        {
            var typeDef = new SimpleTypeDefinition
            {
                AssemblyName = _config.PersistenceManager.Assembly,
                TypeName = _config.PersistenceManager.Type
            };

            return typeDef;
        }

        public IEnumerable<SimpleTypeDefinition> GetActionTypes()
        {
            var ret = new List<SimpleTypeDefinition>();

            foreach (XapSection.ActionTypesElementCollection.AddElement action in _config.ActionTypes)
            {
                if (action.Type.Contains(','))
                {
                    foreach (var type in action.Type.Split(','))
                    {
                        ret.Add(new SimpleTypeDefinition
                        {
                            AssemblyName = action.Assembly,
                            TypeName = type.Trim()
                        });
                    }
                }
                else
                {
                    ret.Add(new SimpleTypeDefinition
                    {
                        AssemblyName = action.Assembly,
                        TypeName = action.Type
                    });
                }
            }

            return ret;
        }

        public IEnumerable<ReporterConfiguration> GetReporterTypes()
        {
            var ret = new List<ReporterConfiguration>();

            foreach (XapSection.ReporterTypesElementCollection.AddElement reporter in _config.ReporterTypes)
            {
                var rc = new ReporterConfiguration
                {
                    AssemblyName = reporter.Assembly,
                    TypeName = reporter.Type,
                    Params = new Dictionary<string,string>()
                };

                foreach (XapSection.ReporterTypesElementCollection.AddElement.ParametersElementCollection.AddElement p in reporter.Parameters)
                {
                    rc.Params[p.Name] = p.Value;
                }


                ret.Add(rc);
            }

            return ret;
        }

        public IEnumerable<Route> GetRouteManyRoutes()
        {
            var ret = new List<Route>();

            foreach (XapSection.RouteManyElementCollection.RouteElement
                r in _config.RouteMany)
            {
                var route = new Route { Name = r.Name };

                foreach (XapSection.RouteManyElementCollection.RouteElement.MatchesElementCollection.MatchElement
                    m in r.Matches)
                {
                    route.Matches.Add(new Match
                    {
                        Inverse = m.Inverse,
                        Property = m.Property,
                        RegEx = m.Regex
                    });
                }

                foreach (XapSection.RouteManyElementCollection.RouteElement.ActionsElementCollection.ActionElement a in r.Actions)
                {
                    var action = new ActionConfiguration { Type = a.Type };

                    foreach (XapSection.RouteManyElementCollection.RouteElement.ActionsElementCollection.ActionElement.ParametersElementCollection.AddElement
                        p in a.Parameters)
                    {
                        action.Params[p.Name] = p.Value;
                    }

                    route.Actions.Add(action);
                }

                ret.Add(route);
            }

            return ret;
        }

        public IEnumerable<Route> GetRouteOneRoutes()
        {
            var ret = new List<Route>();

            foreach (XapSection.RouteOneElementCollection.RouteElement
                r in _config.RouteOne)
            {
                var route = new Route { Name = r.Name };

                foreach (XapSection.RouteOneElementCollection.RouteElement.MatchesElementCollection.MatchElement
                    m in r.Matches)
                {
                    route.Matches.Add(new Match
                    {
                        Inverse = m.Inverse,
                        Property = m.Property,
                        RegEx = m.Regex
                    });
                }

                foreach (XapSection.RouteOneElementCollection.RouteElement.ActionsElementCollection.ActionElement a in r.Actions)
                {
                    var action = new ActionConfiguration { Type = a.Type };

                    foreach (XapSection.RouteOneElementCollection.RouteElement.ActionsElementCollection.ActionElement.ParametersElementCollection.AddElement
                        p in a.Parameters)
                    {
                        action.Params[p.Name] = p.Value;
                    }

                    route.Actions.Add(action);
                }

                ret.Add(route);
            }

            return ret;
        }

        public IEnumerable<ScheduledActionConfiguration> GetScheduledActions()
        {
            var ret = new List<ScheduledActionConfiguration>();

            foreach (XapSection.ScheduledActionsElementCollection.AddElement s in _config.ScheduledActions)
            {
                var c = new ScheduledActionConfiguration()
                {
                    Assembly = s.Assembly,
                    Type = s.Type,
                    Interval = TimeSpan.Parse(s.Interval),
                    Params = new Dictionary<string, string>(),
                };

                foreach (XapSection.ScheduledActionsElementCollection.AddElement.ParametersElementCollection.AddElement
                    p in s.Parameters)
                {
                    c.Params[p.Name] = p.Value;
                }

                ret.Add(c);
            }

            return ret;
        }


        public string WorkerMode
        {
            get
            {
                return _config.WorkerMode;
            }
            set
            {
                
            }
        }

        public void SetAlertProperties(List<AlertProperty> properties)
        {
            
        }

        public void SetAssemblyLoaderType(SimpleTypeDefinition type)
        {
            
        }

        public void SetPersistenceManagerType(SimpleTypeDefinition type)
        {
            
        }

        public void SetAlertQueueType(SimpleTypeDefinition type)
        {
            
        }

        public void SetCheckpointManagerType(SimpleTypeDefinition type)
        {
            
        }

        public void SetActionTypes(IEnumerable<SimpleTypeDefinition> actionTypes)
        {
            
        }

        public void SetReporterTypes(IEnumerable<ReporterConfiguration> reporters)
        {
            
        }

        public void SetRouteManyRoutes(IEnumerable<Route> routes)
        {
            
        }

        public void SetRouteOneRoutes(IEnumerable<Route> routes)
        {
            
        }

        public void SetScheduledActions(IEnumerable<ScheduledActionConfiguration> actions)
        {
            
        }
    }

    public sealed partial class XapSection : System.Configuration.ConfigurationSection
    {

        [System.Configuration.ConfigurationPropertyAttribute("workerMode", IsRequired = true)]
        public string WorkerMode
        {
            get
            {
                return ((string)(this["workerMode"]));
            }
            set
            {
                this["workerMode"] = value;
            }
        }

        [System.Configuration.ConfigurationPropertyAttribute("assemblyLoader")]
        public AssemblyLoaderElement AssemblyLoader
        {
            get
            {
                return ((AssemblyLoaderElement)(this["assemblyLoader"]));
            }
        }

        [System.Configuration.ConfigurationPropertyAttribute("alertProperties")]
        [System.Configuration.ConfigurationCollectionAttribute(typeof(AlertPropertiesElementCollection.AddElement), AddItemName = "add")]
        public AlertPropertiesElementCollection AlertProperties
        {
            get
            {
                return ((AlertPropertiesElementCollection)(this["alertProperties"]));
            }
        }

        [System.Configuration.ConfigurationPropertyAttribute("reporterTypes")]
        [System.Configuration.ConfigurationCollectionAttribute(typeof(ReporterTypesElementCollection.AddElement), AddItemName = "add")]
        public ReporterTypesElementCollection ReporterTypes
        {
            get
            {
                return ((ReporterTypesElementCollection)(this["reporterTypes"]));
            }
        }

        [System.Configuration.ConfigurationPropertyAttribute("checkpointManager")]
        public CheckpointManagerElement CheckpointManager
        {
            get
            {
                return ((CheckpointManagerElement)(this["checkpointManager"]));
            }
        }

        [System.Configuration.ConfigurationPropertyAttribute("persistenceManager")]
        public PersistenceManagerElement PersistenceManager
        {
            get
            {
                return ((PersistenceManagerElement)(this["persistenceManager"]));
            }
        }

        [System.Configuration.ConfigurationPropertyAttribute("queue")]
        public QueueElement Queue
        {
            get
            {
                return ((QueueElement)(this["queue"]));
            }
        }

        [System.Configuration.ConfigurationPropertyAttribute("actionTypes")]
        [System.Configuration.ConfigurationCollectionAttribute(typeof(ActionTypesElementCollection.AddElement), AddItemName = "add")]
        public ActionTypesElementCollection ActionTypes
        {
            get
            {
                return ((ActionTypesElementCollection)(this["actionTypes"]));
            }
        }

        [System.Configuration.ConfigurationPropertyAttribute("scheduledActions")]
        [System.Configuration.ConfigurationCollectionAttribute(typeof(ScheduledActionsElementCollection.AddElement), AddItemName = "add")]
        public ScheduledActionsElementCollection ScheduledActions
        {
            get
            {
                return ((ScheduledActionsElementCollection)(this["scheduledActions"]));
            }
        }

        [System.Configuration.ConfigurationPropertyAttribute("routeMany")]
        [System.Configuration.ConfigurationCollectionAttribute(typeof(RouteManyElementCollection.RouteElement), AddItemName = "route")]
        public RouteManyElementCollection RouteMany
        {
            get
            {
                return ((RouteManyElementCollection)(this["routeMany"]));
            }
        }

        [System.Configuration.ConfigurationPropertyAttribute("routeOne")]
        [System.Configuration.ConfigurationCollectionAttribute(typeof(RouteOneElementCollection.RouteElement), AddItemName = "route")]
        public RouteOneElementCollection RouteOne
        {
            get
            {
                return ((RouteOneElementCollection)(this["routeOne"]));
            }
        }

        public sealed partial class AssemblyLoaderElement : System.Configuration.ConfigurationElement
        {

            [System.Configuration.ConfigurationPropertyAttribute("assembly", IsRequired = true)]
            public string Assembly
            {
                get
                {
                    return ((string)(this["assembly"]));
                }
                set
                {
                    this["assembly"] = value;
                }
            }

            [System.Configuration.ConfigurationPropertyAttribute("type", IsRequired = true)]
            public string Type
            {
                get
                {
                    return ((string)(this["type"]));
                }
                set
                {
                    this["type"] = value;
                }
            }
        }

        public sealed partial class AlertPropertiesElementCollection : System.Configuration.ConfigurationElementCollection
        {

            public AddElement this[int i]
            {
                get
                {
                    return ((AddElement)(this.BaseGet(i)));
                }
            }

            protected override System.Configuration.ConfigurationElement CreateNewElement()
            {
                return new AddElement();
            }

            protected override object GetElementKey(System.Configuration.ConfigurationElement element)
            {
                return ((AddElement)(element)).Name;
            }

            public sealed partial class AddElement : System.Configuration.ConfigurationElement
            {

                [System.Configuration.ConfigurationPropertyAttribute("name", IsRequired = true)]
                public string Name
                {
                    get
                    {
                        return ((string)(this["name"]));
                    }
                    set
                    {
                        this["name"] = value;
                    }
                }

                [System.Configuration.ConfigurationPropertyAttribute("required", IsRequired = true)]
                public bool Required
                {
                    get
                    {
                        return ((bool)(this["required"]));
                    }
                    set
                    {
                        this["required"] = value;
                    }
                }

                [System.Configuration.ConfigurationPropertyAttribute("value", IsRequired = true)]
                public string Value
                {
                    get
                    {
                        return ((string)(this["value"]));
                    }
                    set
                    {
                        this["value"] = value;
                    }
                }

                [System.Configuration.ConfigurationPropertyAttribute("validation", IsRequired = true)]
                public string Validation
                {
                    get
                    {
                        return ((string)(this["validation"]));
                    }
                    set
                    {
                        this["validation"] = value;
                    }
                }
            }
        }

        public sealed partial class ReporterTypesElementCollection : System.Configuration.ConfigurationElementCollection
        {

            public AddElement this[int i]
            {
                get
                {
                    return ((AddElement)(this.BaseGet(i)));
                }
            }

            protected override System.Configuration.ConfigurationElement CreateNewElement()
            {
                return new AddElement();
            }

            protected override object GetElementKey(System.Configuration.ConfigurationElement element)
            {
                return ((AddElement)(element)).Assembly;
            }

            public sealed partial class AddElement : System.Configuration.ConfigurationElement
            {

                [System.Configuration.ConfigurationPropertyAttribute("assembly", IsRequired = true)]
                public string Assembly
                {
                    get
                    {
                        return ((string)(this["assembly"]));
                    }
                    set
                    {
                        this["assembly"] = value;
                    }
                }

                [System.Configuration.ConfigurationPropertyAttribute("type", IsRequired = true)]
                public string Type
                {
                    get
                    {
                        return ((string)(this["type"]));
                    }
                    set
                    {
                        this["type"] = value;
                    }
                }

                [System.Configuration.ConfigurationPropertyAttribute("parameters")]
                [System.Configuration.ConfigurationCollectionAttribute(typeof(ParametersElementCollection.AddElement), AddItemName = "add")]
                public ParametersElementCollection Parameters
                {
                    get
                    {
                        return ((ParametersElementCollection)(this["parameters"]));
                    }
                }

                public sealed partial class ParametersElementCollection : System.Configuration.ConfigurationElementCollection
                {

                    public AddElement this[int i]
                    {
                        get
                        {
                            return ((AddElement)(this.BaseGet(i)));
                        }
                    }

                    protected override System.Configuration.ConfigurationElement CreateNewElement()
                    {
                        return new AddElement();
                    }

                    protected override object GetElementKey(System.Configuration.ConfigurationElement element)
                    {
                        return ((AddElement)(element)).Name;
                    }

                    public sealed partial class AddElement : System.Configuration.ConfigurationElement
                    {

                        [System.Configuration.ConfigurationPropertyAttribute("name", IsRequired = true)]
                        public string Name
                        {
                            get
                            {
                                return ((string)(this["name"]));
                            }
                            set
                            {
                                this["name"] = value;
                            }
                        }

                        [System.Configuration.ConfigurationPropertyAttribute("value", IsRequired = true)]
                        public string Value
                        {
                            get
                            {
                                return ((string)(this["value"]));
                            }
                            set
                            {
                                this["value"] = value;
                            }
                        }
                    }
                }
            }
        }

        public sealed partial class CheckpointManagerElement : System.Configuration.ConfigurationElement
        {

            [System.Configuration.ConfigurationPropertyAttribute("assembly", IsRequired = true)]
            public string Assembly
            {
                get
                {
                    return ((string)(this["assembly"]));
                }
                set
                {
                    this["assembly"] = value;
                }
            }

            [System.Configuration.ConfigurationPropertyAttribute("type", IsRequired = true)]
            public string Type
            {
                get
                {
                    return ((string)(this["type"]));
                }
                set
                {
                    this["type"] = value;
                }
            }
        }

        public sealed partial class PersistenceManagerElement : System.Configuration.ConfigurationElement
        {

            [System.Configuration.ConfigurationPropertyAttribute("assembly", IsRequired = true)]
            public string Assembly
            {
                get
                {
                    return ((string)(this["assembly"]));
                }
                set
                {
                    this["assembly"] = value;
                }
            }

            [System.Configuration.ConfigurationPropertyAttribute("type", IsRequired = true)]
            public string Type
            {
                get
                {
                    return ((string)(this["type"]));
                }
                set
                {
                    this["type"] = value;
                }
            }
        }


        public sealed partial class QueueElement : System.Configuration.ConfigurationElement
        {

            [System.Configuration.ConfigurationPropertyAttribute("assembly", IsRequired = true)]
            public string Assembly
            {
                get
                {
                    return ((string)(this["assembly"]));
                }
                set
                {
                    this["assembly"] = value;
                }
            }

            [System.Configuration.ConfigurationPropertyAttribute("type", IsRequired = true)]
            public string Type
            {
                get
                {
                    return ((string)(this["type"]));
                }
                set
                {
                    this["type"] = value;
                }
            }
        }

        public sealed partial class ActionTypesElementCollection : System.Configuration.ConfigurationElementCollection
        {

            public AddElement this[int i]
            {
                get
                {
                    return ((AddElement)(this.BaseGet(i)));
                }
            }

            protected override System.Configuration.ConfigurationElement CreateNewElement()
            {
                return new AddElement();
            }

            protected override object GetElementKey(System.Configuration.ConfigurationElement element)
            {
                return ((AddElement)(element)).Assembly;
            }

            public sealed partial class AddElement : System.Configuration.ConfigurationElement
            {

                [System.Configuration.ConfigurationPropertyAttribute("assembly", IsRequired = true)]
                public string Assembly
                {
                    get
                    {
                        return ((string)(this["assembly"]));
                    }
                    set
                    {
                        this["assembly"] = value;
                    }
                }

                [System.Configuration.ConfigurationPropertyAttribute("type", IsRequired = true)]
                public string Type
                {
                    get
                    {
                        return ((string)(this["type"]));
                    }
                    set
                    {
                        this["type"] = value;
                    }
                }
            }
        }

        public sealed partial class ScheduledActionsElementCollection : System.Configuration.ConfigurationElementCollection
        {

            public AddElement this[int i]
            {
                get
                {
                    return ((AddElement)(this.BaseGet(i)));
                }
            }

            protected override System.Configuration.ConfigurationElement CreateNewElement()
            {
                return new AddElement();
            }

            protected override object GetElementKey(System.Configuration.ConfigurationElement element)
            {
                return ((AddElement)(element)).Assembly;
            }

            public sealed partial class AddElement : System.Configuration.ConfigurationElement
            {

                [System.Configuration.ConfigurationPropertyAttribute("assembly", IsRequired = true)]
                public string Assembly
                {
                    get
                    {
                        return ((string)(this["assembly"]));
                    }
                    set
                    {
                        this["assembly"] = value;
                    }
                }

                [System.Configuration.ConfigurationPropertyAttribute("type", IsRequired = true)]
                public string Type
                {
                    get
                    {
                        return ((string)(this["type"]));
                    }
                    set
                    {
                        this["type"] = value;
                    }
                }

                [System.Configuration.ConfigurationPropertyAttribute("interval", IsRequired = true)]
                public string Interval
                {
                    get
                    {
                        return ((string)(this["interval"]));
                    }
                    set
                    {
                        this["interval"] = value;
                    }
                }

                [System.Configuration.ConfigurationPropertyAttribute("parameters")]
                [System.Configuration.ConfigurationCollectionAttribute(typeof(ParametersElementCollection.AddElement), AddItemName = "add")]
                public ParametersElementCollection Parameters
                {
                    get
                    {
                        return ((ParametersElementCollection)(this["parameters"]));
                    }
                }

                public sealed partial class ParametersElementCollection : System.Configuration.ConfigurationElementCollection
                {

                    public AddElement this[int i]
                    {
                        get
                        {
                            return ((AddElement)(this.BaseGet(i)));
                        }
                    }

                    protected override System.Configuration.ConfigurationElement CreateNewElement()
                    {
                        return new AddElement();
                    }

                    protected override object GetElementKey(System.Configuration.ConfigurationElement element)
                    {
                        return ((AddElement)(element)).Name;
                    }

                    public sealed partial class AddElement : System.Configuration.ConfigurationElement
                    {

                        [System.Configuration.ConfigurationPropertyAttribute("name", IsRequired = true)]
                        public string Name
                        {
                            get
                            {
                                return ((string)(this["name"]));
                            }
                            set
                            {
                                this["name"] = value;
                            }
                        }

                        [System.Configuration.ConfigurationPropertyAttribute("value", IsRequired = true)]
                        public string Value
                        {
                            get
                            {
                                return ((string)(this["value"]));
                            }
                            set
                            {
                                this["value"] = value;
                            }
                        }
                    }
                }
            }
        }

        public sealed partial class RouteManyElementCollection : System.Configuration.ConfigurationElementCollection
        {

            public RouteElement this[int i]
            {
                get
                {
                    return ((RouteElement)(this.BaseGet(i)));
                }
            }

            protected override System.Configuration.ConfigurationElement CreateNewElement()
            {
                return new RouteElement();
            }

            protected override object GetElementKey(System.Configuration.ConfigurationElement element)
            {
                return ((RouteElement)(element)).Name;
            }

            public sealed partial class RouteElement : System.Configuration.ConfigurationElement
            {

                [System.Configuration.ConfigurationPropertyAttribute("name", IsRequired = true)]
                public string Name
                {
                    get
                    {
                        return ((string)(this["name"]));
                    }
                    set
                    {
                        this["name"] = value;
                    }
                }

                [System.Configuration.ConfigurationPropertyAttribute("matches")]
                [System.Configuration.ConfigurationCollectionAttribute(typeof(MatchesElementCollection.MatchElement), AddItemName = "match")]
                public MatchesElementCollection Matches
                {
                    get
                    {
                        return ((MatchesElementCollection)(this["matches"]));
                    }
                }

                [System.Configuration.ConfigurationPropertyAttribute("actions")]
                [System.Configuration.ConfigurationCollectionAttribute(typeof(ActionsElementCollection.ActionElement), AddItemName = "action")]
                public ActionsElementCollection Actions
                {
                    get
                    {
                        return ((ActionsElementCollection)(this["actions"]));
                    }
                }

                public sealed partial class MatchesElementCollection : System.Configuration.ConfigurationElementCollection
                {

                    public MatchElement this[int i]
                    {
                        get
                        {
                            return ((MatchElement)(this.BaseGet(i)));
                        }
                    }

                    protected override System.Configuration.ConfigurationElement CreateNewElement()
                    {
                        return new MatchElement();
                    }

                    protected override object GetElementKey(System.Configuration.ConfigurationElement element)
                    {
                        return ((MatchElement)(element)).Property;
                    }

                    public sealed partial class MatchElement : System.Configuration.ConfigurationElement
                    {

                        [System.Configuration.ConfigurationPropertyAttribute("property", IsRequired = true)]
                        public string Property
                        {
                            get
                            {
                                return ((string)(this["property"]));
                            }
                            set
                            {
                                this["property"] = value;
                            }
                        }

                        [System.Configuration.ConfigurationPropertyAttribute("regex", IsRequired = true)]
                        public string Regex
                        {
                            get
                            {
                                return ((string)(this["regex"]));
                            }
                            set
                            {
                                this["regex"] = value;
                            }
                        }

                        [System.Configuration.ConfigurationPropertyAttribute("inverse", IsRequired = true)]
                        public bool Inverse
                        {
                            get
                            {
                                return ((bool)(this["inverse"]));
                            }
                            set
                            {
                                this["inverse"] = value;
                            }
                        }
                    }
                }

                public sealed partial class ActionsElementCollection : System.Configuration.ConfigurationElementCollection
                {

                    public ActionElement this[int i]
                    {
                        get
                        {
                            return ((ActionElement)(this.BaseGet(i)));
                        }
                    }

                    protected override System.Configuration.ConfigurationElement CreateNewElement()
                    {
                        return new ActionElement();
                    }

                    protected override object GetElementKey(System.Configuration.ConfigurationElement element)
                    {
                        return ((ActionElement)(element)).Type;
                    }

                    public sealed partial class ActionElement : System.Configuration.ConfigurationElement
                    {

                        [System.Configuration.ConfigurationPropertyAttribute("type", IsRequired = true)]
                        public string Type
                        {
                            get
                            {
                                return ((string)(this["type"]));
                            }
                            set
                            {
                                this["type"] = value;
                            }
                        }

                        [System.Configuration.ConfigurationPropertyAttribute("parameters")]
                        [System.Configuration.ConfigurationCollectionAttribute(typeof(ParametersElementCollection.AddElement), AddItemName = "add")]
                        public ParametersElementCollection Parameters
                        {
                            get
                            {
                                return ((ParametersElementCollection)(this["parameters"]));
                            }
                        }

                        public sealed partial class ParametersElementCollection : System.Configuration.ConfigurationElementCollection
                        {

                            public AddElement this[int i]
                            {
                                get
                                {
                                    return ((AddElement)(this.BaseGet(i)));
                                }
                            }

                            protected override System.Configuration.ConfigurationElement CreateNewElement()
                            {
                                return new AddElement();
                            }

                            protected override object GetElementKey(System.Configuration.ConfigurationElement element)
                            {
                                return ((AddElement)(element)).Name;
                            }

                            public sealed partial class AddElement : System.Configuration.ConfigurationElement
                            {

                                [System.Configuration.ConfigurationPropertyAttribute("name", IsRequired = true)]
                                public string Name
                                {
                                    get
                                    {
                                        return ((string)(this["name"]));
                                    }
                                    set
                                    {
                                        this["name"] = value;
                                    }
                                }

                                [System.Configuration.ConfigurationPropertyAttribute("value", IsRequired = true)]
                                public string Value
                                {
                                    get
                                    {
                                        return ((string)(this["value"]));
                                    }
                                    set
                                    {
                                        this["value"] = value;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public sealed partial class RouteOneElementCollection : System.Configuration.ConfigurationElementCollection
        {

            public RouteElement this[int i]
            {
                get
                {
                    return ((RouteElement)(this.BaseGet(i)));
                }
            }

            protected override System.Configuration.ConfigurationElement CreateNewElement()
            {
                return new RouteElement();
            }

            protected override object GetElementKey(System.Configuration.ConfigurationElement element)
            {
                return ((RouteElement)(element)).Name;
            }

            public sealed partial class RouteElement : System.Configuration.ConfigurationElement
            {

                [System.Configuration.ConfigurationPropertyAttribute("name", IsRequired = true)]
                public string Name
                {
                    get
                    {
                        return ((string)(this["name"]));
                    }
                    set
                    {
                        this["name"] = value;
                    }
                }

                [System.Configuration.ConfigurationPropertyAttribute("matches")]
                [System.Configuration.ConfigurationCollectionAttribute(typeof(MatchesElementCollection.MatchElement), AddItemName = "match")]
                public MatchesElementCollection Matches
                {
                    get
                    {
                        return ((MatchesElementCollection)(this["matches"]));
                    }
                }

                [System.Configuration.ConfigurationPropertyAttribute("actions")]
                [System.Configuration.ConfigurationCollectionAttribute(typeof(ActionsElementCollection.ActionElement), AddItemName = "action")]
                public ActionsElementCollection Actions
                {
                    get
                    {
                        return ((ActionsElementCollection)(this["actions"]));
                    }
                }

                public sealed partial class MatchesElementCollection : System.Configuration.ConfigurationElementCollection
                {

                    public MatchElement this[int i]
                    {
                        get
                        {
                            return ((MatchElement)(this.BaseGet(i)));
                        }
                    }

                    protected override System.Configuration.ConfigurationElement CreateNewElement()
                    {
                        return new MatchElement();
                    }

                    protected override object GetElementKey(System.Configuration.ConfigurationElement element)
                    {
                        return ((MatchElement)(element)).Property;
                    }

                    public sealed partial class MatchElement : System.Configuration.ConfigurationElement
                    {

                        [System.Configuration.ConfigurationPropertyAttribute("property", IsRequired = true)]
                        public string Property
                        {
                            get
                            {
                                return ((string)(this["property"]));
                            }
                            set
                            {
                                this["property"] = value;
                            }
                        }

                        [System.Configuration.ConfigurationPropertyAttribute("regex", IsRequired = true)]
                        public string Regex
                        {
                            get
                            {
                                return ((string)(this["regex"]));
                            }
                            set
                            {
                                this["regex"] = value;
                            }
                        }

                        [System.Configuration.ConfigurationPropertyAttribute("inverse", IsRequired = true)]
                        public bool Inverse
                        {
                            get
                            {
                                return ((bool)(this["inverse"]));
                            }
                            set
                            {
                                this["inverse"] = value;
                            }
                        }
                    }
                }

                public sealed partial class ActionsElementCollection : System.Configuration.ConfigurationElementCollection
                {

                    public ActionElement this[int i]
                    {
                        get
                        {
                            return ((ActionElement)(this.BaseGet(i)));
                        }
                    }

                    protected override System.Configuration.ConfigurationElement CreateNewElement()
                    {
                        return new ActionElement();
                    }

                    protected override object GetElementKey(System.Configuration.ConfigurationElement element)
                    {
                        return ((ActionElement)(element)).Type;
                    }

                    public sealed partial class ActionElement : System.Configuration.ConfigurationElement
                    {

                        [System.Configuration.ConfigurationPropertyAttribute("type", IsRequired = true)]
                        public string Type
                        {
                            get
                            {
                                return ((string)(this["type"]));
                            }
                            set
                            {
                                this["type"] = value;
                            }
                        }

                        [System.Configuration.ConfigurationPropertyAttribute("parameters")]
                        [System.Configuration.ConfigurationCollectionAttribute(typeof(ParametersElementCollection.AddElement), AddItemName = "add")]
                        public ParametersElementCollection Parameters
                        {
                            get
                            {
                                return ((ParametersElementCollection)(this["parameters"]));
                            }
                        }

                        public sealed partial class ParametersElementCollection : System.Configuration.ConfigurationElementCollection
                        {

                            public AddElement this[int i]
                            {
                                get
                                {
                                    return ((AddElement)(this.BaseGet(i)));
                                }
                            }

                            protected override System.Configuration.ConfigurationElement CreateNewElement()
                            {
                                return new AddElement();
                            }

                            protected override object GetElementKey(System.Configuration.ConfigurationElement element)
                            {
                                return ((AddElement)(element)).Name;
                            }

                            public sealed partial class AddElement : System.Configuration.ConfigurationElement
                            {

                                [System.Configuration.ConfigurationPropertyAttribute("name", IsRequired = true)]
                                public string Name
                                {
                                    get
                                    {
                                        return ((string)(this["name"]));
                                    }
                                    set
                                    {
                                        this["name"] = value;
                                    }
                                }

                                [System.Configuration.ConfigurationPropertyAttribute("value", IsRequired = true)]
                                public string Value
                                {
                                    get
                                    {
                                        return ((string)(this["value"]));
                                    }
                                    set
                                    {
                                        this["value"] = value;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

}

