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
using GGM.Serializer.Protobuf;
using GGM.Web.View.Razor;

namespace DemoServer.Service
{
    public class MyWebService : WebService
    {
        [AutoWired]
        public MyWebService(MyController myController, RazorTempleteResolverFactory resolverFactory, ProtobufSerializerFactory serializerFactory) 
            : base(resolverFactory, serializerFactory, new string[] { "http://localhost:8002/" }, myController)
        {
        }

        
        public override Task Boot(string[] arguments)
        {
            Console.WriteLine("WebServer Start!");
            return base.Boot(arguments);
        }
    }
}