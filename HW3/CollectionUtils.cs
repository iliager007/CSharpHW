using System;
using System.Collections.Generic;

public static class CollectionUtils
{
    public static List<T> Distinct<T>(List<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        List<T> result = new();
        HashSet<T> seen = new();

        foreach (T item in source)
        {
            if (seen.Add(item))
            {
                result.Add(item);
            }
        }

        return result;
    }

    public static Dictionary<TKey, List<TValue>> GroupBy<TValue, TKey>(
        List<TValue> source,
        Func<TValue, TKey> keySelector) where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        Dictionary<TKey, List<TValue>> result = new();

        foreach (TValue item in source)
        {
            TKey key = keySelector(item);
            if (!result.TryGetValue(key, out List<TValue>? group))
            {
                group = new List<TValue>();
                result[key] = group;
            }

            group.Add(item);
        }

        return result;
    }

    public static Dictionary<TKey, TValue> Merge<TKey, TValue>(
        Dictionary<TKey, TValue> first,
        Dictionary<TKey, TValue> second,
        Func<TValue, TValue, TValue> conflictResolver) where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        ArgumentNullException.ThrowIfNull(conflictResolver);

        Dictionary<TKey, TValue> result = new();

        foreach (KeyValuePair<TKey, TValue> pair in first)
        {
            result[pair.Key] = pair.Value;
        }

        foreach (KeyValuePair<TKey, TValue> pair in second)
        {
            if (result.TryGetValue(pair.Key, out TValue? existingValue))
            {
                result[pair.Key] = conflictResolver(existingValue, pair.Value);
            }
            else
            {
                result[pair.Key] = pair.Value;
            }
        }

        return result;
    }

    public static T MaxBy<T, TKey>(List<T> source, Func<T, TKey> selector)
        where TKey : IComparable<TKey>
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);

        if (source.Count == 0)
        {
            throw new InvalidOperationException("Collection is empty.");
        }

        T maxItem = source[0];
        TKey maxKey = selector(maxItem);

        for (int i = 1; i < source.Count; i++)
        {
            T currentItem = source[i];
            TKey currentKey = selector(currentItem);

            if (currentKey.CompareTo(maxKey) > 0)
            {
                maxItem = currentItem;
                maxKey = currentKey;
            }
        }

        return maxItem;
    }
}
