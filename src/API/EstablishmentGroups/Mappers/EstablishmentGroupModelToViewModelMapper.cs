using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.API.EstablishmentGroups.ViewModels;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;

namespace DfE.GetInformationAboutSchools.Prototyping.API.EstablishmentGroups.Mappers;

/// <summary>
/// Maps a <see cref="Group"/> domain model into a <see cref="GroupViewModel"/>
/// suitable for API responses. This includes mapping the group's identifying
/// details and its associated establishment summaries.
/// </summary>
public class EstablishmentGroupModelToViewModelMapper :
    IMapper<EstablishmentGroup, EstablishmentGroupViewModel>
{
    /// <summary>
    /// Maps the supplied <see cref="EstablishmentGroup"/> domain model into a
    /// <see cref="EstablishmentGroupViewModel"/> instance.
    /// </summary>
    /// <param name="input">The domain model to map.</param>
    /// <returns>A populated <see cref="EstablishmentGroupViewModel"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="input"/> is null.
    /// </exception>
    public EstablishmentGroupViewModel Map(EstablishmentGroup input)
    {
        return input is null ?
            throw new ArgumentNullException(nameof(input)) :
            MapGroup(input);
    }

    /// <summary>
    /// Maps the core group details (UID, name, type) and its
    /// associated establishments into a <see cref="EstablishmentGroupViewModel"/>.
    /// </summary>
    /// <param name="group">The domain group to map.</param>
    /// <returns>A populated <see cref="EstablishmentGroupViewModel"/>.</returns>
    private EstablishmentGroupViewModel MapGroup(EstablishmentGroup group)
    {
        EstablishmentGroupViewModel viewModel = new()
        {
            UID = group.Identifier.UID,
            GroupName = group.BasicDetails.Name,
            GroupTypeName = group.BasicDetails.GroupType,
            Establishments = MapEstablishments(group.GroupEstablishments)
        };

        return viewModel;
    }

    /// <summary>
    /// Maps a collection of <see cref="EstablishmentOverview"/> domain objects
    /// into a collection of <see cref="EstablishmentOverviewViewModel"/> instances.
    /// </summary>
    /// <param name="establishments">The domain establishments to map.</param>
    /// <returns>A list of mapped establishment view models.</returns>
    private IEnumerable<EstablishmentOverviewViewModel> MapEstablishments(
        IReadOnlyCollection<EstablishmentOverview> establishments)
    {
        List<EstablishmentOverviewViewModel> results = [];

        foreach (EstablishmentOverview establishment in establishments)
        {
            EstablishmentOverviewViewModel viewModel =
                new()
                {
                    Urn = establishment.URN,
                    Name = establishment.Name
                };

            results.Add(viewModel);
        }

        return results;
    }
}
