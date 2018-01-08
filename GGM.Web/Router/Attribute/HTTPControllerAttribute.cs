using GGM.Context.Attribute;
using GGM.Context;

namespace GGM.Web.Router.Attribute
{
    /// <summary>
    ///     클래스가 HTTPController임을 지정하는 애트리뷰트입니다.
    ///     ManagedAttribute를 상속받아 대상 클래스를 Managed로 지정하며, Singleton 타입으로 보관합니다.
    /// </summary>
    public class HTTPControllerAttribute: ManagedAttribute
    {
        //TODO: Proto로 둘지 Singleton으로 둘 지 고려해 보아야함.
        public HTTPControllerAttribute() : base(ManagedType.Singleton) { }
    }
}
