using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Usecases.SearchByFilters;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Infrastructure;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments.DataTransferObjects;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjectShaper;

namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments;

/// <summary>
/// Repository responsible for retrieving establishment data from the SQL database.
/// Executes SQL queries, shapes DTOs, and maps them into domain <see cref="Establishment"/> objects.
/// </summary>
public sealed class EstablishmentsRepository : IEstablishmentsRepository
{
    private readonly ISqlReader _sqlReader;
    private readonly IDataTransferObjectShaper<EstablishmentDataTransferObject> _dataShaper;
    private readonly IMapper<EstablishmentDataTransferObject, Establishment> _establishmentMapper;
    private readonly IMapper<IEnumerable<EstablishmentDataTransferObject>, IReadOnlyCollection<Establishment>> _establishmentsMapper;

    /// <summary>
    /// Creates a new instance of <see cref="EstablishmentsRepository"/>.
    /// </summary>
    /// <param name="sqlReader">Service used to execute SQL queries and materialize DTOs.</param>
    /// <param name="dataShaper">Shapes DTOs based on the fields requested by the caller.</param>
    /// <param name="establishmentMapper">Maps a single DTO into a domain <see cref="Establishment"/>.</param>
    /// <param name="establishmentsMapper">Maps a collection of DTOs into domain <see cref="Establishment"/> objects.</param>
    public EstablishmentsRepository(
        ISqlReader sqlReader,
        IDataTransferObjectShaper<EstablishmentDataTransferObject> dataShaper,
        IMapper<EstablishmentDataTransferObject, Establishment> establishmentMapper,
        IMapper<IEnumerable<EstablishmentDataTransferObject>, IReadOnlyCollection<Establishment>> establishmentsMapper)
    {
        _sqlReader = sqlReader;
        _dataShaper = dataShaper;
        _establishmentMapper = establishmentMapper;
        _establishmentsMapper = establishmentsMapper;
    }

    /// <summary>
    /// Retrieves a single establishment by its URN.
    /// </summary>
    /// <param name="urn">The unique reference number of the establishment.</param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    /// <returns>
    /// A fully populated <see cref="Establishment"/> domain object.
    /// Throws if the URN does not exist or if multiple rows are unexpectedly returned.
    /// </returns>
    public async Task<Establishment> GetEstablishment(int urn, CancellationToken cancellationToken = default)
    {
        const string Sql =
            """
            SELECT
                e.URN,
                e.EstablishmentName,
                et.name AS EstablishmentType,
                ep.name AS EducationPhase,
                e.SchoolWebsite,
                e.TelephoneNum,
                e.Street,
                e.Town,
                e.Postcode,
                es.name AS EstablishmentStatus
            FROM Establishment AS e
            INNER JOIN EstablishmentType et
                ON e.EstablishmentTypeId = et.id
            INNER JOIN EducationPhase ep
                ON e.EducationPhaseId = ep.id
            INNER JOIN EstablishmentStatus es
                ON e.EstablishmentStatusId = es.id
            WHERE e.URN = @URN;
            """;

        EstablishmentDataTransferObject dto =
            await _sqlReader.QuerySingleAsync<EstablishmentDataTransferObject>(
                Sql,
                new { URN = urn },
                cancellationToken);

        return _establishmentMapper.Map(dto);
    }

    /// <summary>
    /// Retrieves all establishments, shaping the returned DTOs based on the fields
    /// requested by the caller.
    /// </summary>
    /// <param name="requiredFields">
    /// A set of field names that must be included in the shaped DTOs.
    /// </param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    /// <returns>
    /// A collection of <see cref="Establishment"/> domain objects.
    /// </returns>
    public async Task<IReadOnlyCollection<Establishment>> GetEstablishments(
        HashSet<string> requiredFields,
        CancellationToken cancellationToken = default)
    {
        const string Sql =
            """
            SELECT
                e.URN,
                e.EstablishmentName,
                et.name AS EstablishmentType,
                ep.name AS EducationPhase,
                e.SchoolWebsite,
                e.TelephoneNum,
                e.Street,
                e.Town,
                e.Postcode,
                es.name AS EstablishmentStatus
            FROM Establishment AS e
            INNER JOIN EstablishmentType et
                ON e.EstablishmentTypeId = et.id
            INNER JOIN EducationPhase ep
                ON e.EducationPhaseId = ep.id
            INNER JOIN EstablishmentStatus es
                ON e.EstablishmentStatusId = es.id;
            """;

        IEnumerable<EstablishmentDataTransferObject> dtos =
            await _sqlReader.QueryAsync<EstablishmentDataTransferObject>(
                Sql,
                new { },
                cancellationToken);

        IEnumerable<EstablishmentDataTransferObject> shapedDtos =
            await _dataShaper.ShapeDataAsync(dtos, requiredFields);

        return _establishmentsMapper.Map(shapedDtos);
    }


