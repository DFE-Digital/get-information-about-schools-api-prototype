using DfE.CleanArchitecture.Common.Domain;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model.ValidationServices.Address;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;

/// <summary>
/// Represents the postal address of an establishment as an immutable
/// domain value object.
/// </summary>
/// <remarks>
/// <para>
/// This value object enforces all domain invariants at creation time. Optional
/// fields may be omitted, but when supplied must conform to the rules of the
/// domain (e.g., valid street format, valid town name, valid postcode).
/// </para>
/// <para>
/// Because this is a value object, equality is determined by the values of its
/// components rather than by identity.
/// </para>
/// </remarks>
public sealed partial class EstablishmentAddress : ValueObject<EstablishmentAddress>
{
    /// <summary>
    /// Gets the street component of the establishment's address.
    /// </summary>
    public string Street { get; }

    /// <summary>
    /// Gets the town or locality component of the establishment's address.
    /// </summary>
    public string Town { get; }

    /// <summary>
    /// Gets the postcode component of the establishment's address.
    /// </summary>
    public string Postcode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EstablishmentAddress"/> class.
    /// Assumes all parameters have already been validated.
    /// </summary>
    /// <param name="street">The validated street component of the address.</param>
    /// <param name="town">The validated town or locality component of the address.</param>
    /// <param name="postcode">The validated postcode component of the address.</param>
    private EstablishmentAddress(string street, string town, string postcode)
    {
        Street = street;
        Town = town;
        Postcode = postcode;
    }

    /// <summary>
    /// Creates a new <see cref="EstablishmentAddress"/> instance after validating
    /// all supplied values against domain rules using the provided validator.
    /// </summary>
    /// <param name="street">The street component of the address.</param>
    /// <param name="town">The town or locality component of the address.</param>
    /// <param name="postcode">The postcode component of the address.</param>
    /// <param name="validator">The validator responsible for enforcing domain rules.</param>
    /// <returns>A fully validated <see cref="EstablishmentAddress"/> value object.</returns>
    /// <exception cref="EstablishmentException">
    /// Thrown when any supplied value violates a domain invariant.
    /// </exception>
    public static EstablishmentAddress Create(
        string? street,
        string? town,
        string? postcode,
        IEstablishmentAddressValidator validator)
    {
        street = street?.Trim();
        town = town?.Trim();
        postcode = postcode?.Trim();

        Validate(street, town, postcode, validator);
        return new EstablishmentAddress(street!, town!, postcode!);
    }

    /// <summary>
    /// Validates all supplied values and throws an exception if any domain rule
    /// is violated.
    /// </summary>
    /// <param name="street">The street value to validate.</param>
    /// <param name="town">The town value to validate.</param>
    /// <param name="postcode">The postcode value to validate.</param>
    /// <param name="validator">The validator responsible for enforcing domain rules.</param>
    private static void Validate(
        string? street,
        string? town,
        string? postcode,
        IEstablishmentAddressValidator validator)
    {
        EnsureStreetIsValidIfProvided(street, validator);
        EnsureTownIsValidIfProvided(town, validator);
        EnsurePostcodeIsValidIfProvided(postcode, validator);
    }

    /// <summary>
    /// Ensures that the street value is valid when supplied.
    /// </summary>
    /// <param name="street">The street value to validate.</param>
    /// <param name="validator">The validator responsible for enforcing domain rules.</param>
    private static void EnsureStreetIsValidIfProvided(
        string? street,
        IEstablishmentAddressValidator validator)
    {
        if (!IsProvided(street) || street!.Length <= 1)
            return;

        if (!validator.IsValidStreet(street))
            throw new EstablishmentException($"A valid street is required {street}.");
    }

    /// <summary>
    /// Ensures that the town value is valid when supplied.
    /// </summary>
    /// <param name="town">The town value to validate.</param>
    /// <param name="validator">The validator responsible for enforcing domain rules.</param>
    private static void EnsureTownIsValidIfProvided(
        string? town,
        IEstablishmentAddressValidator validator)
    {
        if (!IsProvided(town))
            return;

        if (!validator.IsValidTown(town!))
            throw new EstablishmentException($"A valid town name is required {town}.");
    }

    /// <summary>
    /// Ensures that the postcode value is valid when supplied.
    /// </summary>
    /// <param name="postcode">The postcode value to validate.</param>
    /// <param name="validator">The validator responsible for enforcing domain rules.</param>
    private static void EnsurePostcodeIsValidIfProvided(
        string? postcode,
        IEstablishmentAddressValidator validator)
    {
        if (!IsProvided(postcode))
            return;

        if (!validator.IsValidPostcode(postcode!))
            throw new EstablishmentException($"Postcode must be valid when provided: {postcode}.");
    }

    /// <summary>
    /// Determines whether a value has been supplied (non‑null and non‑whitespace).
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns><c>true</c> if the value is supplied; otherwise <c>false</c>.</returns>
    private static bool IsProvided(string? value) =>
        !string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Defines equality based on all component values.
    /// </summary>
    /// <returns>An enumeration of the components that define equality.</returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return Town;
        yield return Postcode;
    }
}
