using System.Threading.Tasks;
using RazorLight;

namespace GGM.Web.View.Razor
{
    public class RazorTemplateResolver : ITempleteResolver
    {
        private readonly RazorLightEngine mEngine;
        
        public RazorTemplateResolver(string resourcePath)
        {
            mEngine = new RazorLightEngineBuilder()
                .UseFilesystemProject(resourcePath)
                .UseMemoryCachingProvider()
                .Build();
        }
        
        public Task<string> Resolve(ViewModel viewModel)
        {
            return mEngine.CompileRenderAsync(viewModel.URL, viewModel.Model);
        }
    }
}