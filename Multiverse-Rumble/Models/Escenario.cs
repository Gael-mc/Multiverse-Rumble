namespace MultiverseRumble.Models
{
    public class Escenario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Universo { get; set; } = "";
        public string ImagenUrl { get; set; } = "";

        // Fondo real del escenario para el canvas de combate. Si está vacío o no carga,
        // el motor dibuja el escenario por código (cielo + piso) como respaldo.
        public string ImagenFondoUrl { get; set; } = "";

        public string Descripcion { get; set; } = "";
    }
}