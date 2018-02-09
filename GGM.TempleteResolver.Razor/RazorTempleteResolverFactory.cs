using System;
using System.IO;
using GGM.Application.Attribute;
using GGM.Context;
using GGM.Context.Attribute;

namespace GGM.Web.View.Razor
{
    [Managed(ManagedType.Singleton)]
    public class RazorTempleteResolverFactory : ITempleteResolverFactory
    {
        [Config("TempleteResolver.Path")]
        public string ConfigPath { get; set; }

        public ITempleteResolver Create()
        {   
            string path;
            if (ConfigPath == null)
            {
                Console.WriteLine("TempleteResolver.Path 의 값이 지정되지 않았습니다. 이는 현재 디렉토리를 기본으로 합니다.");
                ConfigPath = "./";
            }

            path = Path.IsPathRooted(ConfigPath) ? ConfigPath : Path.Combine(Directory.GetCurrentDirectory(), ConfigPath);
            return new RazorTempleteResolver(path);
        }
            
    }
}