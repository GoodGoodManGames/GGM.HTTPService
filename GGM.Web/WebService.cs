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
        public WebService(ITempleteResolverFactory resolverFactory, ISerializerFactory serializerFactory,
            string[] prefixes, params object[] controllers)
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
                        responseBody = await GetSerializedResponseData(result, response);
                        
                        if (responseBody != null)
                            await outputStream.WriteAsync(responseBody, 0, responseBody.Length);
                    }
                }
            }
        }

        private async Task<byte[]> GetSerializedResponseData(object data, HttpListenerResponse Response = null)
        {
            if (data is string)
                return Encoding.UTF8.GetBytes(data as string);
            else if (data is ViewModel)
                return Encoding.UTF8.GetBytes(await TempleteResolver?.Resolve(data as ViewModel));
            else if (data is Response)
            {
                var response = data as Response;
                foreach (var key in response.Header.Keys)
                    Response.AppendHeader(key, response.Header[key]);
                return await GetSerializedResponseData(response.Model);
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