using System.Collections.Generic;

namespace XAP.Interface
{
    public class ActionConfiguration
    {
        public ActionConfiguration()
        {
            this.Params = new Dictionary<string, string>();
        }

        public string Type { get; set; }

        public Dictionary<string, string> Params { get; set; }


    }
}
