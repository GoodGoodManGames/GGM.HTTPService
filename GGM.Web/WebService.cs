using GGM.Application.Service;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GGM.Serializer;
using GGM.Web.Router;
using GGM.Web.Router.Util;
using GGM.Web.View;


namespace GGM.Web
{
    /// <summary>
    ///     웹 서버를 제공하는 Service입니다.
    /// </summary>
    public class WebService : IService
    {
        public WebService(ITempleteResolver resolver, ISerializer serializer,
            string[] prefixes, params object[] controllers)
        {
            Router = new DefaultRouter();
            Controllers = controllers;
            foreach (var controller in controllers)
                Router.RegisterController(controller);

            HttpListener = new HttpListener();
            foreach (var prefix in prefixes)
                HttpListener.Prefixes.Add(prefix);

            TempleteResolver = resolver;
            Serializer = serializer;
        }


        public Guid ID { get; set; }
        public IRouter Router { get; }
        public ITempleteResolver TempleteResolver { get; }
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
                HttpListenerContext context = await HttpListener.GetContextAsync().ConfigureAwait(false);
                object result;
                try
                {
                    result = Router.Route(context.Request, Serializer);
                }
                catch (Exception e)
                {
                    result = e.ToString();
                }

                using (var response = context.Response)
                using (var outputStream = response.OutputStream)
                {
                    if (result != null)
                    {
                        byte[] responseBody = null;
                        responseBody = await GetSerializedResponseData(result, response).ConfigureAwait(false);

                        if (responseBody != null)
                            await outputStream.WriteAsync(responseBody, 0, responseBody.Length).ConfigureAwait(false);
                    }
                }
            }
        }

        private async Task<byte[]> GetSerializedResponseData(object data, HttpListenerResponse httpResponse = null)
        {
            if (data is string message)
                return Encoding.UTF8.GetBytes(message);
            else if (data is ViewModel viewModel)
            {
                var bodyTask = TempleteResolver?.Resolve(viewModel);
                return Encoding.UTF8.GetBytes(await bodyTask.ConfigureAwait(false));
            }
            else if (data is Response response)
            {
                foreach (var key in response.Header.Keys)
                    httpResponse.AppendHeader(key, response.Header[key]);
                return await GetSerializedResponseData(response.Model).ConfigureAwait(false);
            }
            else if (data is Task task)
            {
                await task.ConfigureAwait(false);
                return await GetSerializedResponseData(TaskUtil.GetResultFromTask(task)).ConfigureAwait(false);
            }
            else
                return Serializer.Serialize(data);
        }

        protected void Stop()
        {
            mIsRunning = false;
            HttpListener.Stop();
            HttpListener.Close();
        }
    }
}