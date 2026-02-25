namespace DfE.GetInformationAboutSchools.Prototyping.API.Shared.DynamicViewModelConverters.ConversionRules;

/// <summary>
/// A conversion rule that handles string values, excluding the sentinel
/// value defined by <see cref="UndefinedValue"/> which is treated as a
/// null-equivalent by a separate rule.
/// </summary>
/// <remarks>
/// This rule applies only to non-sentinel strings.  
/// The value is returned unchanged because strings are already scalar
/// values and require no further transformation.
/// </remarks>
public sealed class StringConversionRule : IDynamicConversionRule
{
    /// <summary>
    /// The sentinel string value that represents an undefined or null-equivalent
    /// value in the dynamic conversion pipeline.
    /// </summary>
    public const string UndefinedValue = "UNDEFINED";

    /// <summary>
    /// Determines whether the specified input is a string that is not the
    /// sentinel value <see cref="UndefinedValue"/>.
    /// </summary>
    /// <param name="input">The object to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the input is a non-sentinel string;
    /// otherwise, <c>false</c>.
    /// </returns>
    public bool CanConvert(object input) =>
        input is string str &&
        str != UndefinedValue;

    /// <summary>
    /// Returns the input string unchanged.
    /// </summary>
    /// <param name="input">The string to return.</param>
    /// <param name="recurse">
    /// A delegate used for recursive conversion of nested values.  
    /// This rule does not use recursion because strings are scalar values.
    /// </param>
    /// <returns>
    /// The original string value.
    /// </returns>
    public object? Convert(object input, Func<object?, object?> recurse) => input;
}
