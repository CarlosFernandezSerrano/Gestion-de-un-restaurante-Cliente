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
using System.Text.RegularExpressions;
//UTILIZAR EL COUNT DE CADA TABLA PARA CREAR UN ID NUEVO ÚNICO (ESTO PUEDE DAR PROBLEMAS AL BORRAR ENTRADAS DE LA BASE DE DATOS, SI ESTO SUCEDE, MULTIPLICAR POR 5 Y VOLVER A INTENTAR?)
//Aumentar no parece funcionar correctamente, LOS BOTONES DE INSTANCIA ARTÍCULO EN GESTIONAR PEDIDOS NO VAN BIEN EN GENERAL
//TO DO: ELIMINAR ARTÍCULOS(INST) DE PEDIDOS, TAL VEZ AÑADIR COMENTARIOS A LOS PEDIDOS

public class GestionarPedidosController : MonoBehaviour
{
    //se debe marcar la categoría seleccionada
    public RectTransform fondoPedidos;
    public static GestionarPedidosController instanceGestionarPedidosController { get; set; }
    public GestionarMesasController instanceGestionarMesasController;
    public MétodosAPIController instanceMétodosApiController;
    public TextMeshProUGUI titulo;
    public TextMeshProUGUI articulosPedidos;
    public TextMeshProUGUI selMesa;
    public Articulo articuloPrueba;
    public Pedido pedido;
    public Factura factura;
    public GestionarFacturasController instanceGestionarFacturasController;
    public GameObject canvasMesas;
    public GameObject canvasFacturas;
    public Sprite imagenPrueba;
    public int idMesa;
    public GameObject canvasPedidos;

    void Awake()
    {
        instanceMétodosApiController = MétodosAPIController.InstanceMétodosAPIController;

    }
    void Start()
    {
        if (instanceGestionarPedidosController == null)
        {
            instanceGestionarPedidosController = this;
        }
        instanceGestionarMesasController= GestionarMesasController.InstanceGestionarMesasController;
        instanceMétodosApiController = MétodosAPIController.InstanceMétodosAPIController;
        //CAMBIAR COMO OBTENER MESA
        //Mesa m = new Mesa(1, 0, 0, 0, 0, 0, 0, 0, true, 1, null);
        //idMesa = 1;
        //SceneManager.LoadSceneAsync("General Controller", LoadSceneMode.Additive);
        pedido = new Pedido(69, "16:00", 1, "Completado", 1);
        factura = new Factura(1, 10, true, 1);
    }
    /* Ahora la mesa estará en Factura, así que se tiene que obtener cuando se abre la ventana y se asigna una factura
     * public void cambiarMesa()
    {
        string str2 = "";
        var matches = Regex.Matches(selMesa.text, @"\d+");
        foreach (var match in matches)
        {
            str2 += match;
        }
        idMesa = int.Parse(str2);
        titulo.text = "Pedido para la mesa "+idMesa;
        pedido.mesa = idMesa;
    }*/

    // Start is called before the first frame update
    public void volver()
    {
        canvasPedidos.SetActive(false);
        canvasMesas.SetActive(true);
    }

    //Crear los botones con un for que vaya cambiando de posición. Separación de 30 píxeles +160 tanto para arriba como para abajo. Se empieza en -800 150 y se termina en 360 -280
    //Para coger el nombre del botón, usamos botón.gameObject.name y cogemos nombre.Split("-")[1] para obtener el id (se debe parsear con Int32)
    public void crearBotonArticulo(Articulo art,int x,int y)
    {
        GameObject boton = new GameObject("Articulo-" + art.id);
        boton.transform.SetParent(fondoPedidos);
        RectTransform rt = boton.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(160, 160);
        rt.anchoredPosition = new Vector2(x, y);
        boton.AddComponent<CanvasRenderer>();

        UnityEngine.UI.Image imagen = boton.AddComponent<UnityEngine.UI.Image>();
        imagen.sprite = imagenPrueba;

        GameObject textObject = new GameObject("TextoBotón");
        textObject.transform.SetParent(rt);
        TextMeshProUGUI textMeshPro = textObject.AddComponent<TextMeshProUGUI>();
        textMeshPro.fontSize = 30;
        textMeshPro.alignment = TextAlignmentOptions.Center;
        textMeshPro.color = Color.black;
        textMeshPro.text = art.nombre;
        textObject.transform.localPosition = new Vector2(0,-100);

        Button b = boton.AddComponent<Button>();
        b.onClick.AddListener(()=> addArticulo(art.id));
    }
    public async void crearBotonesCategoria(string s)
    {
        Debug.Log(instanceMétodosApiController);
        string cad = await instanceMétodosApiController.GetDataAsync("articulo/getArticulosCat/" + s);
        Debug.Log(cad);
        List<Articulo> listaArt = JsonConvert.DeserializeObject<List<Articulo>>(cad);
        int x = -800;
        int y = 150;
        foreach (Articulo a in listaArt)
        {
            crearBotonArticulo(a, x, y);
            if (x >= 200)
            {
                x = -800;
                y -= 190;
            }
            else x += 190;
        }
        Debug.Log("Pruebafin");
    }

