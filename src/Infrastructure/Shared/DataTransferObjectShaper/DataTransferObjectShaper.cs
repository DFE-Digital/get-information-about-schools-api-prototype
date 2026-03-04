using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjectShaper;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjectShaper.DataShapingRules;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjectShaper.Factories;
using System.Reflection;

/// <summary>
/// Shapes data transfer objects of type <typeparamref name="TDataObject"/> by applying
/// configured <see cref="IDataShapingRule"/> instances and selecting only the fields
/// explicitly requested by the caller.
/// </summary>
/// <typeparam name="TDataObject">The DTO type being shaped.</typeparam>
public sealed class DataTransferObjectShaper<TDataObject> : IDataTransferObjectShaper<TDataObject>
{
    private readonly IReadOnlyList<IDataShapingRule> _rules;
    private readonly PropertyInfo[] _properties;
    private readonly ITypeFactory _typeFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataTransferObjectShaper{TDataObject}"/> class.
    /// </summary>
    /// <param name="rules">
    /// The shaping rules to apply when transforming property values. Rules are evaluated
    /// in order, and the first rule that can shape a value is used.
    /// </param>
    /// <param name="typeFactory">
    /// A factory responsible for creating new instances of <typeparamref name="TDataObject"/>
    /// during the shaping process.
    /// </param>
    public DataTransferObjectShaper(
        IEnumerable<IDataShapingRule> rules,
        ITypeFactory typeFactory)
    {
        _rules = rules.ToList().AsReadOnly();
        _typeFactory = typeFactory;
        _properties = ReflectionCache.GetProperties(typeof(TDataObject));
    }

    /// <summary>
    /// Shapes a collection of DTOs by selecting only the specified fields and applying
    /// any matching <see cref="IDataShapingRule"/> transformations.
    /// </summary>
    /// <param name="dataObjects">The source DTOs to shape.</param>
    /// <param name="fields">
    /// An array of field names to include in the shaped output. If the array is null
    /// or contains no values, all public properties of <typeparamref name="TDataObject"/>
    /// are included.
    /// </param>
    /// <returns>
    /// A task containing a sequence of shaped <typeparamref name="TDataObject"/> instances.
    /// </returns>
    public Task<IEnumerable<TDataObject>> ShapeDataAsync(
        IEnumerable<TDataObject> dataObjects, HashSet<string> fields)
    {
        IEnumerable<TDataObject> shaped =
            dataObjects.Select(obj => ShapeObject(obj, fields));

        return Task.FromResult(shaped);
    }

    /// <summary>
    /// Shapes a single DTO by selecting only the specified fields and applying any
    /// matching <see cref="IDataShapingRule"/> transformations.
    /// </summary>
    /// <param name="dataObject">The source DTO to shape.</param>
    /// <param name="fields">
    /// An array of field names to include in the shaped output. If the array is null
    /// or contains no values, all public properties of <typeparamref name="TDataObject"/>
    /// are included.
    /// </param>
    /// <returns>
    /// A task containing the shaped <typeparamref name="TDataObject"/> instance.
    /// </returns>
    public Task<TDataObject> ShapeDataAsync(
        TDataObject dataObject, HashSet<string> fields) =>
            Task.FromResult(ShapeObject(dataObject, fields));

    /// <summary>
    /// Creates a shaped instance of <typeparamref name="TDataObject"/> by copying only
    /// the selected fields and applying any applicable shaping rules.
    /// </summary>
    /// <param name="source">The source DTO to shape.</param>
    /// <param name="fields">
    /// The set of field names to include. If empty, all properties are included.
    /// </param>
    private TDataObject ShapeObject(TDataObject source, HashSet<string> fields)
    {
        TDataObject shaped = _typeFactory.CreateInstance<TDataObject>();

        foreach (PropertyInfo prop in _properties)
        {
            if (fields.Count > 0 &&
                !fields.Contains(prop.Name, StringComparer.OrdinalIgnoreCase))
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
    /// Applies the first <see cref="IDataShapingRule"/> capable of shaping the given
    /// value. If no rule applies, <c>null</c> is returned.
    /// </summary>
    /// <param name="type">The property type being shaped.</param>
    /// <param name="value">The original property value.</param>
    /// <returns>
    /// The shaped value produced by the matching rule, or <c>null</c> if no rule applies.
    /// </returns>
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
}
