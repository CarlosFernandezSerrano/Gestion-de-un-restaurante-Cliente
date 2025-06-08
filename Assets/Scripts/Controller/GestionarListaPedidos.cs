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
    public GameObject canvasPedidos;
    public MétodosAPIController instanceMétodosApiController;
    public GestionarPedidosController instanceGestionarPedidosController;
    public GestionarMesasController instanceGestionarMesasController;
    public TextMeshProUGUI selMesa;
    public TMP_InputField selMesaE;
    public Transform espacio;
    public Scrollbar scrollbarLista;
    public static GestionarListaPedidos InstanceGestionarListaPedidos { get; private set; }
    //Debería haber una barra de scroll para explorar los pedidos

    void Start()
    {
        instanceMétodosApiController = MétodosAPIController.InstanceMétodosAPIController;
        instanceGestionarPedidosController = GestionarPedidosController.instanceGestionarPedidosController;
        if (InstanceGestionarListaPedidos == null)
        {
            InstanceGestionarListaPedidos = this;
        }
        //pruebaCrear();
        /*crearBotonesPedidos();
        cambiarEstadoPedido(69, "PRUEBO");*/
    }

    /*public void buscarPorFiltro()
    {
        //esto debería pillar de MesaFiltro
        crearBotonesPedidos(0);
    }*/
    //esto debería tener un order by que empezara desde el ID más alto
    public async Task crearBotonesPedidos(int mesa)
    {
        foreach (Transform t in espacio.transform)
        {
            Destroy(t.gameObject);
            Debug.Log("Se ha eliminado: " + t);
        }
        //LimpiarBotones();
        List<Pedido> lista;
        
        string cad = await instanceMétodosApiController.GetDataAsync("pedido/getTodosPedidos/");
        Debug.Log("CADENA:"+cad);
        lista = JsonConvert.DeserializeObject<List<Pedido>>(cad);
        int i = 0;
        foreach (Pedido p in lista)
        {
            if (mesa == null || mesa == 0|| p.mesa == mesa)
            {
                crearBoton(p, i);
                Debug.Log("Creado botón: " + p.id);
                i++;
            }
        }
    }
    public async Task crearBotonesPedidos(string estado)
    {
        foreach (Transform t in espacio.transform)
        {
            Destroy(t.gameObject);
            Debug.Log("Se ha eliminado: " + t);
        }
        //LimpiarBotones();
        List<Pedido> lista;

        string cad = await instanceMétodosApiController.GetDataAsync("pedido/getTodosPedidos/");
        Debug.Log("CADENA:" + cad);
        lista = JsonConvert.DeserializeObject<List<Pedido>>(cad);
        int i = 0;
        foreach (Pedido p in lista)
        {
            if (estado==null || estado.ToUpper().Equals(p.estado) || estado.ToUpper().Equals(""))
            {
                crearBoton(p, i);
                Debug.Log("Creado botón: " + p.id);
                i++;
            }
        }
    }
    
    /*public void crearBoton(Pedido p, int num)
    {
        GameObject botonP = Instantiate(baseP, fondoPedidos, true);
        botonP.transform.position = new Vector2(950, 820 - num * 150);
        botonP.transform.SetParent(fondoPedidos);
        //botonP.AddComponent<CanvasRenderer>();
        // Crear un GameObject para el botón y asignarle un nombre único.
        botonP.name = "Pedido-" + p.id;
        GameObject tit = botonP.transform.Find("NumPedido").gameObject;
        TextMeshProUGUI texto = tit.GetComponent<TextMeshProUGUI>();
        texto.text = "Pedido " + p.id;
        GameObject fecha = botonP.transform.Find("Fecha").gameObject;
        TextMeshProUGUI textoF = fecha.GetComponent<TextMeshProUGUI>();
        textoF.text = "Mesa " + p.mesa;
        GameObject dropdown = botonP.transform.Find("Dropdown").gameObject;
        TMP_Dropdown drop = dropdown.GetComponent<TMPDropdown>();
        if (p.estado.ToUpper().Equals("APUNTADO"))
            drop.value = 0;
        else if (p.estado.ToUpper().Equals("ENCOCINA"))
            drop.value = 1;
        else if (p.estado.ToUpper().Equals("COMPLETADO"))
            drop.value = 2;
        else if (p.estado.ToUpper().Equals("PAGADO"))
            drop.value = 3;
        else drop.value = 4;
        drop.onValueChanged.AddListener(() =>
        {
            cambiarEstadoPedido(p, drop.options[drop.value].text);
        });
        GameObject modificar = botonP.transform.Find("Modificar").gameObject;
        Button mod = modificar.AddComponent<Button>();
        mod.onClick.AddListener(() => modificarPedido(p));
        GameObject eliminar = botonP.transform.Find("Eliminar").gameObject;
        Button del = eliminar.AddComponent<Button>();
        del.onClick.AddListener(() => eliminarPedido(p.id));

        //modificar.GetComponent<Button>().onClick = () => { Debug.Log("Botón accedido correctamente"); };
    }*/
    public void crearBoton(Pedido p,int num)
    {
        instanceGestionarMesasController = GestionarMesasController.InstanceGestionarMesasController;
        GameObject botonP = Instantiate(baseP, fondoPedidos, true);
        botonP.transform.position = new Vector2(950, 820-num*150);
        botonP.transform.SetParent(fondoPedidos);
        //botonP.AddComponent<CanvasRenderer>();
        // Crear un GameObject para el botón y asignarle un nombre único.
        botonP.name = "Pedido-" + p.id;
        GameObject tit = botonP.transform.Find("NumPedido").gameObject;
        TextMeshProUGUI texto = tit.GetComponent<TextMeshProUGUI>();
        GameObject fecha= botonP.transform.Find("Fecha").gameObject;
        TextMeshProUGUI textoF = fecha.GetComponent<TextMeshProUGUI>();
        if (Usuario.Idioma.CompareTo("Español") == 0)
        {
            texto.text = "Pedido " + p.id;
            textoF.text = "Mesa " + instanceGestionarMesasController.getNumMesa(p.mesa);
        }
        else
        {
            texto.text = "Order " + p.id;
            textoF.text = "Table " + instanceGestionarMesasController.getNumMesa(p.mesa);
        }
        /*if (Usuario.Idioma.CompareTo("Español") == 0)
        {
            texto.text = "Pedido " + p.id;
            textoF.text = "Mesa " + instanceGestionarMesasController.getNumMesa(p.mesa);
        }
        else
        {
            texto.text = "Order " + p.id;
            textoF.text = "Table " + instanceGestionarMesasController.getNumMesa(p.mesa);
        }*/

        GameObject dropdown = botonP.transform.Find("Dropdown").gameObject;
        TMP_Dropdown drop = dropdown.GetComponent<TMP_Dropdown>();
        if (p.estado.ToUpper().Equals("APUNTADO"))
            drop.value = 0;
        else if (p.estado.ToUpper().Equals("ENCOCINA"))
            drop.value = 1;
        else if (p.estado.ToUpper().Equals("COMPLETADO"))
            drop.value = 2;
        else if (p.estado.ToUpper().Equals("PAGADO"))
            drop.value = 3;
        else drop.value = 4;

        if (Usuario.Idioma.CompareTo("Español") != 0)
        {
            drop.options[0].text = "REGISTERED";
            drop.options[1].text = "INKITCHEN";
            drop.options[2].text = "COMPLETED";
            drop.options[3].text = "PAID";
            drop.options[4].text = "STARTED";
        }


        drop.onValueChanged.AddListener((_) =>
        {
            cambiarEstadoPedido(p, drop.options[drop.value].text);
        });
        GameObject modificar = botonP.transform.Find("Modificar").gameObject;
        Button mod = modificar.AddComponent<Button>();
        mod.onClick.AddListener(() => modificarPedido(p));
        GameObject eliminar = botonP.transform.Find("Eliminar").gameObject;
        Button del = eliminar.AddComponent<Button>();
        del.onClick.AddListener(() => eliminarPedido(p.id));


        GameObject modIMG = modificar.transform.Find("Image").gameObject;
        GameObject modTexto = modIMG.transform.Find("Text (TMP)").gameObject;
        TextMeshProUGUI textoMod = modTexto.GetComponent<TextMeshProUGUI>();
        GameObject delIMG = eliminar.transform.Find("Image").gameObject;
        GameObject delTexto = delIMG.transform.Find("Text (TMP)").gameObject;
        TextMeshProUGUI textoDel = delTexto.GetComponent<TextMeshProUGUI>();
        if (Usuario.Idioma.CompareTo("Español") != 0)
        {
            Debug.Log("Debería traducirse");
            textoMod.text = "Modify";
            textoDel.text = "Delete";
        }
        else
        {
            textoMod.text = "Modificar";
            textoDel.text = "Eliminar";
        }
        /*GameObject modificar = botonP.transform.Find("Modificar").gameObject;
        GameObject modTexto = modificar.transform.Find("Text (TMP)").gameObject;
        TextMeshProUGUI textoMod = modTexto.GetComponent<TextMeshProUGUI>();
        Button mod = modificar.AddComponent<Button>();
        mod.onClick.AddListener(() => modificarPedido(p));
        GameObject eliminar = botonP.transform.Find("Eliminar").gameObject;
        GameObject eliTexto = eliminar.transform.Find("Text (TMP)").gameObject;
        TextMeshProUGUI textoDel = eliTexto.GetComponent<TextMeshProUGUI>();


        if (Usuario.Idioma.CompareTo("Español") !=0)
        {
            Debug.Log("Debería traducirse");
            textoMod.text = "Modify";
            textoDel.text = "Delete";
        }
        else
        {
            textoMod.text = "Modificar";
            textoDel.text = "Eliminar";
        }
        Button del = eliminar.AddComponent<Button>();
        del.onClick.AddListener(() => eliminarPedido(p.id));
        
        */
        //modificar.GetComponent<Button>().onClick = () => { Debug.Log("Botón accedido correctamente"); };
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

    public void auxFiltroEstado()
    {
        crearBotonesPedidos(selMesaE.text);
    }

    /*public void pruebaCrear()
    {

        GameObject botonGO = Instantiate(baseP, fondoPedidos, true);
        botonGO.transform.SetParent(fondoPedidos);
        botonGO.AddComponent<CanvasRenderer>();
        // Crear un GameObject para el botón y asignarle un nombre único.
        botonGO.name = "Pedido-" + 1;
        // Referencias de componentes


        //PonerListenerADropdown(dropdown);
        /*
        // Crear un GameObject para el botón y asignarle un nombre único.
        GameObject botonGO = new GameObject("Button-" + trabajador.Id);

        // Establecer el padre para que se muestre en el UI.
        botonGO.transform.SetParent(rtScrollViewContent, false);

        // Agregar el componente RectTransform (se agrega automáticamente al crear UI, pero lo añadimos explícitamente).
        RectTransform rt = botonGO.AddComponent<RectTransform>();

        // Agregar CanvasRenderer para poder renderizar el UI.
        botonGO.AddComponent<CanvasRenderer>();

        // Agregar el componente Image para mostrar el sprite.
        Image imagen = botonGO.AddComponent<Image>();

        // Agrego un componente Button para que sea interactivo
        botonGO.AddComponent<Button>();

        // Creo un nuevo GameObject hijo, el texto del botón
        CrearTextoDelButton(rt, trabajador);
        


    }*/
    public async Task cambiarEstadoPedido(Pedido p,string s)
    {
        string aux=s;
        if (s.Equals("REGISTERED"))
        {
            aux = "APUNTADO";
        }
        else if (s.Equals("INKITCHEN"))
        {
            aux = "ENCOCINA";
        }
        else if (s.Equals("COMPLETED"))
        {
            aux = "COMPLETADO";
        }
        else if (s.Equals("PAID"))
        {
            aux = "PAGADO";
        }
        else if (s.Equals("STARTED"))
        {
            aux = "INICIADO";
        }
        p.estado = aux;
        string cad = await instanceMétodosApiController.PutDataAsync("pedido/cambiarEstado/",p);
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
        instanceGestionarPedidosController.entrarPedidoHecho(p.id);
        canvasPedidos.SetActive(true);
    }
    public async Task eliminarPedido(int idP)
    {
        //ELIMINAR PEDIDO EN SERVER Y RECARGAR LISTA
        //DEBERÍA HABER UN POP UP DE "SEGURO QUE LO QUIERES HACER?"
        string cad = await instanceMétodosApiController.DeleteDataAsync("pedido/borrar/"+idP);
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
        crearBotonesPedidos(0);
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
