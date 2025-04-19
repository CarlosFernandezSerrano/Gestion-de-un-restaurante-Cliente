using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class GestionarTrabajadoresController : MonoBehaviour
{
    [SerializeField] private RectTransform rtScrollViewContent;
    [SerializeField] private TMP_InputField inputFieldBuscarTrabajador;
    [SerializeField] private GameObject scrollViewNombresTrabajadores;
    [SerializeField] private RectTransform rtScrollViewContentNombresTrabajadores;
    [SerializeField] private Button buttonA�adir;
    [SerializeField] private GameObject prefabBot�nConInputFieldYDropdown;
    [SerializeField] private Button buttonGuardar;
    [SerializeField] private Button buttonEliminar;
    [SerializeField] private TMP_Text textoError;

    private List<Trabajador> TrabajadoresEnRestaurante = new List<Trabajador>();
    private List<Trabajador> TrabajadoresSinRestaurante = new List<Trabajador>();

    private bool BuscandoTrabajadoresSinRest = false;
    private string TextoInputFieldAntes = "";
    private Button bot�nPulsadoParaEliminar;
    

    M�todosAPIController instanceM�todosApiController;

    private void Awake()
    {
        SceneManager.LoadSceneAsync("General Controller", LoadSceneMode.Additive);
    }

    // Start is called before the first frame update
    void Start()
    {
        instanceM�todosApiController = M�todosAPIController.InstanceM�todosAPIController;

        TrabajadorController.ComprobandoDatosTrabajador = false;

        InvokeRepeating(nameof(ObtenerTrabajadoresSinRestauranteAsync), 0f, 1f); // Llama a ObtenerTrabajadoresSinRestaurante() cada 1 segundo

        ObtenerTrabajadoresDeUnRestauranteAsync();
    }

    // Update is called once per frame
    void Update()
    {
        GestionarBuscarTrabajador();

        GestionarCambiosEnTrabajadores();

        GestionarHayUnInputFieldVac�oEnNombresTrabajadores();
    }

    private void GestionarHayUnInputFieldVac�oEnNombresTrabajadores()
    {
        foreach (Transform hijo in rtScrollViewContent)
        {
            string nombre = hijo.GetComponentInChildren<TMP_InputField>().text.Trim();
            if (nombre.Length.Equals(0))
            {
                buttonGuardar.interactable = false;
                textoError.text = "Nombre vac�o";
                return;
            }
        }
        textoError.text = "";
    }

    private void GestionarCambiosEnTrabajadores()
    {
        List<Trabajador> trabajadoresEnScrollViewAhora = ObtenerDatosTrabajadoresEnScrollView();

        if (Alg�nNombreORolCambiado(trabajadoresEnScrollViewAhora))
        {
            buttonGuardar.interactable = true;
        }
        else
        {
            buttonGuardar.interactable = false;
        }
    }

    private bool Alg�nNombreORolCambiado(List<Trabajador> trabajadoresEnScrollViewAhora)
    {        
        foreach (Trabajador trabajador in TrabajadoresEnRestaurante)
        {
            int cont = 0;
            foreach (Trabajador trabajadorScrollViewAhora in trabajadoresEnScrollViewAhora)
            {
                // Recorro todos los nombres que hay ahora en el scrollview y compruebo si cada nombre de los TrabajadoresEnRestaurante est�n id�nticos, sino, hay cambios.
                if (trabajador.Nombre.CompareTo(trabajadorScrollViewAhora.Nombre) != 0)
                {
                    cont++;
                }
            }
            // Un nombre ha sido cambiado
            if (cont.Equals(trabajadoresEnScrollViewAhora.Count))
            {
                return true;
            }
        }
        return false;
    }

    private List<Trabajador> ObtenerDatosTrabajadoresEnScrollView()
    {
        List<Trabajador> trabajadores = new List<Trabajador>();
        foreach (Transform hijo in rtScrollViewContent)
        {
            int id = ObtenerIdTrabajadorDeBot�n(hijo);
            string nombre = hijo.GetComponentInChildren<TMP_InputField>().text.Trim();
            TMP_Dropdown dropdown = hijo.GetComponentInChildren<TMP_Dropdown>();
            string rol = dropdown.options[dropdown.value].text;

            switch (rol)
            {
                case "Empleado":
                    trabajadores.Add(new Trabajador(id, nombre, "", 1, 0));
                    break;
                case "Gerente":
                    trabajadores.Add(new Trabajador(id, nombre, "", 2, 0));
                    break;
            }
        }
        return trabajadores;
    }

    private int ObtenerIdTrabajadorDeBot�n(Transform hijo)
    {
        string[] array = hijo.name.Split("-");
        return int.Parse(array[1]);
    }

    private async void ObtenerTrabajadoresSinRestauranteAsync()
    {
        if (!BuscandoTrabajadoresSinRest)
        {
            BuscandoTrabajadoresSinRest = true;
            string cad = await instanceM�todosApiController.GetDataAsync("trabajador/getTrabajadoresSinRestaurante");

            // Deserializo la respuesta
            TrabajadoresSinRestaurante = JsonConvert.DeserializeObject<List<Trabajador>>(cad);
            
            BuscandoTrabajadoresSinRest = false;
        }
    }

    private void GestionarBuscarTrabajador()
    {
        string textoInputField = inputFieldBuscarTrabajador.text.Trim();

        if (textoInputField.Length > 0)
        {
            if (textoInputField.ToLower().CompareTo(TextoInputFieldAntes.ToLower()) != 0)
            {
                List<string> nombres_Trabajadores_Sin_Restaurante = new List<string>();

                // Recorro la lista de todos los trabajadores sin un restaurante_ID
                foreach (Trabajador trabajador in TrabajadoresSinRestaurante)
                {
                    if (trabajador.Nombre.ToLower().Contains(textoInputField.ToLower()))
                    {
                        nombres_Trabajadores_Sin_Restaurante.Add(trabajador.Nombre);
                    }
                }
                TextoInputFieldAntes = textoInputField;
                // Se encuentran trabajadores sin restaurante con un nombre parecido al que se pone en el inputField
                if (nombres_Trabajadores_Sin_Restaurante.Count > 0)
                {
                    scrollViewNombresTrabajadores.SetActive(true);

                    // Tengo que eliminar todos los hijos (botones en este caso) de Content antes de poner nuevos (reservas actualizadas)
                    EliminarObjetosHijoDeScrollView(rtScrollViewContentNombresTrabajadores);

                    // Muestro los trabajadores sin restaurante que coinciden con el contenido del inputField en un scrollview
                    foreach (string nombre in nombres_Trabajadores_Sin_Restaurante)
                    {
                        CrearBot�nNombreTrabajador(nombre);
                    }
                    
                }
                else
                {
                    // Tengo que eliminar todos los hijos (botones en este caso) de Content antes de poner nuevos (reservas actualizadas)
                    EliminarObjetosHijoDeScrollView(rtScrollViewContentNombresTrabajadores);

                    scrollViewNombresTrabajadores.SetActive(false);
                }
            }
            
        }
        else
        {
            // Tengo que eliminar todos los hijos (botones en este caso) de Content antes de poner nuevos (reservas actualizadas)
            EliminarObjetosHijoDeScrollView(rtScrollViewContentNombresTrabajadores);
            TextoInputFieldAntes = "";
            scrollViewNombresTrabajadores.SetActive(false);
        }


        if (ElNombreExiste())
        {
            buttonA�adir.interactable = true;

            // Tengo que eliminar todos los hijos (botones en este caso) de Content antes de poner nuevos (reservas actualizadas)
            EliminarObjetosHijoDeScrollView(rtScrollViewContentNombresTrabajadores);
            scrollViewNombresTrabajadores.SetActive(false);
        }
        else
        {
            buttonA�adir.interactable = false;
        }
    }

    private bool ElNombreExiste()
    {
        foreach (Trabajador trabajador in TrabajadoresSinRestaurante)
        {
            if (trabajador.Nombre.CompareTo(inputFieldBuscarTrabajador.text.Trim()) == 0)
            {
                return true;
            }
        }
        return false;
    }

    private void EliminarObjetosHijoDeScrollView(RectTransform rectTransformContent)
    {
        foreach (Transform hijo in rectTransformContent)
        {
            Destroy(hijo.gameObject);
        }
    }

    private async void ObtenerTrabajadoresDeUnRestauranteAsync()
    {
        Debug.Log("Obtener datos trabajadores");
        string cad = await instanceM�todosApiController.GetDataAsync("trabajador/getTrabajadoresDeRestaurante/" + Usuario.Restaurante_ID);

        // Deserializo la respuesta
        TrabajadoresEnRestaurante = JsonConvert.DeserializeObject<List<Trabajador>>(cad);

        CrearBotonesTrabajadores();
    }

    private void CrearBotonesTrabajadores()
    {
        if (TrabajadoresEnRestaurante.Count > 0)
        {
            foreach (Trabajador trabajador in TrabajadoresEnRestaurante)
            {
                CrearBot�nTrabajador(trabajador);
            }
        }
    }

    private void CrearBot�nTrabajador(Trabajador trabajador)
    {
        Debug.Log("*OK");
        GameObject botonGO = Instantiate(prefabBot�nConInputFieldYDropdown, rtScrollViewContent, false);

        // Crear un GameObject para el bot�n y asignarle un nombre �nico.
        botonGO.name = "Button-" + trabajador.Id;

        // Referencias de componentes
        Button button = botonGO.GetComponent<Button>();
        TMP_InputField inputField = botonGO.GetComponentInChildren<TMP_InputField>();
        TMP_Dropdown dropdown = botonGO.GetComponentInChildren<TMP_Dropdown>();

        inputField.text = trabajador.Nombre;

        List<string> opciones = new List<string> { "Empleado", "Gerente"};
        AgregarOpcionesADropdown(dropdown, opciones);

        switch (trabajador.Rol_ID)
        {
            case 1:
                BuscoElIndiceYLoPongoSiLoEncuentro(dropdown, "Empleado");
                break;
            case 2:
                BuscoElIndiceYLoPongoSiLoEncuentro(dropdown, "Gerente");
                break;
        }

        //Pongo listener al bot�n
        button.onClick.AddListener(() => GestionarActivarBot�nEliminarTrabajador(button));
        
        inputField.onSelect.AddListener((_) =>
        {
            buttonEliminar.interactable = false;
        });

        dropdown.onValueChanged.AddListener((_) =>
        {
            buttonEliminar.interactable = false;
        });

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
        */


    }

    private void GestionarActivarBot�nEliminarTrabajador(Button button)
    {
        bot�nPulsadoParaEliminar = button;

        buttonEliminar.interactable = true;
    }

    private void AgregarOpcionesADropdown(TMP_Dropdown dropdown, List<string> opciones)
    {
        dropdown.ClearOptions();  // Limpia opciones anteriores
        dropdown.AddOptions(opciones);
    }

    private void BuscoElIndiceYLoPongoSiLoEncuentro(TMP_Dropdown dropdown, string valor)
    {
        // Busco el �ndice de ese valor en el Dropdown
        int index = dropdown.options.FindIndex(option => option.text == valor);

        // Si lo encuentra, establecerlo como el seleccionado
        if (index != -1)
        {
            dropdown.value = index;
            dropdown.RefreshShownValue(); // Refresca la UI para mostrar el valor seleccionado.
        }
        else
        {
            Debug.Log("Valor no encontrado");
        }
    }

    private void CrearTextoDelButton(RectTransform rt, Trabajador trabajador)
    {
        // Creo un GameObject para el bot�n y le asigno un nombre �nico.
        GameObject textGO = new GameObject("TMP_Text");

        // Establezco el padre para que se muestre en el UI.
        textGO.transform.SetParent(rt, false);

        // Agrego el componente RectTransform (se agrega autom�ticamente al crear UI, pero lo a�ado expl�citamente).
        RectTransform rtText = textGO.AddComponent<RectTransform>();
        // Anclas estiradas (stretch) en ambas direcciones
        rtText.anchorMin = new Vector2(0, 0);
        rtText.anchorMax = new Vector2(1, 1);

        // M�rgenes todos a 0 (equivale a Left, Right, Top, Bottom en el inspector)
        rtText.offsetMin = Vector2.zero;
        rtText.offsetMax = Vector2.zero;

        // Centrado por si acaso (aunque no influye mucho cuando est� estirado)
        rtText.anchoredPosition = Vector2.zero;

        // Agrego CanvasRenderer para poder renderizar el UI.
        textGO.AddComponent<CanvasRenderer>();

        // Agrego el componente TMP_Text para mostrar el sprite.
        TMP_Text textoBot�n = textGO.AddComponent<TextMeshProUGUI>();
        textoBot�n.fontStyle = FontStyles.Bold;
        textoBot�n.fontSize = 46;
        textoBot�n.alignment = TextAlignmentOptions.Left;

        switch (trabajador.Rol_ID)
        {
            case 1:
                textoBot�n.text = "                     " + trabajador.Nombre + "                                " + "Empleado";
                break;
            case 2:
                textoBot�n.text = "                     " + trabajador.Nombre + "                                " + "Gerente";
                break;
        }
    }

    private void CrearBot�nNombreTrabajador(string nombreTrabajador)
    {
        // Crear un GameObject para el bot�n y asignarle un nombre �nico.
        GameObject botonGO = new GameObject("Button-" + nombreTrabajador);

        // Establecer el padre para que se muestre en el UI.
        botonGO.transform.SetParent(rtScrollViewContentNombresTrabajadores, false);

        // Agregar el componente RectTransform (se agrega autom�ticamente al crear UI, pero lo a�adimos expl�citamente).
        RectTransform rt = botonGO.AddComponent<RectTransform>();

        // Agregar CanvasRenderer para poder renderizar el UI.
        botonGO.AddComponent<CanvasRenderer>();

        // Agregar el componente Image para mostrar el sprite.
        Image imagen = botonGO.AddComponent<Image>();

        // Agrego un componente Button para que sea interactivo
        Button button = botonGO.AddComponent<Button>();

        //Pongo listener al bot�n
        button.onClick.AddListener(() => PonerEnInputFieldElTextoDelBot�nSeleccionado(button));

        // Creo un nuevo GameObject hijo, el texto del bot�n
        CrearTextoDelButtonNombreTrabajador(rt, nombreTrabajador);
    }

    private void PonerEnInputFieldElTextoDelBot�nSeleccionado(Button button)
    {
        Debug.Log("+Pasa por listener bot�n");
        inputFieldBuscarTrabajador.text = button.transform.Find("TMP_Text").GetComponent<TMP_Text>().text;
    }

    private void CrearTextoDelButtonNombreTrabajador(RectTransform rt, string nombreTrabajador)
    {
        // Creo un GameObject para el bot�n y le asigno un nombre �nico.
        GameObject textGO = new GameObject("TMP_Text");

        // Establezco el padre para que se muestre en el UI.
        textGO.transform.SetParent(rt, false);

        // Agrego el componente RectTransform (se agrega autom�ticamente al crear UI, pero lo a�ado expl�citamente).
        RectTransform rtText = textGO.AddComponent<RectTransform>();
        // Anclas estiradas (stretch) en ambas direcciones
        rtText.anchorMin = new Vector2(0, 0);
        rtText.anchorMax = new Vector2(1, 1);

        // M�rgenes todos a 0 (equivale a Left, Right, Top, Bottom en el inspector)
        rtText.offsetMin = Vector2.zero;
        rtText.offsetMax = Vector2.zero;

        // Centrado por si acaso (aunque no influye mucho cuando est� estirado)
        rtText.anchoredPosition = Vector2.zero;

        // Agrego CanvasRenderer para poder renderizar el UI.
        textGO.AddComponent<CanvasRenderer>();

        // Agrego el componente TMP_Text para mostrar el sprite.
        TMP_Text textoBot�n = textGO.AddComponent<TextMeshProUGUI>();
        textoBot�n.fontStyle = FontStyles.Bold;
        textoBot�n.fontSize = 46;
        textoBot�n.alignment = TextAlignmentOptions.Left;

        textoBot�n.text = " "+nombreTrabajador;
    }

    public async void A�adirTrabajadorARestaurante()
    {
        buttonA�adir.interactable = false;

        string nombreTrabajador = inputFieldBuscarTrabajador.text.Trim();

        int id_Trabajador = ObtenerIdTrabajadorPorNombre(nombreTrabajador);

        Trabajador trabajador = new Trabajador(id_Trabajador, "", "", 0, Usuario.Restaurante_ID);

        string cad = await instanceM�todosApiController.PutDataAsync("trabajador/actualizarRestauranteIDTrabajador", trabajador);

        // Deserializo la respuesta
        Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

        if (resultado.Result.Equals(1))
        {
            inputFieldBuscarTrabajador.text = "";
            EliminarObjetosHijoDeScrollView(rtScrollViewContent);
            ObtenerTrabajadoresDeUnRestauranteAsync();
        }
        else
        {
            inputFieldBuscarTrabajador.text = "Error";
        }

        
    }

    private int ObtenerIdTrabajadorPorNombre(string nombreTrabajador)
    {
        foreach (Trabajador trabajador in TrabajadoresSinRestaurante)
        {
            if (trabajador.Nombre.CompareTo(nombreTrabajador) == 0)
            {
                return trabajador.Id;
            }
        }
        return 0;
    }

    public void IrALaEscenaMain()
    {
        SceneManager.LoadScene("Main");
    }

}
