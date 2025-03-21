using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Model
{
    public enum EstadoReserva
    {
        Pendiente,
        Confirmada,
        Cancelada
    }

    class Reserva
    {
        public int Id { get; set; }
        public string Fecha { get; set; } // "YYYY-MM-DD"
        public string Hora { get; set; } // "HH:mm:ss"
        public string Estado { get; set; }
        public int Cliente_Id { get; set; }
        public int Mesa_Id { get; set; }

        public Reserva(string fecha, string hora, string estado, int cliente, int mesa)
        {
            this.Fecha = fecha;
            this.Hora = hora;
            this.Cliente_Id = cliente;
            this.Estado = estado;
            this.Mesa_Id = mesa;
        }

        [JsonConstructor]
        public Reserva(int id, string fecha, string hora, string estado, int cliente_id, int mesa_id)
        {
            this.Id = id;
            this.Fecha = fecha;
            this.Hora = hora;
            this.Estado = estado;
            this.Cliente_Id = cliente_id;
            this.Mesa_Id = mesa_id;
        }

        public string Mostrar()
        {
            return this.Id + " " + this.Fecha + " " + this.Hora + " " + this.Cliente_Id + " " + this.Estado + " " + this.Mesa_Id;
        }
    }
}
