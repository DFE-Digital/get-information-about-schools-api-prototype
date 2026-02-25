using System.Dynamic;
using System.Reflection;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Shared.DynamicViewModelConverters.ConversionRules;

/// <summary>
/// A fallback conversion rule that transforms arbitrary CLR objects into
/// dynamic <see cref="ExpandoObject"/> representations by converting their
/// public instance properties.
/// </summary>
/// <remarks>
/// This rule applies when no other <see cref="IDynamicConversionRule"/> matches.
/// It reflects over the object's public instance properties, recursively
/// converting each value using the supplied <paramref name="recurse"/> delegate.
/// <para>
/// Properties whose converted values are <c>null</c> are omitted from the
/// resulting dynamic object.
/// </para>
/// </remarks>
public sealed class ObjectConversionRule : IDynamicConversionRule
{
    /// <summary>
    /// Determines whether this rule can convert the specified input.
    /// </summary>
    /// <param name="input">The object to evaluate.</param>
    /// <returns>
    /// Always returns <c>true</c>, making this rule the fallback handler
    /// when no other rule applies.
    /// </returns>
    public bool CanConvert(object input) => true; // fallback

    /// <summary>
    /// Converts the specified object into an <see cref="ExpandoObject"/> by
    /// reflecting over its public instance properties and recursively converting
    /// each property value.
    /// </summary>
    /// <param name="input">The object to convert.</param>
    /// <param name="recurse">
    /// A delegate used to recursively convert nested values using the same
    /// rule pipeline as the parent converter.
    /// </param>
    /// <returns>
    /// An <see cref="ExpandoObject"/> containing the converted property values,
    /// or <c>null</c> if all properties convert to <c>null</c>.
    /// </returns>
    public object? Convert(object input, Func<object?, object?> recurse)
    {
        IDictionary<string, object?> expando = new ExpandoObject();
        Type type = input.GetType();

        foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            object? value = prop.GetValue(input);
            object? cleaned = recurse(value);

            if (cleaned != null)
            {
                expando[prop.Name] = cleaned;
            }
        }

        return expando.Count > 0 ? expando : null;
    }
}
