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
        
        public object Model { get;  set; }
        public Dictionary<string, string> Header { get; set; } = new Dictionary<string, string>();

        public Response(object model) : this(model, new Dictionary<string, string>())
        {
        }

        public Response(object model, IEnumerable<KeyValuePair<string, string>> header)
        {
            Model = model;
            SetHeader(header);
        }
        
        public Response SetHeader(string key, string value)
        {
            Header.Add(key, value);
            return this;
        }

        private void SetHeader(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            
            foreach (var keyValuePair in keyValuePairs)
                Header.Add(keyValuePair.Key, keyValuePair.Value);
        }

        public Response SetHeaders(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            foreach(var keyValuePair in keyValuePairs)
            {
                Header.Add(keyValuePair.Key, keyValuePair.Value);
            }
            return this;
        }
    }
}
