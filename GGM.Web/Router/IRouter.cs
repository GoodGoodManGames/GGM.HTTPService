using System.Net;

namespace GGM.Web.Router
{
    /// <summary>
    ///     Request에 따라 적절한 콜백을 실행하는 Router의 인터페이스입니다.
    /// </summary>
    public interface IRouter
    {
        /// <summary>
        ///     Controller를 등록합니다.
        /// </summary>
        void RegisterController(object controller);

        /// <summary>
        ///     입력하는 Request에 맞는 콜백을 실행시키고 결과를 반환합니다.
        /// </summary>
        /// <param name="request">해당 루틴의 request</param>
        /// <returns>응답에 보내질 내용</returns>
        object Route(HttpListenerRequest request);
    }
}
