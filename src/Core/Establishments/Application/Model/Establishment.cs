using DfE.CleanArchitecture.Common.Domain;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;

/// <summary>
/// Represents an Establishment as a domain aggregate root.
/// <para>
/// This aggregate follows a read‑only domain model where:
/// </para>
/// <list type="bullet">
///   <item>
///     <description>
///     <b>Value objects enforce their own invariants</b> — 
///     <see cref="EstablishmentDetails"/> cannot be created in an invalid state.
///     </description>
///   </item>
///   <item>
///     <description>
///     <b>The aggregate root enforces only composition</b> — 
///     it ensures required components are present, but does not re‑validate them.
///     </description>
///   </item>
///   <item>
///     <description>
///     <b>Immutability is guaranteed</b> — 
///     once constructed, the aggregate represents a complete and consistent snapshot.
///     </description>
///   </item>
/// </list>
/// <para>
/// This keeps the aggregate small, intention‑revealing, and aligned with DDD:
/// value objects guarantee correctness; the aggregate guarantees structure.
/// </para>
/// </summary>
public sealed class Establishment : AggregateRoot<EstablishmentIdentifier>
{
    /// <summary>
    /// The basic details of the establishment.
    /// Guaranteed to be valid because the value object enforces its own invariants.
    /// </summary>
    public EstablishmentDetails BasicDetails { get; }

    /// <summary>
    /// Creates a new read‑only Establishment aggregate.
    /// Only structural invariants are enforced here; value objects are assumed valid.
    /// </summary>
    /// <param name="identifier">The unique identifier for the establishment.</param>
    /// <param name="basicDetails">A fully constructed, valid details object.</param>
    /// <exception cref="EstablishmentException">
    /// Thrown when <paramref name="basicDetails"/> is null.
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
    /// Factory method for creating an Establishment aggregate.
    /// </summary>
    public static Establishment Create(
        EstablishmentIdentifier identifier,
        EstablishmentDetails basicDetails) =>
        new(identifier, basicDetails);
}
