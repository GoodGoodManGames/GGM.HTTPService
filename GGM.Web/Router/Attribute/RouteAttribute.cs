using System;

namespace GGM.Web.Router.Attribute
{
    /// <summary>
    ///     Route매핑을 지정하는 애트리뷰트입니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RouteAttribute : System.Attribute
    {
        public RouteAttribute(HTTPMethodType httpMethodType, string routeUrl)
        {
            HttpMethodType = httpMethodType;
            URLPattern = routeUrl;
        }
        
        public HTTPMethodType HttpMethodType { get; }
        public string URLPattern { get; }
    }

    public class GetAttribute : RouteAttribute
    {
        public GetAttribute(string routeUrl) : base(HTTPMethodType.Get, routeUrl) { }
    }

    public class PostAttribute : RouteAttribute
    {
        public PostAttribute(string routeUrl) : base(HTTPMethodType.Post, routeUrl) { }
    }

    public class PutAttribute : RouteAttribute
    {
        public PutAttribute(string routeUrl) : base(HTTPMethodType.Put, routeUrl) { }
    }

    public class DeleteAttribute : RouteAttribute
    {
        public DeleteAttribute(string routeUrl) : base(HTTPMethodType.Delete, routeUrl) { }
    }
}