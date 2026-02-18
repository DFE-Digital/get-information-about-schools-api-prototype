using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.EstablishmentGroups.DataTransferObjects;

namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.EstablishmentGroups.Mappers;

/// <summary>
/// Maps a collection of <see cref="EstablishmentGroupDataTransferObject"/> instances
/// into a collection of <see cref="EstablishmentGroup"/> domain models.
/// </summary>
public sealed class EstablishmentGroupsDtoToModelMapper :
    IMapper<IEnumerable<EstablishmentGroupDataTransferObject>, IReadOnlyCollection<EstablishmentGroup>>
{
    private readonly IMapper<
        EstablishmentGroupDataTransferObject, EstablishmentGroup> _groupDtoToModelMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="EstablishmentGroupsDtoToModelMapper"/> class.
    /// Ensures that a valid single‑item mapper is provided for mapping
    /// individual <see cref="EstablishmentGroupDataTransferObject"/> instances.
    /// </summary>
    /// <param name="groupDtoToModelMapper">
    /// The mapper responsible for converting a single DTO into a <see cref="EstablishmentGroup"/> domain model.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="groupDtoToModelMapper"/> is null.
    /// </exception>
    public EstablishmentGroupsDtoToModelMapper(
        IMapper<EstablishmentGroupDataTransferObject, EstablishmentGroup> groupDtoToModelMapper)
    {
        _groupDtoToModelMapper = groupDtoToModelMapper ??
            throw new ArgumentNullException(nameof(groupDtoToModelMapper));
    }

    /// <summary>
    /// Maps the supplied collection of DTOs into a collection of validated
    /// <see cref="EstablishmentGroup"/> aggregates.
    /// </summary>
    /// <param name="input">The DTO collection to map.</param>
    /// <returns>A read‑only collection of mapped <see cref="EstablishmentGroup"/> instances.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="input"/> is null.
    /// </exception>
    public IReadOnlyCollection<EstablishmentGroup> Map(
        IEnumerable<EstablishmentGroupDataTransferObject> input)
    {
        ArgumentNullException.ThrowIfNull(input);

        List<EstablishmentGroup> groups = [];

        foreach (EstablishmentGroupDataTransferObject dto in input)
        {
            EstablishmentGroup mappedGroup =
                _groupDtoToModelMapper.Map(dto);

            groups.Add(mappedGroup);
        }

        return groups.AsReadOnly();
    }
}
