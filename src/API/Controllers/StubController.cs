using Microsoft.AspNetCore.Mvc;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StubController : ControllerBase
    {
        private readonly ILogger<StubController> _logger;

        public StubController(ILogger<StubController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetStubData")]
        public IEnumerable<object> Get()
        {
            return [.. Enumerable.Range(1, 5).Select(index => new object
            {
                
            })];
        }
    }
}
