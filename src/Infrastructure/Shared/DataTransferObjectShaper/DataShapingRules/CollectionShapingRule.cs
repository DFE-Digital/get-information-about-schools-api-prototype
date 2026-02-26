using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjectShaper.Factories;
using System.Collections;

namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjectShaper.DataShapingRules;

/// <summary>
/// Provides shaping logic for collection types by recursively shaping each element
/// within the collection. This rule applies to arrays, lists, and any type implementing
/// <see cref="IEnumerable"/>, excluding <see cref="string"/>.
/// </summary>
public sealed class CollectionShapingRule : IDataShapingRule
{
    private readonly ICollectionFactory _collectionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionShapingRule"/> class.
    /// </summary>
    /// <param name="collectionFactory">
    /// Factory responsible for creating strongly typed collection instances during shaping.
    /// </param>
    public CollectionShapingRule(ICollectionFactory collectionFactory)
    {
        _collectionFactory = collectionFactory;
    }

    /// <summary>
    /// Determines whether this rule can shape the specified value.
    /// </summary>
    /// <param name="targetType">The expected target collection type.</param>
    /// <param name="value">The value to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the value is an <see cref="IEnumerable"/> and not a <see cref="string"/>;
    /// otherwise, <c>false</c>.
    /// </returns>
    public bool CanShape(Type targetType, object? value) =>
        value is IEnumerable &&
        targetType != typeof(string);

    /// <summary>
    /// Shapes a collection by recursively shaping each element and producing a new
    /// collection instance of the appropriate type.
    /// </summary>
    /// <param name="targetType">The type of collection to produce.</param>
    /// <param name="value">The source collection value.</param>
    /// <param name="recurse">
    /// A delegate used to recursively shape individual elements.
    /// </param>
    /// <returns>
    /// A newly created collection instance containing shaped elements, or <c>null</c>
    /// if shaping is not possible.
    /// </returns>
    public object? Shape(
        Type targetType,
        object? value,
        Func<Type, object?, object?> recurse)
    {
        IEnumerable enumerable = (IEnumerable)value!;

        Type elementType =
            targetType.IsArray
                ? targetType.GetElementType()!
                : targetType.GetGenericArguments()
                    .FirstOrDefault() ?? typeof(object);

        List<object?> items = [];

        foreach (object? item in enumerable)
        {
            items.Add(recurse(elementType, item));
        }

        return _collectionFactory.CreateListInstance(
            listType: targetType,
            elementType: elementType,
            items: [.. items!]);
    }
}
