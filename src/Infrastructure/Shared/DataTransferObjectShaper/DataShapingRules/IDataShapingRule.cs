namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjectShaper.DataShapingRules;

/// <summary>
/// Defines a rule that can shape a value into a form suitable for assignment
/// into a shaped DTO.
/// </summary>
public interface IDataShapingRule
{
    /// <summary>
    /// Determines whether this rule can shape the specified value.
    /// </summary>
    /// <param name="targetType">The expected target type.</param>
    /// <param name="value">The value to evaluate.</param>
    bool CanShape(Type targetType, object? value);

    /// <summary>
    /// Shapes the specified value.
    /// </summary>
    /// <param name="targetType">The expected target type.</param>
    /// <param name="value">The value to shape.</param>
    /// <param name="recurse">A delegate used to recursively shape nested values.</param>
    object? Shape(
        Type targetType,
        object? value,
        Func<Type, object?, object?> recurse);
}

