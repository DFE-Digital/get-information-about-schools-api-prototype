using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjectShaper;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjectShaper.DataShapingRules;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjectShaper.Factories;
using System.Reflection;

/// <summary>
/// Shapes data transfer objects by applying a set of <see cref="IDataShapingRule"/> rules
/// and selecting only the requested fields.
/// </summary>
/// <typeparam name="TDataObject">The type of DTO being shaped.</typeparam>
public sealed class DataTransferObjectShaper<TDataObject> : IDataTransferObjectShaper<TDataObject>
{
    private readonly IReadOnlyList<IDataShapingRule> _rules;
    private readonly PropertyInfo[] _properties;
    private readonly ITypeFactory _typeFactory;

    /// <summary>
    /// Creates a new instance of <see cref="DataTransferObjectShaper{TDataObject}"/>.
    /// </summary>
    /// <param name="rules">The shaping rules to apply.</param>
    /// <param name="typeFactory">Factory used to create shaped DTO instances.</param>
    public DataTransferObjectShaper(
        IEnumerable<IDataShapingRule> rules,
        ITypeFactory typeFactory)
    {
        _rules = rules.ToList().AsReadOnly();
        _typeFactory = typeFactory;
        _properties =
            ReflectionCache
                .GetProperties(typeof(TDataObject));
    }

    /// <summary>
    /// Shapes a collection of DTOs according to the specified field list.
    /// </summary>
    /// <param name="dataObjects">The source DTOs.</param>
    /// <param name="fields">Comma‑separated list of fields to include, or null for all fields.</param>
    /// <returns>A shaped collection of DTOs.</returns>
    public Task<IEnumerable<TDataObject>> ShapeDataAsync(
        IEnumerable<TDataObject> dataObjects, string? fields)
    {
        IEnumerable<TDataObject> shaped =
            dataObjects.Select(obj => ShapeObject(obj, fields));

        return Task.FromResult(shaped);
    }

    /// <summary>
    /// Shapes a single DTO according to the specified field list.
    /// </summary>
    /// <param name="dataObject">The source DTO.</param>
    /// <param name="fields">Comma‑separated list of fields to include, or null for all fields.</param>
    /// <returns>A shaped DTO.</returns>
    public Task<TDataObject> ShapeDataAsync(
        TDataObject dataObject, string? fields) =>
            Task.FromResult(ShapeObject(dataObject, fields));

    /// <summary>
    /// Applies field selection and shaping rules to a single DTO instance.
    /// </summary>
    private TDataObject ShapeObject(TDataObject source, string? fields)
    {
        HashSet<string> selected = ParseFields(fields);
        TDataObject shaped = _typeFactory.CreateInstance<TDataObject>();

        foreach (PropertyInfo prop in _properties)
        {
            if (selected.Count > 0 &&
                !selected.Contains(prop.Name, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            object? value = prop.GetValue(source);
            object? shapedValue = Normalize(prop.PropertyType, value);
            prop.SetValue(shaped, shapedValue);
        }

        return shaped!;
    }

    /// <summary>
    /// Applies the first matching <see cref="IDataShapingRule"/> to the given value.
    /// </summary>
    private object? Normalize(Type type, object? value)
    {
        foreach (IDataShapingRule rule in _rules)
        {
            if (rule.CanShape(type, value))
            {
                return rule.Shape(type, value, Normalize);
            }
        }

        return null;
    }

    /// <summary>
    /// Parses a comma‑separated list of field names into a case‑insensitive set.
    /// </summary>
    /// <param name="fields">The raw field list.</param>
    /// <returns>A set of selected field names.</returns>
    private static HashSet<string> ParseFields(string? fields)
    {
        const char Delimiter = ',';

        if (string.IsNullOrWhiteSpace(fields))
        {
            return new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);
        }

        return fields
            .Split(Delimiter,
                StringSplitOptions.TrimEntries |
                StringSplitOptions.RemoveEmptyEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
