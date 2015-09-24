using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAP.Interface
{
    public class RouterResponse
    {
        public RouterResponse()
        {
            this.RouteManyRoutes = new List<Route>();
        }

        public ICollection<Route> RouteManyRoutes { get; set; }

        public Route RouteOneRoute { get; set; }

    }
}
