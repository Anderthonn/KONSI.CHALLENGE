using KONSI.CHALLENGE.SERVICES.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KONSI.CHALLENGE.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly IMainService _mainService;

        public MainController(IMainService mainService)
        {
            _mainService = mainService;
        }

        [Route("v1/process/{cpf}"), HttpGet]
        public async Task<IActionResult> Index(string cpf)
        {
            try
            {
                return Ok(await _mainService.ProcessCPF(cpf));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}