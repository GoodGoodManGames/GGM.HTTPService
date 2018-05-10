using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace GGM.Web.Router.Util
{
    public static class HttpListenerRequestExtension
    {
        public static HTTPMethodType GetHTTPMethod(this HttpListenerRequest self)
        {
            switch(self.HttpMethod)
            {
                case "GET":
                    return HTTPMethodType.Get;
                case "POST":
                    return HTTPMethodType.Post;
                case "PUT":
                    return HTTPMethodType.Put;
                case "DELETE":
                    return HTTPMethodType.Delete;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
