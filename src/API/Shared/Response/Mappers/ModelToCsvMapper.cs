using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response.Mappers.Options;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;
using Microsoft.Extensions.Options;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response.Mappers;

/// <summary>
/// Maps a domain <see cref="Establishment"/> instance into a CSV row
/// using column paths defined in <see cref="CsvMappingOptions"/>.
/// </summary>
/// <remarks>
/// This mapper uses simple reflection to walk property paths such as
/// <c>"Address.Postcode"</c>. Missing properties or null values are
/// converted to empty strings to ensure CSV stability.
/// </remarks>
public sealed class ModelToCsvMapper : IMapper<Establishment, string[]>
{
    private readonly CsvMappingOptions _csvMappingOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelToCsvMapper"/> class.
    /// </summary>
    /// <param name="csvMappingOptions">
    /// The CSV mapping configuration containing column paths and headers.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="csvMappingOptions"/> or its value is null.
    /// </exception>
    public ModelToCsvMapper(IOptions<CsvMappingOptions> csvMappingOptions)
    {
        _csvMappingOptions = csvMappingOptions?.Value
            ?? throw new ArgumentNullException(nameof(csvMappingOptions));

        if (_csvMappingOptions.Columns is null || _csvMappingOptions.Headers is null){
            throw new InvalidOperationException(
                "CSV mapping configuration is missing required fields.");
        }

        if (_csvMappingOptions.Columns.Length == 0){
            throw new InvalidOperationException(
                "CSV mapping configuration contains no column definitions.");
        }
    }

    /// <summary>
    /// Gets the CSV header row defined in configuration.
    /// </summary>
    public string[] Headers => _csvMappingOptions.Headers;

    /// <summary>
    /// Maps the supplied <see cref="Establishment"/> into a CSV row.
    /// </summary>
    /// <param name="model">The establishment domain model to map.</param>
    /// <returns>
    /// An array of string values representing the CSV row.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="model"/> is null.
    /// </exception>
    public string[] Map(Establishment model)
    {
        ArgumentNullException.ThrowIfNull(model);

        string[] values = new string[_csvMappingOptions.Columns.Length];

        for (int i = 0; i < _csvMappingOptions.Columns.Length; i++)
        {
            string columnPath = _csvMappingOptions.Columns[i];
            string? value = GetValue(model, columnPath);
            values[i] = value ?? string.Empty;
        }

        return values;
    }

    /// <summary>
    /// Walks a dotted property path (e.g. <c>"Address.Postcode"</c>)
    /// and returns the corresponding value from the supplied object.
    /// </summary>
    /// <param name="obj">The root object to evaluate.</param>
    /// <param name="path">The dotted property path.</param>
    /// <returns>
    /// The string representation of the resolved value, or <c>null</c>
    /// if any part of the path cannot be resolved.
    /// </returns>
    private static string? GetValue(object? obj, string path)
    {
        if (obj is null){
            return null;
        }

        const char PropertySeparator = '.';
        string[] parts = path.Split(PropertySeparator);
        object? current = obj;

        foreach (string part in parts)
        {
            if (current is null){
                return null;
            }

            Type type = current.GetType();
            System.Reflection.PropertyInfo? property = type.GetProperty(part);

            if (property is null){
                return null;
            }

            current = property.GetValue(current);
        }

        return current?.ToString();
    }
}
