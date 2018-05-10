namespace GGM.Web.Router
{
    public class RouteInfo
    {
        public RouteInfo(object controller, HTTPMethodType methodType, PathToRegex pathToRoute, DefaultRouter.RouterCallback routerCallback)
        {
            Controller = controller;
            MethodType = methodType;
            PathToRegex = pathToRoute;
            RouterCallback = routerCallback;
        }
        
        public object Controller { get; }
        public HTTPMethodType MethodType { get; }
        public PathToRegex PathToRegex { get; }
        public DefaultRouter.RouterCallback RouterCallback { get; }

        public bool IsMatched(HTTPMethodType methodType, string url)
        {
            return MethodType == methodType && PathToRegex.IsMatched(url);
        }
    }
}