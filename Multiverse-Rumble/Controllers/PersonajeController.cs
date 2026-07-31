using Microsoft.AspNetCore.Mvc;
using MultiverseRumble.Models;

namespace MultiverseRumble.Controllers
{
    public class PersonajeController : Controller
    {
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

        public IActionResult Index()
        {
            return View(_personajes);
        }

        public IActionResult Detalle(int id)
        {
            var personaje = _personajes.FirstOrDefault(p => p.Id == id);
            if (personaje == null) return NotFound();
            return View(personaje);
        }

        public IActionResult Agregar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Agregar(Personaje personaje)
        {
            personaje.Id = _personajes.Count + 1;
            _personajes.Add(personaje);
            return RedirectToAction("Index");
        }

        public IActionResult Eliminar(int id)
        {
            var personaje = _personajes.FirstOrDefault(p => p.Id == id);
            if (personaje != null) _personajes.Remove(personaje);
            return RedirectToAction("Index");
        }
    }
}