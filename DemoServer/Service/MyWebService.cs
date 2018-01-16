using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DemoServer.Controller;
using GGM.Context.Attribute;
using GGM.Web;
using GGM.Web.Router;
using GGM.Application.Attribute;
using GGM.Serializer;
using GGM.Web.View.Razor;

namespace DemoServer.Service
{
    public class MyWebService : WebService
    {
        [AutoWired]
        public MyWebService(MyController myController, RazorTemplateResolverFactory resolverFactory) : base(resolverFactory, null, new string[] { "http://localhost:8002/" }, myController)
        {
        }

        
        public override Task Boot(string[] arguments)
        {
            Console.WriteLine("WebServer Start!");
            Serializer = new TestSerializer();
            return base.Boot(arguments);
        }
    }

    public class TestSerializer : ISerializer
    {
        public T Deserialize<T>(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public byte[] Serialize<T>(T data)
        {
            return Encoding.UTF8.GetBytes(data.GetType().Name);
        }
    }
}