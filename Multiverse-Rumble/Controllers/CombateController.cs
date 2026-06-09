using Microsoft.AspNetCore.Mvc;
using MultiverseRumble.Models;

namespace MultiverseRumble.Controllers
{
    public class CombateController : Controller
    {
        private static List<Combate> _historial = new();

        private static List<Personaje> _personajes = new()
        {
            new Personaje { Id = 1, Nombre = "Goku", Universo = "Dragon Ball", Franquicia = "Anime", Vida = 100, Ataque = 15, Defensa = 8, Velocidad = 9 },
            new Personaje { Id = 2, Nombre = "Superman", Universo = "DC Comics", Franquicia = "Comic", Vida = 100, Ataque = 14, Defensa = 10, Velocidad = 8 },
            new Personaje { Id = 3, Nombre = "Naruto", Universo = "Naruto", Franquicia = "Anime", Vida = 100, Ataque = 12, Defensa = 7, Velocidad = 10 },
            new Personaje { Id = 4, Nombre = "Luffy", Universo = "One Piece", Franquicia = "Anime", Vida = 100, Ataque = 13, Defensa = 6, Velocidad = 9 },
        };

        private static List<Escenario> _escenarios = new()
        {
            new Escenario { Id = 1, Nombre = "Namek", Universo = "Dragon Ball" },
            new Escenario { Id = 2, Nombre = "Ciudad Metropolis", Universo = "DC Comics" },
            new Escenario { Id = 3, Nombre = "Valle del Fin", Universo = "Naruto" },
        };

        public IActionResult Seleccion()
        {
            ViewBag.Personajes = _personajes;
            ViewBag.Escenarios = _escenarios;
            return View();
        }

        public IActionResult Arena(int p1Id, int p2Id, int escenarioId)
        {
            var p1 = _personajes.FirstOrDefault(p => p.Id == p1Id);
            var p2 = _personajes.FirstOrDefault(p => p.Id == p2Id);
            var escenario = _escenarios.FirstOrDefault(e => e.Id == escenarioId);

            if (p1 == null || p2 == null || escenario == null)
                return RedirectToAction("Seleccion");

            ViewBag.Personaje1 = p1;
            ViewBag.Personaje2 = p2;
            ViewBag.Escenario = escenario;
            return View();
        }

        [HttpPost]
        public IActionResult GuardarResultado(string ganador, int p1Id, int p2Id, int escenarioId)
        {
            _historial.Add(new Combate
            {
                Id = _historial.Count + 1,
                Jugador1Id = p1Id,
                Jugador2Id = p2Id,
                EscenarioId = escenarioId,
                Ganador = ganador,
                Fecha = DateTime.Now
            });
            return RedirectToAction("Historial");
        }

        public IActionResult Historial()
        {
            return View(_historial);
        }
    }
}