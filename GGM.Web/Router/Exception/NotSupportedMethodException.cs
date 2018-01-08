namespace GGM.Web.Router.Exception
{
    public class NotSupportedMethodException : System.Exception
    {
        public NotSupportedMethodException(string value)
            : base($"{value}는 지원하지 않는 HTTPMethod입니다.") { }
    }
}