    public async void addArticulo(int id)
    {
        //Cantidad será siempre 1, al pulsar un botón se añadirá 1 a la cantidad, tal vez se debiera cambiar el botón para añadir varios de una vez. El servidor manejará si se crea realmente una instancia nueva o se añade 1 a una instancia existente
        InstanciaArticulo anyadido = new InstanciaArticulo(pedido.id,id,1);
        string cad = await instanceMétodosApiController.GetDataAsync("InstanciaArticulo/existeInstancia/"+id+"/"+pedido.id+"/");

        bool existe = JsonConvert.DeserializeObject<bool>(cad);
        string cad2;
        if (existe)
        {
            cad2 = await instanceMétodosApiController.PutDataAsync("InstanciaArticulo/aumentar/",anyadido);
        }
        else
        {
            cad2 = await instanceMétodosApiController.PostDataAsync("InstanciaArticulo/crearInstancia/", anyadido);
        }

        Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad2);
        if (resultado.Result.Equals(1))
        {
            Debug.Log("Registros exitosos de instanciaartículo"+cad2);
        }
        else
        {
            Debug.Log("Error en los registros "+cad2);
        }
        actualizarArticulos();
    }

    public async void actualizarArticulos()
    {
        string cad = await instanceMétodosApiController.GetDataAsync("pedido/getArticulos/" + pedido.id);
        Debug.Log("Cadena de artículos:"+cad);
        List<InstanciaArticulo> listaArt = JsonConvert.DeserializeObject<List<InstanciaArticulo>>(cad);
        string texto = "";
        foreach (InstanciaArticulo i in listaArt)
        {
            Debug.Log("Articulo: "+i.idArticulo);
            string cad2 = await instanceMétodosApiController.GetDataAsync("articulo/getArticulo/" + i.idArticulo);
            Debug.Log(cad2);
            Articulo ar= JsonConvert.DeserializeObject<Articulo>(cad2);
            texto += ar.nombre + " " + i.cantidad + "\n";
            Debug.Log("Finalizado un loop instancias:" + cad);
        }
        articulosPedidos.text = texto;
    }
    //Repetir usando servidor
    /* public void addArticulo(Articulo a)
     {
         Debug.Log("Funciona");
         //InstanciaArticulo art = new InstanciaArticulo(a.id,pedido.id,1,10);
         bool existe = false;
         /* 
          * foreach (InstanciaArticulo ar in pedido.listaArticulos)
         {
             if (art.idArticulo == ar.idArticulo)
             {
                 existe = true;
                 art = ar;
             }
         }
         if (!existe)
         {
             pedido.AddArticulo(art);
         }
         else
         {
             art.cantidad = art.cantidad + 1;
         }
         articulosPedidos.text = pedido.listarArticulos();
     }*/
    public void pasarAFacturas()
    {
        instanceGestionarFacturasController = new GestionarFacturasController();
        Debug.Log("fACTURA:"+factura.id);
        instanceGestionarFacturasController.entrarFactura(factura.id);
        Debug.Log("mesa"+factura.mesa);
        canvasFacturas.SetActive(true);
        Debug.Log("activa"+factura.activa);
        canvasPedidos.SetActive(false);
        Debug.Log(factura);
    }
    
    private async Task registrarPedido(Pedido p)
    {
        string cad = await instanceMétodosApiController.PostDataAsync("pedido/crearPedido",p);
        Debug.Log(cad);
    }
    public async void entrarPedido(int mesa)
    {
        canvasPedidos.SetActive(true);
        idMesa = mesa;
        //crear pedido, crear factura si no existe
        string cad=await instanceMétodosApiController.GetDataAsync("factura/obtenerActiva/"+ idMesa);
        Factura f=  JsonConvert.DeserializeObject<Factura>(cad);
        Debug.Log("Probar:" + f.Mostrar());
        if (f == null)
        {
            cad = await instanceMétodosApiController.GetDataAsync("factura/getNumFacturas/");
            int ia= JsonConvert.DeserializeObject<int>(cad);
            ia++;
            f = new Factura(ia,0,true,idMesa);
        }
        factura= f;
        cad = await instanceMétodosApiController.GetDataAsync("pedido/getNumPedidos/");
        int i = JsonConvert.DeserializeObject<int>(cad);
        //OBTENER LA HORA DE VERDAD
        pedido = new Pedido(i+1,"16:00",idMesa,"APUNTADO",f.id);
    }
    public void finalizarPedido()
    {
        registrarPedido(pedido);
    }

}