    public async Task<IReadOnlyCollection<Establishment>> SearchFuzzyAsync(
        string term,
        double similarityThreshold,
        int limit,
        CancellationToken cancellationToken)
    {
        const string Sql =
            """
        SELECT
            e.URN,
            e.EstablishmentName,
            et.name AS EstablishmentType,
            es.name AS EstablishmentStatus,
            e.Street,
            e.Town,
            e.Postcode,
            e.SchoolWebsite,
            e.TelephoneNum

        FROM Establishment AS e
        INNER JOIN EstablishmentType et ON e.EstablishmentTypeId = et.id
        INNER JOIN EstablishmentStatus es ON e.EstablishmentStatusId = es.id

        WHERE
            (
                -- NUMERIC MODE: URN prefix only
                @term ~ '^[0-9]+$'
                AND CAST(e.URN AS TEXT) LIKE @term || '%'
            )
            OR
            (
                -- TEXT MODE: substring + fuzzy
                @term !~ '^[0-9]+$'
                AND (
                    e.EstablishmentName ILIKE '%' || @term || '%'
                    OR e.Town ILIKE '%' || @term || '%'
                    OR e.EstablishmentName % @term
                    OR e.Town % @term
                )
            )

        ORDER BY
            e.URN ASC
        LIMIT @limit;
        """;

        var dtos = await _sqlReader.QueryAsync<EstablishmentDataTransferObject>(
            Sql,
            new { term, threshold = similarityThreshold, limit },
            cancellationToken);

        return _establishmentsMapper.Map(dtos);
    }



    public async Task<EstablishmentFilterSearchResponse> SearchFilteredAsync(
    EstablishmentFilterCriteria criteria,
    double similarityThreshold,
    CancellationToken cancellationToken)
    {
        int offset = (criteria.PageNumber - 1) * criteria.PageSize;

        const string FilteredSql =
        """
        SELECT
            e.URN,
            e.EstablishmentName,
            et.name AS EstablishmentType,
            ep.name AS EducationPhase,
            e.SchoolWebsite,
            e.TelephoneNum,
            e.Street,
            e.Town,
            e.Postcode,
            es.name AS EstablishmentStatus
        FROM Establishment AS e
        INNER JOIN EstablishmentType et ON e.EstablishmentTypeId = et.id
        INNER JOIN EducationPhase ep ON e.EducationPhaseId = ep.id
        INNER JOIN EstablishmentStatus es ON e.EstablishmentStatusId = es.id
        WHERE
            (
                @Statuses IS NULL
                OR cardinality(@Statuses) = 0
                OR es.name = ANY(@Statuses::text[])
            )
            AND
            (
                @Types IS NULL
                OR cardinality(@Types) = 0
                OR et.name = ANY(@Types::text[])
            )
        ORDER BY 
            e.EstablishmentName ASC,
            e.URN ASC
        OFFSET @Offset LIMIT @PageSize;
        """;




        var parameters = new
        {
            Statuses = criteria.Statuses?.ToArray(),
            Types = criteria.Types?.ToArray(),
            Offset = offset,
            PageSize = criteria.PageSize
        };

        var dtos = await _sqlReader.QueryAsync<EstablishmentDataTransferObject>(
            FilteredSql,
            parameters,
            cancellationToken);

        var results = _establishmentsMapper.Map(dtos);

        return new EstablishmentFilterSearchResponse
        {
            Results = results,
            TotalCount = 0,
            Facets = new EstablishmentFacetCounts
            {
                StatusCounts = new Dictionary<string, int>(),
                TypeCounts = new Dictionary<string, int>()
            }
        };
    }


}
