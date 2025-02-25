using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // Atributo obligatorio
public class Cliente 
{
    public static Cliente instanceCiente { get; private set; }

    public string nombre { get; set; }
    public string contraseña { get; set; }
    public string rol { get; set; }
    public string cantMapas { get; set; }
    public List<string> restaurantes { get; set; }

    public Cliente(string nombre, string contraseña, string rol, string cantMapas, List<string> restaurantes)
    {
        this.nombre = nombre;
        this.contraseña = contraseña;
        this.rol = rol;
        this.cantMapas = cantMapas;
        this.restaurantes = restaurantes;
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
        return this.nombre +" "+ this.contraseña + " " + this.rol + " " + this.cantMapas + " " + mostrarLista("restaurantes",this.restaurantes);
    }

    private string mostrarLista(string cad1, List<string> restaurantes)
    {
        string cad = ""+cad1;
        foreach (string nombreRestaurante in restaurantes)
        {
            cad += nombreRestaurante + ", ";
        }
        return cad;

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
