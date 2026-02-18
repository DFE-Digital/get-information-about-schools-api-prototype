using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.EstablishmentGroups.DataTransferObjects;

namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.EstablishmentGroups.Mappers;

/// <summary>
/// Maps a <see cref="EstablishmentGroupDataTransferObject"/>
/// into a <see cref="EstablishmentGroup"/> domain model.
/// </summary>
public sealed class EstablishmentGroupDtoToModelMapper :
    IMapper<EstablishmentGroupDataTransferObject, EstablishmentGroup>
{
    /// <summary>
    /// Maps the supplied <see cref="EstablishmentGroupDataTransferObject"/> into a fully validated
    /// <see cref="EstablishmentGroup"/> aggregate.
    /// </summary>
    /// <param name="input">The DTO containing group data.</param>
    /// <returns>A constructed <see cref="EstablishmentGroup"/> domain model.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="input"/> is null.
    /// </exception>
    public EstablishmentGroup Map(EstablishmentGroupDataTransferObject input)
    {
        ArgumentNullException.ThrowIfNull(input);

        // Create value objects.
        EstablishmentGroupIdentifier identifier = new(input.UID);
        EstablishmentGroupDetails details =
            EstablishmentGroupDetails.Create(input.GroupName, input.GroupTypeName);

        // Map establishments (adjust if you later support multiple).
        List<EstablishmentOverview> establishments =
        [
            EstablishmentOverview.Create(
                input.EstablishmentUrn,
                input.EstablishmentName)
        ];

        // Create the aggregate.
        EstablishmentGroup group =
            EstablishmentGroup.Create(identifier, details, establishments);

        return group;
    }
}
