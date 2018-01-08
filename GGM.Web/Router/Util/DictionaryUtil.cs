using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GGM.Web.Router.Util
{
    internal static class DictionaryUtil
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue GetValue<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary[key];
        }
    }
}
