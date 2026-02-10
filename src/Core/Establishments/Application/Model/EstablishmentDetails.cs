using DfE.CleanArchitecture.Common.Domain;
using System.Text.RegularExpressions;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;

/// <summary>
/// Represents the core identifying details of an establishment as an immutable
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
public sealed partial class EstablishmentDetails : ValueObject<EstablishmentDetails>
{
    /// <summary>
    /// Gets the establishment's official name. This value is always required.
    /// </summary>
    public string Name { get; }

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
    /// Initializes a new instance of the <see cref="EstablishmentDetails"/> class.
    /// Assumes all parameters have already been validated.
    /// </summary>
    /// <param name="name">The establishment's official name.</param>
    /// <param name="websiteUrl">The establishment's website URL, if known.</param>
    /// <param name="telephoneNumber">The establishment's telephone number, if known.</param>
    private EstablishmentDetails(
        string name,
        string? websiteUrl,
        string? telephoneNumber)
    {
        Name = name;
        WebsiteUrl = websiteUrl;
        TelephoneNumber = telephoneNumber;
    }

    /// <summary>
    /// Creates a new <see cref="EstablishmentDetails"/> instance after validating
    /// all supplied values against domain rules.
    /// </summary>
    /// <param name="name">The establishment's official name.</param>
    /// <param name="websiteUrl">The establishment's website URL (optional).</param>
    /// <param name="telephoneNumber">The establishment's telephone number (optional).</param>
    /// <returns>A fully validated <see cref="EstablishmentDetails"/> value object.</returns>
    /// <exception cref="EstablishmentException">
    /// Thrown when any supplied value violates a domain invariant.
    /// </exception>
    public static EstablishmentDetails Create(
        string? name,
        string? websiteUrl,
        string? telephoneNumber)
    {
        Validate(name, websiteUrl, telephoneNumber);
        return new EstablishmentDetails(name!, websiteUrl, telephoneNumber);
    }

    /// <summary>
    /// Validates all supplied values and throws an exception if any domain rule
    /// is violated.
    /// </summary>
    /// <param name="name">The establishment's name.</param>
    /// <param name="websiteUrl">The establishment's website URL.</param>
    /// <param name="telephoneNumber">The establishment's telephone number.</param>
    /// <exception cref="EstablishmentException">
    /// Thrown when validation fails.
    /// </exception>
    private static void Validate(
        string? name,
        string? websiteUrl,
        string? telephoneNumber)
    {
        EnsureNameIsProvided(name);
        EnsureWebsiteUrlIsValidIfProvided(websiteUrl);
        EnsureTelephoneNumberIsValidIfProvided(telephoneNumber);
    }

    /// <summary>
    /// Ensures that the establishment name is present and non‑empty.
    /// </summary>
    /// <param name="name">The name to validate.</param>
    /// <exception cref="EstablishmentException">
    /// Thrown when the name is null, empty, or whitespace.
    /// </exception>
    private static void EnsureNameIsProvided(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new EstablishmentException("School name is required.");
    }

    /// <summary>
    /// Ensures that the website URL is valid when supplied.
    /// </summary>
    /// <param name="websiteUrl">The website URL to validate.</param>
    /// <exception cref="EstablishmentException">
    /// Thrown when the URL is supplied but does not match the expected format.
    /// </exception>
    private static void EnsureWebsiteUrlIsValidIfProvided(string? websiteUrl)
    {
        if (!IsProvided(websiteUrl))
            return;

        if (!IsValidWebsiteUrl(websiteUrl!))
            throw new EstablishmentException(
                $"Website URL must be a valid URL when provided: {websiteUrl!}.");
    }

    /// <summary>
    /// Ensures that the telephone number is valid when supplied.
    /// </summary>
    /// <param name="telephoneNumber">The telephone number to validate.</param>
    /// <exception cref="EstablishmentException">
    /// Thrown when the number is supplied but does not match the expected UK format.
    /// </exception>
    private static void EnsureTelephoneNumberIsValidIfProvided(string? telephoneNumber)
    {
        if (!IsProvided(telephoneNumber))
            return;

        if (!IsValidTelephoneNumber(telephoneNumber!))
            throw new EstablishmentException(
                $"Telephone number must be a valid UK number when provided: {telephoneNumber!}.");
    }

    /// <summary>
    /// Determines whether a value has been supplied (non‑null and non‑whitespace).
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns><c>true</c> if the value is supplied; otherwise <c>false</c>.</returns>
    private static bool IsProvided(string? value) =>
        !string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Returns the telephone number as the string representation of this value object.
    /// </summary>
    /// <returns>The telephone number, or an empty string if none is supplied.</returns>
    public override string ToString() =>
        TelephoneNumber ?? string.Empty;

    /// <summary>
    /// Defines equality based on all component values.
    /// </summary>
    /// <returns>An enumeration of the components that define equality.</returns>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Name;
        yield return WebsiteUrl;
        yield return TelephoneNumber;
    }

    /// <summary>
    /// Determines whether the supplied telephone number matches the UK format.
    /// </summary>
    /// <param name="telephoneNumber">The telephone number to validate.</param>
    /// <returns><c>true</c> if the number is valid; otherwise <c>false</c>.</returns>
    private static bool IsValidTelephoneNumber(string telephoneNumber) =>
        TelephoneNumberValidation().IsMatch(telephoneNumber);

    /// <summary>
    /// Regular expression pattern for validating UK telephone numbers.
    /// Supports:
    /// - National format starting with 0 (11 digits)
    /// - International format starting with 44 or +44
    /// - Raw 10–11 digit numbers with no prefix
    /// </summary>
    private const string TelephoneNumberPattern = @"^(?:\d{7,14}|0\d{6,13}|44\d{5,12})$";

    /// <summary>
    /// Compiled regular expression for telephone number validation.
    /// </summary>
    [GeneratedRegex(TelephoneNumberPattern)]
    private static partial Regex TelephoneNumberValidation();

    /// <summary>
    /// Determines whether the supplied website URL matches a valid URL pattern.
    /// </summary>
    /// <param name="websiteUrl">The URL to validate.</param>
    /// <returns><c>true</c> if the URL is valid; otherwise <c>false</c>.</returns>
    private static bool IsValidWebsiteUrl(string websiteUrl) =>
        WebsiteUrlValidation().IsMatch(websiteUrl);

    /// <summary>
    /// Regular expression pattern for validating website URLs.
    /// This pattern is intentionally forgiving to accommodate the wide variety of
    /// real‑world historical URL formats, including partially formed or
    /// inconsistent entries. It accepts optional http/https schemes, optional "www."
    /// prefixes, multi‑part domain names, optional paths, and even common malformed
    /// variants such as missing slashes after "http:" or trailing dots at the end
    /// of the URL. The goal is to recognise plausible website URLs without enforcing
    /// strict RFC compliance.
    /// </summary>
    private const string WebsiteUrlPattern =
        @"^(?:https?:\/\/|https?:|http:|www\.)?[A-Za-z0-9.-]+\.[A-Za-z]{2,}(?:\/\S*)?\.{0,2}$";

    /// <summary>
    /// Compiled regular expression for website URL validation.
    /// </summary>
    [GeneratedRegex(WebsiteUrlPattern, RegexOptions.IgnoreCase)]
    private static partial Regex WebsiteUrlValidation();
}
