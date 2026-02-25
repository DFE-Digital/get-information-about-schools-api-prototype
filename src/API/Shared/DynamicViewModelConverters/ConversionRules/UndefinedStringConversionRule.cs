namespace DfE.GetInformationAboutSchools.Prototyping.API.Shared.DynamicViewModelConverters.ConversionRules;

/// <summary>
/// A conversion rule that identifies and handles the sentinel string value
/// defined by <see cref="UndefinedValue"/>, treating it as a null-equivalent
/// within the dynamic conversion pipeline.
/// </summary>
/// <remarks>
/// This rule is responsible solely for detecting the undefined sentinel value.
/// When matched, it returns <c>null</c>, allowing downstream consumers to treat
/// the value as intentionally absent.
/// </remarks>
public sealed class UndefinedStringConversionRule : IDynamicConversionRule
{
    /// <summary>
    /// The sentinel string value that represents an undefined or null-equivalent
    /// value in the dynamic conversion pipeline.
    /// </summary>
    public const string UndefinedValue = "UNDEFINED";

    /// <summary>
    /// Determines whether the specified input is a string equal to the
    /// sentinel value <see cref="UndefinedValue"/>.
    /// </summary>
    /// <param name="input">The object to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the input is the sentinel undefined string;
    /// otherwise, <c>false</c>.
    /// </returns>
    public bool CanConvert(object input) =>
        input is string str &&
        str == UndefinedValue;

    /// <summary>
    /// Converts the sentinel undefined string into <c>null</c>.
    /// </summary>
    /// <param name="input">The undefined string value.</param>
    /// <param name="recurse">
    /// A delegate used for recursive conversion of nested values.  
    /// This rule does not use recursion because the undefined value
    /// is treated as a terminal case.
    /// </param>
    /// <returns>
    /// Always returns <c>null</c> to indicate that the value should be
    /// omitted from the resulting dynamic structure.
    /// </returns>
    public object? Convert(object input, Func<object?, object?> recurse) => null;
}
