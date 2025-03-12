using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Model
{
    class Rol
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
