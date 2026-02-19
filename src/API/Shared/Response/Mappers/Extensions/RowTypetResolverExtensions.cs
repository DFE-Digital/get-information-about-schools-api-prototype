using System.Collections;
using System.Reflection;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response.Mappers.Extensions;

/// <summary>
/// Provides extension methods for resolving CSV rows from a model instance
/// using precomputed property chains.
/// </summary>
/// <remarks>
/// These helpers are used internally by the CSV mapping engine to expand
/// scalar and collection properties into one or more CSV rows.
/// </remarks>
public static class RowTypetResolverExtensions
{
    /// <summary>
    /// Expands a model instance into one or more CSV rows based on the supplied
    /// property chains. Collection properties generate multiple rows.
    /// </summary>
    /// <param name="model">The model instance being mapped.</param>
    /// <param name="chains">The property chains representing each CSV column.</param>
    /// <returns>
    /// An enumerable sequence of CSV rows, where each row is represented as a
    /// <see cref="string"/> array.
    /// </returns>
    public static IEnumerable<string[]> ExpandRows(this object model, List<PropertyInfo[]> chains)
    {
        List<object?> resolved = ResolveTopLevelValues(model, chains);
        List<int> collectionIndexes = GetCollectionColumnIndexes(resolved);

        // No collections → single row
        if (collectionIndexes.Count == 0)
        {
            yield return BuildScalarRow(resolved);
            yield break;
        }

        // Use the first collection column as the primary row-expanding source
        int primaryIndex = collectionIndexes[0];

        if (resolved[primaryIndex] is not IEnumerable primaryCollection)
            yield break;

        foreach (object? element in primaryCollection)
        {
            yield return BuildRow(resolved, chains, collectionIndexes, element);
        }
    }

    /// <summary>
    /// Resolves the top-level values for each property chain without expanding collections.
    /// </summary>
    /// <param name="model">The model instance being evaluated.</param>
    /// <param name="chains">The property chains to evaluate.</param>
    /// <returns>
    /// A list of resolved values corresponding to each property chain.
    /// </returns>
    public static List<object?> ResolveTopLevelValues(this object model, List<PropertyInfo[]> chains)
    {
        List<object?> resolved = new(chains.Count);

        foreach (PropertyInfo[] chain in chains)
        {
            object? value = ResolveValue(model, chain);
            resolved.Add(value);
        }

        return resolved;
    }

    /// <summary>
    /// Builds a CSV row for a specific element of the primary collection column.
    /// </summary>
    /// <param name="resolved">The previously resolved top-level values.</param>
    /// <param name="chains">The property chains for each column.</param>
    /// <param name="collectionIndexes">Indexes of columns that represent collections.</param>
    /// <param name="element">The current element from the primary collection.</param>
    /// <returns>A fully resolved CSV row.</returns>
    private static string[] BuildRow(
        List<object?> resolved,
        List<PropertyInfo[]> chains,
        List<int> collectionIndexes,
        object? element)
    {
        string[] row = new string[resolved.Count];

        for (int col = 0; col < resolved.Count; col++)
        {
            // Scalar column → reuse resolved value
            if (!collectionIndexes.Contains(col))
            {
                row[col] = resolved[col]?.ToString() ?? string.Empty;
                continue;
            }

            // Collection column → resolve value from the element
            PropertyInfo[] fullChain = chains[col];
            PropertyInfo[] postChain = GetPostCollectionChain(fullChain);

            object? value = ResolveElementValue(element, postChain);
            row[col] = value?.ToString() ?? string.Empty;
        }

        return row;
    }

    /// <summary>
    /// Builds a single CSV row when no collections are present.
    /// </summary>
    /// <param name="resolved">The resolved scalar values.</param>
    /// <returns>A single CSV row.</returns>
    private static string[] BuildScalarRow(List<object?> resolved)
    {
        string[] row = new string[resolved.Count];

        for (int i = 0; i < resolved.Count; i++)
        {
            row[i] = resolved[i]?.ToString() ?? string.Empty;
        }

        return row;
    }

    /// <summary>
    /// Resolves a value by walking a property chain from the root model.
    /// Stops early if a collection property is encountered.
    /// </summary>
    /// <param name="model">The model instance.</param>
    /// <param name="chain">The property chain to evaluate.</param>
    /// <returns>
    /// The resolved value, or <c>null</c> if any part of the chain evaluates to null.
    /// </returns>
    private static object? ResolveValue(object model, PropertyInfo[] chain)
    {
        object? current = model;

        foreach (PropertyInfo property in chain)
        {
            if (current is null)
                return null;

            // Stop at collection boundary
            if (property.PropertyType.IsCollectionType())
                return property.GetValue(current);

            current = property.GetValue(current);
        }

        return current;
    }

    /// <summary>
    /// Extracts the portion of a property chain that occurs after the collection property.
    /// </summary>
    /// <param name="fullChain">The full property chain.</param>
    /// <returns>
    /// A sub-chain representing the properties to evaluate on each collection element.
    /// </returns>
    private static PropertyInfo[] GetPostCollectionChain(PropertyInfo[] fullChain)
    {
        int index =
            Array.FindLastIndex(fullChain, property =>
                property.PropertyType.IsCollectionType());

        return fullChain[(index + 1)..];
    }

    /// <summary>
    /// Identifies which resolved values represent collections and therefore
    /// require row expansion.
    /// </summary>
    /// <param name="resolved">The resolved top-level values.</param>
    /// <returns>
    /// A list of column indexes that contain collection values.
    /// </returns>
    private static List<int> GetCollectionColumnIndexes(List<object?> resolved)
    {
        List<int> indexes = [];

        for (int i = 0; i < resolved.Count; i++)
        {
            object? value = resolved[i];

            if (value is IEnumerable && value is not string)
                indexes.Add(i);
        }

        return indexes;
    }

    /// <summary>
    /// Resolves a value from a collection element by walking the remaining property chain.
    /// </summary>
    /// <param name="element">The current collection element.</param>
    /// <param name="postChain">The property chain to evaluate on the element.</param>
    /// <returns>
    /// The resolved value, or <c>null</c> if any part of the chain evaluates to null.
    /// </returns>
    private static object? ResolveElementValue(object? element, PropertyInfo[] postChain)
    {
        object? current = element;

        foreach (PropertyInfo property in postChain)
        {
            if (current is null)
                return null;

            current = property.GetValue(current);
        }

        return current;
    }
}
