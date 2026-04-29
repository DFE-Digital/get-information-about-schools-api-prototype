using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.API.EstablishmentGroups.GetEstablishmentGroups.Controllers;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.ModelBinding.Attributes;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response.Mappers;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Usecases.GetEstablishment.Request;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Usecases.GetEstablishments.Request;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Usecases.SearchByFilters;
using DfE.GetInformationAboutSchools.Prototyping.Core.Shared.Application.Usecases;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Establishments.GetEstablishments.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class EstablishmentsController : ControllerBase
{
    private readonly ILogger<EstablishmentsController> _logger;
    private readonly IUseCase<
        GetEstablishmentsByRequiredFieldsRequest,
        UseCaseResponse<IReadOnlyCollection<Establishment>>> _getEstablishmentsUseCase;
    private readonly IUseCase<
        GetEstablishmentByUrnRequest,
        UseCaseResponse<Establishment>> _getEstablishmentUseCase;
    private readonly IMapper<Establishment, object?> _modelToViewModelMapper;
    private readonly ICsvResponseBuilder _csvResponseBuilder;
    private readonly ICsvMapper<Establishment> _modelToCsvMapper;

    private readonly IUseCase<
    SearchEstablishmentsRequest,
    UseCaseResponse<SearchEstablishmentsResponse>> _searchUseCase;

    public EstablishmentsController(
        ILogger<EstablishmentsController> logger,
        IUseCase<
            GetEstablishmentsByRequiredFieldsRequest,
            UseCaseResponse<IReadOnlyCollection<Establishment>>> getEstablishmentsUseCase,
        IUseCase<
             GetEstablishmentByUrnRequest,
             UseCaseResponse<Establishment>> getEstablishmentUseCase,
        ICsvResponseBuilder csvResponseBuilder,
        IMapper<Establishment, object?> modelToViewModelMapper,
        ICsvMapper<Establishment> modelToCsvMapper,
        IUseCase<
    SearchEstablishmentsRequest,
    UseCaseResponse<SearchEstablishmentsResponse>> searchUseCase)
    {
        _logger = logger;
        _getEstablishmentUseCase = getEstablishmentUseCase;
        _getEstablishmentsUseCase = getEstablishmentsUseCase;
        _csvResponseBuilder = csvResponseBuilder;
        _modelToViewModelMapper = modelToViewModelMapper;
        _modelToCsvMapper = modelToCsvMapper;
        _searchUseCase = searchUseCase;

    }

    [HttpGet("health", Name = "HealthCheck")]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "Service is running",
            timestamp = DateTime.UtcNow
        });
    }

    [HttpGet("{urn:int}", Name = "GetEstablishmentByUrn")]
    public async Task<IActionResult> GetByUrn(int urn, CancellationToken cancellationToken = default)
    {
        UseCaseResponse<Establishment> result =
            await _getEstablishmentUseCase
                .HandleRequestAsync(
                    GetEstablishmentByUrnRequest.Create(urn), cancellationToken);

        if (!result.SuccessfulRequest)
        {
            return Problem(
                detail: result.ErrorMessage ?? "Unknown error",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        if (!result.HasValidModel())
        {
            return NotFound($"No establishment found for URN {urn}.");
        }

        object? viewModel =
            _modelToViewModelMapper.Map(result.Model!);

        return Ok(viewModel);
    }

    [HttpGet(Name = "GetEstablishments")]
    public async Task<IActionResult> Get(
        [FromQuery]
        [RequestWithRequiredFields("Establishments")]
        GetEstablishmentsRequest requiredEstablishmentFields,
        CancellationToken cancellationToken = default)
    {
        UseCaseResponse<IReadOnlyCollection<Establishment>> result =
            await _getEstablishmentsUseCase
                .HandleRequestAsync(
                    GetEstablishmentsByRequiredFieldsRequest
                        .Create(requiredEstablishmentFields.Fields), cancellationToken);

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
        async IAsyncEnumerable<object?> StreamResults(
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
    public async Task<IActionResult> GetCsv(
        [FromQuery]
        [RequestWithRequiredFields("Establishments")]
        GetEstablishmentsRequest requiredFields,
        CancellationToken cancellationToken)
    {
        UseCaseResponse<IReadOnlyCollection<Establishment>> result =
            await _getEstablishmentsUseCase
                .HandleRequestAsync(
                    GetEstablishmentsByRequiredFieldsRequest
                        .Create(requiredFields.Fields), cancellationToken);

        if (!result.SuccessfulRequest)
        {
            return Problem(
                detail: result.ErrorMessage ?? "Unknown error",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        if (!result.HasValidModel())
        {
            return Problem(
                detail: "Use case returned no establishment data.",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        return await _csvResponseBuilder.WriteCsvAsync(
            Response,
            rows: result.Model!,
            headerColumns: _modelToCsvMapper.Headers,
            rowSelector: row => _modelToCsvMapper.Map(row),
            fileName: "establishments.csv",
            cancellationToken);
    }



    [HttpGet("search/fuzzy", Name = "SearchEstablishmentsFuzzy")]
    public async Task<IActionResult> SearchFuzzy(
    [FromQuery] string term,
    CancellationToken cancellationToken = default)
    {
        var request = SearchEstablishmentsRequest.Fuzzy(term);

        var result = await _searchUseCase.HandleRequestAsync(request, cancellationToken);

        if (!result.SuccessfulRequest)
        {
            return Problem(
                detail: result.ErrorMessage ?? "Unknown error",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        if (result.Model?.Fuzzy?.Results is null)
        {
            return NotFound("No establishments matched the fuzzy search.");
        }

        var mapped = result.Model.Fuzzy.Results
            .Select(e => _modelToViewModelMapper.Map(e));

        return Ok(mapped);
    }

    [HttpGet("search/filtered", Name = "SearchEstablishmentsFiltered")]
    public async Task<IActionResult> SearchFiltered(
    [FromQuery] string? status,
    [FromQuery] string? type,
    [FromQuery] string? text,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 20,
    CancellationToken cancellationToken = default)
    {
        var criteria = new EstablishmentFilterCriteria
        {
            Status = status,
            Type = type,
            Text = text,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var request = SearchEstablishmentsRequest.Filtered(criteria);

        var result = await _searchUseCase.HandleRequestAsync(request, cancellationToken);

        if (!result.SuccessfulRequest)
        {
            return Problem(
                detail: result.ErrorMessage ?? "Unknown error",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        if (result.Model?.Filtered?.Results is null)
        {
            return NotFound("No establishments matched the filtered search.");
        }

        var mapped = result.Model.Filtered.Results
            .Select(e => _modelToViewModelMapper.Map(e));

        return Ok(new
        {
            results = mapped,
            totalCount = result.Model.Filtered.TotalCount,
            facets = result.Model.Filtered.Facets
        });
    }


}
