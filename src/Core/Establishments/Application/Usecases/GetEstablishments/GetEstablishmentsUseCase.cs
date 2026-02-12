using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Infrastructure;
using DfE.GetInformationAboutSchools.Prototyping.Core.Shared.Application.Usecases;
using Microsoft.Extensions.Logging;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Usecases.GetEstablishments;

/// <summary>
/// Handles the retrieval of all establishments by orchestrating the
/// <see cref="IEstablishmentsRepository"/> and returning a structured
/// <see cref="UseCaseResponse{T}"/>.
/// </summary>
/// <remarks>
/// This use case does not require an input request object. It logs structured
/// diagnostic information, including cancellation events and exception details.
/// </remarks>
public sealed class GetEstablishmentsUseCase :
    IUseCaseResponseOnly<UseCaseResponse<IReadOnlyCollection<Establishment>>>
{
    private readonly ILogger<GetEstablishmentsUseCase> _logger;
    private readonly IEstablishmentsRepository _establishmentsRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetEstablishmentsUseCase"/> class.
    /// </summary>
    /// <param name="logger">The logger used for structured diagnostic logging.</param>
    /// <param name="establishmentsRepository">The repository used to retrieve establishment data.</param>
    public GetEstablishmentsUseCase(
        ILogger<GetEstablishmentsUseCase> logger,
        IEstablishmentsRepository establishmentsRepository)
    {
        _logger = logger;
        _establishmentsRepository = establishmentsRepository;
    }

    /// <summary>
    /// Executes the use case and returns a collection of establishments.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token that may be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="UseCaseResponse{T}"/> containing either the retrieved establishments
    /// or an error message if the operation fails.
    /// </returns>
    public async Task<UseCaseResponse<IReadOnlyCollection<Establishment>>> HandleRequestAsync(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Starting {UseCase} execution. Cancellation requested: {IsCancellationRequested}",
            nameof(GetEstablishmentsUseCase),
            cancellationToken.IsCancellationRequested);

        try
        {
            IReadOnlyCollection<Establishment> results =
                await _establishmentsRepository
                    .GetEstablishments(cancellationToken);

            _logger.LogInformation(
                "{UseCase} successfully retrieved {Count} establishments.",
                nameof(GetEstablishmentsUseCase),
                results.Count);

            return UseCaseResponse<IReadOnlyCollection<Establishment>>.Success(results);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(
                "{UseCase} execution was cancelled by the caller.",
                nameof(GetEstablishmentsUseCase));

            return UseCaseResponse<IReadOnlyCollection<Establishment>>
                .Failure("The request was cancelled.");
        }
        catch (EstablishmentException ex)
        {
            const string message =
                "Failed to retrieve establishment information from the repository.";

            _logger.LogWarning(
                ex,
                "{UseCase} encountered a domain-specific error: {Message}",
                nameof(GetEstablishmentsUseCase),
                message);

            return UseCaseResponse<IReadOnlyCollection<Establishment>>.Failure(message);
        }
        catch (Exception ex)
        {
            const string message =
                "An unexpected error occurred while processing the request.";

            _logger.LogError(
                ex,
                "{UseCase} encountered an unexpected error: {Message}",
                nameof(GetEstablishmentsUseCase),
                message);

            return UseCaseResponse<IReadOnlyCollection<Establishment>>.Failure(message);
        }
    }
}
