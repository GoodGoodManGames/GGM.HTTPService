using System.Text.RegularExpressions;
using GGM.Web.Router.Attribute;

namespace GGM.Web.Router
{
    public class RouteInfo
    {
        public RouteInfo(object controller, HTTPMethod method, PathToRegex pathToRoute, DefaultRouter.RouterCallback routerCallback)
        {
            Controller = controller;
            Method = method;
            PathToRegex = pathToRoute;
            RouterCallback = routerCallback;
        }
        
        public object Controller { get; }
        public HTTPMethod Method { get; }
        public PathToRegex PathToRegex { get; }
        public DefaultRouter.RouterCallback RouterCallback { get; }

        public bool IsMatched(HTTPMethod method, string url)
        {
            return Method == method && PathToRegex.IsMatched(url);
        }
    }
}
