using Microsoft.AspNetCore.Mvc;
using MultiverseRumble.Controllers;
using MultiverseRumble.Models;
using Xunit;

namespace Multiverse_Rumble.Tests.Controllers
{
    public class PersonajeControllerTests
    {
        [Fact]
        public void Detalle_ConIdExistente_RegresaViewConElPersonajeCorrecto()
        {
            // Arrange
            var controller = new PersonajeController();

            // Act
            var resultado = controller.Detalle(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(resultado);
            var personaje = Assert.IsType<Personaje>(viewResult.Model);
            Assert.Equal("Alguien que no existe", personaje.Nombre);
        }

        [Fact]
        public void Detalle_ConIdInexistente_RegresaNotFound()
        {
            // Arrange
            var controller = new PersonajeController();

            // Act
            var resultado = controller.Detalle(9999);

            // Assert
            Assert.IsType<NotFoundResult>(resultado);
        }

        [Fact]
        public void Agregar_Post_AgregaElPersonajeYRedirigeAIndex()
        {
            // Arrange
            var controller = new PersonajeController();
            var nuevoPersonaje = new Personaje { Nombre = "Personaje_Prueba_Agregar" };

            // Act
            var resultado = controller.Agregar(nuevoPersonaje);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("Index", redirect.ActionName);

            var indexResultado = controller.Index();
            var viewResult = Assert.IsType<ViewResult>(indexResultado);
            var personajes = Assert.IsAssignableFrom<List<Personaje>>(viewResult.Model);
            Assert.Contains(personajes, p => p.Nombre == "Personaje_Prueba_Agregar");
        }

        [Fact]
        public void Eliminar_ConIdExistente_LoQuitaDeLaLista()
        {
            // Arrange
            var controller = new PersonajeController();
            var personajeTemporal = new Personaje { Nombre = "Personaje_Prueba_Eliminar" };
            controller.Agregar(personajeTemporal);

            var indexAntes = Assert.IsType<ViewResult>(controller.Index());
            var listaAntes = Assert.IsAssignableFrom<List<Personaje>>(indexAntes.Model);
            var idAsignado = listaAntes.Single(p => p.Nombre == "Personaje_Prueba_Eliminar").Id;

            // Act
            controller.Eliminar(idAsignado);

            // Assert
            var indexDespues = Assert.IsType<ViewResult>(controller.Index());
            var listaDespues = Assert.IsAssignableFrom<List<Personaje>>(indexDespues.Model);
            Assert.DoesNotContain(listaDespues, p => p.Nombre == "Personaje_Prueba_Eliminar");
        }
    }
}
