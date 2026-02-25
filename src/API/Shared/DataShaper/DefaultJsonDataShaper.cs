using System.Reflection;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Shared.DataShaper;

/// <summary>
/// Provides functionality for shaping objects of type <typeparamref name="TDataObject"/> 
/// into dynamic <see cref="ShapedEntity"/> projections containing only the requested fields.
/// Supports nested objects, collections, and JSON-friendly shaping.
/// </summary>
/// <typeparam name="TDataObject">
/// The source type from which shaped projections are created.
/// </typeparam>
public class DefaultJsonDataShaper<TDataObject> : IDataShaper<TDataObject>
{
    private readonly PropertyInfo[] _properties;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataShaper{TDataObject}"/> class.
    /// Reflection metadata for the target type is cached for performance.
    /// </summary>
    public DefaultJsonDataShaper()
    {
        _properties = typeof(TDataObject)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);
    }

    /// <summary>
    /// Shapes a collection of objects by selecting only the specified fields.
    /// </summary>
    /// <param name="dataObjects">
    /// The collection of source objects to shape.
    /// </param>
    /// <param name="fields">
    /// A comma-separated list of field names to include in the shaped output.
    /// If <c>null</c> or empty, all public properties are included.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a sequence of 
    /// <see cref="ShapedEntity"/> instances with only the requested fields.
    /// </returns>
    public Task<IEnumerable<ShapedEntity>> ShapeDataAsync(
        IEnumerable<TDataObject> dataObjects, string? fields)
    {
        var shapedList = dataObjects.Select(obj => ShapeObject(obj, fields));
        return Task.FromResult(shapedList);
    }

    /// <summary>
    /// Shapes a single object by selecting only the specified fields.
    /// </summary>
    /// <param name="dataObject">
    /// The source object to shape.
    /// </param>
    /// <param name="fields">
    /// A comma-separated list of field names to include in the shaped output.
    /// If <c>null</c> or empty, all public properties are included.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a 
    /// <see cref="ShapedEntity"/> with only the requested fields.
    /// </returns>
    public Task<ShapedEntity> ShapeDataAsync(TDataObject dataObject, string? fields)
    {
        var shaped = ShapeObject(dataObject, fields);
        return Task.FromResult(shaped);
    }

    /// <summary>
    /// Shapes a single object into a <see cref="ShapedEntity"/> by selecting only the requested fields.
    /// Handles nested objects and collections recursively.
    /// </summary>
    /// <param name="source">The object to shape.</param>
    /// <param name="fields">The fields to include, or <c>null</c> for all fields.</param>
    /// <returns>A shaped entity containing the selected fields.</returns>
    private ShapedEntity ShapeObject(TDataObject source, string? fields)
    {
        var shaped = new ShapedEntity();
        var selectedFields = ParseFields(fields);

        foreach (var property in _properties)
        {
            if (selectedFields.Count > 0 &&
                !selectedFields.Contains(property.Name, StringComparer.OrdinalIgnoreCase))
                continue;

            var value = property.GetValue(source);
            shaped[property.Name] = NormalizeValue(value);
        }

        return shaped;
    }

    /// <summary>
    /// Parses a comma-separated list of field names into a case-insensitive set.
    /// </summary>
    /// <param name="fields">
    /// The raw field string, or <c>null</c> to indicate all fields.
    /// </param>
    /// <returns>
    /// A set of field names. An empty set indicates that all fields should be included.
    /// </returns>
    private static HashSet<string> ParseFields(string? fields)
    {
        if (string.IsNullOrWhiteSpace(fields))
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        const char FieldSeparator = ',';

        return fields
            .Split(FieldSeparator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Normalizes a value for inclusion in a <see cref="ShapedEntity"/>.
    /// Handles primitives, complex objects, and collections.
    /// </summary>
    /// <param name="value">The value to normalize.</param>
    /// <returns>
    /// A primitive value, a nested <see cref="ShapedEntity"/>, or a collection of normalized values.
    /// </returns>
    private static object? NormalizeValue(object? value)
    {
        if (value is null)
            return null;

        // Primitive or simple types
        if (value.GetType().IsPrimitive ||
            value is string ||
            value is DateTime ||
            value is Guid ||
            value is decimal)
        {
            return value;
        }

        // Collections
        if (value is IEnumerable<object> list)
        {
            return list.Select(NormalizeValue).ToList();
        }

        // Complex object → nested ShapedEntity
        var nested = new ShapedEntity();
        var props = value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in props)
        {
            nested[prop.Name] = NormalizeValue(prop.GetValue(value));
        }

        return nested;
    }
}
