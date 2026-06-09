using Microsoft.AspNetCore.Mvc;
using MultiverseRumble.Models;

namespace MultiverseRumble.Controllers
{
    public class PersonajeController : Controller
    {
        private static List<Personaje> _personajes = new()
        {
            new Personaje { Id = 1, Nombre = "Goku", Universo = "Dragon Ball", Franquicia = "Anime", ImagenUrl = "/img/goku.png", Vida = 100, Ataque = 15, Defensa = 8, Velocidad = 9 },
            new Personaje { Id = 2, Nombre = "Superman", Universo = "DC Comics", Franquicia = "Comic", ImagenUrl = "/img/superman.png", Vida = 100, Ataque = 14, Defensa = 10, Velocidad = 8 },
            new Personaje { Id = 3, Nombre = "Naruto", Universo = "Naruto", Franquicia = "Anime", ImagenUrl = "/img/naruto.png", Vida = 100, Ataque = 12, Defensa = 7, Velocidad = 10 },
            new Personaje { Id = 4, Nombre = "Luffy", Universo = "One Piece", Franquicia = "Anime", ImagenUrl = "/img/luffy.png", Vida = 100, Ataque = 13, Defensa = 6, Velocidad = 9 },
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