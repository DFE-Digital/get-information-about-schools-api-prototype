using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjectShaper.Factories;
using System.Collections;

namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjectShaper.DataShapingRules;

/// <summary>
/// Provides shaping logic for complex object types by creating a new instance of the
/// target type and recursively shaping each of its public properties.
/// </summary>
/// <remarks>
/// This rule applies to non‑primitive, non‑string, non‑collection types. It is typically
/// used to shape nested DTOs or domain objects where each property may itself require
/// shaping via other <see cref="IDataShapingRule"/> implementations.
/// </remarks>
public sealed class ComplexObjectShapingRule : IDataShapingRule
{
    private readonly ITypeFactory _typeFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComplexObjectShapingRule"/> class.
    /// </summary>
    /// <param name="typeFactory">
    /// Factory used to create new instances of complex object types during shaping.
    /// </param>
    public ComplexObjectShapingRule(ITypeFactory typeFactory)
    {
        _typeFactory = typeFactory;
    }

    /// <summary>
    /// Determines whether this rule can shape the specified value into the target type.
    /// </summary>
    /// <param name="targetType">The type expected for the shaped output.</param>
    /// <param name="value">The value to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the value is non‑null, the target type is not primitive, not a
    /// <see cref="string"/>, and not a collection type; otherwise, <c>false</c>.
    /// </returns>
    public bool CanShape(Type targetType, object? value) =>
        value is not null &&
        !targetType.IsPrimitive &&
        targetType != typeof(string) &&
        !typeof(IEnumerable).IsAssignableFrom(targetType);
    

    /// <summary>
    /// Shapes a complex object by creating a new instance of the target type and recursively
    /// shaping each of its public properties.
    /// </summary>
    /// <param name="targetType">The type of object to create and populate.</param>
    /// <param name="value">The source object whose properties will be read.</param>
    /// <param name="recurse">
    /// A delegate used to recursively shape property values, allowing nested objects to be
    /// processed by other <see cref="IDataShapingRule"/> implementations.
    /// </param>
    /// <returns>
    /// A newly created instance of <paramref name="targetType"/> with all properties shaped
    /// recursively, or <c>null</c> if shaping cannot be performed.
    /// </returns>
    public object? Shape(Type targetType, object? value, Func<Type, object?, object?> recurse)
    {
        object instance = _typeFactory.CreateInstance(targetType);

        foreach (var prop in ReflectionCache.GetProperties(targetType))
        {
            object? propValue = prop.GetValue(value);
            object? shaped = recurse(prop.PropertyType, propValue);
            prop.SetValue(instance, shaped);
        }

        return instance;
    }
}
