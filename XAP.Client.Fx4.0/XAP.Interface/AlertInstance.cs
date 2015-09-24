using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using XAP.Common;

namespace XAP.Interface
{
    [Serializable]
    [DataContract]
    public class AlertInstance
    {
        public AlertInstance()
        {
            Properties = new List<AlertProperty>();
            Traces = new List<AlertTrace>();
        }

        private static readonly DataContractSerializer _dcs = new DataContractSerializer(typeof(AlertInstance),
                        new[] { typeof(AlertProperty) });

        /// <summary>
        /// alert Instance identifier defined by the system reporting this alert.
        /// </summary>
        [DataMember]
        public Guid XapId { get; set; }

        [DataMember]
        public string Reporter { get; set; }

        [DataMember]
        public DateTime Reported { get; set; }

        [DataMember]
        public List<AlertProperty> Properties
        {
            get;
            set;
        }

        [XmlIgnore]
        public AlertProperty this[string key]
        {
            get
            {
                var prop = this.Properties.FirstOrDefault(x => x.Name == key);
                return prop;
            }
            set
            {
                var property = this.Properties.FirstOrDefault(x => x.Name == key);

                if (property != null)
                {
                    this.Properties.Remove(property);
                }

                this.Properties.Add(value);
            }
        }

        public override string ToString()
        {
            return Reporter ?? "" + ":" + XapId;
        }

        public List<AlertTrace> Traces
        {
            get;
            private set;
        }

        public void AddProperty(string name, string value)
        {
            this.Properties.Add(new AlertProperty { Name = name, Value = value });
        }

        public void AddTrace(string message)
        {
            var previousActivity = Trace.CorrelationManager.ActivityId;
            Trace.CorrelationManager.ActivityId = this.XapId;
            Tracing.AlertTrace.TraceInformation(message);
            Trace.CorrelationManager.ActivityId = previousActivity;
        }

        public void AddTrace(string message, params object[] args)
        {
            this.AddTrace(string.Format(message, args));
        }

        public void AddTrace(TraceEventType type, int id, string message)
        {
            var previousActivity = Trace.CorrelationManager.ActivityId;
            Trace.CorrelationManager.ActivityId = this.XapId;
            Tracing.AlertTrace.TraceEvent(type, id, message);
            Trace.CorrelationManager.ActivityId = previousActivity;
        }

        public void AddTrace(TraceEventType type, int id, string message, params object[] args)
        {
            this.AddTrace(type, id, string.Format(message, args));
        }

        public void TraceProperties()
        {
            var previousActivity = Trace.CorrelationManager.ActivityId;
            Trace.CorrelationManager.ActivityId = this.XapId;
            
            Tracing.AlertTrace.TraceInformation(this.AsXml());
            Trace.CorrelationManager.ActivityId = previousActivity;
        }

        public static bool TryParse(string xml, out AlertInstance alert)
        {
            using (StringReader sr = new StringReader(xml))
            {
                using (XmlReader xr = XmlReader.Create(sr))
                {
                    try
                    {
                        alert = _dcs.ReadObject(xr) as AlertInstance;
                        return alert != null;
                    }
                    catch (Exception)
                    {
                        alert = null;
                        return false;
                    }
                }
            }
        }

        public string AsXml()
        {
            try
            {
                using (var str = new MemoryStream())
                {
                    _dcs.WriteObject(str, this);
                    str.Position = 0;
                    using (var reader = new StreamReader(str))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Tracing.AlertTrace.TraceEvent(TraceEventType.Error, -999, "could not serialize alert instance:" + ex);
                return null;
            }
        }
    }
}
