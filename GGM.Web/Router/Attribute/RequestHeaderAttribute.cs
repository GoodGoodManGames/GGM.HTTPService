using System;

namespace GGM.Web.Router.Attribute
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class RequestHeaderAttribute : System.Attribute
    {
        public string RequestHeader { get; set; }

        public RequestHeaderAttribute(string header)
        {
            RequestHeader = header;
        }
    }
}