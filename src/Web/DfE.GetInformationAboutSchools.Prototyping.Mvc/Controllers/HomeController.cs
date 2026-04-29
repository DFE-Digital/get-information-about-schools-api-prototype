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

        public IActionResult Index()
        {
            return View();
        }

        // This is the endpoint your JS will call for autocomplete
        [HttpGet("/typeahead")]
        public async Task<IActionResult> TypeAhead([FromQuery] string term, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Ok(Array.Empty<object>());

            var request = SearchEstablishmentsRequest.Fuzzy(term);

            var result = await _searchUseCase.HandleRequestAsync(request, cancellationToken);

            if (!result.SuccessfulRequest || result.Model?.Fuzzy?.Results is null)
                return Ok(Array.Empty<object>());

            // Return a simple JSON projection
            var response = result.Model.Fuzzy.Results.Select(e => new
            {
                urn = e.Identifier.Urn,
                establishmentName = e.BasicDetails.Name,
                town = e.Address.Town
            });

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
