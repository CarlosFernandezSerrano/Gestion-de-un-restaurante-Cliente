using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // Atributo obligatorio
public class Cliente 
{
    public static Cliente instanceCiente { get; private set; }

    public string id { get; set; }
    public string nombre { get; set; }
    public string edad { get; set; }
    public string correo { get; set; }

    public Cliente(string id, string nombre, string edad, string correo)
    {
        this.id = id;
        this.nombre = nombre;
        this.edad = edad;
        this.correo = correo;
    }

    private void Awake()
    {
        if (instanceCiente == null)
        {
            instanceCiente = this;
        }
    }
    public string mostrar()
    {
        return this.id +" "+ this.nombre + " " + this.edad + " " + this.correo;
    }

    public string AJSONString(string cad)
    {
        return "\"" + cad + "\"";
    }
    public String toJSONString()
    {
        return "{" + string.Format("\"nombre\": {0}, \"puntos\": {1}",
                                  AJSONString(this.nombre),
                                  AJSONString(this.edad)) + "}";
    }
}
