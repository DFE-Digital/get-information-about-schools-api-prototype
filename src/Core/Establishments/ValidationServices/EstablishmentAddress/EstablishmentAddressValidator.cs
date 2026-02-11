using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model.ValidationServices.Address;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.ValidationServices.EstablishmentAddress;

/// <summary>
/// Provides validation logic for establishment address components by delegating
/// to a generic <see cref="IRegexValidationService"/> that evaluates values
/// against named regular expression patterns.
/// </summary>
/// <remarks>
/// <para>
/// This validator acts as a thin, domain‑specific wrapper around the generic
/// validation service. It exposes strongly‑named methods for validating street,
/// town, and postcode values, while the underlying pattern definitions remain
/// configuration‑driven.
/// </para>
/// <para>
/// Because the pattern keys are defined as constants, this class avoids the
/// risk of typos and ensures consistency with configuration.
/// </para>
/// </remarks>
public sealed class EstablishmentAddressValidator : IEstablishmentAddressValidator
{
    /// <summary>
    /// The configuration key used to retrieve the regular expression pattern
    /// for validating street values.
    /// </summary>
    private const string StreetPatternKey = "Street";

    /// <summary>
    /// The configuration key used to retrieve the regular expression pattern
    /// for validating town or locality values.
    /// </summary>
    private const string TownPatternKey = "Town";

    /// <summary>
    /// The configuration key used to retrieve the regular expression pattern
    /// for validating postcode values.
    /// </summary>
    private const string PostcodePatternKey = "Postcode";


    private readonly IRegexValidationService _regex;

    /// <summary>
    /// Initializes a new instance of the <see cref="EstablishmentAddressValidator"/> class.
    /// </summary>
    /// <param name="regex">
    /// The generic validation service responsible for evaluating values against
    /// configured regular expression patterns.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="regex"/> is <c>null</c>.
    /// </exception>
    public EstablishmentAddressValidator(IRegexValidationService regex)
    {
        _regex = regex ?? throw new ArgumentNullException(nameof(regex));
    }

    /// <summary>
    /// Determines whether the supplied street value is valid according to the
    /// configured <c>Street</c> validation pattern.
    /// </summary>
    /// <param name="street">The street value to validate.</param>
    /// <returns>
    /// <c>true</c> if the value matches the configured pattern; otherwise <c>false</c>.
    /// </returns>
    public bool IsValidStreet(string street) =>
        _regex.IsValid(StreetPatternKey, street);

    /// <summary>
    /// Determines whether the supplied town value is valid according to the
    /// configured <c>Town</c> validation pattern.
    /// </summary>
    /// <param name="town">The town value to validate.</param>
    /// <returns>
    /// <c>true</c> if the value matches the configured pattern; otherwise <c>false</c>.
    /// </returns>
    public bool IsValidTown(string town) =>
        _regex.IsValid(TownPatternKey, town);

    /// <summary>
    /// Determines whether the supplied postcode value is valid according to the
    /// configured <c>Postcode</c> validation pattern.
    /// </summary>
    /// <param name="postcode">The postcode value to validate.</param>
    /// <returns>
    /// <c>true</c> if the value matches the configured pattern; otherwise <c>false</c>.
    /// </returns>
    public bool IsValidPostcode(string postcode) =>
        _regex.IsValid(PostcodePatternKey, postcode);
}