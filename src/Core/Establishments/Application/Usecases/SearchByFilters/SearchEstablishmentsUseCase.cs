using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Infrastructure;
using DfE.GetInformationAboutSchools.Prototyping.Core.Shared.Application.Usecases;
using Microsoft.Extensions.Logging;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Usecases.SearchByFilters
{

    public sealed class SearchEstablishmentsUseCase :
     IUseCase<SearchEstablishmentsRequest, UseCaseResponse<SearchEstablishmentsResponse>>
    {
        private readonly ILogger<SearchEstablishmentsUseCase> _logger;
        private readonly IEstablishmentsRepository _repository;

        public SearchEstablishmentsUseCase(
            ILogger<SearchEstablishmentsUseCase> logger,
            IEstablishmentsRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<UseCaseResponse<SearchEstablishmentsResponse>> HandleRequestAsync(
            SearchEstablishmentsRequest request,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Starting {UseCase} execution. Cancellation requested: {Cancelled}",
                nameof(SearchEstablishmentsUseCase),
                cancellationToken.IsCancellationRequested);

            try
            {
                SearchEstablishmentsResponse response;

                if (request.Term is not null)
                {
                    response = await ExecuteFuzzyAsync(request, cancellationToken);
                }
                else if (request.FilterCriteria is not null)
                {
                    response = await ExecuteFilteredAsync(request, cancellationToken);
                }
                else
                {
                    throw new InvalidOperationException(
                        "Search request must contain either a term or filter criteria.");
                }

                _logger.LogInformation(
                    "{UseCase} completed successfully.",
                    nameof(SearchEstablishmentsUseCase));

                return UseCaseResponse<SearchEstablishmentsResponse>.Success(response);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(
                    "{UseCase} execution was cancelled.",
                    nameof(SearchEstablishmentsUseCase));

                return UseCaseResponse<SearchEstablishmentsResponse>.Failure(
                    "The request was cancelled.");
            }
            catch (EstablishmentException ex)
            {
                const string message =
                    "A domain validation error occurred during the search operation.";

                _logger.LogWarning(
                    ex,
                    "{UseCase} encountered a domain-specific error: {Message}",
                    nameof(SearchEstablishmentsUseCase),
                    message);

                return UseCaseResponse<SearchEstablishmentsResponse>.Failure(message);
            }
            catch (Exception ex)
            {
                const string message =
                    "An unexpected error occurred while processing the search request.";

                _logger.LogError(
                    ex,
                    "{UseCase} encountered an unexpected error: {Message}",
                    nameof(SearchEstablishmentsUseCase),
                    message);

                return UseCaseResponse<SearchEstablishmentsResponse>.Failure(message);
            }
        }

        private async Task<SearchEstablishmentsResponse> ExecuteFuzzyAsync(
            SearchEstablishmentsRequest request,
            CancellationToken cancellationToken)
        {
            var results = await _repository.SearchFuzzyAsync(
                request.Term!,
                request.SimilarityThreshold,
                limit: 20,
                cancellationToken);

            return new SearchEstablishmentsResponse
            {
                Fuzzy = new EstablishmentFuzzySearchResponse
                {
                    Results = results
                }
            };
        }

        private async Task<SearchEstablishmentsResponse> ExecuteFilteredAsync(
            SearchEstablishmentsRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _repository.SearchFilteredAsync(
                request.FilterCriteria!,
                request.SimilarityThreshold,
                cancellationToken);

            return new SearchEstablishmentsResponse
            {
                Filtered = response
            };
        }
    }

}