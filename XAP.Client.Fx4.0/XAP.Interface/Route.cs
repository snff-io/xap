using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAP.Interface
{
    public class Route
    {
        public Route()
        {
            this.Matches = new List<Match>();
            this.Actions = new List<ActionConfiguration>();
        }

        public string Name { get; set; }

        public ICollection<Match> Matches { get; set; }

        public ICollection<ActionConfiguration> Actions { get; set; }
    }
}
