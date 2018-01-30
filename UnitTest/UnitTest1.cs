using System;
using System.Threading.Tasks;
using UnitTest.Controller;
using GGM.Web;
using GGM.Web.Router;
using GGM.Web.MessageExample;
using Xunit;

namespace UnitTest
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var service = new WebService(null, new TestObjFactory(), new[] { "http://localhost:8002/" }, new TestController());
            await service.Boot(new string[] { });
        }

        [Fact]
        public void URLToPatternTest()
        {
            var result = PathToRegex.URLToPattern("/{user_id}/friend/{friend_id}/teams");

        }
    }
}
