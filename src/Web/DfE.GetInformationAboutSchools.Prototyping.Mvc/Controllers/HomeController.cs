using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Usecases.SearchByFilters;
using DfE.GetInformationAboutSchools.Prototyping.Core.Shared.Application.Usecases;
using DfE.GetInformationAboutSchools.Prototyping.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DfE.GetInformationAboutSchools.Prototyping.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUseCase<
            SearchEstablishmentsRequest,
            UseCaseResponse<SearchEstablishmentsResponse>> _searchUseCase;

        public HomeController(
            IUseCase<
                SearchEstablishmentsRequest,
                UseCaseResponse<SearchEstablishmentsResponse>> searchUseCase)
        {
            _searchUseCase = searchUseCase;
        }

        public IActionResult Index() => View();

        // -------------------------------
        // FUZZY TYPEAHEAD ENDPOINT
        // -------------------------------
        [HttpGet("/typeahead")]
        public async Task<IActionResult> TypeAhead([FromQuery] string term, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Ok(Array.Empty<object>());

            var request = SearchEstablishmentsRequest.Fuzzy(term);

            var result = await _searchUseCase.HandleRequestAsync(request, cancellationToken);

            if (!result.SuccessfulRequest || result.Model?.Fuzzy?.Results is null)
                return Ok(Array.Empty<object>());

            var response = result.Model.Fuzzy.Results.Select(e => new
            {
                urn = e.Identifier.Urn,
                establishmentName = e.BasicDetails.Name,
                town = e.Address.Town
            });

            return Ok(response);
        }

        // -------------------------------
        // FILTERED SEARCH ENDPOINT
        // -------------------------------
        [HttpGet("/filtered")]
        public async Task<IActionResult> Filtered(
            [FromQuery] string[]? statuses,
            [FromQuery] string[]? types,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var criteria = new EstablishmentFilterCriteria
            {
                Statuses = statuses,
                Types = types,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var request = SearchEstablishmentsRequest.Filtered(criteria);

            var result = await _searchUseCase.HandleRequestAsync(request, cancellationToken);

            if (!result.SuccessfulRequest || result.Model?.Filtered?.Results is null)
            {
                return Ok(new
                {
                    results = Array.Empty<object>(),
                    totalCount = 0
                });
            }

            var response = new
            {
                results = result.Model.Filtered.Results.Select(e => new
                {
                    urn = e.Identifier.Urn,
                    name = e.BasicDetails.Name,
                    type = e.BasicDetails.EstablishmentType,
                    phase = e.BasicDetails.PhaseOfEducation,
                    status = e.BasicDetails.Status,
                    town = e.Address.Town
                }),
                totalCount = result.Model.Filtered.TotalCount
            };

            return Ok(response);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
