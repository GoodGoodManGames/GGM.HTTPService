using System;
using System.Dynamic;
using System.Net;
using System.Threading.Tasks;
using GGM.Web;
using GGM.Web.Router;
using GGM.Web.Router.Attribute;
using GGM.Web.View;
using System.Collections.Generic;

namespace DemoServer.Controller
{
    [HTTPController]
    public class MyController
    {
        [Get("/gettest")]
        public string GetTest()
        {
            return "ggms";
        }

        [Get("/gettest/{user_id}/{user_name}")]
        public ViewModel GetTest(HttpListenerRequest request
                              , [Path("user_id")] string userID
                              , [Path("user_name")] string userName)
        {
            return ViewModel.Get("index").SetModel(new TestModel(userName, int.Parse(userID)));
        }

        // Warring : TestModel은 IMessage가 아니므로 프로토버퍼에 의해 Serialzie 되지 못함.
        [Get("/serializer")]
        public TestModel GetSerializerTest()
        {
            return new TestModel("asd", 123);
        }

        [Get("/response")]
        public Response GetResponse()
        {
            return Response.SetBody(ViewModel.Get("index").SetModel(new TestModel("GoodGoodMan is Woman", 10)))
                        .SetHeader("GoodGoodMan_is", "Woman")
                        .SetHeaders(new Dictionary<string, string>());//IEnumerable<KeyValuePair<TKey, TValue>>
        }
    }

    public class TestModel
    {
        public TestModel(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public string Name { get; }
        public int Age { get; }
    }
}