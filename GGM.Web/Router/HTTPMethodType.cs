using System.Collections.Generic;

namespace GGM.Web.Router
{
    public enum HTTPMethodType
    {
        Get, Post, Put, Delete
    }

    class HTTPMethodComparer : IEqualityComparer<HTTPMethodType>
    {

        public bool Equals(HTTPMethodType x, HTTPMethodType y)
        {
            return (int)x == (int)y;
        }

        public int GetHashCode(HTTPMethodType obj)
        {
            return ((int)obj).GetHashCode();
        }
    }
}
