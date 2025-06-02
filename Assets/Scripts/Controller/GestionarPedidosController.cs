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

public class GestionarPedidosController : MonoBehaviour
{
    //se debe marcar la categoría seleccionada
    public RectTransform fondoPedidos;
    public GestionarPedidosController instanceGestionarPedidosController;
    public MétodosAPIController instanceMétodosApiController;
    public TextMeshProUGUI titulo;
    public TextMeshProUGUI articulosPedidos;
    public TextMeshProUGUI selMesa;
    public Articulo articuloPrueba;
    public Pedido pedido;
    public Factura factura;
    public GestionarFacturasController instanceGestionarFacturasController;
    public GameObject canvasFacturas;
    public Sprite imagenPrueba;
    public int idMesa;
    void Awake()
    {
        instanceMétodosApiController = MétodosAPIController.InstanceMétodosAPIController;
        if (instanceGestionarPedidosController == null)
        {
            instanceGestionarPedidosController = this;
        }
        //CAMBIAR COMO OBTENER MESA
        //Mesa m = new Mesa(1, 0, 0, 0, 0, 0, 0, 0, true, 1, null);
        //idMesa = 1;
        articuloPrueba = new Articulo(1, "coca cola", (float)1.50, "bebidas");
        factura = new Factura(1,(float)1.0,new List<Pedido>());
        pedido = new Pedido(1, "16:00", 1, "COMPLETADO");
        //addArticulo(articuloPrueba);
        probarGuardar();
    }
    void Start()
    {
        instanceMétodosApiController = MétodosAPIController.InstanceMétodosAPIController;

        TrabajadorController.ComprobandoDatosTrabajador = false;


    }
    public void cambiarMesa()
    {
        string str2 = "";
        var matches = Regex.Matches(selMesa.text, @"\d+");
        foreach (var match in matches)
        {
            str2 += match;
        }
        idMesa = int.Parse(str2);
        titulo.text = "" + idMesa;
    }

    public void volverAMain()
    {
        SceneManager.LoadScene("Main");
    }


    public void crearBotonArticulo()
    {
        GameObject boton = new GameObject("Articulo-" + 5);
        boton.transform.SetParent(fondoPedidos);
        RectTransform rt = boton.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200, 200);
        boton.AddComponent<CanvasRenderer>();
        UnityEngine.UI.Image imagen = boton.AddComponent<UnityEngine.UI.Image>();
        imagen.sprite = imagenPrueba;
        rt.anchoredPosition = new Vector2(0, 0);
        rt.sizeDelta = new Vector2(150, 200); 
        GameObject textGO = new GameObject("TMP_Text");

        // Establezco el padre para que se muestre en el UI.
        textGO.transform.SetParent(rt, false);

        // Agrego el componente RectTransform (se agrega automáticamente al crear UI, pero lo añado explícitamente).
        RectTransform rtText = textGO.AddComponent<RectTransform>();
        // Anclas estiradas (stretch) en ambas direcciones
        rtText.anchorMin = new Vector2(0, 0);
        rtText.anchorMax = new Vector2(1, 1);

        // Márgenes todos a 0 (equivale a Left, Right, Top, Bottom en el inspector)
        rtText.offsetMin = Vector2.zero;
        rtText.offsetMax = Vector2.zero;

        // Centrado por si acaso (aunque no influye mucho cuando está estirado)
        rtText.anchoredPosition = Vector2.zero;

        // Agrego CanvasRenderer para poder renderizar el UI.
        textGO.AddComponent<CanvasRenderer>();

        // Agrego el componente TMP_Text para mostrar el sprite.
        TMP_Text textoBotón = textGO.AddComponent<TextMeshProUGUI>();
        textoBotón.fontStyle = FontStyles.Bold;
        textoBotón.fontSize = 20;
        textoBotón.alignment = TextAlignmentOptions.Center;
        textoBotón.text = "Prueba";

        boton.AddComponent<Button>();
    }

    /*public void addArticulo(Articulo a)
    {
        InstanciaArticulo art = new InstanciaArticulo(a, pedido.id, 1, 10);
        bool existe = false;
        foreach (InstanciaArticulo ar in pedido.listaArticulos)
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
        //articulosPedidos.text = pedido.listarArticulos();
    }*/

    public void pasarAFacturas()
    {
        instanceGestionarFacturasController.pedido = pedido;
        instanceGestionarFacturasController.factura = factura;
        canvasFacturas.SetActive(true);
    }

    private async Task registrarPedido(Pedido p)
    {
        Pedido ped = new Pedido(1,"123",1,"COMPLETADO");
        Debug.Log("Pedido:"+ped.Mostrar());
        string cad = await instanceMétodosApiController.PostDataAsync("pedido/makePedido", ped);
        Debug.Log("Error:"+cad);
        //Tal vez se deba usar int en vez de Result
        //En principio no se usará la respuesta para nada, salvo comprobar si el programa funciona correctamente
        //Resultado resultado=JsonConvert.DeserializeObject<Resultado>(cad);
    }
    public async Task probarGuardar()
    {
        articulosPedidos.text = await instanceMétodosApiController.GetDataAsync("pedido/getNumPedidos");
    }
    public async void finalizarPedido()
    {
        await registrarPedido(pedido);
    }

}
