using KONSI.CHALLENGE.DOMAIN;
using KONSI.CHALLENGE.SERVICES.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KONSI.CHALLENGE.WEBUI.Controllers
{
    public class MainController : Controller
    {
        private readonly IMainService _mainService;

        public MainController(IMainService mainService)
        {
            _mainService = mainService;
        }

        public IActionResult Index()
        {
            try
            {
                return View(new ConsultationBenefits());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public async Task<IActionResult> ConsultationBenefits(string cpf)
        {
            try
            {
                var consultationBenefits = await _mainService.ProcessCPF(cpf);
                return Json(consultationBenefits); // Retorna os dados como JSON
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}