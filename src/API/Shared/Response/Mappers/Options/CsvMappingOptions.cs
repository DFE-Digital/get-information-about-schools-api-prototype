namespace DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response.Mappers.Options;

/// <summary>
/// Represents configuration for CSV mapping, including the ordered list of
/// column property paths and the corresponding CSV header names.
/// </summary>
/// <remarks>
/// The <see cref="Columns"/> array defines the dotted property paths used to
/// extract values from the domain model (e.g. <c>"Address.Postcode"</c>).
/// The <see cref="Headers"/> array defines the CSV header row.
/// Both arrays must be the same length.
/// </remarks>
public sealed class CsvMappingOptions
{
    /// <summary>
    /// Gets the dotted property paths used to extract values from the model.
    /// Example: <c>"Address.Postcode"</c>.
    /// </summary>
    public string[] Columns { get; init; } = [];

    /// <summary>
    /// Gets the CSV header names corresponding to <see cref="Columns"/>.
    /// </summary>
    public string[] Headers { get; init; } = [];

    /// <summary>
    /// Indicates whether the configuration contains valid column and header definitions.
    /// </summary>
    public bool IsValid =>
        Columns.Length > 0 &&
        Headers.Length > 0 &&
        Columns.Length == Headers.Length;
}
