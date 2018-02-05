using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GGM.Web.Router.Attribute;
using GGM.Web.Router.Util;
using GGM.Web.View;
using GGM.Serializer;
using static System.Reflection.Emit.OpCodes;
using System.Text;

namespace GGM.Web.Router
{
    public class DefaultRouter : IRouter
    {
        public delegate object RouterCallback(HttpListenerRequest request, RouteInfo routeInfo, ISerializer serializer);

        protected List<RouteInfo> RouteInfos { get; } = new List<RouteInfo>();

        /// <summary>
        ///     Controller를 등록합니다.
        ///     이는 각 메소드 위에 지정된 RouteAttribute에 따라 등록됩니다.
        /// </summary>
        /// <param name="controller"></param>
        public void RegisterController(object controller)
        {
            var controllerType = controller.GetType();
            var methodInfos = controllerType.GetMethods();
            var controlInfos = methodInfos
                .Select(GetRouteAttributePair)
                .Where(item => item.RouteAttribute != null);

            foreach (var controlInfo in controlInfos)
            {
                var methodInfo = controlInfo.Info;
                var callback = CreateDelegate(methodInfo, methodInfo.GetParameters());
                Debug.Assert(callback != null);

                var httpMethod = controlInfo.RouteAttribute.HTTPMethod;
                var pattern = controlInfo.RouteAttribute.URLPattern;
                var pathToRegex = new PathToRegex(pattern);
                RouteInfos.Add(new RouteInfo(controller, httpMethod, pathToRegex, callback));
            }
        }

        private static (MethodInfo Info, RouteAttribute RouteAttribute) GetRouteAttributePair(MethodInfo info)
        {
            return (info, info.GetCustomAttribute<RouteAttribute>());
        }

        /// <summary>
        ///     요청을 처리하여 매칭되는 RouterCallback을 호출합니다.
        /// </summary>
        /// <returns>Response에 클라에 보내질 문자열.</returns>
        public object Route(HttpListenerRequest request, ISerializer serializer)
        {
            var uri = request.Url;
            var url = uri.AbsolutePath;
            HTTPMethod method = request.GetHTTPMethod();

            RouteInfo targetRouteInfo = null;
            foreach (var routeInfo in RouteInfos)
            {
                if (!routeInfo.IsMatched(method, url))
                    continue;
                targetRouteInfo = routeInfo;
            }

            if (targetRouteInfo == null)
                return "wrong path";
            return targetRouteInfo.RouterCallback(request, targetRouteInfo, serializer);
        }

        #region CreateRouterCallback

