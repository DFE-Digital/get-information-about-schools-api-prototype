using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Groups.Infrastructure;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.EstablishmentGroups.DataTransferObjects;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjectShaper;

namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.EstablishmentGroups;

/// <summary>
/// Repository responsible for retrieving establishment group data from the database.
/// Uses SQL queries, DTO shaping, and mapping to domain models.
/// </summary>
public sealed class EstablishmentGroupsRepository : IEstablishmentGroupsRepository
{
    private readonly ISqlReader _sqlReader;
    private readonly IDataTransferObjectShaper<EstablishmentGroupDataTransferObject> _dataShaper;
    private readonly IMapper<IEnumerable<EstablishmentGroupDataTransferObject>, IReadOnlyCollection<EstablishmentGroup>> _establishmentGroupsMapper;

    /// <summary>
    /// Creates a new instance of <see cref="EstablishmentGroupsRepository"/>.
    /// </summary>
    /// <param name="sqlReader">Reads SQL query results into DTOs.</param>
    /// <param name="dataShaper">Shapes DTOs based on required fields.</param>
    /// <param name="establishmentGroupsMapper">Maps DTOs into domain models.</param>
    public EstablishmentGroupsRepository(
        ISqlReader sqlReader,
        IDataTransferObjectShaper<EstablishmentGroupDataTransferObject> dataShaper,
        IMapper<IEnumerable<EstablishmentGroupDataTransferObject>, IReadOnlyCollection<EstablishmentGroup>> establishmentGroupsMapper)
    {
        _sqlReader = sqlReader;
        _dataShaper = dataShaper;
        _establishmentGroupsMapper = establishmentGroupsMapper;
    }

    /// <summary>
    /// Retrieves a single establishment group by its UID.
    /// </summary>
    /// <param name="uid">The unique identifier of the establishment group.</param>
    /// <param name="cancellationToken">Token for cancelling the operation.</param>
    /// <returns>
    /// The matching <see cref="EstablishmentGroup"/> if found; otherwise <c>null</c>.
    /// Throws if more than one group is unexpectedly returned.
    /// </returns>
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

        IEnumerable<EstablishmentGroupDataTransferObject> dtos =
            await _sqlReader.QueryAsync<EstablishmentGroupDataTransferObject>(
                Sql,
                new { UID = uid },
                cancellationToken);

        IReadOnlyCollection<EstablishmentGroup> groups =
            _establishmentGroupsMapper.Map(dtos);

        if (groups.Count == 0)
            return null;

        if (groups.Count > 1)
        {
            throw new InvalidOperationException(
                $"Multiple establishment groups returned for UID {uid}, expected exactly one.");
        }

        return groups.Single();
    }

    /// <summary>
    /// Retrieves all establishment groups, shaping the returned DTOs
    /// based on the required fields requested by the caller.
    /// </summary>
    /// <param name="requiredFields">
    /// A set of field names that must be included in the shaped DTOs.
    /// </param>
    /// <param name="cancellationToken">Token for cancelling the operation.</param>
    /// <returns>
    /// A collection of <see cref="EstablishmentGroup"/> domain models.
    /// </returns>
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
                ON eg.type_code = egt.code;
            """;

        IEnumerable<EstablishmentGroupDataTransferObject> dtos =
            await _sqlReader.QueryAsync<EstablishmentGroupDataTransferObject>(
                Sql,
                new { },
                cancellationToken);

        // UID must always be included for grouping and mapping
        requiredFields.Add(nameof(EstablishmentGroupDataTransferObject.UID));

        IEnumerable<EstablishmentGroupDataTransferObject> shapedDtos =
            await _dataShaper.ShapeDataAsync(dtos, requiredFields);

        return _establishmentGroupsMapper.Map(shapedDtos);
    }
}
