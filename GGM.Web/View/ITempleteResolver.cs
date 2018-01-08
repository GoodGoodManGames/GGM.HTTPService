using System.Threading.Tasks;

namespace GGM.Web.View
{
    /// <summary>
    ///     입력받은 Model을 Templete에 매핑하는 역할을 수행하는 TempleteResolver의 인터페이스입니다.
    ///     사용자는 사용할 TempleteEngine을 해당 인터페이스로 구현한 클래스에 감싸 사용합니다.
    /// </summary>
    public interface ITempleteResolver
    {
        /// <summary>
        ///     입력받은 모델을 템플릿에 
        /// </summary>
        /// <param name="viewModel">Resolved에 쓰일 모델</param>
        /// <returns>Resolved된 문자열</returns>
        Task<string> Resolve(ViewModel viewModel);
    }
}