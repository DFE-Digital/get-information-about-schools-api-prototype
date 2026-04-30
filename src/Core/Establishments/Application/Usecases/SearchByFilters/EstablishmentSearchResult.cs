using DfE.CleanArchitecture.Common.Application;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Shared.Application.Usecases;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Usecases.SearchByFilters
{
    public sealed class SearchEstablishmentsRequest :
    IUseCaseRequest<UseCaseResponse<SearchEstablishmentsResponse>>
    {
        public string? Term { get; }
        public EstablishmentFilterCriteria? FilterCriteria { get; }
        public double SimilarityThreshold { get; }

        private SearchEstablishmentsRequest(
            string? term,
            EstablishmentFilterCriteria? filterCriteria,
            double similarityThreshold)
        {
            Term = term;
            FilterCriteria = filterCriteria;
            SimilarityThreshold = similarityThreshold;
        }

        public static SearchEstablishmentsRequest Fuzzy(
            string term,
            double threshold = 0.6) =>
            new(term, null, threshold);

        public static SearchEstablishmentsRequest Filtered(
            EstablishmentFilterCriteria criteria,
            double threshold = 0.6) =>
            new(null, criteria, threshold);
    }

    public sealed class SearchEstablishmentsResponse
    {
        public EstablishmentFuzzySearchResponse? Fuzzy { get; init; }
        public EstablishmentFilterSearchResponse? Filtered { get; init; }
    }

    public sealed class EstablishmentFilterCriteria
    {
        public string? Text { get; init; }
        public IReadOnlyCollection<string>? Statuses { get; init; }
        public IReadOnlyCollection<string>? Types { get; init; }

        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 20;
    }


    public sealed class EstablishmentFacetCounts
    {
        public IReadOnlyDictionary<string, int> StatusCounts { get; init; } = new Dictionary<string, int>();
        public IReadOnlyDictionary<string, int> TypeCounts { get; init; } = new Dictionary<string, int>();
    }


    public sealed class EstablishmentFilterSearchResponse
    {
        public IReadOnlyCollection<Establishment> Results { get; init; } = [];
        public int TotalCount { get; init; }
        public EstablishmentFacetCounts Facets { get; init; } = new();
    }

    public sealed class EstablishmentFuzzySearchResponse
    {
        public IReadOnlyCollection<Establishment> Results { get; init; } = [];
    }

}

