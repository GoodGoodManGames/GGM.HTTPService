using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace GGM.Web.Router.Util
{
    public static class HttpListenerRequestUtil
    {
        public static HTTPMethod GetHTTPMethod(this HttpListenerRequest self)
        {
            switch(self.HttpMethod)
            {
                case "GET":
                    return HTTPMethod.Get;
                case "POST":
                    return HTTPMethod.Post;
                case "PUT":
                    return HTTPMethod.Put;
                case "DELETE":
                    return HTTPMethod.Delete;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
