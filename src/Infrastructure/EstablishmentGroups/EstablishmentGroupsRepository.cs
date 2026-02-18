using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Groups.Infrastructure;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.EstablishmentGroups.DataTransferObjects;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared;

namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.EstablishmentGroups;

public sealed class EstablishmentGroupsRepository : IEstablishmentGroupsRepository
{
    private readonly ISqlReader _sqlReader;
    private readonly IMapper<
        IEnumerable<EstablishmentGroupDataTransferObject>, IReadOnlyCollection<EstablishmentGroup>> _groupsMapper;

    public EstablishmentGroupsRepository(
        ISqlReader sqlReader,
        IMapper<IEnumerable<EstablishmentGroupDataTransferObject>, IReadOnlyCollection<EstablishmentGroup>> groupsMapper)
    {
        _sqlReader = sqlReader;
        _groupsMapper = groupsMapper;
    }

    public async Task<IReadOnlyCollection<EstablishmentGroup>> GetEstablishmentGroups(
        CancellationToken cancellationToken = default)
    {
        const string Sql =
            """
            SELECT
                eg.id AS UID,
                eg.name AS GroupName,
                egt.name AS GroupTypeName,
                gl.urn AS EstablishmentURN,
                e.EstablishmentName
            FROM EstablishmentGroup AS eg
            INNER JOIN GroupLink gl
                ON eg.id = gl.group_id
            INNER JOIN Establishment AS e
                ON e.URN = gl.urn
            INNER JOIN EstablishmentGroupType AS egt
                ON eg.type_code = egt.code
            """;

        IEnumerable<EstablishmentGroupDataTransferObject> dtos =
            await _sqlReader.QueryAsync<EstablishmentGroupDataTransferObject>(
                Sql,
                new { URN = "" },   // This is a bug and needs to be fixed in the sql framework to allow for no parameters to be passed in!
                cancellationToken);

        var mapped = _groupsMapper.Map(dtos);

        return Array.AsReadOnly([.. mapped]);
    }
}




