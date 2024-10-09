using System.Linq;
using System.Collections.Generic;

namespace Enigmatic.Core
{
    public static class EnumerableExtantion
    {
        public static Queue<TSource> ToQueue<TSource>(this IEnumerable<TSource> sources)
        {
            Queue<TSource> queue = new Queue<TSource>(sources.Count());

            foreach (TSource source in sources)
                queue.Enqueue(source);

            return queue;
        }

        public static Queue<TSource> ToQueue<TSource>(this IEnumerable<TSource> sources, int startIndex)
        {
            Queue<TSource> queue = new Queue<TSource>(sources.Count());

            for (int i = (int)startIndex; i < sources.Count(); i++)
                queue.Enqueue(sources.ElementAt(i));

            return queue;
        }
    }
}
