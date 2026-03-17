using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.API.Establishments.GetEstablishments.Controllers;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.ModelBinding.Attributes;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response.Mappers;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Usecases.GetEstablishment;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Usecases.GetEstablishments.Request;
using DfE.GetInformationAboutSchools.Prototyping.Core.Shared.Application.Usecases;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace DfE.GetInformationAboutSchools.Prototyping.API.EstablishmentGroups.GetEstablishmentGroups.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class EstablishmentGroupsController : ControllerBase
{
    private readonly IUseCase<
        GetEstablishmentGroupsByRequiredFieldsRequest,
        UseCaseResponse<IReadOnlyCollection<EstablishmentGroup>>> _getEstablishmentGroupsUseCase;
    private readonly IUseCase<
        GetEstablishmentGroupByUidRequest,
        UseCaseResponse<EstablishmentGroup>> _getEstablishmentGroupByUid;
    private readonly IMapper<
        EstablishmentGroup, object?> _modelToViewModelMapper;
    private readonly ICsvResponseBuilder _csvResponseBuilder;
    private readonly ICsvMapper<EstablishmentGroup> _modelToCsvMapper;

    public EstablishmentGroupsController(
        IUseCase<
            GetEstablishmentGroupsByRequiredFieldsRequest,
            UseCaseResponse<IReadOnlyCollection<EstablishmentGroup>>> getEstablishmentGroupsUseCase,
        IUseCase<
            GetEstablishmentGroupByUidRequest,
            UseCaseResponse<EstablishmentGroup>> getEstablishmentGroupByUid,
        ICsvResponseBuilder csvResponseBuilder,
        IMapper<EstablishmentGroup, object?> modelToViewModelMapper,
        ICsvMapper<EstablishmentGroup> modelToCsvMapper)
    {
        _getEstablishmentGroupsUseCase = getEstablishmentGroupsUseCase;
        _getEstablishmentGroupByUid = getEstablishmentGroupByUid;
        _modelToViewModelMapper = modelToViewModelMapper;
        _csvResponseBuilder = csvResponseBuilder;
        _modelToCsvMapper = modelToCsvMapper;
    }

    [HttpGet("{uid:int}", Name = "GetEstablishmentGroupByUid")]
    public async Task<IActionResult> GetByUid(
        int uid, CancellationToken cancellationToken = default)
    {
        UseCaseResponse<EstablishmentGroup> result =
            await _getEstablishmentGroupByUid
                .HandleRequestAsync(
                    GetEstablishmentGroupByUidRequest.Create(uid), cancellationToken);

        if (!result.SuccessfulRequest)
        {
            return Problem(
                detail: result.ErrorMessage ?? "Unknown error",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        if (!result.HasValidModel())
        {
            return NotFound($"No establishment group found for UID {uid}.");
        }

        object? viewModel =
            _modelToViewModelMapper.Map(result.Model!);

        return Ok(viewModel);
    }

    [HttpGet(Name = "GetEstablishmentGroups")]
    public async Task<IActionResult> Get(
        [FromQuery]
        [RequestWithRequiredFields("EstablishmentGroups")]
        GetEstablishmentGroupsRequest requiredEstablishmentGroupFields,
        CancellationToken cancellationToken = default)
    {
        UseCaseResponse<IReadOnlyCollection<EstablishmentGroup>> result =
            await _getEstablishmentGroupsUseCase
                .HandleRequestAsync(
                    GetEstablishmentGroupsByRequiredFieldsRequest
                        .Create(requiredEstablishmentGroupFields.Fields), cancellationToken);

        if (!result.SuccessfulRequest)
        {
            return Problem(
                detail: result.ErrorMessage ?? "Unknown error",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        if (!result.HasValidModel())
        {
            return Problem(
                detail: "Use case returned no establishment group data.",
                statusCode: StatusCodes.Status404NotFound);
        }

#pragma warning disable CS1998
        async IAsyncEnumerable<object?> StreamResults(
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            foreach (EstablishmentGroup group in result.Model!)
            {
                ct.ThrowIfCancellationRequested();
                yield return _modelToViewModelMapper.Map(group);
            }
        }
#pragma warning restore CS1998

        return Ok(StreamResults(cancellationToken));
    }

    [HttpGet("csv", Name = "GetEstablishmentGroupsCsv")]
    public async Task<IActionResult> GetCsv(
        [FromQuery]
        [RequestWithRequiredFields("EstablishmentGroups")]
        GetEstablishmentGroupsRequest requiredEstablishmentGroupFields,
        CancellationToken cancellationToken = default)
    {
        UseCaseResponse<IReadOnlyCollection<EstablishmentGroup>> result =
            await _getEstablishmentGroupsUseCase
                .HandleRequestAsync(
                    GetEstablishmentGroupsByRequiredFieldsRequest
                        .Create(requiredEstablishmentGroupFields.Fields), cancellationToken);

        if (!result.SuccessfulRequest)
        {
            return Problem(
                detail: result.ErrorMessage ?? "Unknown error",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        if (!result.HasValidModel())
        {
            return Problem(
                detail: "Use case returned no establishment groups data.",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        return await _csvResponseBuilder.WriteCsvAsync(
            Response,
            rows: result.Model!,
            headerColumns: _modelToCsvMapper.Headers,
            rowSelector: group => _modelToCsvMapper.Map(group),
            fileName: "establishmentGroups.csv",
            cancellationToken);
    }
}
