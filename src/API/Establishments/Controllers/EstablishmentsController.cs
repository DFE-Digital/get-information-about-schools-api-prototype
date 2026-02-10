using DfE.GetInformationAboutSchools.Prototyping.API.Establishments.Controllers.Response;
using DfE.GetInformationAboutSchools.Prototyping.API.Establishments.ViewModels;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Establishments.Controllers;

[ApiController]
[Route("[controller]")]
public class EstablishmentsController : ControllerBase
{
    private readonly ILogger<EstablishmentsController> _logger;
    private readonly IUseCaseResponseOnly<
        UseCaseResponse<IReadOnlyCollection<Establishment>>> _useCase;
    private readonly ICsvResponseBuilder _csvResponseBuilder;

    public EstablishmentsController(
        ILogger<EstablishmentsController> logger,
        IUseCaseResponseOnly<
            UseCaseResponse<IReadOnlyCollection<Establishment>>> useCase,
        ICsvResponseBuilder csvResponseBuilder)
    {
        _logger = logger;
        _useCase = useCase;
        _csvResponseBuilder = csvResponseBuilder;
    }

    [HttpGet(Name = "GetEstablishments")]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        UseCaseResponse<IReadOnlyCollection<Establishment>> result =
            await _useCase.HandleRequestAsync(cancellationToken);

        if (!result.SuccessfulRequest)
        {
            return Problem(
                detail: result.ErrorMessage ?? "Unknown error",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        if (!result.HasValidModel())
        {
            return Problem(
                detail: "Use case returned no data.",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        #pragma warning disable CS1998
        async IAsyncEnumerable<EstablishmentViewModel> StreamResults(
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            foreach (Establishment establishment in result.Model!)
            {
                ct.ThrowIfCancellationRequested();

                yield return new EstablishmentViewModel
                {
                    URN = establishment.Identifier.Urn,
                    Name = establishment.BasicDetails.Name
                };
            }
        }
        #pragma warning restore CS1998

        return Ok(StreamResults(cancellationToken));
    }

    [HttpGet("csv", Name = "GetEstablishmentsCsv")]
    public async Task<IActionResult> GetCsv(CancellationToken cancellationToken)
    {
        UseCaseResponse<IReadOnlyCollection<Establishment>> result =
            await _useCase.HandleRequestAsync(cancellationToken);

        if (!result.SuccessfulRequest)
        {
            return Problem(
                detail: result.ErrorMessage ?? "Unknown error",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        if (!result.HasValidModel())
        {
            return Problem(
                detail: "Use case returned no data.",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        // Delegate the entire CSV streaming workflow to the response builder.
        return await _csvResponseBuilder.WriteCsvAsync(
            Response,
            result.Model!,
            ["URN", "EstablishmentName"],
            row =>
            [
                row.Identifier!.Urn.ToString(),
                row.BasicDetails?.Name!
            ],
            "establishments.csv",
            cancellationToken);
    }
}
