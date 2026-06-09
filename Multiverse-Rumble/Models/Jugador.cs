namespace MultiverseRumble.Models
{
    public class Jugador
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public int PersonajeId { get; set; }
        public Personaje? Personaje { get; set; }
        public string Controles { get; set; } = ""; // WASD o Flechas
    }
}