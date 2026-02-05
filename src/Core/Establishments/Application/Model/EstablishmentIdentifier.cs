using System.Text.RegularExpressions;
using DfE.CleanArchitecture.Common.Domain;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;

/// <summary>
/// Represents a strongly‑typed URN identifier for an establishment.
/// <para>
/// As a value object, this type guarantees that any instance is valid at the
/// moment of creation. This ensures:
/// </para>
/// <list type="bullet">
///   <item>
///     <description>
///     <b>No invalid identifiers can exist</b> — construction fails fast if the
///     URN does not meet the required format.
///     </description>
///   </item>
///   <item>
///     <description>
///     <b>Immutability</b> — once created, the URN cannot change, ensuring
///     consistent identity throughout the domain.
///     </description>
///   </item>
///   <item>
///     <description>
///     <b>Type‑safety</b> — prevents accidental misuse of raw integers where a
///     URN is required.
///     </description>
///   </item>
/// </list>
/// <para>
/// This aligns with DDD principles: value objects enforce their own invariants,
/// allowing aggregate roots to assume correctness and focus solely on composition.
/// </para>
/// </summary>
public sealed partial class EstablishmentIdentifier : ValueObject<EstablishmentIdentifier>
{
    /// <summary>
    /// The establishment's URN. Guaranteed to be a valid 6‑digit number.
    /// </summary>
    public int Urn { get; }

    /// <summary>
    /// Creates a new <see cref="EstablishmentIdentifier"/> instance.
    /// Validation is performed immediately to ensure the identifier is always valid.
    /// </summary>
    /// <param name="urn">A 6‑digit numeric URN.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the URN does not match the required format.
    /// </exception>
    public EstablishmentIdentifier(int urn)
    {
        if (!IsValidUrn(urn))
            throw new ArgumentException(
                "URN must be a valid 6-digit numeric value.", nameof(urn));

        Urn = urn;
    }

    /// <summary>
    /// Returns the URN as a string representation of this value object.
    /// </summary>
    public override string ToString() => Urn.ToString();

    /// <summary>
    /// Defines equality based on the URN value.
    /// Value objects are equal when all their defining fields match.
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Urn;
    }

    /// <summary>
    /// Determines whether the supplied URN matches the required 6‑digit pattern.
    /// Encapsulated as a helper to keep validation logic intention‑revealing.
    /// </summary>
    private static bool IsValidUrn(int urn) =>
        UrnValidation().IsMatch(urn.ToString());

    /// <summary>
    /// Regular expression pattern for validating a 6‑digit URN.
    /// </summary>
    private const string UrnPattern = @"^\d{6}$";

    /// <summary>
    /// Compiled regular expression for URN validation.
    /// Generated at compile time for performance and correctness.
    /// </summary>
    [GeneratedRegex(UrnPattern)]
    private static partial Regex UrnValidation();
}
