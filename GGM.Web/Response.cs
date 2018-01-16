using GGM.Web.View;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace GGM.Web.Router
{
    public class Response
    {
        public static Response SetBody(object model) => new Response(model);
        
        public object Model { get; private set; }
        public Dictionary<string, string> Header { get; private set; }

        public Response(object model) : this(model, new Dictionary<string, string>())
        {
        }

        public Response(object model, IEnumerable<KeyValuePair<string, string>> header)
        {
            Model = model;
            Header = header as Dictionary<string, string>;
        }
        
        public Response SetHeader(string key, string value)
        {
            Header.Add(key, value);
            return this;
        }

        public Response SetHeaders(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            foreach(KeyValuePair<string, string> keyValuePair in keyValuePairs)
            {
                Header.Add(keyValuePair.Key, keyValuePair.Value);
            }
            return this;
        }
    }
}
