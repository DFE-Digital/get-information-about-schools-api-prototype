using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.ValidationServices;

/// <summary>
/// Provides a generic, configuration‑driven validation service that compiles
/// and caches regular expression patterns for use across multiple domain
/// validation components.
/// </summary>
/// <remarks>
/// <para>
/// This service loads all validation patterns from configuration at startup,
/// compiles them into <see cref="Regex"/> instances, and exposes a simple
/// method for validating values against named patterns.
/// </para>
/// <para>
/// Because patterns are compiled once and cached, this service is efficient
/// and suitable for repeated use across the application layer.
/// </para>
/// </remarks>
public class RegexValidationService : IRegexValidationService
{
    /// <summary>
    /// A dictionary of compiled regular expression patterns keyed by their
    /// configuration names.
    /// </summary>
    private readonly Dictionary<string, Regex> _compiled;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegexValidationService"/> class.
    /// </summary>
    /// <param name="options">
    /// The configuration options containing the dictionary of validation
    /// patterns to compile and cache.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the <paramref name="options"/> or its <c>Value</c> property
    /// is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the configuration does not contain any validation patterns.
    /// </exception>
    public RegexValidationService(IOptions<ValidationPatterns> options)
    {
        if (options?.Value == null)
            throw new ArgumentNullException(nameof(options),
                "Validation patterns configuration is missing.");

        if (options.Value.Patterns == null || options.Value.Patterns.Count == 0)
            throw new ArgumentException(
                "No validation patterns were provided in configuration.",
                nameof(options));

        _compiled = options.Value.Patterns
            .ToDictionary(
                kvp => kvp.Key,
                kvp => new Regex(kvp.Value, RegexOptions.Compiled | RegexOptions.IgnoreCase)
            );
    }

    /// <summary>
    /// Validates the supplied value using the regular expression pattern
    /// associated with the specified key.
    /// </summary>
    /// <param name="patternKey">
    /// The key identifying the validation pattern to use. Must not be <c>null</c>
    /// or whitespace.
    /// </param>
    /// <param name="value">
    /// The value to validate. Must not be <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the value matches the configured pattern; otherwise <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="patternKey"/> or <paramref name="value"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="patternKey"/> is empty or whitespace.
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when no pattern exists for the supplied <paramref name="patternKey"/>.
    /// </exception>
    public bool IsValid(string patternKey, string value)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(value, nameof(value));
        ArgumentNullException.ThrowIfNullOrEmpty(patternKey, nameof(patternKey));

        if (string.IsNullOrWhiteSpace(patternKey))
            throw new ArgumentException("Pattern key cannot be empty or whitespace.", nameof(patternKey));

        if (!_compiled.TryGetValue(patternKey, out var regex))
            throw new KeyNotFoundException(
                $"No validation pattern found for key '{patternKey}'.");

        return regex.IsMatch(value);
    }

}
