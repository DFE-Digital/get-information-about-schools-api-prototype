using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Groups.Infrastructure;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.EstablishmentGroups.DataTransferObjects;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjectShaper;

namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.EstablishmentGroups;

public sealed class EstablishmentGroupsRepository : IEstablishmentGroupsRepository
{
    private readonly ISqlReader _sqlReader;
    private readonly IDataTransferObjectShaper<
        EstablishmentGroupDataTransferObject> _dataShaper;
    private readonly IMapper<
        IEnumerable<EstablishmentGroupDataTransferObject>,
        IReadOnlyCollection<EstablishmentGroup>> _establishmentGroupsMapper;

    public EstablishmentGroupsRepository(
        ISqlReader sqlReader,
        IDataTransferObjectShaper<
            EstablishmentGroupDataTransferObject> dataShaper,
        IMapper<
            IEnumerable<EstablishmentGroupDataTransferObject>,
            IReadOnlyCollection<EstablishmentGroup>> establishmentGroupsMapper)
    {
        _sqlReader = sqlReader;
        _dataShaper = dataShaper;
        _establishmentGroupsMapper = establishmentGroupsMapper;
    }

    public async Task<EstablishmentGroup?> GetEstablishmentGroup(
        int uid,
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
            WHERE eg.id = @UID;
            """;

        // NOTE: Passing an empty object is a workaround for the SQL framework's requirement
        // that a parameters object must always be supplied.
        IEnumerable<EstablishmentGroupDataTransferObject> dtos =
            await _sqlReader.QueryAsync<EstablishmentGroupDataTransferObject>(
                Sql,
                new { UID = uid },
                cancellationToken);

        IReadOnlyCollection<EstablishmentGroup> groups =
            _establishmentGroupsMapper.Map(dtos);

        if (groups.Count == 0)
        {
            return null; // or throw a domain exception if preferred
        }

        if (groups.Count > 1)
        {
            throw new InvalidOperationException(
                $"Multiple establishment groups returned for UID {uid}, expected exactly one.");
        }

        return groups.Single();
    }

    public async Task<IReadOnlyCollection<EstablishmentGroup>> GetEstablishmentGroups(
        HashSet<string> requiredFields,
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

        IEnumerable<EstablishmentGroupDataTransferObject> shapedDtos =
            await _dataShaper.ShapeDataAsync(dtos, requiredFields);

        return _establishmentGroupsMapper.Map(shapedDtos);
    }
}
