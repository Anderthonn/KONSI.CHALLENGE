using KONSI.CHALLENGE.DOMAIN;
using KONSI.CHALLENGE.SERVICES.Interfaces;
using KONSI.CHALLENGE.WEBUI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KONSI.CHALLENGE.WEBUI.Tests.Controllers
{
    public class MainControllerTests
    {
        [Fact]
        public async Task ConsultationBenefits_ReturnsJsonResult_WhenValidCpf()
        {
            // Arrange
            var mainServiceMock = new Mock<IMainService>();
            var controller = new MainController(mainServiceMock.Object);
            var expectedConsultationBenefits = new List<ConsultationBenefits>(); // Lista vazia para simular benefícios retornados pelo serviço

            mainServiceMock.Setup(x => x.ProcessCPF(It.IsAny<string>())).ReturnsAsync(expectedConsultationBenefits);

            // Act
            var result = await controller.ConsultationBenefits("12345678901");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(expectedConsultationBenefits, jsonResult.Value);
        }

        [Fact]
        public async Task ConsultationBenefits_ReturnsInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            var mainServiceMock = new Mock<IMainService>();
            var controller = new MainController(mainServiceMock.Object);
            var expectedErrorMessage = "Erro ao processar CPF.";

            mainServiceMock.Setup(x => x.ProcessCPF(It.IsAny<string>())).ThrowsAsync(new Exception(expectedErrorMessage));

            // Act
            var result = await controller.ConsultationBenefits("12345678901");

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.Equal(expectedErrorMessage, objectResult.Value);
        }

    }
}
