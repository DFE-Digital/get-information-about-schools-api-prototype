using DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response.Mappers.Extensions;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response.Mappers.Options;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response.Mappers;

/// <summary>
/// Maps a model of type <typeparamref name="TModel"/> into one or more CSV rows.
/// Supports:
/// <list type="bullet">
///   <item><description>Scalar properties</description></item>
///   <item><description>Nested object properties</description></item>
///   <item><description>Collection expansion into multiple rows</description></item>
/// </list>
/// </summary>
/// <typeparam name="TModel">The model type being mapped.</typeparam>
public sealed class ModelToCsvMapper<TModel> : ICsvMapper<TModel>
{
    private readonly CsvMappingOptions _options;
    private readonly List<PropertyInfo[]> _propertyChains;

    /// <summary>
    /// Creates a new instance of the CSV mapper using the configured mapping dictionary.
    /// </summary>
    /// <param name="allMappings">The full mapping dictionary injected via configuration.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no mapping exists for <typeparamref name="TModel"/> or the mapping is invalid.
    /// </exception>
    public ModelToCsvMapper(IOptions<CsvMappingDictionary> allMappings)
    {
        ArgumentNullException.ThrowIfNull(allMappings?.Value);

        string key = typeof(TModel).Name;

        if (!allMappings.Value.TryGetValue(key, out CsvMappingOptions? options))
            throw new InvalidOperationException($"No CSV mapping configuration found for model type '{key}'.");

        if (!options.IsValid)
            throw new InvalidOperationException($"CSV mapping for '{key}' is invalid: {options.ValidationError}");

        _options = options;
        _propertyChains = BuildPropertyChains(options.Columns);
    }

    /// <summary>
    /// Gets the CSV header row defined in the mapping configuration.
    /// </summary>
    public string[] Headers => _options.Headers;

    /// <summary>
    /// Maps a model instance into one or more CSV rows.
    /// </summary>
    /// <param name="model">The model instance to map.</param>
    /// <returns>An enumerable of CSV rows.</returns>
    public IEnumerable<string[]> Map(TModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return model.ExpandRows(_propertyChains);
    }

    /// <summary>
    /// Builds a list of property chains from configured property paths.
    /// Each chain represents a sequence of properties to traverse when extracting values.
    /// </summary>
    /// <param name="paths">The property paths defined in the mapping configuration.</param>
    /// <returns>A list of property chains.</returns>
    private static List<PropertyInfo[]> BuildPropertyChains(string[] paths)
    {
        // Constants used only within this method
        const char PropertySeparator = '.';
        const string CollectionMarker = "[]";
        const int CollectionMarkerLength = 2;

        List<PropertyInfo[]> chains = new(paths.Length);

        foreach (string path in paths)
        {
            string[] parts = path.Split(PropertySeparator);
            Type currentType = typeof(TModel);
            List<PropertyInfo> chain = [];

            foreach (string rawPart in parts)
            {
                bool isCollectionMarker = rawPart.EndsWith(CollectionMarker, StringComparison.Ordinal);
                string part = isCollectionMarker
                    ? rawPart[..^CollectionMarkerLength]
                    : rawPart;

                PropertyInfo? property = currentType.GetProperty(part);
                if (property is null)
                {
                    // Invalid path: store an empty chain to preserve index alignment
                    chain.Clear();
                    break;
                }

                chain.Add(property);
                currentType = property.PropertyType;

                // If the property is a collection, move to its element type
                if (isCollectionMarker || currentType.IsCollectionType())
                {
                    Type? elementType = currentType.GetGenericArguments().FirstOrDefault();
                    if (elementType != null)
                        currentType = elementType;
                }
            }

            chains.Add([.. chain]);
        }

        return chains;
    }
}
