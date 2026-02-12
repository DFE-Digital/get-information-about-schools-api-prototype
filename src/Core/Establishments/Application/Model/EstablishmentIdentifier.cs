using System.Text.RegularExpressions;
using DfE.CleanArchitecture.Common.Domain;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;

/// <summary>
/// Strongly‑typed URN identifier for an establishment.
/// </summary>
/// <remarks>
/// Validates the URN on construction to ensure only well‑formed identifiers exist.
/// </remarks>
public sealed partial class EstablishmentIdentifier : ValueObject<EstablishmentIdentifier>
{
    /// <summary>
    /// Gets the establishment's URN. Always a valid 6‑digit number.
    /// </summary>
    public int Urn { get; }

    /// <summary>
    /// Creates a new <see cref="EstablishmentIdentifier"/> with a validated URN.
    /// </summary>
    /// <param name="urn">A 6‑digit numeric URN.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="urn"/> does not match the required format.
    /// </exception>
    public EstablishmentIdentifier(int urn)
    {
        if (!IsValidUrn(urn))
            throw new ArgumentException(
                "URN must be a valid 6-digit numeric value.", nameof(urn));

        Urn = urn;
    }

    /// <summary>
    /// Returns the URN as a string.
    /// </summary>
    public override string ToString() => Urn.ToString();

    /// <summary>
    /// Defines equality based solely on the URN value.
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Urn;
    }

    /// <summary>
    /// Checks whether the supplied URN matches the required 6‑digit pattern.
    /// </summary>
    /// <param name="urn">The URN to validate.</param>
    /// <returns><c>true</c> if the URN is valid; otherwise, <c>false</c>.</returns>
    private static bool IsValidUrn(int urn) =>
        UrnValidation().IsMatch(urn.ToString());

    /// <summary>
    /// Regular expression pattern for validating a 5 to 7‑digit URN.
    /// </summary>
    private const string UrnPattern = @"^\d{5,7}$";

    /// <summary>
    /// Compiled regular expression for URN validation.
    /// </summary>
    [GeneratedRegex(UrnPattern)]
    private static partial Regex UrnValidation();
}
