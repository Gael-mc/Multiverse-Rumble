using Microsoft.AspNetCore.Mvc;
using MultiverseRumble.Models;

namespace MultiverseRumble.Controllers
{
    public class EscenarioController : Controller
    {
        private static List<Escenario> _escenarios = new()
        {
            new Escenario { Id = 1, Nombre = "Namek", Universo = "Dragon Ball", ImagenUrl = "/img/namek.png", ImagenFondoUrl = "/img/escenarios/namek.png", Descripcion = "Planeta de los Namekianos" },
            new Escenario { Id = 2, Nombre = "Gotham City", Universo = "DC Comics", ImagenUrl = "/img/gotham.png", ImagenFondoUrl = "/img/escenarios/gotham.png", Descripcion = "Azotea gótica vigilada por Batman" },
            new Escenario { Id = 3, Nombre = "Ciudad de México", Universo = "CDMX", ImagenUrl = "/img/cdmx.png", ImagenFondoUrl = "/img/escenarios/cdmx.png", Descripcion = "Parada de camión y puesto de periódicos en plena CDMX" },
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