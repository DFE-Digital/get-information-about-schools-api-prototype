using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Usecases.GetEstablishment.Request;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Infrastructure;
using DfE.GetInformationAboutSchools.Prototyping.Core.Shared.Application.Usecases;
using Microsoft.Extensions.Logging;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Usecases.GetEstablishment;

/// <summary>
/// Handles the retrieval of a single <see cref="Establishment"/> identified by its URN.
/// </summary>
/// <remarks>
/// This use case accepts a strongly typed request object containing a validated URN.
/// It logs structured diagnostic information throughout execution, including cancellation
/// events, domain‑specific exceptions, and unexpected errors.
/// </remarks>
/// 
public sealed class GetEstablishmentByUrnUseCase :
    IUseCase<GetEstablishmentByUrnRequest, UseCaseResponse<Establishment>>
{
    /// <summary>
    /// The logger used for structured diagnostic logging.
    /// </summary>
    private readonly ILogger<GetEstablishmentByUrnUseCase> _logger;

    /// <summary>
    /// The repository used to retrieve establishment data from the persistence layer.
    /// </summary>
    private readonly IEstablishmentsRepository _establishmentsRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetEstablishmentByUrnUseCase"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger used to record structured diagnostic and operational information.
    /// </param>
    /// <param name="establishmentsRepository">
    /// The repository responsible for retrieving establishment data.
    /// </param>
    public GetEstablishmentByUrnUseCase(
        ILogger<GetEstablishmentByUrnUseCase> logger,
        IEstablishmentsRepository establishmentsRepository)
    {
        _logger = logger;
        _establishmentsRepository = establishmentsRepository;
    }

    /// <summary>
    /// Executes the use case and retrieves the establishment associated with the
    /// URN provided in the request.
    /// </summary>
    /// <param name="request">
    /// The request containing the six‑digit URN of the establishment to retrieve.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that may be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="UseCaseResponse{T}"/> containing either the retrieved
    /// <see cref="Establishment"/> or an error message if the operation fails.
    /// </returns>
    /// <remarks>
    /// This method logs the start and end of execution, including the URN being queried.
    /// It handles cancellation, domain‑specific exceptions, and unexpected errors,
    /// returning an appropriate failure response in each case.
    /// </remarks>
    public async Task<UseCaseResponse<Establishment>> HandleRequestAsync(
        GetEstablishmentByUrnRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Starting {UseCase} execution. Cancellation requested: {IsCancellationRequested}",
            nameof(GetEstablishmentByUrnUseCase),
            cancellationToken.IsCancellationRequested);

        try
        {
            Establishment result =
                await _establishmentsRepository
                    .GetEstablishment(request.Urn, cancellationToken);

            _logger.LogInformation(
                "{UseCase} successfully retrieved establishment for URN {Urn}.",
                nameof(GetEstablishmentByUrnUseCase),
                request.Urn);

            return UseCaseResponse<Establishment>.Success(result);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(
                "{UseCase} execution was cancelled by the caller.",
                nameof(GetEstablishmentByUrnUseCase));

            return UseCaseResponse<Establishment>
                .Failure("The request was cancelled.");
        }
        catch (EstablishmentException ex)
        {
            const string message =
                "Failed to retrieve the establishment from the repository.";

            _logger.LogWarning(
                ex,
                "{UseCase} encountered a domain-specific error: {Message}",
                nameof(GetEstablishmentByUrnUseCase),
                message);

            return UseCaseResponse<Establishment>.Failure(message);
        }
        catch (Exception ex)
        {
            const string message =
                "An unexpected error occurred while processing the request.";

            _logger.LogError(
                ex,
                "{UseCase} encountered an unexpected error: {Message}",
                nameof(GetEstablishmentByUrnUseCase),
                message);

            return UseCaseResponse<Establishment>.Failure(message);
        }
    }
}
