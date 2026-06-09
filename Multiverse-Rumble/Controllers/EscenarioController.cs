using Microsoft.AspNetCore.Mvc;
using MultiverseRumble.Models;

namespace MultiverseRumble.Controllers
{
    public class EscenarioController : Controller
    {
        private static List<Escenario> _escenarios = new()
        {
            new Escenario { Id = 1, Nombre = "Namek", Universo = "Dragon Ball", ImagenUrl = "/img/namek.png", Descripcion = "Planeta de los Namekianos" },
            new Escenario { Id = 2, Nombre = "Ciudad Metrópolis", Universo = "DC Comics", ImagenUrl = "/img/metropolis.png", Descripcion = "Ciudad protegida por Superman" },
            new Escenario { Id = 3, Nombre = "Valle del Fin", Universo = "Naruto", ImagenUrl = "/img/valle.png", Descripcion = "Lugar del combate final entre Naruto y Sasuke" },
        };

        public IActionResult Index()
        {
            return View(_escenarios);
        }

        public IActionResult Detalle(int id)
        {
            var escenario = _escenarios.FirstOrDefault(e => e.Id == id);
            if (escenario == null) return NotFound();
            return View(escenario);
        }
    }
}