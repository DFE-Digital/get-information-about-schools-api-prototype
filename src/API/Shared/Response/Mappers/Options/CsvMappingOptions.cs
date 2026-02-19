namespace DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response.Mappers.Options;

/// <summary>
/// Represents configuration for a single CSV mapping, including the ordered list of
/// column property paths and the corresponding CSV header names.
/// </summary>
/// <remarks>
/// This is one entry inside the CsvMappings dictionary, keyed by model type name.
/// </remarks>
public sealed class CsvMappingOptions
{
    /// <summary>
    /// Gets the dotted property paths used to extract values from the model.
    /// Example: "Address.Postcode".
    /// </summary>
    public string[] Columns { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Gets the CSV header names corresponding to <see cref="Columns"/>.
    /// </summary>
    public string[] Headers { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Indicates whether the configuration contains valid column and header definitions.
    /// </summary>
    public bool IsValid =>
        Columns.Length > 0 &&
        Headers.Length > 0 &&
        Columns.Length == Headers.Length;

    /// <summary>
    /// Provides a human-readable explanation of why the configuration is invalid.
    /// </summary>
    public string? ValidationError
    {
        get
        {
            if (Columns.Length == 0)
                return "No Columns defined.";

            if (Headers.Length == 0)
                return "No Headers defined.";

            if (Columns.Length != Headers.Length)
                return $"Columns count ({Columns.Length}) does not match Headers count ({Headers.Length}).";

            return null;
        }
    }
}