namespace MultiverseRumble.Models
{
    public class Personaje
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Universo { get; set; } = "";
        public string Franquicia { get; set; } = "";
        public string ImagenUrl { get; set; } = "";
        public int Vida { get; set; } = 100;
        public int Ataque { get; set; } = 10;
        public int Defensa { get; set; } = 5;
        public int Velocidad { get; set; } = 5;
        public List<Habilidad> Habilidades { get; set; } = new();
    }
}