using DfE.CleanArchitecture.Common.Domain;
using System.Text.RegularExpressions;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;

/// <summary>
/// Immutable value object containing the core details of an establishment.
/// </summary>
/// <remarks>
/// All invariants are validated at creation time, ensuring no invalid instance can exist.
/// </remarks>
public sealed partial class EstablishmentDetails : ValueObject<EstablishmentDetails>
{
    /// <summary>
    /// Gets the establishment's name. Guaranteed to be non‑empty.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the establishment's website URL. Guaranteed to be a valid URL.
    /// </summary>
    public string WebsiteUrl { get; }

    /// <summary>
    /// Gets the establishment's telephone number. Guaranteed to be a valid UK number.
    /// </summary>
    public string TelephoneNumber { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EstablishmentDetails"/> class.
    /// Assumes all parameters have already been validated.
    /// </summary>
    private EstablishmentDetails(
        string name,
        string websiteUrl,
        string telephoneNumber)
    {
        Name = name;
        WebsiteUrl = websiteUrl;
        TelephoneNumber = telephoneNumber;
    }

    /// <summary>
    /// Creates a validated <see cref="EstablishmentDetails"/> instance.
    /// </summary>
    /// <param name="name">The establishment's name.</param>
    /// <param name="websiteUrl">The establishment's website URL.</param>
    /// <param name="telephoneNumber">A valid UK telephone number.</param>
    /// <returns>A fully validated <see cref="EstablishmentDetails"/> value object.</returns>
    /// <exception cref="EstablishmentException">
    /// Thrown when any parameter is missing or fails validation.
    /// </exception>
    public static EstablishmentDetails Create(
        string name,
        string websiteUrl,
        string telephoneNumber)
    {
        Validate(name, websiteUrl, telephoneNumber);
        return new EstablishmentDetails(name, websiteUrl, telephoneNumber);
    }

    /// <summary>
    /// Validates all supplied values and throws if any invariant is violated.
    /// </summary>
    private static void Validate(
        string name,
        string websiteUrl,
        string telephoneNumber)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new EstablishmentException("School name is required.");

        if (string.IsNullOrWhiteSpace(websiteUrl))
            throw new EstablishmentException("Website URL is required.");

        if (!IsValidWebsiteUrl(websiteUrl))
            throw new EstablishmentException("Website URL must be a valid URL.");

        if (string.IsNullOrWhiteSpace(telephoneNumber))
            throw new EstablishmentException("Telephone number is required.");

        if (!IsValidTelephoneNumber(telephoneNumber))
            throw new EstablishmentException("Telephone number must be a valid UK number.");
    }

    /// <summary>
    /// Returns the telephone number as the string representation of this value object.
    /// </summary>
    public override string ToString() => TelephoneNumber;

    /// <summary>
    /// Defines equality based on all component values.
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return WebsiteUrl;
        yield return TelephoneNumber;
    }

    /// <summary>
    /// Checks whether the supplied telephone number matches the UK format.
    /// </summary>
    private static bool IsValidTelephoneNumber(string telephoneNumber) =>
        TelephoneNumberValidation().IsMatch(telephoneNumber);

    /// <summary>
    /// Regular expression pattern for validating UK telephone numbers.
    /// </summary>
    private const string TelephoneNumberPattern = @"^(\+44\s?7\d{9}|0\d{10})$";

    /// <summary>
    /// Compiled regular expression for telephone number validation.
    /// </summary>
    [GeneratedRegex(TelephoneNumberPattern)]
    private static partial Regex TelephoneNumberValidation();

    /// <summary>
    /// Checks whether the supplied website URL matches a valid URL pattern.
    /// </summary>
    private static bool IsValidWebsiteUrl(string websiteUrl) =>
        WebsiteUrlValidation().IsMatch(websiteUrl);

    /// <summary>
    /// Regular expression pattern for validating website URLs.
    /// Allows optional http/https, domain names, and optional paths.
    /// </summary>
    private const string WebsiteUrlPattern =
        @"^(https?:\/\/)?([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}(\/.*)?$";

    /// <summary>
    /// Compiled regular expression for website URL validation.
    /// </summary>
    [GeneratedRegex(WebsiteUrlPattern, RegexOptions.IgnoreCase)]
    private static partial Regex WebsiteUrlValidation();
}
