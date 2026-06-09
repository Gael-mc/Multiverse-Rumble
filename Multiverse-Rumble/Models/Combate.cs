namespace MultiverseRumble.Models
{
    public class Combate
    {
        public int Id { get; set; }
        public int Jugador1Id { get; set; }
        public Jugador? Jugador1 { get; set; }
        public int Jugador2Id { get; set; }
        public Jugador? Jugador2 { get; set; }
        public int EscenarioId { get; set; }
        public Escenario? Escenario { get; set; }
        public string? Ganador { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}