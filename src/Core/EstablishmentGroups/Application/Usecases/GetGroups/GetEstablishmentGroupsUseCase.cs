using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Groups.Infrastructure;
using DfE.GetInformationAboutSchools.Prototyping.Core.Shared.Application.Usecases;
using Microsoft.Extensions.Logging;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Usecases.GetGroups;

public sealed class GetEstablishmentGroupsUseCase :
    IUseCaseResponseOnly<UseCaseResponse<IReadOnlyCollection<EstablishmentGroup>>>
{
    private readonly ILogger<GetEstablishmentGroupsUseCase> _logger;
    private readonly IEstablishmentGroupsRepository _groupsRepository;

    public GetEstablishmentGroupsUseCase(
        ILogger<GetEstablishmentGroupsUseCase> logger,
        IEstablishmentGroupsRepository groupsRepository)
    {
        _logger = logger;
        _groupsRepository = groupsRepository;
    }

    public async Task<UseCaseResponse<IReadOnlyCollection<EstablishmentGroup>>> HandleRequestAsync(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Starting {UseCase} execution. Cancellation requested: {IsCancellationRequested}",
            nameof(GetEstablishmentGroupsUseCase),
            cancellationToken.IsCancellationRequested);

        try
        {
            IReadOnlyCollection<EstablishmentGroup> results =
                await _groupsRepository
                    .GetEstablishmentGroups(cancellationToken);

            _logger.LogInformation(
                "{UseCase} successfully retrieved {Count} groups.",
                nameof(GetEstablishmentGroupsUseCase),
                results.Count);

            return UseCaseResponse<IReadOnlyCollection<EstablishmentGroup>>.Success(results);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(
                "{UseCase} execution was cancelled by the caller.",
                nameof(GetEstablishmentGroupsUseCase));

            return UseCaseResponse<IReadOnlyCollection<EstablishmentGroup>>
                .Failure("The request was cancelled.");
        }
        catch (EstablishmentGroupException ex)
        {
            const string message =
                "Failed to retrieve group information from the repository.";

            _logger.LogWarning(
                ex,
                "{UseCase} encountered a domain-specific error: {Message}",
                nameof(GetEstablishmentGroupsUseCase),
                message);

            return UseCaseResponse<IReadOnlyCollection<EstablishmentGroup>>.Failure(message);
        }
        catch (Exception ex)
        {
            const string message =
                "An unexpected error occurred while processing the request.";

            _logger.LogError(
                ex,
                "{UseCase} encountered an unexpected error: {Message}",
                nameof(GetEstablishmentGroupsUseCase),
                message);

            return UseCaseResponse<IReadOnlyCollection<EstablishmentGroup>>.Failure(message);
        }
    }
}
