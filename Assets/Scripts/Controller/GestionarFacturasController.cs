using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GestionarFacturasController : MonoBehaviour
{//QUEDA POR HACER: PARTE DE GESTIONAR FACTURAS (MOSTRAR TODOS LOS ARTÍCULOS Y EL TOTAL, FINALIZAR LA FACTURA CAMBIÁNDOLE EL ACTIVA)
    //DEBERÍA TAMBIÉN CAMBIARSE EL TOTAL DE LA FACTURA EN LA BASE DE DATOS AL CALCULAR SU TOTAL MEDIANTE GETTOTAL
    public Factura factura { get; set; }
    public int pedido { get; set; }
    public MétodosAPIController instanceMétodosApiController;
    public GestionarPedidosController instanceGestionarPedidosController;
    public TextMeshProUGUI ID;
    public TextMeshProUGUI Descripcion;
    public TextMeshProUGUI Cantidad;
    public TextMeshProUGUI Importe;
    public TextMeshProUGUI Total;
    public TextMeshProUGUI RestauranteTexto;
    public GameObject canvasMesas;
    public GameObject canvasFactura;
    public string restaurante;
    public static GestionarFacturasController instanceGestionarFacturasController;
    // Start is called before the first frame update
    void Start()
    {
        instanceMétodosApiController = MétodosAPIController.InstanceMétodosAPIController;
        instanceGestionarPedidosController = GestionarPedidosController.instanceGestionarPedidosController;
        instanceGestionarFacturasController=this;
    /*try
    {
        factura = instanceGestionarPedidosController.factura;
    }
    catch(Exception e)
    {
        factura = new Factura(1, 0, true, 1);
    }*/
    //mostrarDatosFactura();
}

    public void imprimirFactura()
    {
        //En un restaurante real, esto imprimiría la factura en un recibo
        Debug.Log(factura.Mostrar());
    }
    //CRASHEA, MIRAR
    public async void realizarPago()
    {
        //Puesto que no tenemos una pasarela de pago implementada, esto simplemente pasará la factura al estado no activo
        //Se deberían dropear todos los pedidos, que a su vez dropearán todas las instancias
        //AL DROPEAR PEDIDOS SE DEBERÍA VER CÓMO CONSEGUIR EL SIGUIENTE ID
        factura.activa = false;
        string cad = await instanceMétodosApiController.PutDataAsync("factura/actualizarActiva/", factura);
        Debug.Log(cad);
        Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

        if (resultado.Result.Equals(1))
        {
            Debug.Log("Estado cambiado correctamente");
        }
        else
        {
            Debug.Log("Error al finalizar factura");
        }
    }

    public async void entrarFactura(int idF)
    {
        Debug.Log("Pruebafac");
        Debug.Log("facturaasd");
        //this.pedido = idP;
        //Debug.Log("idp:"+idP+"idA"+idF);
        instanceMétodosApiController = MétodosAPIController.InstanceMétodosAPIController;
        string cad = await instanceMétodosApiController.GetDataAsync("factura/getFactura/" + idF);
        Debug.Log("cadena:"+cad);
        factura = JsonConvert.DeserializeObject<Factura>(cad);
        Debug.Log(factura);
        mostrarDatosFactura();
        //mostrarDatosFactura();
    }

    public async Task getNombreRestaurante()
    {
        string cad = await instanceMétodosApiController.GetDataAsync("mesa/ObtenerRestaurante/" + factura.mesa);
        Debug.Log("resultado 1:"+cad);
        int IDRes = JsonConvert.DeserializeObject<int>(cad);
        string cad2 = await instanceMétodosApiController.GetDataAsync("restaurante/getNombrePorId/" + IDRes);
        Debug.Log("resultado 2:"+cad2);
        if (Usuario.Idioma.CompareTo("Español") != 0)
        {
            RestauranteTexto.text = "Restaurant";
        }
        else
        {
            RestauranteTexto.text = "Restaurante";
        }
        RestauranteTexto.text = RestauranteTexto.text + cad2;
    }

    public async Task mostrarDatosFactura()
    {
        //En caso de pedir una misma cosa en varios pedidos (p.ej, una coca cola al inicio de la sesión y luego otra) se mostrarán como artículos diferentes. Se debería cambiar esto luego.
        string cad = await instanceMétodosApiController.GetDataAsync("factura/getPedidos/" + factura.id);
        Debug.Log(cad);
        List<Pedido> listaPed = JsonConvert.DeserializeObject<List<Pedido>>(cad);
        List<InstanciaArticulo> listaArt = new List<InstanciaArticulo>();
        ID.text = "";
        Descripcion.text = "";
        Cantidad.text = "";
        Importe.text = "";
        foreach (Pedido p in listaPed)
        {
            string cad2 = await instanceMétodosApiController.GetDataAsync("pedido/getArticulos/" + p.id);
            Debug.Log(cad2);
            List<InstanciaArticulo> listaAux= JsonConvert.DeserializeObject<List<InstanciaArticulo>>(cad2);
            foreach (InstanciaArticulo art in listaAux)
            {
                listaArt.Add(art);
            }
        }
        foreach(InstanciaArticulo a in listaArt)
        {
            string cad3 = await instanceMétodosApiController.GetDataAsync("articulo/getArticulo/" + a.idArticulo);
            Debug.Log(cad3);
            Articulo aux= JsonConvert.DeserializeObject<Articulo>(cad3);
            string nombre = aux.nombre + "\n";
            ID.text = ID.text + a.idArticulo + "\n";
            Descripcion.text = Descripcion.text + nombre+"\n";
            Cantidad.text = Cantidad.text +a.cantidad+ "\n";
            Importe.text = Importe.text + (a.cantidad*aux.precio)+"\n";
        }
        string cad4 = await instanceMétodosApiController.GetDataAsync("factura/getTotal/" + factura.id);
        Debug.Log(cad4);
        float tot = JsonConvert.DeserializeObject<float>(cad4);
        Total.text = "TOTAL: "+tot+" €";
        getNombreRestaurante();
        Debug.Log("Sale");
    }

    public void volver()
    {
        canvasMesas.SetActive(true);
        canvasFactura.SetActive(false);
    }
}
