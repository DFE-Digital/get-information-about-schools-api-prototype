using DfE.CleanArchitecture.Common.Domain;
using System.Text.RegularExpressions;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;

/// <summary>
/// Represents the immutable details of an establishment.
/// <para>
/// This value object validates all invariants at creation time. If an instance
/// exists, it is guaranteed to be valid. This ensures:
/// </para>
/// <list type="bullet">
///   <item>
///     <description>
///     <b>No invalid state can be represented</b> — construction fails fast
///     when required fields are missing or malformed.
///     </description>
///   </item>
///   <item>
///     <description>
///     <b>Immutability</b> — all properties are set once and never changed,
///     providing a consistent snapshot of establishment details.
///     </description>
///   </item>
///   <item>
///     <description>
///     <b>Simplicity for aggregate roots</b> — aggregates can assume this
///     object is valid and only enforce composition rules.
///     </description>
///   </item>
/// </list>
/// </summary>
public sealed partial class EstablishmentDetails : ValueObject<EstablishmentDetails>
{
    /// <summary>
    /// The establishment's name. Guaranteed to be non‑empty.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The establishment's website URL. Guaranteed to be non‑empty.
    /// </summary>
    public string WebsiteUrl { get; }

    /// <summary>
    /// The establishment's telephone number. Guaranteed to be a valid UK number.
    /// </summary>
    public string TelephoneNumber { get; }

    /// <summary>
    /// Private constructor used only after successful validation.
    /// Ensures immutability by preventing property mutation after creation.
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
    /// Factory method for creating a new <see cref="EstablishmentDetails"/> instance.
    /// All invariants are validated before construction, ensuring the resulting
    /// value object is always valid.
    /// </summary>
    public static EstablishmentDetails Create(
        string name,
        string websiteUrl,
        string telephoneNumber)
    {
        Validate(name, websiteUrl, telephoneNumber);
        return new EstablishmentDetails(name, websiteUrl, telephoneNumber);
    }

    /// <summary>
    /// Validates all invariants for this value object.
    /// Throws an <see cref="EstablishmentException"/> if any rule is violated.
    /// This ensures no invalid instance can ever be created.
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
    /// Defines equality based on the value object's components.
    /// Value objects are equal when all their defining fields match.
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return WebsiteUrl;
        yield return TelephoneNumber;
    }

    /// <summary>
    /// Determines whether the supplied telephone number matches the UK number pattern.
    /// Encapsulated as a helper to keep validation logic intention‑revealing.
    /// </summary>
    private static bool IsValidTelephoneNumber(string telephoneNumber) =>
        TelephoneNumberValidation().IsMatch(telephoneNumber);

    /// <summary>
    /// Regular expression pattern for validating UK telephone numbers.
    /// </summary>
    private const string TelephoneNumberPattern = @"^(\+44\s?7\d{9}|0\d{10})$";

    /// <summary>
    /// Compiled regular expression for telephone number validation.
    /// Generated at compile time for performance and correctness.
    /// </summary>
    [GeneratedRegex(TelephoneNumberPattern)]
    private static partial Regex TelephoneNumberValidation();
}