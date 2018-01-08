using System.Collections.Generic;

namespace GGM.Web.Router
{
    public enum HTTPMethod
    {
        Get, Post, Put, Delete
    }

    class HTTPMethodComparer : IEqualityComparer<HTTPMethod>
    {

        public bool Equals(HTTPMethod x, HTTPMethod y)
        {
            return (int)x == (int)y;
        }

        public int GetHashCode(HTTPMethod obj)
        {
            return ((int)obj).GetHashCode();
        }
    }
}
