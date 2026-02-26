using System.Collections.Concurrent;
using System.Reflection;

/// <summary>
/// Provides cached reflection metadata used during data‑shaping operations.
/// </summary>
/// <remarks>
/// Reflection is relatively expensive, especially when performed repeatedly across
/// many objects or within recursive shaping rules. This cache stores the public
/// instance properties of each type the first time they are requested, ensuring
/// subsequent lookups are fast and allocation‑free.
/// </remarks>
internal static class ReflectionCache
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> Cache = new();

    /// <summary>
    /// Retrieves the cached public instance properties for the specified type.
    /// </summary>
    /// <param name="type">The type whose properties should be retrieved.</param>
    /// <returns>
    /// An array of <see cref="PropertyInfo"/> representing the public instance
    /// properties of the given <paramref name="type"/>. If the type has not been
    /// encountered before, its properties are retrieved via reflection and stored
    /// for future use.
    /// </returns>
    public static PropertyInfo[] GetProperties(Type type) =>
        Cache.GetOrAdd(
            type,
            type => type.GetProperties(
                BindingFlags.Public |
                BindingFlags.Instance));
}
