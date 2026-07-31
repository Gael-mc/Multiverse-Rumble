using Microsoft.AspNetCore.Mvc;
using MultiverseRumble.Models;

namespace MultiverseRumble.Controllers
{
    public class CombateController : Controller
    {
        private static List<Combate> _historial = new();

        private static List<Personaje> _personajes = new()
        {
            new Personaje
            {
                Id = 1, Nombre = "Goku", Universo = "Dragon Ball", Franquicia = "Anime",
                ImagenUrl = "/img/personajes/goku.png",
                SpriteIdleUrl = "/img/personajes/goku_reposo.png", SpriteAtaqueUrl = "/img/personajes/goku_ataque.png",
                SpriteSaltoUrl = "/img/personajes/goku_salto.png", SpriteCaminarUrl = "/img/personajes/goku_caminar.png",
                ColorPrincipal = "#ff8a00", ColorSecundario = "#2e5fdc",
                Vida = 100, Ataque = 15, Defensa = 8, Velocidad = 9
            },
            new Personaje
            {
                Id = 2, Nombre = "Naruto", Universo = "Naruto", Franquicia = "Anime",
                ImagenUrl = "/img/personajes/naruto.png",
                SpriteIdleUrl = "/img/personajes/naruto_reposo.png", SpriteAtaqueUrl = "/img/personajes/naruto_ataque.png",
                SpriteSaltoUrl = "/img/personajes/naruto_salto.png", SpriteCaminarUrl = "/img/personajes/naruto_caminar.png",
                ColorPrincipal = "#ff7a00", ColorSecundario = "#1b1b1b",
                Vida = 100, Ataque = 12, Defensa = 7, Velocidad = 10
            },
            new Personaje
            {
                Id = 3, Nombre = "Luffy", Universo = "One Piece", Franquicia = "Anime",
                ImagenUrl = "/img/personajes/luffy.png",
                SpriteIdleUrl = "/img/personajes/luffy_reposo.png", SpriteAtaqueUrl = "/img/personajes/luffy_ataque.png",
                SpriteSaltoUrl = "/img/personajes/luffy_salto.png", SpriteCaminarUrl = "/img/personajes/luffy_caminar.png",
                ColorPrincipal = "#d62828", ColorSecundario = "#1d3d8f",
                Vida = 100, Ataque = 13, Defensa = 6, Velocidad = 9
            },
            new Personaje
            {
                Id = 4, Nombre = "Scooby-Doo", Universo = "Hanna-Barbera", Franquicia = "Caricatura",
                ImagenUrl = "/img/personajes/scooby.png",
                SpriteIdleUrl = "/img/personajes/scooby_reposo.png", SpriteAtaqueUrl = "/img/personajes/scooby_ataque.png",
                SpriteSaltoUrl = "/img/personajes/scooby_salto.png", SpriteCaminarUrl = "/img/personajes/scooby_caminar.png",
                ColorPrincipal = "#8b5a2b", ColorSecundario = "#d9b382",
                Vida = 100, Ataque = 10, Defensa = 9, Velocidad = 8
            },
            new Personaje
            {
                Id = 5, Nombre = "Superman", Universo = "DC Comics", Franquicia = "Comic",
                ImagenUrl = "/img/personajes/superman.png",
                SpriteIdleUrl = "/img/personajes/superman_reposo.png", SpriteAtaqueUrl = "/img/personajes/superman_ataque.png",
                SpriteSaltoUrl = "/img/personajes/superman_salto.png", SpriteCaminarUrl = "/img/personajes/superman_caminar.png",
                ColorPrincipal = "#2166c4", ColorSecundario = "#d62828",
                Vida = 100, Ataque = 14, Defensa = 10, Velocidad = 8
            },
        };

        private static List<Escenario> _escenarios = new()
        {
            new Escenario { Id = 1, Nombre = "Namek", Universo = "Dragon Ball", ImagenFondoUrl = "/img/escenarios/namek.png" },
            new Escenario { Id = 2, Nombre = "Gotham City", Universo = "DC Comics", ImagenFondoUrl = "/img/escenarios/gotham.png" },
            new Escenario { Id = 3, Nombre = "Ciudad de México", Universo = "CDMX", ImagenFondoUrl = "/img/escenarios/cdmx.png" },
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