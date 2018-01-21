using GGM.Application.Service;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GGM.Serializer;
using GGM.Web.Router;
using GGM.Web.View;
using System.Collections.Generic;

namespace GGM.Web
{
    /// <summary>
    ///     웹 서버를 제공하는 Service입니다.
    /// </summary>
    public class WebService : IService
    {
        public WebService(ITempleteResolverFactory resolverFactory, ISerializerFactory serializerFactory, string[] prefixes, params object[] controllers)
        {
            Router = new DefaultRouter();
            Controllers = controllers;
            foreach (var controller in controllers)
                Router.RegisterController(controller);

            HttpListener = new HttpListener();
            foreach (var prefix in prefixes)
                HttpListener.Prefixes.Add(prefix);

            if (resolverFactory != null)
                TempleteResolver = resolverFactory.Create();
            if (serializerFactory != null)
                Serializer = serializerFactory.Create();
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
                                responseBody = GetSerializedResponseData(resultText);
                                break;
                            case ViewModel viewModel:
                                responseBody = await GetSerializedResponseData(viewModel);
                                break;
                            case Response bodyWithHeader:
                                var header = bodyWithHeader.Header;
                                var body = bodyWithHeader.Model;
                                foreach (var key in header.Keys)
                                    response.AddHeader(key, header[key]);
                                responseBody = GetSerializedResponseData(body);
                                break;
                            case object data:
                                responseBody = GetSerializedResponseData(data);
                                break;
                        }

                        if (responseBody != null)
                            await outputStream.WriteAsync(responseBody, 0, responseBody.Length);
                    }
                }
            }
        }

        private byte[] GetSerializedResponseData(string resultText)
        {
            return Encoding.UTF8.GetBytes(resultText);
        }

        private async Task<byte[]> GetSerializedResponseData(ViewModel viewModel)
        {
            return Encoding.UTF8.GetBytes(await TempleteResolver?.Resolve(viewModel));
        }

        private byte[] GetSerializedResponseData(Response response)
        {
            return GetSerializedResponseData(response.Model);
        }

        private byte[] GetSerializedResponseData(object data)
        {
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