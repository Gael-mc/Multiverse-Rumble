using Microsoft.AspNetCore.Mvc;
using MultiverseRumble.Controllers;
using MultiverseRumble.Models;
using Xunit;

namespace Multiverse_Rumble.Tests.Controllers
{
    public class CombateControllerTests
    {
        [Fact]
        public void Arena_ConIdsValidos_RegresaViewConLosDatosCorrectos()
        {
            // Arrange
            var controller = new CombateController();

            // Act
            var resultado = controller.Arena(p1Id: 1, p2Id: 2, escenarioId: 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(resultado);
            var personaje1 = Assert.IsType<Personaje>(viewResult.ViewData["Personaje1"]);
            var personaje2 = Assert.IsType<Personaje>(viewResult.ViewData["Personaje2"]);
            var escenario = Assert.IsType<Escenario>(viewResult.ViewData["Escenario"]);

            Assert.Equal("Goku", personaje1.Nombre);
            Assert.Equal("Naruto", personaje2.Nombre);
            Assert.Equal("Namek", escenario.Nombre);
        }

        [Fact]
        public void Arena_ConIdDePersonajeInexistente_RedirigeASeleccion()
        {
            // Arrange
            var controller = new CombateController();

            // Act
            var resultado = controller.Arena(p1Id: 9999, p2Id: 2, escenarioId: 1);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("Seleccion", redirect.ActionName);
        }

        [Fact]
        public void GuardarResultado_AgregaElCombateAlHistorialYRedirige()
        {
            // Arrange
            var controller = new CombateController();
            const string marcaDePrueba = "Ganador_Prueba_Unica_123";

            // Act
            var resultado = controller.GuardarResultado(marcaDePrueba, p1Id: 1, p2Id: 2, escenarioId: 1);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("Historial", redirect.ActionName);

            var historialResultado = controller.Historial();
            var viewResult = Assert.IsType<ViewResult>(historialResultado);
            var historial = Assert.IsAssignableFrom<List<Combate>>(viewResult.Model);
            Assert.Contains(historial, c => c.Ganador == marcaDePrueba);
        }
    }
}
