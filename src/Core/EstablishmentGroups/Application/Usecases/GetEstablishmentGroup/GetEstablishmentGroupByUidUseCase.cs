using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Usecases.GetEstablishment;
using DfE.GetInformationAboutSchools.Prototyping.Core.Groups.Infrastructure;
using DfE.GetInformationAboutSchools.Prototyping.Core.Shared.Application.Usecases;
using Microsoft.Extensions.Logging;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Usecases.GetGroup;

/// <summary>
/// Handles a request to retrieve a single <see cref="EstablishmentGroup"/>
/// identified by its unique identifier (UID).
/// </summary>
/// <remarks>
/// This use case validates the request, retrieves the corresponding establishment group
/// from the repository, and returns a structured <see cref="UseCaseResponse{T}"/>.
/// It also logs execution details and handles cancellation, domain errors, and unexpected failures.
/// </remarks>
public sealed class GetEstablishmentGroupByUidUseCase :
    IUseCase<
        GetEstablishmentGroupByUidRequest,
        UseCaseResponse<EstablishmentGroup>>
{
    /// <summary>
    /// The logger used for structured diagnostic logging.
    /// </summary>
    private readonly ILogger<GetEstablishmentGroupByUidUseCase> _logger;

    /// <summary>
    /// The repository used to retrieve establishment group data from the persistence layer.
    /// </summary>
    private readonly IEstablishmentGroupsRepository _establishmentGroupRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetEstablishmentGroupByUidUseCase"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger used to record structured diagnostic and operational information.
    /// </param>
    /// <param name="establishmentGroupsRepository">
    /// The repository responsible for retrieving establishment group data.
    /// </param>
    public GetEstablishmentGroupByUidUseCase(
        ILogger<GetEstablishmentGroupByUidUseCase> logger,
        IEstablishmentGroupsRepository establishmentGroupsRepository)
    {
        _logger = logger;
        _establishmentGroupRepository = establishmentGroupsRepository;
    }

    /// <summary>
    /// Executes the use case and retrieves the establishment group associated with
    /// the UID provided in the request.
    /// </summary>
    /// <param name="request">
    /// The request containing the 4‑ to 5‑digit UID of the establishment group to retrieve.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that may be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="UseCaseResponse{T}"/> containing either the retrieved
    /// <see cref="EstablishmentGroup"/> or an error message if the operation fails.
    /// </returns>
    /// <remarks>
    /// This method logs the start and end of execution, including the UID being queried.
    /// It handles cancellation, domain‑specific exceptions, and unexpected errors,
    /// returning an appropriate failure response in each case.
    /// </remarks>
    public async Task<UseCaseResponse<EstablishmentGroup>> HandleRequestAsync(
        GetEstablishmentGroupByUidRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Starting {UseCase} execution. Cancellation requested: {IsCancellationRequested}",
            nameof(GetEstablishmentGroupByUidUseCase),
            cancellationToken.IsCancellationRequested);

        try
        {
            EstablishmentGroup result =
                await _establishmentGroupRepository
                    .GetEstablishmentGroup(request.UID, cancellationToken);

            _logger.LogInformation(
                "{UseCase} successfully retrieved establishment group for UID {UID}.",
                nameof(GetEstablishmentGroupByUidUseCase),
                request.UID);

            return UseCaseResponse<EstablishmentGroup>.Success(result);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(
                "{UseCase} execution was cancelled by the caller.",
                nameof(GetEstablishmentGroupByUidUseCase));

            return UseCaseResponse<EstablishmentGroup>
                .Failure("The request was cancelled.");
        }
        catch (EstablishmentGroupException ex)
        {
            const string message =
                "Failed to retrieve the establishment group from the repository.";

            _logger.LogWarning(
                ex,
                "{UseCase} encountered a domain-specific error: {Message}",
                nameof(GetEstablishmentGroupByUidUseCase),
                message);

            return UseCaseResponse<EstablishmentGroup>.Failure(message);
        }
        catch (Exception ex)
        {
            const string message =
                "An unexpected error occurred while processing the request.";

            _logger.LogError(
                ex,
                "{UseCase} encountered an unexpected error: {Message}",
                nameof(GetEstablishmentGroupByUidUseCase),
                message);

            return UseCaseResponse<EstablishmentGroup>.Failure(message);
        }
    }
}
