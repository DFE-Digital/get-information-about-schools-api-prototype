namespace DfE.GetInformationAboutSchools.Prototyping.API.Shared.DynamicViewModelConverters;

/// <summary>
/// Converts arbitrary CLR objects into dynamic representations using a set of
/// <see cref="IDynamicConversionRule"/> implementations.  
/// <para>
/// This converter delegates the transformation logic to the injected rules,
/// evaluating them in order until one indicates it can handle the input.
/// </para>
/// <para>
/// The conversion process is recursive: rules may call back into
/// <see cref="ToDynamic(object?)"/> to process nested values.
/// </para>
/// </summary>
public sealed class DynamicViewModelConverter
{
    private readonly IReadOnlyList<IDynamicConversionRule> _rules;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicViewModelConverter"/> class.
    /// </summary>
    /// <param name="rules">
    /// The ordered collection of <see cref="IDynamicConversionRule"/> instances that
    /// define how different input types should be transformed.  
    /// The order of rules matters: the first rule whose
    /// <see cref="IDynamicConversionRule.CanConvert(object)"/> method returns <c>true</c>
    /// will be used to perform the conversion.
    /// </param>
    public DynamicViewModelConverter(IEnumerable<IDynamicConversionRule> rules)
    {
        _rules = [.. rules];
    }

    /// <summary>
    /// Converts the specified input object into a dynamic representation.
    /// </summary>
    /// <param name="input">The object to convert.</param>
    /// <returns>
    /// A dynamic object (typically an <see cref="ExpandoObject"/> or a list of dynamic values),
    /// or <c>null</c> if the input is <c>null</c> or no rule produces a meaningful result.
    /// </returns>
    /// <remarks>
    /// This method evaluates the configured rules in order.  
    /// The first rule that reports it can handle the input is used to perform the conversion.
    /// </remarks>
    public object? ToDynamic(object? input)
    {
        if (input == null)
        {
            return null;
        }

        foreach (var rule in _rules)
        {
            if (rule.CanConvert(input))
            {
                return rule.Convert(input, ToDynamic);
            }
        }

        return null;
    }
}