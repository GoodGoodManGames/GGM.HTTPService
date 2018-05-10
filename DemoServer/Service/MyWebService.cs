using System;
using System.Threading.Tasks;
using DemoServer.Controller;
using GGM.Context.Attribute;
using GGM.Web;
using GGM.Serializer.Protobuf;
using GGM.Web.View.Razor;

namespace DemoServer.Service
{
    public class MyWebService : WebService
    {
        [AutoWired]
        public MyWebService(MyController myController, RazorTemplateResolver resolver, ProtobufSerializer serializer) 
            : base(resolver, serializer, myController)
        {
        }

        
        public override Task Boot(string[] arguments)
        {
            Console.WriteLine("WebServer Start!");
            return base.Boot(arguments);
        }
    }
}