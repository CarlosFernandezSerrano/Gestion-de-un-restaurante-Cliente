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
        public EstadoReserva Estado { get; set; }
        public int Cliente_Id { get; set; }
        public int Mesa_Id { get; set; }

        public Reserva(string fecha, string hora, EstadoReserva estado, int cliente, int mesa)
        {
            this.Fecha = fecha;
            this.Hora = hora;
            this.Cliente_Id = cliente;
            this.Estado = estado;
            this.Mesa_Id = mesa;
        }

        public Reserva(int id, string fecha, string hora, EstadoReserva estado, int cliente, int mesa)
        {
            this.Id = id;
            this.Fecha = fecha;
            this.Hora = hora;
            this.Cliente_Id = cliente;
            this.Estado = estado;
            this.Mesa_Id = mesa;
        }

        public string mostrar()
        {
            return this.Id + " " + this.Fecha + " " + this.Hora + " " + this.Cliente_Id + " " + this.Estado + " " + this.Mesa_Id;
        }
    }
}
