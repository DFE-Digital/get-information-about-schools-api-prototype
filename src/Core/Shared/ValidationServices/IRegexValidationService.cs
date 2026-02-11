namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.ValidationServices;

/// <summary>
/// Provides access to dynamically configured validation patterns and
/// exposes a generic method for validating values against named patterns.
/// </summary>
public interface IRegexValidationService
{
    /// <summary>
    /// Validates the supplied value using the regular expression pattern
    /// associated with the specified key.
    /// </summary>
    /// <param name="patternKey">
    /// The key identifying the validation pattern to use.
    /// </param>
    /// <param name="value">
    /// The value to validate.
    /// </param>
    /// <returns>
    /// <c>true</c> if the value matches the pattern; otherwise <c>false</c>.
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when no pattern exists for the supplied <paramref name="patternKey"/>.
    /// </exception>
    bool IsValid(string patternKey, string value);
}