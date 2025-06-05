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
//CAMBIAR LISTA DE PEDIDOS PARA REALIZARLA CON UN SCROLLVIEW
public class GestionarListaPedidos : MonoBehaviour
{
    // Start is called before the first frame update
    private List<Pedido> lista;
    public RectTransform fondoPedidos;
    public GameObject baseP;
    public GameObject canvasMesas;
    public GameObject canvasLista;
    public M�todosAPIController instanceM�todosApiController;
    public GestionarPedidosController instanceGestionarPedidosController;
    public TextMeshProUGUI selMesa;
    public Transform espacio;
    public static GestionarListaPedidos InstanceGestionarListaPedidos { get; private set; }
    //Deber�a haber una barra de scroll para explorar los pedidos

    void Start()
    {
        instanceM�todosApiController = M�todosAPIController.InstanceM�todosAPIController;
        instanceGestionarPedidosController = GestionarPedidosController.instanceGestionarPedidosController;
        if (InstanceGestionarListaPedidos == null)
        {
            InstanceGestionarListaPedidos = this;
        }
        //pruebaCrear();
        /*crearBotonesPedidos();
        cambiarEstadoPedido(69, "PRUEBO");*/
    }


    //esto deber�a tener un order by que empezara desde el ID m�s alto
    public async void crearBotonesPedidos(int mesa)
    {
        foreach (Transform t in espacio.transform)
        {
            Destroy(t.gameObject);
        }
        //LimpiarBotones();
        List<Pedido> lista;
        
        string cad = await instanceM�todosApiController.GetDataAsync("pedido/getTodosPedidos/");
        Debug.Log("CADENA:"+cad);
        lista = JsonConvert.DeserializeObject<List<Pedido>>(cad);
        int i = 0;
        foreach (Pedido p in lista)
        {
            if (mesa == null || mesa == 0|| p.mesa == mesa)
            {
                crearBoton(p, i);
                Debug.Log("Creado bot�n: " + p.id);
                i++;
            }
        }
    }
    public void crearBoton(Pedido p,int num)
    {
        GameObject botonP = Instantiate(baseP, fondoPedidos, true);
        botonP.transform.position = new Vector2(950, 820-num*150);
        botonP.transform.SetParent(fondoPedidos);
        botonP.AddComponent<CanvasRenderer>();
        // Crear un GameObject para el bot�n y asignarle un nombre �nico.
        botonP.name = "Pedido-" + p.id;
        GameObject tit = botonP.transform.Find("NumPedido").gameObject;
        TextMeshProUGUI texto = tit.GetComponent<TextMeshProUGUI>();
        texto.text = "Pedido "+p.id;
        GameObject fecha= botonP.transform.Find("Fecha").gameObject;
        TextMeshProUGUI textoF = fecha.GetComponent<TextMeshProUGUI>();
        textoF.text = p.fecha;
        GameObject dropdown = botonP.transform.Find("Dropdown").gameObject;
        TMP_Dropdown drop = dropdown.GetComponent<TMP_Dropdown>();
        drop.onValueChanged.AddListener((_) =>
        {
            cambiarEstadoPedido(p.id,drop.options[drop.value].text);
        });
        GameObject modificar = botonP.transform.Find("Modificar").gameObject;
        Button mod = modificar.AddComponent<Button>();
        mod.onClick.AddListener(() => modificarPedido(p));
        GameObject eliminar = botonP.transform.Find("Eliminar").gameObject;
        Button del = eliminar.AddComponent<Button>();
        del.onClick.AddListener(() => eliminarPedido(p.id));

        //modificar.GetComponent<Button>().onClick = () => { Debug.Log("Bot�n accedido correctamente"); };
    }
    public void auxFiltro()
    {
        string str2 = "";
        var matches = Regex.Matches(selMesa.text, @"\d+");
        foreach (var match in matches)
        {
            str2 += match;
        }
        int idMesa = int.Parse(str2);
        crearBotonesPedidos(idMesa);
    }

    /*public void pruebaCrear()
    {

        GameObject botonGO = Instantiate(baseP, fondoPedidos, true);
        botonGO.transform.SetParent(fondoPedidos);
        botonGO.AddComponent<CanvasRenderer>();
        // Crear un GameObject para el bot�n y asignarle un nombre �nico.
        botonGO.name = "Pedido-" + 1;
        // Referencias de componentes


        //PonerListenerADropdown(dropdown);
        /*
        // Crear un GameObject para el bot�n y asignarle un nombre �nico.
        GameObject botonGO = new GameObject("Button-" + trabajador.Id);

        // Establecer el padre para que se muestre en el UI.
        botonGO.transform.SetParent(rtScrollViewContent, false);

        // Agregar el componente RectTransform (se agrega autom�ticamente al crear UI, pero lo a�adimos expl�citamente).
        RectTransform rt = botonGO.AddComponent<RectTransform>();

        // Agregar CanvasRenderer para poder renderizar el UI.
        botonGO.AddComponent<CanvasRenderer>();

        // Agregar el componente Image para mostrar el sprite.
        Image imagen = botonGO.AddComponent<Image>();

        // Agrego un componente Button para que sea interactivo
        botonGO.AddComponent<Button>();

        // Creo un nuevo GameObject hijo, el texto del bot�n
        CrearTextoDelButton(rt, trabajador);
        


    }*/
    public async void  cambiarEstadoPedido(int idP,string s)
    {
        string cad = await instanceM�todosApiController.PutDataAsync("pedido/cambiarEstado/"+idP,s);
        Debug.Log("CAMBIO ESTADO"+cad);
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
    public void modificarPedido(Pedido p)
    {
        //OBTENER MESA Y FACTURA A PARTIR DE PEDIDO Y LUEGO IR A GESTIONAR PEDIDOS (EL CANVAS)
        instanceGestionarPedidosController.entrarPedido(p.mesa);
    }
    public async void eliminarPedido(int idP)
    {
        //ELIMINAR PEDIDO EN SERVER Y RECARGAR LISTA
        //DEBER�A HABER UN POP UP DE "SEGURO QUE LO QUIERES HACER?"
        string cad = await instanceM�todosApiController.DeleteDataAsync("pedido/borrar/"+idP);
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
        //FALTA ELIMINAR BOTONES
    }
    public void entrarLista(int mesa)
    {
        //SI ES 0, NO SE SETEA, EN OTRO CASO, SE PASA A BUSCAR CON EL FILTRO DE MESA
        canvasLista.SetActive(true);
        Debug.Log("Se ha activado canvas");
        crearBotonesPedidos(mesa);
    }
    public void volver()
    {
        canvasMesas.SetActive(true);
        canvasLista.SetActive(false);
    }
}
