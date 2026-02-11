namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.ValidationServices;

/// <summary>
/// Represents the collection of validation patterns used by the application
/// to validate establishment‑related fields such as street names, towns,
/// postcodes, website URLs, and telephone numbers.
/// </summary>
/// <remarks>
/// <para>
/// This class is bound from configuration (typically <c>appsettings.json</c>)
/// and provides a flexible dictionary of named regular expression patterns.
/// </para>
/// <para>
/// Because the patterns are stored in a dictionary rather than as fixed
/// properties, new validation rules can be introduced without requiring
/// code changes—only configuration updates.
/// </para>
/// </remarks>
public sealed class ValidationPatterns
{
    /// <summary>
    /// Gets or sets the dictionary of validation patterns, keyed by a
    /// descriptive pattern name (e.g., <c>"Street"</c>, <c>"Town"</c>,
    /// <c>"Postcode"</c>, <c>"Website"</c>, <c>"Telephone"</c>).
    /// </summary>
    /// <value>
    /// A dictionary where each key identifies a validation rule and each value
    /// contains the corresponding regular expression pattern.
    /// </value>
    public Dictionary<string, string> Patterns { get; set; } = [];
}