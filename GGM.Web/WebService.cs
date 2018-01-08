using GGM.Application.Service;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GGM.Serializer;
using GGM.Web.Router;
using GGM.Web.View;

namespace GGM.Web
{
    /// <summary>
    ///     웹 서버를 제공하는 Service입니다.
    /// </summary>
    public class WebService : IService
    {
        public WebService(IRouter router, string[] prefixes, params object[] controllers)
        {
            Router = router ?? new DefaultRouter();
            Controllers = controllers;
            foreach (var controller in controllers)
                Router.RegisterController(controller);

            HttpListener = new HttpListener();
            foreach (var prefix in prefixes)
                HttpListener.Prefixes.Add(prefix);
        }

        public Guid ID { get; set; }
        public IRouter Router { get; }
        public ITempleteResolver TempleteResolver { get; protected set; }
        public ISerializer Serializer { get; protected set; }
        protected object[] Controllers { get; }
        protected HttpListener HttpListener { get; }
        private bool mIsRunning = false;

        /// <summary>
        ///     WebService를 시작합니다.
        /// </summary>
        public virtual async Task Boot(string[] arguments)
        {
            if (TempleteResolver == null)
                Console.WriteLine($"{GetType().Name + ID}의 TempleteResolver가 지정되지 않았습니다.");
            if (Serializer == null)
                Console.WriteLine($"{GetType().Name + ID}의 Serializer가 지정되지 않았습니다.");

            mIsRunning = true;
            HttpListener.Start();
            while (mIsRunning)
            {
                HttpListenerContext context = await HttpListener.GetContextAsync();
                object result;
                try
                {
                    result = Router.Route(context.Request);
                }
                catch (Exception e)
                {
                    result = e.ToString();
                }

                using (var response = context.Response)
                using (var outputStream = response.OutputStream)
                using (var writer = new StreamWriter(outputStream))
                {
                    if (result != null)
                    {
                        byte[] responseBody = null;
                        switch (result)
                        {
                            case string resultText:
                                responseBody = Encoding.UTF8.GetBytes(resultText);
                                break;
                            case ViewModel viewModel:
                                responseBody = Encoding.UTF8.GetBytes(await TempleteResolver?.Resolve(viewModel));
                                break;
                            case object data:
                                responseBody = Serializer.Serialize(data);
                                break;
                        }

                        if (responseBody != null)
                            await outputStream.WriteAsync(responseBody, 0, responseBody.Length);
                    }
                }
            }
        }

        protected void Stop()
        {
            mIsRunning = false;
            HttpListener.Stop();
            HttpListener.Close();
        }
    }
}