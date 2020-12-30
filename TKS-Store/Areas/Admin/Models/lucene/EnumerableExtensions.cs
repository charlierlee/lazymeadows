using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucene.NET.HowTo.ExamplesFacade
{
    public static class EnumerableExtensions
    {
        public static void forEach<T>(this IEnumerable<T> enumerator, Action<T> a)
        {
            foreach (var i in enumerator) a(i);
        }

        public static IEnumerable<R> forEach<T, R>(this IEnumerable<T> enumerator, Func<T, R> f)
        {
            return enumerator.Select(f);
        }

        public static void forEachWord(this string phrase, Action<string> a)
        {
            phrase
                .Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                .forEach(a);
        }
    }
}
