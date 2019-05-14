using System.Collections;
using System.Collections.Generic;

namespace SelectedTextSpeach
{
    public static class IListExtensions
    {
        public static List<T> ToList<T>(this IList source)
        {
            var list = new List<T>();
            foreach (var item in source)
            {
                if (item != null)
                {
                    list.Add((T)item);
                }
            }
            return list;
        }
    }
}
