namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjectShaper.DataShapingRules;

/// <summary>
/// Provides shaping logic for primitive and scalar values that can be copied directly
/// without transformation. This includes built‑in primitives, <see cref="string"/>,
/// <see cref="DateTime"/>, <see cref="Guid"/>, and <see cref="decimal"/>.
/// </summary>
/// <remarks>
/// This rule acts as a fast‑path for simple values that do not require recursion or
/// structural shaping. It ensures that common scalar types are passed through unchanged,
/// improving performance and avoiding unnecessary processing.
/// </remarks>
public sealed class PrimitiveValueShapingRule : IDataShapingRule
{
    /// <summary>
    /// Determines whether this rule can shape the specified value.
    /// </summary>
    /// <param name="targetType">The expected target type for the shaped output.</param>
    /// <param name="value">The value to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the value is non‑null and the target type represents a primitive
    /// or scalar type that can be copied directly; otherwise, <c>false</c>.
    /// </returns>
    public bool CanShape(Type targetType, object? value) =>
        value is not null &&
            (targetType.IsPrimitive ||
            targetType == typeof(string) ||
            targetType == typeof(DateTime) ||
            targetType == typeof(Guid) ||
            targetType == typeof(decimal));

    /// <summary>
    /// Shapes a primitive or scalar value by returning it unchanged.
    /// </summary>
    /// <param name="targetType">The type expected for the shaped output.</param>
    /// <param name="value">The source value to return.</param>
    /// <param name="recurse">
    /// A delegate used for recursive shaping of nested values.  
    /// This rule does not invoke it because primitive values are terminal cases.
    /// </param>
    /// <returns>
    /// The original <paramref name="value"/> unchanged.
    /// </returns>
    public object? Shape(
        Type targetType,
        object? value,
        Func<Type, object?, object?> recurse) => value;
}
