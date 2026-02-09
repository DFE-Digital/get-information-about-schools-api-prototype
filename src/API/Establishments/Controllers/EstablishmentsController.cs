using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Establishments.Controllers;

[ApiController]
[Route("[controller]")]
public class EstablishmentsController : ControllerBase
{
    private readonly ILogger<EstablishmentsController> _logger;
    private readonly IUseCaseResponseOnly<
        UseCaseResponse<IReadOnlyCollection<Establishment>>> _useCase;

    public EstablishmentsController(
        ILogger<EstablishmentsController> logger,
        IUseCaseResponseOnly<
            UseCaseResponse<IReadOnlyCollection<Establishment>>> useCase)
    {
        _logger = logger;
        _useCase = useCase;
    }

    [HttpGet(Name = "GetEstablishments")]
    public IEnumerable<object> Get()
    {
        return [.. Enumerable.Range(1, 5).Select(index => new object
        {
            
        })];
    }
}
