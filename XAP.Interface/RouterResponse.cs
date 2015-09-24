using System.Collections.Generic;

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
