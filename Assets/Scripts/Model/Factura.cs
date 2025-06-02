using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Factura
{
    public int id { get; set; }
    public float total { get; set; }
    //public List<Pedido> listaPedidos { get; set; }

    public Factura(int id, float total, List<Pedido> listaPedidos)
    {
        this.id = id;
        this.total = total;
        //this.listaPedidos = listaPedidos;
    }

    [JsonConstructor]
    public Factura(int id, float total)
    {
        this.id = id;
        //this.listaPedidos = new List<Pedido>();
        this.total = 0;
    }

    /*public string MostrarPedidos()
    {
        string str= "";
        foreach (Pedido p in this.listaPedidos)
        {
            str += p.Mostrar()+"\n";
        }
        return str;
    }

    public string Mostrar()
    {
        return "" + id + " " + total + MostrarPedidos();
    }
    public void calcularTotal()
    {
            this.total = 0;
        foreach (Pedido p in listaPedidos)
        {
            total += p.totalPedido();
        }
    }
	public void addPedido(Pedido p){
		listaPedidos.Add(p);
	}*/
}
