using System;
using System.Collections.Generic;
using System.Net;

namespace GGM.Web
{
    public class Response
    {
        public static Response SetBody(object model) => new Response(model);

        public object Model { get; }
        public Dictionary<string, string> Header { get; } = new Dictionary<string, string>();
        public int StatusCode { get; private set; } = 200;

        public Response(object model) : this(model, new Dictionary<string, string>()) { }

        public Response(object model, IEnumerable<KeyValuePair<string, string>> header)
        {
            if(header == null)
                throw new ArgumentNullException(nameof(header));
            
            Model = model ?? throw new ArgumentNullException(nameof(model));
            SetHeaders(header);
        }

        public Response SetHeader(string key, string value)
        {
            Header.Add(key, value);
            return this;
        }
        
        public Response SetHeaders(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            if(keyValuePairs == null)
                throw new ArgumentNullException(nameof(keyValuePairs));
            
            foreach (var keyValuePair in keyValuePairs)
                Header.Add(keyValuePair.Key, keyValuePair.Value);

            return this;
        }

        public Response SetStatusCode(HttpStatusCode statusCode)
        {
            StatusCode = (int) statusCode;
            return this;
        }
    }
}