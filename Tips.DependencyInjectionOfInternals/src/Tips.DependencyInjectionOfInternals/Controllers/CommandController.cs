using Microsoft.AspNetCore.Mvc;
using Tips.DependencyInjectionOfInternals.Business;
using Tips.DependencyInjectionOfInternals.Business.Models;

namespace Tips.DependencyInjectionOfInternals.Controllers
{
    [ApiController]
    public class CommandController : ControllerBase
    {
        private readonly IBusinessService _businessService;

        public CommandController(IBusinessService businessService)
        {
            _businessService = businessService;
        }

        [Route("v1/api/commands/{commandType=1}")]
        [HttpGet]
        public ActionResult Get(int commandType)
        {
            var request = new ProcessRequest { CommandType = (CommandType)commandType };
            var response = _businessService.Process(request);
            return Ok(response);
        }
    }
}
