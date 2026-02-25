using System.Collections;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Shared.DynamicViewModelConverters.ConversionRules;

/// <summary>
/// A conversion rule that transforms <see cref="IEnumerable"/> instances into
/// lists of dynamically converted values.
/// </summary>
/// <remarks>
/// This rule applies to any object implementing <see cref="IEnumerable"/>,
/// except for <see cref="string"/> which is treated as a scalar value by other rules.
/// <para>
/// Each element in the sequence is recursively converted using the supplied
/// <paramref name="recurse"/> delegate, ensuring that nested objects, dictionaries,
/// and collections are processed consistently by the full rule pipeline.
/// </para>
/// </remarks>
public sealed class EnumerableConversionRule : IDynamicConversionRule
{
    /// <summary>
    /// Determines whether the specified input is an <see cref="IEnumerable"/>
    /// that is not a <see cref="string"/>.
    /// </summary>
    /// <param name="input">The object to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the input is a non-string <see cref="IEnumerable"/>; otherwise, <c>false</c>.
    /// </returns>
    public bool CanConvert(object input) =>
        input is IEnumerable &&
        input is not string;

    /// <summary>
    /// Converts the specified enumerable into a list of dynamically converted values.
    /// </summary>
    /// <param name="input">The enumerable to convert.</param>
    /// <param name="recurse">
    /// A delegate used to recursively convert each element using the same
    /// rule pipeline as the parent converter.
    /// </param>
    /// <returns>
    /// A list of converted values, or <c>null</c> if all elements convert to <c>null</c>.
    /// </returns>
    public object? Convert(object input, Func<object?, object?> recurse)
    {
        IEnumerable enumerable = (IEnumerable)input;
        List<object?> cleanedList = [];

        foreach (object? item in enumerable)
        {
            object? cleaned = recurse(item);

            if (cleaned != null)
            {
                cleanedList.Add(cleaned);
            }
        }

        return cleanedList.Count > 0 ? cleanedList : null;
    }
}
