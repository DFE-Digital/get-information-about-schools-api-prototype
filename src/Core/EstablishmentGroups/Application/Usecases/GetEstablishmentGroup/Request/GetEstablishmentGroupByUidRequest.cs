using DfE.CleanArchitecture.Common.Application;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Shared.Application.Usecases;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Usecases.GetEstablishment;

/// <summary>
/// Represents a request to retrieve a single <see cref="EstablishmentGroup"/>
/// identified by its unique identifier (UID).
/// </summary>
/// <remarks>
/// The UID must be a 4‑ or 5‑digit numeric value.  
/// An exception is thrown if the supplied UID does not meet this requirement.
/// </remarks>
public sealed class GetEstablishmentGroupByUidRequest
    : IUseCaseRequest<UseCaseResponse<EstablishmentGroup>>
{
    /// <summary>
    /// Gets the 4‑ to 5‑digit unique identifier of the establishment group to retrieve.
    /// </summary>
    public int UID { get; }

    /// <summary>
    /// Creates a validated <see cref="GetEstablishmentGroupByUidRequest"/> instance.
    /// </summary>
    /// <param name="uid">The 4‑ to 5‑digit UID of the establishment group.</param>
    /// <returns>
    /// A validated <see cref="GetEstablishmentGroupByUidRequest"/> instance.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the supplied UID is not between 1000 and 99999 inclusive.
    /// </exception>
    public static GetEstablishmentGroupByUidRequest Create(int uid) => new(uid);

    /// <summary>
    /// Initializes a new instance of the <see cref="GetEstablishmentGroupByUidRequest"/> class.
    /// </summary>
    /// <param name="uid">The 4‑ to 5‑digit UID of the establishment group.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the supplied UID is not between 1000 and 99999 inclusive.
    /// </exception>
    private GetEstablishmentGroupByUidRequest(int uid)
    {
        if (uid < 1000 || uid > 99999)
        {
            throw new ArgumentOutOfRangeException(
                nameof(uid),
                "UID must be a 4‑ to 5‑digit number.");
        }

        UID = uid;
    }
}