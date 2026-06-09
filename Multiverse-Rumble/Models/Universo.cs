namespace MultiverseRumble.Models
{
    public class Universo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Tipo { get; set; } = ""; // Anime, Pelicula, Comic, Videojuego
        public string Descripcion { get; set; } = "";
    }
}