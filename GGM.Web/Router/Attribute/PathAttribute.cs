using System;

namespace GGM.Web.Router.Attribute
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class PathAttribute : System.Attribute
    {
        public PathAttribute(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}