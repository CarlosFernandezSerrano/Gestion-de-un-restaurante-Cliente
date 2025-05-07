using Newtonsoft.Json;

namespace Assets.Scripts.Model
{
    public class Rol
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public Rol(string nombre)
        {
            this.Nombre = nombre;
        }

        [JsonConstructor]
        public Rol(int id, string nombre)
        {
            this.Id = id;
            this.Nombre = nombre;
        }

        public string mostrar()
        {
            return this.Id + " " + this.Nombre;
        }
    }
}
