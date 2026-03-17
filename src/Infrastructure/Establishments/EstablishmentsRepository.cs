using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Infrastructure;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments.DataTransferObjects;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjectShaper;

namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments;

/// <summary>
/// Provides access to establishment data stored in the SQL database.
/// Responsible for executing queries and mapping results into domain
/// <see cref="Establishment"/> objects.
/// </summary>
public sealed class EstablishmentsRepository : IEstablishmentsRepository
{
    private readonly ISqlReader _sqlReader;
    private readonly IDataTransferObjectShaper<EstablishmentDataTransferObject> _dataShaper;
    private readonly IMapper<
        EstablishmentDataTransferObject, Establishment> _establishmentMapper;
    private readonly IMapper<
        IEnumerable<EstablishmentDataTransferObject>,
        IReadOnlyCollection<Establishment>> _establishmentsMapper;

    public EstablishmentsRepository(
        ISqlReader sqlReader,
        IDataTransferObjectShaper<EstablishmentDataTransferObject> dataShaper,
        IMapper<
            EstablishmentDataTransferObject, Establishment> establishmentMapper,
        IMapper<
            IEnumerable<EstablishmentDataTransferObject>,
            IReadOnlyCollection<Establishment>> establishmentsMapper)
    {
        _sqlReader = sqlReader;
        _dataShaper = dataShaper;
        _establishmentMapper = establishmentMapper;
        _establishmentsMapper = establishmentsMapper;
    }

    public async Task<Establishment> GetEstablishment(
        int urn, CancellationToken cancellationToken = default)
    {
        const string Sql =
            """
            SELECT
                URN,
                EstablishmentName,
                et.name AS EstablishmentType,
                ep.name AS EducationPhase,
                WebsiteAddress,
                TelephoneNumber,
                Street,
                Town,
                Postcode,
                es.name AS EstablishmentStatus
            FROM Establishment AS e
            INNER JOIN EducationPhase ep
                ON e.educationPhase_code = ep.code
            INNER JOIN EstablishmentType et
                ON e.type_code = et.code
            INNER JOIN EstablishmentStatus es
                ON e.status_code = es.code
            WHERE URN = @URN;
            """;

        EstablishmentDataTransferObject? dto =
            await _sqlReader.QuerySingleAsync<EstablishmentDataTransferObject>(
                Sql,
                new { URN = urn },
                cancellationToken);

        return _establishmentMapper.Map(dto);
    }

    /// <summary>
    /// Retrieves all establishments from the database and maps them into
    /// domain <see cref="Establishment"/> instances.
    /// </summary>
    /// <param name="cancellationToken">A token that may be used to cancel the asynchronous operation.</param>
    /// <returns>
    /// A read-only collection of <see cref="Establishment"/> objects representing
    /// the establishments stored in the database.
    /// </returns>
    public async Task<IReadOnlyCollection<Establishment>> GetEstablishments(
        HashSet<string> requiredFields,
        CancellationToken cancellationToken = default)
    {
        const string Sql =
            """
            SELECT
                URN,
                EstablishmentName,
                et.name AS EstablishmentType,
                ep.name AS EducationPhase,
                WebsiteAddress,
                TelephoneNumber,
                Street,
                Town,
                Postcode,
                es.name AS EstablishmentStatus
            FROM Establishment AS e
            INNER JOIN EducationPhase ep
                ON e.educationPhase_code = ep.code
            INNER JOIN EstablishmentType et
                ON e.type_code = et.code
            INNER JOIN EstablishmentStatus es
                ON e.status_code = es.code;
            """;

        IEnumerable<EstablishmentDataTransferObject> dtos =
            await _sqlReader.QueryAsync<EstablishmentDataTransferObject>(
                Sql,
                new { URN = "" },   // This is a bug and needs to be fixed in the sql framework to allow for no parameters to be passed in!
                cancellationToken);

        IEnumerable<EstablishmentDataTransferObject> shapedDtos =
            await _dataShaper.ShapeDataAsync(dtos, requiredFields);

        return _establishmentsMapper.Map(shapedDtos);
    }
}
