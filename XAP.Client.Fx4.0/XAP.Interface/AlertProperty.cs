using System;
using System.Runtime.Serialization;

namespace XAP.Interface
{
    [Serializable]
    [DataContract]
    public class AlertProperty
    {
        public AlertProperty()
        {
            Value = String.Empty;
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public string Validation { get; set; }

        [DataMember]
        public bool Required { get; set; }

        [OnDeserialized]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            Value = Value ?? String.Empty;
        }
    }
}
