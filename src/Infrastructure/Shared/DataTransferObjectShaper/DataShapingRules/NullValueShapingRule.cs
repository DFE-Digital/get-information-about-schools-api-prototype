namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjectShaper.DataShapingRules;

/// <summary>
/// Provides shaping logic for <c>null</c> values.  
/// This rule acts as a terminal case in the shaping pipeline:  
/// when the input value is <c>null</c>, no further shaping is performed.
/// </summary>
/// <remarks>
/// This rule ensures that <c>null</c> values are preserved consistently across
/// all shaping operations. It is typically evaluated before more complex rules,
/// preventing unnecessary recursion or type handling.
/// </remarks>
public sealed class NullValueShapingRule : IDataShapingRule
{
    /// <summary>
    /// Determines whether this rule can shape the specified value.
    /// </summary>
    /// <param name="targetType">The expected target type for the shaped value.</param>
    /// <param name="value">The value to evaluate.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="value"/> is <c>null</c>; otherwise, <c>false</c>.
    /// </returns>
    public bool CanShape(Type targetType, object? value) =>
        value is null;

    /// <summary>
    /// Shapes a <c>null</c> value.  
    /// Since <c>null</c> cannot be transformed further, this method always returns <c>null</c>.
    /// </summary>
    /// <param name="targetType">The type expected for the shaped output.</param>
    /// <param name="value">The source value, which will always be <c>null</c>.</param>
    /// <param name="recurse">
    /// A delegate used for recursive shaping of nested values.  
    /// This rule does not invoke it because <c>null</c> is a terminal case.
    /// </param>
    /// <returns>Always returns <c>null</c>.</returns>
    public object? Shape(
        Type targetType,
        object? value,
        Func<Type, object?, object?> recurse) => null;
}
