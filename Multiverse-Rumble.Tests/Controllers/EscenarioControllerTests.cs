using Microsoft.AspNetCore.Mvc;
using MultiverseRumble.Controllers;
using MultiverseRumble.Models;
using Xunit;

namespace Multiverse_Rumble.Tests.Controllers
{
    public class EscenarioControllerTests
    {
        [Fact]
        public void Index_RegresaLaListaCompletaDeEscenarios()
        {
            // Arrange
            var controller = new EscenarioController();

            // Act
            var resultado = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(resultado);
            var escenarios = Assert.IsAssignableFrom<List<Escenario>>(viewResult.Model);
            Assert.Contains(escenarios, e => e.Nombre == "Namek");
        }

        [Fact]
        public void Detalle_ConIdExistente_RegresaViewConElEscenarioCorrecto()
        {
            // Arrange
            var controller = new EscenarioController();

            // Act
            var resultado = controller.Detalle(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(resultado);
            var escenario = Assert.IsType<Escenario>(viewResult.Model);
            Assert.Equal("Namek", escenario.Nombre);
            Assert.Equal("Dragon Ball", escenario.Universo);
        }

        [Fact]
        public void Detalle_ConIdInexistente_RegresaNotFound()
        {
            // Arrange
            var controller = new EscenarioController();

            // Act
            var resultado = controller.Detalle(9999);

            // Assert
            Assert.IsType<NotFoundResult>(resultado);
        }
    }
}
