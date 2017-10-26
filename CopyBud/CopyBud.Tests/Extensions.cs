using System.Collections.Generic;


namespace CopyBud.Tests
{
    public static class ICollectionEx
        {
        public static T AddRange<T, I>(this T collection, IEnumerable<I> sequence) where T : ICollection<I>
            {
            foreach (var element in sequence)
                collection.Add(element);
            return collection;
            }
        }
}
