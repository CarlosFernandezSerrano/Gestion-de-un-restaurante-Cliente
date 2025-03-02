using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Model
{
    class Restaurante
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string HoraApertura { get; set; }
        public string HoraCierre { get; set; }
        public List<Mesa> Mesas { get; set; } = new List<Mesa>();


        public Restaurante(string nombre, string horaApertura, string horaCierre, List<Mesa> mesas)
        {
            this.Nombre = nombre;
            this.HoraApertura = horaApertura;
            this.HoraCierre = horaCierre;
            this.Mesas = mesas;
        }

        [JsonConstructor]
        public Restaurante(int Id, string nombre, string horaApertura, string horaCierre, List<Mesa> mesas)
        {
            this.Id = Id;
            this.Nombre = nombre;
            this.HoraApertura = horaApertura;
            this.HoraCierre = horaCierre;
            this.Mesas = mesas;
        }

        public string mostrar()
        {
            return this.Id + " " + this.Nombre + " " + this.HoraApertura + " " + this.HoraCierre + " " + this.Mesas + " " + mostrarLista("Mesas: ",this.Mesas);
        }

        private string mostrarLista(string cad1, List<Mesa> mesas)
        {
            string cad = "" + cad1 + ": ";
            foreach (Mesa mesa in mesas)
            {
                cad += mesa.mostrar() + ", \n";
            }
            return cad;

        }
    }
}
