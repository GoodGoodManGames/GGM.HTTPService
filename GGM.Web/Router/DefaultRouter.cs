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
using static System.Reflection.Emit.OpCodes;

namespace GGM.Web.Router
{
    public class DefaultRouter : IRouter
    {
        public delegate object RouterCallback(HttpListenerRequest request, RouteInfo routeInfo);

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
        public object Route(HttpListenerRequest request)
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
            return targetRouteInfo.RouterCallback(request, targetRouteInfo);
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
                , new[] {typeof(HttpListenerRequest), typeof(RouteInfo)}
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
                if (parameterInfo.ParameterType == typeof(HttpListenerRequest))
                {
                    il.Emit(Ldarg_0); // HttpListenerRequest 로드
                    continue;
                }

                var pathAttribute = parameterInfo.GetCustomAttribute<PathAttribute>();
                Debug.Assert(pathAttribute != null);

                il.Emit(Ldloc, declarePathValues); // { [PathValues] }
                il.Emit(Ldstr, pathAttribute.Path); // { [PathValues] [Path] }
                il.Emit(Call, typeof(DictionaryUtil)
                    .GetMethod(nameof(DictionaryUtil.GetValue))
                    .MakeGenericMethod(typeof(string), typeof(string))); // { [Value] }
            }

            il.Emit(Call, methodInfo); // [result]
            il.Emit(Ret);
            return dynamicMethod.CreateDelegate(typeof(RouterCallback)) as RouterCallback;
        }

        #endregion CreateRouterCallback
    }
}