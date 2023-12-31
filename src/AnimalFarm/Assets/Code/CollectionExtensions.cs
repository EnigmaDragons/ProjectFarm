using System;
using System.Collections.Generic;
using System.Linq;

public static class CollectionExtensions
{
    public static bool None<T>(this IEnumerable<T> items, Func<T, bool> condition) => !items.Any(condition);
    public static T[] AsArray<T>(this T item) => new [] {item};
    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action) => items.ToList().ForEach(action);
    public static IEnumerable<T> Concat<T>(this T item, IEnumerable<T> items)  => item.AsArray().Concat(items);
    public static IEnumerable<T> Concat<T>(this IEnumerable<T> items, T item) => items.Concat(item.AsArray());
    public static IEnumerable<T> ConcatIfNotNull<T>(this IEnumerable<T> items, T maybeItem) => maybeItem != null ? items.Concat(maybeItem) : items;
    public static IEnumerable<T> ConcatIf<T>(this IEnumerable<T> items, T item, Func<T, bool> condition) => item != null && condition(item) ? items.Concat(item) : items;
    public static IEnumerable<T> Except<T>(this IEnumerable<T> items, T item) => items.Except(item.AsArray());
    public static bool None<T>(this IEnumerable<T> items) => !items.Any();
    public static IEnumerable<T> WrappedWith<T>(this IEnumerable<T> items, T wrapping) => wrapping.AsArray().Concat(items).Concat(wrapping);
    public static Maybe<T> FirstAsMaybe<T>(this IEnumerable<T> items, Func<T, bool> condition) where T : class => new Maybe<T>(items.FirstOrDefault(condition));
    public static TValue ValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> d, TKey key, TValue defaultValue) => d.TryGetValue(key, out var value) ? value : defaultValue;
    public static TValue ValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> d, TKey key, Func<TValue> getDefault) => d.TryGetValue(key, out var value) ? value : getDefault();
    public static T Random<T>(this IEnumerable<T> t) => t.ToArray().Random();
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> t) => new HashSet<T>(t);
    public static Queue<T> ToQueue<T>(this IEnumerable<T> t) => new Queue<T>(t);
}
