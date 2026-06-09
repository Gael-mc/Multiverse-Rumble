namespace MultiverseRumble.Models
{
    public class Habilidad
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public int Dano { get; set; } = 10;
        public string Tipo { get; set; } = ""; // Normal, Especial, Ultimate
        public int PersonajeId { get; set; }
    }
}