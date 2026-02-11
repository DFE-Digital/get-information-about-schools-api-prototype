using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model.ValidationServices.ContactDetails;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.ValidationServices.EstablishmentContactDetails;

/// <summary>
/// Provides validation logic for establishment contact‑related fields by
/// delegating to a generic <see cref="IRegexValidationService"/> that evaluates
/// values against named regular expression patterns.
/// </summary>
/// <remarks>
/// <para>
/// This validator acts as a thin, domain‑specific wrapper around the generic
/// validation service. It exposes strongly‑named methods for validating website
/// URLs and telephone numbers, while the underlying pattern definitions remain
/// configuration‑driven.
/// </para>
/// <para>
/// Pattern keys are defined as constants to avoid duplication and reduce the
/// risk of typographical errors when referencing configuration.
/// </para>
/// </remarks>
public sealed class EstablishmentContactDetailsValidator : IEstablishmentContactDetailsValidator
{
    /// <summary>
    /// The configuration key used to retrieve the regular expression pattern
    /// for validating website URL values.
    /// </summary>
    private const string WebsitePatternKey = "Website";

    /// <summary>
    /// The configuration key used to retrieve the regular expression pattern
    /// for validating telephone number values.
    /// </summary>
    private const string TelephonePatternKey = "Telephone";

    private readonly IRegexValidationService _regex;

    /// <summary>
    /// Initializes a new instance of the <see cref="EstablishmentContactDetailsValidator"/> class.
    /// </summary>
    /// <param name="regex">
    /// The generic validation service responsible for evaluating values against
    /// configured regular expression patterns.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="regex"/> is <c>null</c>.
    /// </exception>
    public EstablishmentContactDetailsValidator(IRegexValidationService regex)
    {
        _regex = regex ?? throw new ArgumentNullException(nameof(regex));
    }

    /// <summary>
    /// Determines whether the supplied website URL is valid according to the
    /// configured <c>Website</c> validation pattern.
    /// </summary>
    /// <param name="website">The website URL to validate.</param>
    /// <returns>
    /// <c>true</c> if the value matches the configured pattern; otherwise <c>false</c>.
    /// </returns>
    public bool IsValidWebsite(string website) =>
        _regex.IsValid(WebsitePatternKey, website);

    /// <summary>
    /// Determines whether the supplied telephone number is valid according to
    /// the configured <c>Telephone</c> validation pattern.
    /// </summary>
    /// <param name="telephone">The telephone number to validate.</param>
    /// <returns>
    /// <c>true</c> if the value matches the configured pattern; otherwise <c>false</c>.
    /// </returns>
    public bool IsValidTelephone(string telephone) =>
        _regex.IsValid(TelephonePatternKey, telephone);
}
