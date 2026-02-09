using DfE.CleanArchitecture.Common.Domain;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;

/// <summary>
/// Aggregate root representing an establishment within the domain.
/// </summary>
/// <remarks>
/// The aggregate is immutable and only enforces structural completeness.
/// All supplied value objects are assumed to be pre‑validated.
/// </remarks>
public sealed class Establishment : AggregateRoot<EstablishmentIdentifier>
{
    /// <summary>
    /// Gets the core details of the establishment.
    /// </summary>
    public EstablishmentDetails BasicDetails { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Establishment"/> class.
    /// </summary>
    /// <param name="identifier">
    /// The unique identifier assigned to this establishment.
    /// </param>
    /// <param name="basicDetails">
    /// A valid <see cref="EstablishmentDetails"/> instance describing the establishment.
    /// </param>
    /// <exception cref="EstablishmentException">
    /// Thrown when <paramref name="basicDetails"/> is <c>null</c>.
    /// </exception>
    public Establishment(
        EstablishmentIdentifier identifier,
        EstablishmentDetails basicDetails)
        : base(identifier)
    {
        BasicDetails = basicDetails
            ?? throw new EstablishmentException(
                "An initialised 'EstablishmentDetails' object must be provided.");
    }

    /// <summary>
    /// Creates a new establishment aggregate.
    /// </summary>
    /// <param name="identifier">The establishment's unique identifier.</param>
    /// <param name="basicDetails">Validated establishment details.</param>
    /// <returns>A new <see cref="Establishment"/> instance.</returns>
    public static Establishment Create(
        EstablishmentIdentifier identifier,
        EstablishmentDetails basicDetails) =>
        new(identifier, basicDetails);
}
