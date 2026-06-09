using Microsoft.AspNetCore.Mvc;
using MultiverseRumble.Models;

namespace MultiverseRumble.Controllers
{
    public class JugadorController : Controller
    {
        private static List<Jugador> _jugadores = new()
        {
            new Jugador { Id = 1, Nombre = "Jugador 1", Controles = "WASD + F/G" },
            new Jugador { Id = 2, Nombre = "Jugador 2", Controles = "Flechas + L/K" },
        };

        public IActionResult Index()
        {
            return View(_jugadores);
        }
    }
}