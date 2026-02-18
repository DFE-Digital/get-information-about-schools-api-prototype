using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.API.EstablishmentGroups.ViewModels;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Shared.Application.Usecases;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace DfE.GetInformationAboutSchools.Prototyping.API.EstablishmentGroups.GetEstablishmentGroups.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class EstablishmentGroupsController : ControllerBase
{
    private readonly IUseCaseResponseOnly<
        UseCaseResponse<IReadOnlyCollection<EstablishmentGroup>>> _getGroupsUseCase;

    private readonly IMapper<
        EstablishmentGroup, EstablishmentGroupViewModel> _modelToViewModelMapper;

    public EstablishmentGroupsController(
        IUseCaseResponseOnly<
            UseCaseResponse<IReadOnlyCollection<EstablishmentGroup>>> getGroupsUseCase,
        IMapper<
            EstablishmentGroup, EstablishmentGroupViewModel> modelToViewModelMapper)
    {
        _getGroupsUseCase = getGroupsUseCase;
        _modelToViewModelMapper = modelToViewModelMapper;
    }

    [HttpGet(Name = "GetEstablishmentGroups")]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        UseCaseResponse<IReadOnlyCollection<EstablishmentGroup>> result =
            await _getGroupsUseCase
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
                detail: "Use case returned no group data.",
                statusCode: StatusCodes.Status404NotFound);
        }

        #pragma warning disable CS1998
        async IAsyncEnumerable<EstablishmentGroupViewModel> StreamResults(
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
}
