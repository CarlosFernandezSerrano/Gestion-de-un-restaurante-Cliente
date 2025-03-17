using Newtonsoft.Json;
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
        public float Width { get; set; }
        public float Height { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public int CantPers { get; set; }
        public bool Disponible { get; set; }
        public int Restaurante_ID { get; set; }

        public Mesa(float posX, float posY, float width, float height, float scaleX, float scaleY, int cantPers, bool disponible, int restaurante_Id)
        {
            this.PosX = posX;
            this.PosY = posY;
            this.Width = width;
            this.Height = height;
            this.ScaleX = scaleX;
            this.ScaleY = scaleY;
            this.CantPers = cantPers;
            this.Disponible = disponible;
            this.Restaurante_ID = restaurante_Id;
        }

        [JsonConstructor]
        public Mesa(int id, float posX, float posY, float width, float height, float scaleX, float scaleY, int cantPers, bool disponible, int restaurante_Id)
        {
            this.Id = id;
            this.PosX = posX;
            this.PosY = posY;
            this.Width = width;
            this.Height = height;
            this.ScaleX = scaleX;
            this.ScaleY = scaleY;
            this.CantPers = cantPers;
            this.Disponible = disponible;
            this.Restaurante_ID = restaurante_Id;
        }

        public string Mostrar()
        {
            return this.Id + " " + this.PosX + " " + this.PosY + " " + this.ScaleX + " " + this.ScaleY + " " + this.CantPers + " " + this.Disponible + " " + this.Restaurante_ID;
        }
    }
}
