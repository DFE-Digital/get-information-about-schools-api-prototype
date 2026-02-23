using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjectShaper.Factories;
using System.Collections;
using System.Reflection;

namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjectShaper;

/// <summary>
/// Provides functionality for shaping instances of <typeparamref name="TDataObject"/> 
/// into new objects of the same type, containing only the fields requested.
/// Unselected fields are left at their default values.
/// </summary>
/// <typeparam name="TDataObject">The type being shaped.</typeparam>
public class DefaultDataShaper<TDataObject> : IDataShaper<TDataObject>
{
    private readonly PropertyInfo[] _properties;
    private readonly ITypeFactory _typeFactory;
    private readonly ICollectionFactory _collectionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultDataShaper{TDataObject}"/> class.
    /// </summary>
    /// <param name="typeFactory">Factory used to create instances of objects and nested types.</param>
    /// <param name="collectionFactory">Factory used to create and populate collection instances.</param>
    public DefaultDataShaper(
        ITypeFactory typeFactory,
        ICollectionFactory collectionFactory)
    {
        _typeFactory = typeFactory;
        _collectionFactory = collectionFactory;

        _properties =
            typeof(TDataObject)
                .GetProperties(
                    BindingFlags.Public |
                    BindingFlags.Instance);
    }

    /// <summary>
    /// Shapes a sequence of <typeparamref name="TDataObject"/> instances by including
    /// only the specified fields. A new instance is created for each source object.
    /// </summary>
    /// <param name="dataObjects">The collection of objects to shape.</param>
    /// <param name="fields">
    /// A comma-separated list of field names to include. If null or empty,
    /// all public properties are included.
    /// </param>
    /// <returns>
    /// A task containing a sequence of shaped <typeparamref name="TDataObject"/> instances.
    /// </returns>
    public Task<IEnumerable<TDataObject>> ShapeDataAsync(
        IEnumerable<TDataObject> dataObjects, string? fields)
    {
        IEnumerable<TDataObject> shaped =
            dataObjects
                .Select(obj => ShapeObject(obj, fields))
                .Where(obj => obj is not null)!;

        return Task.FromResult(shaped);
    }

    /// <summary>
    /// Shapes a single <typeparamref name="TDataObject"/> instance by including
    /// only the specified fields. A new instance is created and populated.
    /// </summary>
    /// <param name="dataObject">The object to shape.</param>
    /// <param name="fields">
    /// A comma-separated list of field names to include. If null or empty,
    /// all public properties are included.
    /// </param>
    /// <returns>
    /// A task containing the shaped <typeparamref name="TDataObject"/> instance.
    /// </returns>
    public Task<TDataObject> ShapeDataAsync(
        TDataObject dataObject, string? fields)
    {
        TDataObject? shaped = ShapeObject(dataObject, fields);
        return Task.FromResult(shaped);
    }

    /// <summary>
    /// Creates a shaped instance of <typeparamref name="TDataObject"/> by copying
    /// only the selected fields from the source object. Unselected fields remain default.
    /// </summary>
    /// <param name="source">The source object to shape.</param>
    /// <param name="fields">The fields to include.</param>
    /// <returns>A new shaped instance of <typeparamref name="TDataObject"/>.</returns>
    private TDataObject ShapeObject(TDataObject source, string? fields)
    {
        HashSet<string> selectedFields = ParseFields(fields);

        // Create a new instance of TDataObject using TypeFactory.
        TDataObject? shaped = _typeFactory.CreateInstance<TDataObject>();

        foreach (var property in _properties)
        {
            if (selectedFields.Count > 0 &&
                !selectedFields.Contains(
                    property.Name, StringComparer.OrdinalIgnoreCase))
            {
                // Leave default/null.
                continue;
            }

            object? value = property.GetValue(source);
            object? shapedValue = NormalizeValue(property.PropertyType, value);

            property.SetValue(shaped, shapedValue);
        }

        return shaped;
    }

    /// <summary>
    /// Parses a comma-separated list of field names into a case-insensitive set.
    /// An empty set indicates that all fields should be included.
    /// </summary>
    /// <param name="fields">The raw field list.</param>
    /// <returns>A set of normalized field names.</returns>
    private static HashSet<string> ParseFields(string? fields)
    {
        if (string.IsNullOrWhiteSpace(fields))
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        const char FieldSeparator = ',';

        return fields
            .Split(FieldSeparator,
                StringSplitOptions.TrimEntries |
                StringSplitOptions.RemoveEmptyEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Normalizes a value for assignment into a shaped object. Handles primitives,
    /// complex objects, and collections recursively.
    /// </summary>
    /// <param name="targetType">The expected target type.</param>
    /// <param name="value">The value to normalize.</param>
    /// <returns>A normalized value suitable for assignment.</returns>
    private object? NormalizeValue(Type targetType, object? value)
    {
        if (value is null)
            return null;

        // Primitive or simple types.
        if (targetType.IsPrimitive ||
            targetType == typeof(string) ||
            targetType == typeof(DateTime) ||
            targetType == typeof(Guid) ||
            targetType == typeof(decimal))
        {
            return value;
        }

        // Collections
        if (typeof(IEnumerable).IsAssignableFrom(targetType) &&
            targetType != typeof(string))
        {
            Type? elementType =
                targetType.IsArray
                    ? targetType.GetElementType()
                    : targetType.GetGenericArguments().FirstOrDefault();

            elementType ??= typeof(object);

            List<object?> items = [];

            foreach (var item in (IEnumerable)value)
            {
                items.Add(NormalizeValue(elementType, item));
            }

            return _collectionFactory.CreateListInstance(
                listType: targetType,
                elementType: elementType,
                items: [.. items!]);
        }

        // Complex object → recursively shape into a new instance
        object? nested = _typeFactory.CreateInstance(targetType);

        foreach (PropertyInfo? prop in
            targetType.GetProperties(
                BindingFlags.Public |
                BindingFlags.Instance))
        {
            object? propValue = prop.GetValue(value);
            object? shapedValue = NormalizeValue(prop.PropertyType, propValue);
            prop.SetValue(nested, shapedValue);
        }

        return nested;
    }
}
