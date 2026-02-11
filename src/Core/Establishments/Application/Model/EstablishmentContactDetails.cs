using DfE.CleanArchitecture.Common.Domain;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model.ValidationServices.ContactDetails;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;

/// <summary>
/// Represents the contact details of an establishment as an immutable
/// domain value object.
/// </summary>
/// <remarks>
/// <para>
/// This value object enforces all domain invariants at creation time. Optional
/// fields may be omitted, but when supplied must conform to the rules of the
/// domain (e.g., valid URL format, valid UK telephone number).
/// </para>
/// <para>
/// Because this is a value object, equality is determined by the values of its
/// components rather than by identity.
/// </para>
/// </remarks>
public sealed partial class EstablishmentContactDetails : ValueObject<EstablishmentContactDetails>
{
    /// <summary>
    /// Gets the establishment's website URL, if known. When supplied, it must
    /// conform to a valid URL format.
    /// </summary>
    public string? WebsiteUrl { get; }

    /// <summary>
    /// Gets the establishment's telephone number, if known. When supplied, it
    /// must conform to a valid UK telephone number format.
    /// </summary>
    public string? TelephoneNumber { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EstablishmentContactDetails"/> class.
    /// Assumes all parameters have already been validated.
    /// </summary>
    /// <param name="websiteUrl">The establishment's website URL, if known.</param>
    /// <param name="telephoneNumber">The establishment's telephone number, if known.</param>
    private EstablishmentContactDetails(
        string? websiteUrl,
        string? telephoneNumber)
    {
        WebsiteUrl = websiteUrl;
        TelephoneNumber = telephoneNumber;
    }

    /// <summary>
    /// Creates a new <see cref="EstablishmentContactDetails"/> instance after validating
    /// all supplied values against domain rules using the provided validator.
    /// </summary>
    /// <param name="websiteUrl">The establishment's website URL (optional).</param>
    /// <param name="telephoneNumber">The establishment's telephone number (optional).</param>
    /// <param name="validator">The validator responsible for enforcing domain rules.</param>
    /// <returns>A fully validated <see cref="EstablishmentContactDetails"/> value object.</returns>
    /// <exception cref="EstablishmentException">
    /// Thrown when any supplied value violates a domain invariant.
    /// </exception>
    public static EstablishmentContactDetails Create(
        string? websiteUrl,
        string? telephoneNumber,
        IEstablishmentContactDetailsValidator validator)
    {
        Validate(websiteUrl, telephoneNumber, validator);
        return new EstablishmentContactDetails(websiteUrl, telephoneNumber);
    }

    /// <summary>
    /// Validates all supplied values and throws an exception if any domain rule
    /// is violated.
    /// </summary>
    /// <param name="websiteUrl">The establishment's website URL.</param>
    /// <param name="telephoneNumber">The establishment's telephone number.</param>
    /// <param name="validator">The validator responsible for enforcing domain rules.</param>
    /// <exception cref="EstablishmentException">
    /// Thrown when validation fails.
    /// </exception>
    private static void Validate(
        string? websiteUrl,
        string? telephoneNumber,
        IEstablishmentContactDetailsValidator validator)
    {
        EnsureWebsiteUrlIsValidIfProvided(websiteUrl, validator);
        EnsureTelephoneNumberIsValidIfProvided(telephoneNumber, validator);
    }

    /// <summary>
    /// Ensures that the website URL is valid when supplied.
    /// </summary>
    /// <param name="websiteUrl">The website URL to validate.</param>
    /// <param name="validator">The validator responsible for enforcing domain rules.</param>
    /// <exception cref="EstablishmentException">
    /// Thrown when the URL is supplied but does not match the expected format.
    /// </exception>
    private static void EnsureWebsiteUrlIsValidIfProvided(
        string? websiteUrl,
        IEstablishmentContactDetailsValidator validator)
    {
        if (!IsProvided(websiteUrl))
            return;

        if (!validator.IsValidWebsite(websiteUrl!))
            throw new EstablishmentException(
                $"Website URL must be a valid URL when provided: {websiteUrl!}.");
    }

    /// <summary>
    /// Ensures that the telephone number is valid when supplied.
    /// </summary>
    /// <param name="telephoneNumber">The telephone number to validate.</param>
    /// <param name="validator">The validator responsible for enforcing domain rules.</param>
    /// <exception cref="EstablishmentException">
    /// Thrown when the number is supplied but does not match the expected format.
    /// </exception>
    private static void EnsureTelephoneNumberIsValidIfProvided(
        string? telephoneNumber,
        IEstablishmentContactDetailsValidator validator)
    {
        if (!IsProvided(telephoneNumber))
            return;

        if (!validator.IsValidTelephone(telephoneNumber!))
            throw new EstablishmentException(
                $"Telephone number must be a valid UK number when provided: {telephoneNumber!}.");
    }

    /// <summary>
    /// Determines whether a value has been supplied (non‑null and non‑whitespace).
    /// </summary>
    private static bool IsProvided(string? value) =>
        !string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Defines equality based on all component values.
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return WebsiteUrl!;
        yield return TelephoneNumber!;
    }
}
