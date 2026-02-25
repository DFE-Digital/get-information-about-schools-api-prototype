namespace DfE.GetInformationAboutSchools.Prototyping.API.Shared.DynamicViewModelConverters.ConversionRules;

/// <summary>
/// A conversion rule that handles CLR value types such as numeric types,
/// booleans, enums, and <see cref="System.DateTime"/>.
/// </summary>
/// <remarks>
/// Value types are already self-contained scalar values and require no
/// transformation.  
/// This rule simply returns the input unchanged.
/// </remarks>
public sealed class ValueTypeConversionRule : IDynamicConversionRule
{
    /// <summary>
    /// Determines whether the specified input is a value type.
    /// </summary>
    /// <param name="input">The object to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the input's runtime type is a value type;
    /// otherwise, <c>false</c>.
    /// </returns>
    public bool CanConvert(object input) =>
        input.GetType().IsValueType;

    /// <summary>
    /// Returns the input value unchanged.
    /// </summary>
    /// <param name="input">The value type instance to return.</param>
    /// <param name="recurse">
    /// A delegate used for recursive conversion of nested values.  
    /// This rule does not use recursion because value types are terminal values.
    /// </param>
    /// <returns>
    /// The original value type instance.
    /// </returns>
    public object? Convert(object input, Func<object?, object?> recurse) => input;
}