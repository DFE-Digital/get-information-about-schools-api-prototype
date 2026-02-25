namespace DfE.GetInformationAboutSchools.Prototyping.API.Shared.DynamicViewModelConverters;

/// <summary>
/// Defines a rule used by <see cref="DynamicViewModelConverter"/> to determine
/// whether a given input object can be transformed, and to perform that
/// transformation when applicable.
/// </summary>
/// <remarks>
/// Implementations of this interface encapsulate a single conversion strategy.
/// The converter evaluates rules in order, selecting the first rule whose
/// <see cref="CanConvert(object)"/> method returns <c>true</c>.
/// </remarks>
public interface IDynamicConversionRule
{
    /// <summary>
    /// Determines whether this rule is capable of converting the specified input.
    /// </summary>
    /// <param name="input">The object to evaluate.</param>
    /// <returns>
    /// <c>true</c> if this rule can convert the input; otherwise, <c>false</c>.
    /// </returns>
    bool CanConvert(object input);

    /// <summary>
    /// Converts the specified input object into a dynamic representation.
    /// </summary>
    /// <param name="input">The object to convert.</param>
    /// <param name="recurse">
    /// A delegate that can be used to recursively convert nested values using the
    /// same rule pipeline. Implementations should call this function when they
    /// encounter child objects, collections, or dictionary values.
    /// </param>
    /// <returns>
    /// A dynamic representation of the input (typically an <see cref="ExpandoObject"/>,
    /// a list of dynamic values, or a primitive), or <c>null</c> if the rule
    /// determines that the input has no meaningful representation.
    /// </returns>
    object? Convert(object input, Func<object?, object?> recurse);
}