        /// <summary>
        ///     Route 시 실행해줄 RouterCallback를 생성합니다.
        /// </summary>
        private RouterCallback CreateDelegate(MethodInfo methodInfo, ParameterInfo[] parameterInfos)
        {
            var dynamicMethod = new DynamicMethod(
                $"RouterCallback+{methodInfo.Name}:{Guid.NewGuid()}"
                , typeof(string)
                , new[] {typeof(HttpListenerRequest), typeof(RouteInfo), typeof(ISerializer)}
                , GetType());
            var il = dynamicMethod.GetILGenerator();
            il.Emit(Ldarg_1); // [RouteInfo]
            il.Emit(Dup); // [RouteInfo] [RouteInfo]
            il.Emit(Call,
                typeof(RouteInfo).GetProperty(nameof(RouteInfo.PathToRegex))
                    .GetGetMethod()); // [RouteInfo] [PathToRegex]
            il.Emit(Ldarg_0); // [RouteInfo] [PathToRegex] [HttpListenerRequest]
            il.Emit(Call,
                typeof(HttpListenerRequest).GetProperty(nameof(HttpListenerRequest.Url))
                    .GetGetMethod()); // [RouteInfo] [PathToRegex] [Uri]
            il.Emit(Call,
                typeof(Uri).GetProperty(nameof(Uri.AbsolutePath)).GetGetMethod()); // [RouteInfo] [PathToRegex] [Path]
            il.Emit(Call, typeof(PathToRegex).GetMethod(nameof(PathToRegex.Execute))); // [RouteInfo] [PathValues]
            var declarePathValues = il.DeclareLocal(typeof(Dictionary<string, string>));
            il.Emit(Stloc, declarePathValues); // [RouteInfo]
            il.Emit(Call, typeof(RouteInfo).GetProperty(nameof(RouteInfo.Controller)).GetGetMethod()); // [Controller]


            foreach (var parameterInfo in parameterInfos)
            {
                var parameterType = parameterInfo.ParameterType;

                if (parameterType == typeof(HttpListenerRequest))
                {
                    il.Emit(Ldarg_0); // HttpListenerRequest 로드
                    continue;
                }

                if (parameterInfo.IsDefined(typeof(BodyAttribute)))
                {
                    var methodAttribute = methodInfo.GetCustomAttribute<RouteAttribute>();
                    if (methodAttribute is GetAttribute ||
                        methodAttribute is DeleteAttribute)
                        throw new System.Exception("Doesn't have BodyData");

                    il.Emit(Ldarg_0); // [HttpListenerRequest]
                    il.Emit(Call, typeof(DefaultRouter).GetMethod(nameof(DefaultRouter.GetRequestBody))); // [requestBody]
                    var requestBody = il.DeclareLocal(typeof(byte[]));
                    il.Emit(Stloc, requestBody);

                    if (parameterType == typeof(byte[]))
                        il.Emit(Ldloc, requestBody); // [requestBody]
                    else if (parameterType == typeof(string))
                    {
                        il.Emit(Call, typeof(Encoding).GetProperty(nameof(Encoding.UTF8)).GetGetMethod()); // [Encoding]
                        il.Emit(Ldloc, requestBody); // [Encoding] [requestBody]
                        il.Emit(Call, typeof(Encoding).GetMethod(nameof(Encoding.GetString), new Type[] { typeof(byte[]) })); // [BodyToString]
                    }
                    else
                    {
                        if (parameterType.IsValueType)
                            throw new NotImplementedException();
                        il.Emit(Ldarg_2); // [GenericSerializer]
                        il.Emit(Ldloc, requestBody); // [GenericSerializer] [requestBody]
                        il.Emit(Call, typeof(ISerializer).GetMethod(nameof(ISerializer.Deserialize)).MakeGenericMethod(parameterType)); // [BodyToObject]
                    }
                    continue;
                }

                if (parameterInfo.IsDefined(typeof(RequestHeaderAttribute)))
                {
                    Label notNull = il.DefineLabel();
                    il.Emit(Ldarg_0); // [HttpRequest]
                    il.Emit(Call, typeof(HttpListenerRequest).GetProperty(nameof(HttpListenerRequest.Headers)).GetGetMethod()); // [HttpRequestHeaders]
                    var HttpHeaders = il.DeclareLocal(typeof(System.Collections.Specialized.NameValueCollection));
                    il.Emit(Stloc, HttpHeaders);

                    string requestHeader = parameterInfo.GetCustomAttribute<RequestHeaderAttribute>().RequestHeader;

                    il.Emit(Ldloc, HttpHeaders); // [HttpRequestHeaders]
                    il.Emit(Ldstr, requestHeader); // [HttpRequestHeadersKeys] [TargetRequestHeader]
                    il.Emit(Callvirt, typeof(System.Collections.Specialized.NameValueCollection).GetMethod(nameof(System.Collections.Specialized.NameValueCollection.Get), new Type[] { typeof(string) })); // [TagetHeaderValue]
                    var headerValue = il.DeclareLocal(typeof(string));
                    il.Emit(Stloc, headerValue);

                    il.Emit(Ldloc, headerValue); // [TargetHeaderValue]
                    il.Emit(Brtrue, notNull);
                    il.Emit(Ldstr, "There is no Key in RequestHeader"); // [exceptionString]
                    var exceptionConstructorInfo = typeof(System.Exception).GetConstructor(new Type[] { typeof(string) }); // [exceptionString]
                    il.Emit(Newobj, exceptionConstructorInfo); // [exceptionString] [ExceptionConstructor]
                    il.Emit(Throw);

                    il.MarkLabel(notNull);
                    il.Emit(Ldloc, headerValue);
                }

                if (parameterInfo.IsDefined(typeof(PathAttribute)))
                {
                    var pathAttribute = parameterInfo.GetCustomAttribute<PathAttribute>();
                    Debug.Assert(pathAttribute != null);

                    il.Emit(Ldloc, declarePathValues); // { [PathValues] }
                    il.Emit(Ldstr, pathAttribute.Path); // { [PathValues] [Path] }
                    il.Emit(Call, typeof(DictionaryUtil)
                        .GetMethod(nameof(DictionaryUtil.GetValue))
                        .MakeGenericMethod(typeof(string), typeof(string))); // { [Value] }
                }
            }

            il.Emit(Call, methodInfo); // [result]
            il.Emit(Ret);

            return dynamicMethod.CreateDelegate(typeof(RouterCallback)) as RouterCallback;
        }

        #endregion CreateRouterCallback

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GetRequestBody(HttpListenerRequest request)
        {
            using (System.IO.Stream inputStream = request.InputStream)
            {
                var requestBody = new byte[request.ContentLength64];
                inputStream.Read(requestBody, 0, (int)request.ContentLength64);
                return requestBody;
            }
        }
        
    }
}