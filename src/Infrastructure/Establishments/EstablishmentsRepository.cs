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
            e.URN = @Term
            OR word_similarity (e.EstablishmentName, @Term) >= @Threshold
            OR word_similarity (e.Town, @Term) >= @Threshold
        ORDER BY
            CASE WHEN e.URN = @Term THEN 1 ELSE 0 END DESC,
            word_similarity (e.EstablishmentName, @Term) DESC,
            word_similarity (e.Town, @Term) DESC
        LIMIT @Limit;
        """;

        IEnumerable<EstablishmentDataTransferObject> dtos = await _sqlReader.QueryAsync<EstablishmentDataTransferObject>(
            Sql,
            new
            {
                Term = term,
                Threshold = similarityThreshold,
                Limit = limit
            },
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
            (@Status IS NULL OR es.name = @Status)
            AND (@Type IS NULL OR et.name = @Type)
            AND (
                @Text IS NULL
                OR similarity(e.EstablishmentName, @Text) >= @Threshold
                OR similarity(e.Town, @Text) >= @Threshold
            )
        ORDER BY e.EstablishmentName ASC
        OFFSET @Offset LIMIT @PageSize;
        """;

        const string CountSql =
            """
        SELECT COUNT(*)
        FROM Establishment AS e
        INNER JOIN EstablishmentType et ON e.EstablishmentTypeId = et.id
        INNER JOIN EducationPhase ep ON e.EducationPhaseId = ep.id
        INNER JOIN EstablishmentStatus es ON e.EstablishmentStatusId = es.id
        WHERE
            (@Status IS NULL OR es.name = @Status)
            AND (@Type IS NULL OR et.name = @Type)
            AND (
                @Text IS NULL
                OR similarity(e.EstablishmentName, @Text) >= @Threshold
                OR similarity(e.Town, @Text) >= @Threshold
            );
        """;

        const string StatusFacetSql =
            """
        SELECT es.name AS Key, COUNT(*) AS Count
        FROM Establishment e
        INNER JOIN EstablishmentStatus es ON e.EstablishmentStatusId = es.id
        GROUP BY es.name;
        """;

        const string TypeFacetSql =
            """
        SELECT et.name AS Key, COUNT(*) AS Count
        FROM Establishment e
        INNER JOIN EstablishmentType et ON e.EstablishmentTypeId = et.id
        GROUP BY et.name;
        """;

        var parameters = new
        {
            Status = criteria.Status,
            Type = criteria.Type,
            Text = criteria.Text,
            Threshold = similarityThreshold,
            Offset = offset,
            PageSize = criteria.PageSize
        };

        //
        // 1. Filtered results
        //
        var dtos = await _sqlReader.QueryAsync<EstablishmentDataTransferObject>(
            FilteredSql,
            parameters,
            cancellationToken);

        var results = _establishmentsMapper.Map(dtos);

        //
        // 2. Total count
        //
        int totalCount = await _sqlReader.QuerySingleAsync<int>(
            CountSql,
            parameters,
            cancellationToken);

        //
        // 3. Status facet
        //
        var statusRows = await _sqlReader.QueryAsync<(string Key, int Count)>(
            StatusFacetSql,
            null,
            cancellationToken);

        var statusFacet = statusRows.ToDictionary(x => x.Key, x => x.Count);

        //
        // 4. Type facet
        //
        var typeRows = await _sqlReader.QueryAsync<(string Key, int Count)>(
            TypeFacetSql,
            null,
            cancellationToken);

        var typeFacet = typeRows.ToDictionary(x => x.Key, x => x.Count);

        return new EstablishmentFilterSearchResponse
        {
            Results = results,
            TotalCount = totalCount,
            Facets = new EstablishmentFacetCounts
            {
                StatusCounts = statusFacet,
                TypeCounts = typeFacet
            }
        };
    }




}
