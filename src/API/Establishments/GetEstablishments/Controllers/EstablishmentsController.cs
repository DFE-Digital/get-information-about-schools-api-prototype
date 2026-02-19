using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.API.Establishments.ViewModels;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response.Mappers;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Usecases.GetEstablishment.Request;
using DfE.GetInformationAboutSchools.Prototyping.Core.Shared.Application.Usecases;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Establishments.GetEstablishments.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class EstablishmentsController : ControllerBase
{
    private readonly ILogger<EstablishmentsController> _logger;
    private readonly IUseCaseResponseOnly<
        UseCaseResponse<IReadOnlyCollection<Establishment>>> _getEstablishmentsUseCase;
    private readonly IUseCase<
        GetEstablishmentByUrnRequest, UseCaseResponse<Establishment>> _getEstablishmentUseCase;
    private readonly IMapper<Establishment, EstablishmentViewModel> _modelToViewModelMapper;
    private readonly ICsvResponseBuilder _csvResponseBuilder;
    private readonly ModelToCsvMapper<Establishment> _modelToCsvMapper;

    public EstablishmentsController(
        ILogger<EstablishmentsController> logger,
        IUseCaseResponseOnly<
            UseCaseResponse<IReadOnlyCollection<Establishment>>> getEstablishmentsUseCase,
        IUseCase<
             GetEstablishmentByUrnRequest, UseCaseResponse<Establishment>> getEstablishmentUseCase,
        ICsvResponseBuilder csvResponseBuilder,
        IMapper<Establishment, EstablishmentViewModel> modelToViewModelMapper,
        IMapper<Establishment, string[]> modelToCsvMapper)
    {
        _logger = logger;
        _getEstablishmentUseCase = getEstablishmentUseCase;
        _getEstablishmentsUseCase = getEstablishmentsUseCase;
        _csvResponseBuilder = csvResponseBuilder;
        _modelToViewModelMapper = modelToViewModelMapper;

        _modelToCsvMapper =
            modelToCsvMapper as ModelToCsvMapper<Establishment>
            ?? throw new InvalidOperationException("Expected ModelToCsvMapper instance.");
    }

    [HttpGet("{urn:int}", Name = "GetEstablishmentByUrn")]
    public async Task<IActionResult> GetByUrn(int urn, CancellationToken cancellationToken = default)
    {
        UseCaseResponse<Establishment> result =
            await _getEstablishmentUseCase
                .HandleRequestAsync(
                    GetEstablishmentByUrnRequest.Create(urn), cancellationToken);

        if (!result.SuccessfulRequest){
            return Problem(
                detail: result.ErrorMessage ?? "Unknown error",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        if (!result.HasValidModel()){
            return NotFound($"No establishment found for URN {urn}.");
        }

        EstablishmentViewModel viewModel =
            _modelToViewModelMapper.Map(result.Model!);

        return Ok(viewModel);
    }

    [HttpGet(Name = "GetEstablishments")]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        UseCaseResponse<IReadOnlyCollection<Establishment>> result =
            await _getEstablishmentsUseCase
                .HandleRequestAsync(cancellationToken);

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
                statusCode: StatusCodes.Status404NotFound);
        }

        #pragma warning disable CS1998
        async IAsyncEnumerable<EstablishmentViewModel> StreamResults(
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            foreach (Establishment establishment in result.Model!)
            {
                ct.ThrowIfCancellationRequested();

                yield return _modelToViewModelMapper.Map(establishment);
            }
        }
        #pragma warning restore CS1998

        return Ok(StreamResults(cancellationToken));
    }

    [HttpGet("csv", Name = "GetEstablishmentsCsv")]
    public async Task<IActionResult> GetCsv(CancellationToken cancellationToken)
    {
        UseCaseResponse<IReadOnlyCollection<Establishment>> result =
            await _getEstablishmentsUseCase
                .HandleRequestAsync(cancellationToken);

        if (!result.SuccessfulRequest){
            return Problem(
                detail: result.ErrorMessage ?? "Unknown error",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        if (!result.HasValidModel()){
            return Problem(
                detail: "Use case returned no establishment data.",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        // Delegate the entire CSV streaming workflow to the response builder.
        return await
            _csvResponseBuilder.WriteCsvAsync(
                Response,
                rows: result.Model!,
                headerColumns: _modelToCsvMapper.Headers,
                rowSelector: row => _modelToCsvMapper.Map(row),
                fileName: "establishments.csv",
                cancellationToken);
    }
}
