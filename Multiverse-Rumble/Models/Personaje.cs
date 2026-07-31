namespace MultiverseRumble.Models
{
    public class Personaje
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Universo { get; set; } = "";
        public string Franquicia { get; set; } = "";
        public string ImagenUrl { get; set; } = "";

        // Sprites para el combate (reposo / ataque / salto / caminar). Si no existen o no cargan,
        // el motor de combate dibuja un bloque de color de respaldo usando ColorPrincipal/ColorSecundario.
        public string SpriteIdleUrl { get; set; } = "";
        public string SpriteAtaqueUrl { get; set; } = "";
        public string SpriteSaltoUrl { get; set; } = "";
        public string SpriteCaminarUrl { get; set; } = "";
        public string ColorPrincipal { get; set; } = "#888888";
        public string ColorSecundario { get; set; } = "#333333";

        public int Vida { get; set; } = 100;
        public int Ataque { get; set; } = 10;
        public int Defensa { get; set; } = 5;
        public int Velocidad { get; set; } = 5;
        public List<Habilidad> Habilidades { get; set; } = new();
    }
}