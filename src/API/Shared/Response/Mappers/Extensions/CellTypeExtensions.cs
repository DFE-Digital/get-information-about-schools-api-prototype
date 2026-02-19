using System.Collections;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response.Mappers.Extensions;

/// <summary>
/// Provides helper methods for determining the type characteristics of model properties
/// when mapping objects into CSV rows.
/// </summary>
public static class CellTypeExtensions
{
    /// <summary>
    /// Determines whether a given <see cref="Type"/> represents a collection type
    /// suitable for expansion into multiple CSV rows.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns>
    /// <c>true</c> if the type is a generic collection implementing <see cref="IEnumerable"/>,
    /// excluding <see cref="string"/> which is also enumerable but treated as a scalar;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method is used by the CSV mapping engine to detect when a property
    /// should be expanded into multiple rows. Strings are explicitly excluded
    /// because they implement <see cref="IEnumerable"/> but should be treated
    /// as atomic values rather than collections.
    /// </remarks>
    public static bool IsCollectionType(this Type type)
    {
        return type != typeof(string)
            && type.IsGenericType
            && typeof(IEnumerable).IsAssignableFrom(type);
    }
}