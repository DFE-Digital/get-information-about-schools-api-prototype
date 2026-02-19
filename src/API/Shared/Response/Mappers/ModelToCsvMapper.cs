using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response.Mappers.Options;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response.Mappers;

/// <summary>
/// Maps any domain model into a CSV row using column paths defined in
/// <see cref="CsvMappingOptions"/>. The correct mapping is selected based on
/// the model type name (e.g. "Establishment", "EstablishmentGroup").
/// </summary>
/// <typeparam name="TModel">The domain model type being mapped.</typeparam>
public sealed class ModelToCsvMapper<TModel> : IMapper<TModel, string[]>
{
    private readonly CsvMappingOptions _options;
    private readonly List<PropertyInfo[]> _propertyChains;

    public ModelToCsvMapper(IOptions<CsvMappingDictionary> allMappings)
    {
        if (allMappings?.Value is null)
        {
            throw new ArgumentNullException(nameof(allMappings));
        }

        string key = typeof(TModel).Name;

        if (!allMappings.Value.TryGetValue(key, out CsvMappingOptions? options))
        {
            throw new InvalidOperationException(
                $"No CSV mapping configuration found for model type '{key}'. " +
                $"Ensure you have a CsvMappings:{key} section in configuration.");
        }

        _options = options;

        if (_options.Columns is null || _options.Headers is null)
        {
            throw new InvalidOperationException(
                $"CSV mapping for '{key}' is missing Columns or Headers.");
        }

        if (_options.Columns.Length == 0)
        {
            throw new InvalidOperationException(
                $"CSV mapping for '{key}' contains no column definitions.");
        }

        _propertyChains = BuildPropertyChains(_options.Columns);
    }

    /// <summary>
    /// Gets the CSV header row defined in configuration.
    /// </summary>
    public string[] Headers => _options.Headers;

    /// <summary>
    /// Maps the supplied model into a CSV row.
    /// </summary>
    public string[] Map(TModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        string[] values = new string[_propertyChains.Count];

        for (int i = 0; i < _propertyChains.Count; i++)
        {
            object? value = ResolveValue(model, _propertyChains[i]);
            values[i] = value?.ToString() ?? string.Empty;
        }

        return values;
    }

    /// <summary>
    /// Precomputes the property chains for each dotted path.
    /// </summary>
    private static List<PropertyInfo[]> BuildPropertyChains(string[] paths)
    {
        List<PropertyInfo[]> chains = new(paths.Length);

        foreach (string path in paths)
        {
            string[] parts = path.Split('.');
            Type currentType = typeof(TModel);

            List<PropertyInfo> chain = new();

            foreach (string part in parts)
            {
                PropertyInfo? property = currentType.GetProperty(part);

                if (property is null)
                {
                    // Invalid path → store empty chain so ResolveValue returns null
                    chain.Clear();
                    break;
                }

                chain.Add(property);
                currentType = property.PropertyType;
            }

            chains.Add(chain.ToArray());
        }

        return chains;
    }

    /// <summary>
    /// Walks a precomputed property chain and returns the value.
    /// </summary>
    private static object? ResolveValue(object model, PropertyInfo[] chain)
    {
        object? current = model;

        foreach (PropertyInfo property in chain)
        {
            if (current is null)
            {
                return null;
            }

            current = property.GetValue(current);
        }

        return current;
    }
}
