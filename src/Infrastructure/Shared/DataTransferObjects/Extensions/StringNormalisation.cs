namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjects.Extensions;

/// <summary>
/// Provides extension methods for applying consistent string normalisation
/// across data transfer objects.
/// </summary>
public static class StringNormalisation
{
    /// <summary>
    /// The default value returned when a string is null, empty,
    /// or consists only of whitespace characters.
    /// </summary>
    private const string DefaultValue = "UNDEFINED";

    /// <summary>
    /// Normalises a string by converting null, empty, or whitespace-only
    /// values into a predefined default value.
    /// </summary>
    /// <param name="value">
    /// The input string to normalise. May be null.
    /// </param>
    /// <returns>
    /// The original string if it contains non‑whitespace characters;
    /// otherwise, the default value <c>"UNDEFINED"</c>.
    /// </returns>
    /// <example>
    /// <code>
    /// string? input = "   ";
    /// string result = input.Normalise(); // returns "UNDEFINED"
    /// </code>
    /// </example>
    public static string Normalise(this string? value) =>
        string.IsNullOrWhiteSpace(value) ? DefaultValue : value;
}
