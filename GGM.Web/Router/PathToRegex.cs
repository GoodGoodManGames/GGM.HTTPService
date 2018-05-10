using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using GGM.Web.Router.Exception;

namespace GGM.Web.Router
{
    public class PathToRegex
    {
        private const string ALL_CHAR_PATTERN = @"(\w+)";
        private const string END_CHAR = @"$";

        public PathToRegex(string urlPattern)
        {
            UrlPattern = urlPattern;
            var result = URLToPattern(UrlPattern);
            Keys = result.keys;
            Regex = new Regex(result.pattern, RegexOptions.Compiled);
        }

        public string UrlPattern { get; }
        public Regex Regex { get; }
        public string[] Keys { get; }
        
        public bool IsMatched(string url)
        {
            return Regex.IsMatch(url);
        }

        /// <summary>
        ///     url을 Regex에 맞게 처리하여 PathValues로 바꾸어 반환한다.
        /// </summary>
        /// <param name="url">요청 url</param>
        /// <returns>url을 정규표현식으로 값을 추출한 뒤 Key와 매핑한 값, 만일 Key가 없을시 null을 반환한다.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Dictionary<string, string> Execute(string url)
        {
            // 템플릿이 없는 패턴인 경우 굳이 PathValues를 만들어 주지 않는다.
            if (Keys.Length == 0)
                return null;

            var matches = Regex.Matches(url);
            var matchMap = new Dictionary<string, string>(matches.Count);
            
            var match = matches.First();
            var group = match.Groups;

            // Groups의 첫번째 요소는 전체 매칭된 값이므로 무시하여야 한다.
            for (int i = 1; i < group.Count; i++)
                matchMap.Add(Keys[i-1], group[i].Value);
            return matchMap;
        }

        public static (string pattern, string[] keys) URLToPattern(string urlPattern)
        {
            string pattern = urlPattern;
            var keys = new List<string>();
            int leftBraceIndex = -1;
            while ((leftBraceIndex = pattern.IndexOf('{', leftBraceIndex + 1)) != -1)
            {
                int rightBraceIndex = pattern.IndexOf('}', leftBraceIndex);
                if (rightBraceIndex == -1)
                    throw new WrongPatternException(urlPattern);

                var key = pattern.Substring(leftBraceIndex + 1, rightBraceIndex - leftBraceIndex -1);
                keys.Add(key);
                pattern = pattern.Remove(leftBraceIndex, rightBraceIndex - leftBraceIndex + 1);
                pattern = pattern.Insert(leftBraceIndex, ALL_CHAR_PATTERN);    
            }
            pattern = pattern + END_CHAR;
            return (pattern, keys.ToArray());
        }
    }
}