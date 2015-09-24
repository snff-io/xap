using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using XAP.Interface;

namespace XAP.Engine.Router
{
    public class RouteManager : IRouteManager
    {
        IConfiguration _config;
        IEnumerable<Route> _routeManyRoutes;
        IEnumerable<Route> _routeOneRoutes;

        public void Initialize(IContext context)
        {
            _config = context.Configuration;
            _routeManyRoutes = _config.GetRouteManyRoutes();
            _routeOneRoutes = _config.GetRouteOneRoutes();
        }

        public RouterResponse Match(AlertInstance alert)
        {
            var ret = new RouterResponse();
            alert.AddTrace("Calculating alert processing routes");

            foreach (var route in _routeManyRoutes)
            {
                var curMatched = 0;

                foreach (var match in route.Matches.Where(match => alert[match.Property] != null))
                {
                    if (alert[match.Property].Value == null)
                    {
                        alert[match.Property].Value = string.Empty;
                    }

                    var isMatch = Regex.IsMatch(alert[match.Property].Value, match.RegEx);

                    if ((isMatch && !match.Inverse) || (!isMatch && match.Inverse))
                    {
                        curMatched++;
                    }
                }

                if (curMatched != route.Matches.Count())
                {
                    continue;
                }

                ret.RouteManyRoutes.Add(route);
                alert.AddTrace("Adding route '{0}' based on {1} rules matching alert properties", route.Name, curMatched);
            }
            
            Route bestMatchedRoute = null;
            alert.AddTrace("Calculating alert action routes");
            foreach (var route in _routeOneRoutes)
            {
                var curMatched = 0;

                foreach (var match in route.Matches.Where(match => alert[match.Property] != null))
                {
                    if (alert[match.Property].Value == null)
                    {
                        alert[match.Property].Value = string.Empty;
                    }

                    bool isMatch = Regex.IsMatch(alert[match.Property].Value, match.RegEx);

                    if ((isMatch && !match.Inverse) || (!isMatch && match.Inverse))
                    {
                        curMatched++;
                    }
                }

                if (curMatched != route.Matches.Count)
                {
                    continue;
                }

                if (bestMatchedRoute == null || bestMatchedRoute.Matches.Count < route.Matches.Count)
                {
                    bestMatchedRoute = route;
                }
            }

            ret.RouteOneRoute = bestMatchedRoute;

            if (ret.RouteOneRoute != null)
            {
                alert.AddTrace("Adding route '{0}'", ret.RouteOneRoute.Name);
            }
            return ret;
        }
    }
}
