using System.Collections;
using System.Dynamic;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Shared.DynamicViewModelConverters.ConversionRules;

/// <summary>
/// A conversion rule that transforms <see cref="IDictionary"/> instances into
/// dynamic <see cref="ExpandoObject"/> representations.
/// </summary>
/// <remarks>
/// This rule handles any object implementing <see cref="IDictionary"/>.  
/// Each dictionary entry is recursively converted using the supplied
/// <c>recurse</c> delegate, allowing nested objects, collections, and
/// dictionaries to be processed consistently by the full rule pipeline.
/// </remarks>
public sealed class DictionaryConversionRule : IDynamicConversionRule
{
    /// <summary>
    /// Determines whether the specified input is an <see cref="IDictionary"/>.
    /// </summary>
    /// <param name="input">The object to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the input implements <see cref="IDictionary"/>; otherwise, <c>false</c>.
    /// </returns>
    public bool CanConvert(object input) => input is IDictionary;

    /// <summary>
    /// Converts the specified dictionary into an <see cref="ExpandoObject"/>,
    /// recursively converting each value using the provided <paramref name="recurse"/> function.
    /// </summary>
    /// <param name="input">The dictionary to convert.</param>
    /// <param name="recurse">
    /// A delegate used to recursively convert nested values using the same
    /// rule pipeline as the parent converter.
    /// </param>
    /// <returns>
    /// An <see cref="ExpandoObject"/> containing the converted dictionary entries.
    /// Entries whose converted values are <c>null</c> are omitted.
    /// </returns>
    public object? Convert(object input, Func<object?, object?> recurse)
    {
        IDictionary dict = (IDictionary)input;
        IDictionary<string, object?> expando = new ExpandoObject();

        foreach (DictionaryEntry entry in dict)
        {
            object? cleaned = recurse(entry.Value);

            if (cleaned != null)
            {
                expando[entry.Key.ToString()!] = cleaned;
            }
        }

        return expando;
    }
}