using Newtonsoft.Json;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
    public class Restaurante
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string HoraApertura { get; set; }
        public string HoraCierre { get; set; }
        public string TiempoParaComer { get; set; }
        public List<Mesa> Mesas { get; set; } = new List<Mesa>();
        public List<Trabajador> Trabajadores { get; set; } = new List<Trabajador>();

        public static string TiempoPermitidoParaComer { get; set; } = "00:00";

        public Restaurante(string nombre, string horaApertura, string horaCierre, string tiempoParaComer, List<Mesa> mesas, List<Trabajador> trabajadores)
        {
            this.Nombre = nombre;
            this.HoraApertura = horaApertura;
            this.HoraCierre = horaCierre;
            this.TiempoParaComer = tiempoParaComer;
            this.Mesas = mesas;
            this.Trabajadores = trabajadores;
        }

        [JsonConstructor]
        public Restaurante(int Id, string nombre, string horaApertura, string horaCierre, string tiempoParaComer, List<Mesa> mesas, List<Trabajador> trabajadores)
        {
            this.Id = Id;
            this.Nombre = nombre;
            this.HoraApertura = horaApertura;
            this.HoraCierre = horaCierre;
            this.TiempoParaComer = tiempoParaComer;
            this.Mesas = mesas;
            this.Trabajadores = trabajadores;
        }

        public string mostrar()
        {
            return this.Id + " Nombre restaurante: " + this.Nombre + " Hora apertura: " + this.HoraApertura + " Hora cierre: " + this.HoraCierre + " " + this.Mesas + " " + MostrarListaMesas("Mesas: ",this.Mesas) + " " + MostrarListaTrabajadores("Trabajadores: ", this.Trabajadores);
        }

        private string MostrarListaMesas(string cad1, List<Mesa> mesas)
        {
            string cad = "" + cad1 + ": ";
            foreach (Mesa mesa in mesas)
            {
                cad += mesa.Mostrar() + ", \n";
            }
            return cad;

        }

        private string MostrarListaTrabajadores(string cad1, List<Trabajador> trabajadores)
        {
            string cad = "" + cad1 + ": ";
            foreach (Trabajador trabajador in trabajadores)
            {
                cad += trabajador.mostrar() + ", \n";
            }
            return cad;

        }
    }
}
