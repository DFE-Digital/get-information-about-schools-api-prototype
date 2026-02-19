using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.EstablishmentGroups.DataTransferObjects;

namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.EstablishmentGroups.Mappers;

/// <summary>
/// Maps one or more <see cref="EstablishmentGroupDataTransferObject"/> rows into
/// one or more <see cref="EstablishmentGroup"/> aggregates.
/// </summary>
/// <remarks>
/// The input may contain:
/// - Multiple rows for a single group (one per linked establishment), or
/// - Rows for multiple groups mixed together.
///
/// Rows are grouped by UID. Each group of rows is mapped into a single
/// <see cref="EstablishmentGroup"/> containing:
/// - Group-level metadata (UID, name, type)
/// - A read-only collection of <see cref="EstablishmentOverview"/> entries.
/// </remarks>
public sealed class EstablishmentGroupsDtoToModelMapper :
    IMapper<IEnumerable<EstablishmentGroupDataTransferObject>, IReadOnlyCollection<EstablishmentGroup>>
{
    public IReadOnlyCollection<EstablishmentGroup> Map(
        IEnumerable<EstablishmentGroupDataTransferObject> input)
    {
        ArgumentNullException.ThrowIfNull(input);

        List<EstablishmentGroupDataTransferObject> rows = [.. input];

        if (rows.Count == 0)
        {
            return [];
        }

        // Group rows by UID (each group becomes one EstablishmentGroup aggregate).
        IEnumerable<IGrouping<int, EstablishmentGroupDataTransferObject>> groupedByUid =
            rows.GroupBy(row => row.UID);

        List<EstablishmentGroup> groups = [];

        foreach (IGrouping<int, EstablishmentGroupDataTransferObject> groupRows in groupedByUid)
        {
            EstablishmentGroupDataTransferObject first = groupRows.First();

            EstablishmentGroupIdentifier identifier = new(first.UID);

            EstablishmentGroupDetails details =
                EstablishmentGroupDetails.Create(
                    first.GroupName, first.GroupTypeName);

            List<EstablishmentOverview> establishmentList = [];

            foreach (EstablishmentGroupDataTransferObject row in groupRows)
            {
                EstablishmentOverview overview =
                    EstablishmentOverview.Create(
                        row.EstablishmentUrn, row.EstablishmentName);

                establishmentList.Add(overview);
            }

            EstablishmentGroup group =
                EstablishmentGroup.Create(
                    identifier, details, establishmentList.AsReadOnly());

            groups.Add(group);
        }

        return groups.AsReadOnly();
    }
}
