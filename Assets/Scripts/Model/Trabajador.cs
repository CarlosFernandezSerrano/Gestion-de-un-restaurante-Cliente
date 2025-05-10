using Newtonsoft.Json;


public class Trabajador 
{

    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Password { get; set; }
    public int Rol_ID { get; set; }
    public int Restaurante_ID { get; set; }

    public Trabajador(string nombre, string password, int rol_ID, int restaurante_ID)
    {
        this.Nombre = nombre;
        this.Password = password;
        this.Rol_ID = rol_ID;
        this.Restaurante_ID = restaurante_ID;
    }

    [JsonConstructor]
    public Trabajador(int id, string nombre, string password, int rol_ID, int restaurante_ID)
    {
        this.Id = id;
        this.Nombre = nombre;
        this.Password = password;
        this.Rol_ID = rol_ID;
        this.Restaurante_ID = restaurante_ID;
    }

    public string mostrar()
    {
        return this.Id + " " + this.Nombre +" "+ this.Password + " " + this.Rol_ID + " " + this.Restaurante_ID;
    }

    /*public string AJSONString(string cad)
    {
        return "\"" + cad + "\"";
    }
    public String toJSONString()
    {
        return "{" + string.Format("\"nombre\": {0}, \"puntos\": {1}",
                                  AJSONString(this.nombre),
                                  AJSONString(this.edad)) + "}";
    }*/
}
