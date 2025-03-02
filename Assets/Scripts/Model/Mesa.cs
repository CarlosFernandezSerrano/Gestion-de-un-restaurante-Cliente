using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Model
{
    class Mesa
    {
        public int Id { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public bool Disponible { get; set; }
        public int Restaurante_ID { get; set; }

        public Mesa(float posX, float posY, float scaleX, float scaleY, bool disponible, int restaurante_Id)
        {
            this.PosX = posX;
            this.PosY = posY;
            this.ScaleX = scaleX;
            this.ScaleY = scaleY;
            this.Disponible = disponible;
            this.Restaurante_ID = restaurante_Id;
        }

        public Mesa(int id, float posX, float posY, float scaleX, float scaleY, bool disponible, int restaurante_Id)
        {
            this.Id = id;
            this.PosX = posX;
            this.PosY = posY;
            this.ScaleX = scaleX;
            this.ScaleY = scaleY;
            this.Disponible = disponible;
            this.Restaurante_ID = restaurante_Id;
        }

        public string mostrar()
        {
            return this.Id + " " + this.PosX + " " + this.PosY + " " + this.ScaleX + " " + this.ScaleY + " " + this.Disponible + " " + this.Restaurante_ID;
        }
    }
}
