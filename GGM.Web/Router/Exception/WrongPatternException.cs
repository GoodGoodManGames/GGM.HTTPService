namespace GGM.Web.Router.Exception
{
    public class WrongPatternException : System.Exception
    {
        public WrongPatternException() : this(null) { }

        public WrongPatternException(string urlPattern)
            : base($"입력값 : {urlPattern}은 잘못된 패턴입니다. GGMWeb은 {{key}} 형식의 패턴만을 지원합니다.") { }
    }
}
