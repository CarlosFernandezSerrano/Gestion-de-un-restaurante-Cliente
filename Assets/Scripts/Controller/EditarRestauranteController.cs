using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class EditarRestauranteController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown horaApertura;
    [SerializeField] private TMP_Dropdown minutoApertura;
    [SerializeField] private TMP_Dropdown horaCierre;
    [SerializeField] private TMP_Dropdown minutoCierre;
    [SerializeField] private TMP_InputField inputFieldNombreRestaurante;
    [SerializeField] private GameObject imgHayCambiosSinGuardar;
    [SerializeField] private UnityEngine.UI.Button buttonGuardar;
    [SerializeField] private UnityEngine.UI.Button buttonVolver;
    [SerializeField] private UnityEngine.UI.Button buttonA�adirMesa;



    private string NombreRestaurante;
    private string HoraApertura;
    private string HoraCierre;
    private List<Mesa> Mesas;

    // Contenedor padre donde se agregar�n los botones
    public Transform buttonParent;

    // Sprite que cargo desde Resources.
    private Sprite mesaSprite;

    ButtonMesaController instanceButtonMesaController;
    M�todosAPIController instanceM�todosApiController;


    void Awake()
    {
        SceneManager.LoadSceneAsync("General Controller", LoadSceneMode.Additive);
    }

    // Start is called before the first frame update
    void Start()
    {
        instanceButtonMesaController = ButtonMesaController.InstanceButtonMesaController;
        instanceM�todosApiController = M�todosAPIController.InstanceM�todosAPIController;
        
        TrabajadorController.ComprobandoDatosTrabajador = false;

        InicializarValoresDropdowns();

        ObtenerDatosRestauranteAsync();

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void InicializarValoresDropdowns()
    {
        List<string> opcionesHoras = new List<string> { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24" };
        List<string> opcionesMinutos = new List<string> { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09",
                                                          "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
                                                          "20", "21", "22", "23", "24", "25", "26", "27", "28", "29",
                                                          "30", "31", "32", "33", "34", "35", "36", "37", "38", "39",
                                                          "40", "41", "42", "43", "44", "45", "46", "47", "48", "49",
                                                          "50", "51", "52", "53", "54", "55", "56", "57", "58", "59" };
        AgregarOpcionesADropdown(horaApertura, opcionesHoras);
        AgregarOpcionesADropdown(minutoApertura, opcionesMinutos);
        AgregarOpcionesADropdown(horaCierre, opcionesHoras);
        AgregarOpcionesADropdown(minutoCierre, opcionesMinutos);
    }

    private async void ObtenerDatosRestauranteAsync()
    {
        string cad = await instanceM�todosApiController.GetDataAsync("restaurante/getRestaurantePorId/" + PlayerPrefs.GetInt("Restaurante_ID Usuario"));

        // Deserializo la respuesta
        Restaurante restaurante = JsonConvert.DeserializeObject<Restaurante>(cad);

        NombreRestaurante =  restaurante.Nombre;
        HoraApertura = restaurante.HoraApertura;
        HoraCierre = restaurante.HoraCierre;
        Mesas = restaurante.Mesas;

        inputFieldNombreRestaurante.text = NombreRestaurante;

        Debug.Log("Hora Apertura: " + restaurante.HoraApertura + "; Hora Cierre: " + restaurante.HoraCierre);

        AsignarHorasEnDropdowns();

        mesaSprite = Resources.Load<Sprite>("Editar Restaurante/mantelMesa");
        CrearBotonesMesas();
    }

    private void CrearBotonesMesas()
    {
        // El restaurante tiene mesas
        if (Mesas.Count > 0)
        {
            Debug.Log("Hay mesas");
            foreach (var mesa in Mesas)
            {
                CrearBotonMesa(mesa);
            }
        }
        else
        {
            Debug.Log("No hay mesas");
        }
    }

    private void CrearBotonMesa(Mesa mesa)
    {
        // Crear un GameObject para el bot�n y asignarle un nombre �nico.
        GameObject botonGO = new GameObject("Button" + mesa.Id);

        // Establecer el padre para que se muestre en el UI.
        botonGO.transform.SetParent(buttonParent, false);

        // Agregar el componente RectTransform (se agrega autom�ticamente al crear UI, pero lo a�adimos expl�citamente).
        RectTransform rt = botonGO.AddComponent<RectTransform>();
        // Opcional: definir un tama�o por defecto para el bot�n.
        rt.sizeDelta = new Vector2(100, 50);

        // Agregar CanvasRenderer para poder renderizar el UI.
        botonGO.AddComponent<CanvasRenderer>();

        // Agregar el componente Image para mostrar el sprite.
        UnityEngine.UI.Image imagen = botonGO.AddComponent<UnityEngine.UI.Image>();
        if (mesaSprite != null)
        {
            imagen.sprite = mesaSprite;
        }

        // Crear el componente Button para gestionar clics.
        UnityEngine.UI.Button boton = botonGO.AddComponent<UnityEngine.UI.Button>();
        // Capturamos el id de la mesa para usarlo en el listener.
        int idMesa = mesa.Id;
        boton.onClick.AddListener(() => OnMesaButtonClicked(idMesa));

        // Configurar la posici�n y escala del bot�n bas�ndose en las propiedades de la mesa.
        rt.anchoredPosition = new Vector2(mesa.PosX, mesa.PosY);
        rt.localScale = new Vector3(mesa.ScaleX, mesa.ScaleY, 1f);

        // (Opcional) Crear un objeto hijo para mostrar el texto en el bot�n.
        /*GameObject textoGO = new GameObject("Text");
        textoGO.transform.SetParent(botonGO.transform, false);

        // Agregar el componente Text para mostrar el nombre.
        Text texto = textoGO.AddComponent<Text>();
        texto.text = "Button " + mesa.Id;
        texto.alignment = TextAnchor.MiddleCenter;
        // Asignar una fuente por defecto (Arial viene integrada en Unity).
        texto.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        texto.color = Color.black;

        // Ajustar el RectTransform del texto para que ocupe todo el bot�n.
        RectTransform rtTexto = textoGO.GetComponent<RectTransform>();
        rtTexto.anchorMin = Vector2.zero;
        rtTexto.anchorMax = Vector2.one;
        rtTexto.offsetMin = Vector2.zero;
        rtTexto.offsetMax = Vector2.zero;*/
    }

    // M�todo que se ejecuta cuando se hace clic en un bot�n de mesa.
    void OnMesaButtonClicked(int id)
    {
        Debug.Log("Se ha clicado el bot�n de la mesa con ID: " + id);
    }

    private void AsignarHorasEnDropdowns()
    {
        // Hora apertura
        string[] horaAperturaArray = HoraApertura.Split(":");
        string hora_Apertura = horaAperturaArray[0].Trim();
        string minuto_Apertura = horaAperturaArray[1].Trim();

        // Hora cierre
        string[] horaCierreArray = HoraCierre.Split(":");
        string hora_Cierre = horaCierreArray[0].Trim();
        string minuto_Cierre = horaCierreArray[1].Trim();

        BuscoElIndiceYLoPongoSiLoEncuentro(horaApertura, hora_Apertura);
        BuscoElIndiceYLoPongoSiLoEncuentro(minutoApertura, minuto_Apertura);
        BuscoElIndiceYLoPongoSiLoEncuentro(horaCierre, hora_Cierre);
        BuscoElIndiceYLoPongoSiLoEncuentro(minutoCierre, minuto_Cierre);
    }

    private void BuscoElIndiceYLoPongoSiLoEncuentro(TMP_Dropdown horaOMinuto, string hora_O_Minuto)
    {
        // Busco el �ndice de ese valor en el Dropdown
        int index = horaOMinuto.options.FindIndex(option => option.text == hora_O_Minuto);

        // Si lo encuentra, establecerlo como el seleccionado
        if (index != -1)
        {
            horaOMinuto.value = index;
            horaOMinuto.RefreshShownValue(); // Refresca la UI para mostrar el valor seleccionado.
        }
        else
        {
            Debug.Log("Valor no encontrado");
        }
    }

    private void AgregarOpcionesADropdown(TMP_Dropdown dropdown, List<string> opciones)
    {
        dropdown.ClearOptions();  // Limpia opciones anteriores
        dropdown.AddOptions(opciones);
    }

    public void AgregarMesa()
    {
        instanceButtonMesaController.SpawnButton();
    }

    public void Guardar()
    {
        GestionarGuardarDatosRestauranteAsync();
    }

    private async void GestionarGuardarDatosRestauranteAsync()
    {
        Restaurante restaurante = RellenarRestauranteSiActualizara();
        Restaurante rest = ObtenerRestauranteConNuevasMesasCreadasParaRegistrar();
        

        bool b = await NombreRestauranteV�lidoDistintoYNoRepetidoEnLaBDD();

        // Nombre cambiado y comprobado que se puede actualizar porque no existe otro restaurante con ese nuevo nombre
        if (b)
        {
            Debug.Log("Hay cambios 1 ");
            await ActualizarRestauranteEnBDDAsync(restaurante);
            if (rest.Mesas.Count > 0)
            {
                await RegistrarMesasNuevasEnBDDAsync(rest);
            }
            else
            {
                Debug.Log("No hay mesas nuevas para registrar");
            }

            ObtenerDatosRestauranteAsync();
        }
        else if (HorasDistintasEnRestaurante() && NombreEsIgualQueEnLaBDD() || NombreEsIgualQueEnLaBDD() && MesasDistintasEnRestaurante())
        {
            Debug.Log("Hay cambios 2");
            await ActualizarRestauranteEnBDDAsync(restaurante);
            if (rest.Mesas.Count > 0)
            {
                await RegistrarMesasNuevasEnBDDAsync(rest);
            }
            else
            {
                Debug.Log("No hay mesas nuevas para registrar");
            }
            ObtenerDatosRestauranteAsync();
        }
        else
        {
            Debug.Log("No hay cambios");
        }
    }

    private async Task ActualizarRestauranteEnBDDAsync(Restaurante restaurante)
    {
        string cad = await instanceM�todosApiController.PutDataAsync("restaurante/actualizarRestaurante/", restaurante);
        // Deserializo la respuesta
        Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

        if (resultado.Result.Equals(1))
        {
            Debug.Log("Actualizaci�n exitosa");
        }
        else
        {
            Debug.Log("La actualizaci�n sali� mal");
        }
    }

    private async Task RegistrarMesasNuevasEnBDDAsync(Restaurante rest)
    {
        string cad = await instanceM�todosApiController.PostDataAsync("restaurante/registrarMesas/", rest);

        // Deserializo la respuesta
        Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

        if (resultado.Result.Equals(1))
        {
            Debug.Log("Registros exitosos");
        }
        else
        {
            Debug.Log("Error en los registros");
        }
    }

    private Restaurante RellenarRestauranteSiActualizara()
    {
        int id = PlayerPrefs.GetInt("Restaurante_ID Usuario");
        string nombreRestaurante = inputFieldNombreRestaurante.text.Trim();
        nombreRestaurante = Regex.Replace(nombreRestaurante, @"\s+", " "); // Reemplaza m�ltiples espacios por uno

        string hora_Apertura = horaApertura.options[horaApertura.value].text+" : "+ minutoApertura.options[minutoApertura.value].text;
        string hora_Cierre = horaCierre.options[horaCierre.value].text + " : " + minutoCierre.options[minutoCierre.value].text;

        List<Mesa> mesas = ObtenerListaDeMesasDelEditorActualizadas();

        return new Restaurante(id, nombreRestaurante, hora_Apertura, hora_Cierre, mesas, new List<Trabajador>());
    }

    private List<Mesa> ObtenerListaDeMesasDelEditorActualizadas()
    {
        List<Mesa> mesasNuevas = new List<Mesa>();

        int restauranteIdUsuario = PlayerPrefs.GetInt("Restaurante_ID Usuario");

        // Recorro cada mesa en la lista del restaurante.
        foreach (Mesa mesa in Mesas)
        {
            // Generamos el nombre que asignamos al bot�n (por ejemplo, "Button1" para la mesa con Id = 1).
            string nombreBoton = "Button" + mesa.Id;

            // Buscamos el bot�n en el contenedor de botones.
            Transform botonTransform = buttonParent.Find(nombreBoton);

            // Si no se encuentra el bot�n, se detecta un cambio.
            if (botonTransform == null)
            {
                Debug.Log("Cambio detectado: El bot�n " + nombreBoton + " no existe, ha sido eliminado.");
                continue;
            }

            // Obtengo el RectTransform para acceder a la posici�n y escala.
            RectTransform rt = botonTransform.GetComponent<RectTransform>();

            float posX = rt.anchoredPosition.x;
            float posY = rt.anchoredPosition.y;
            float scaleX = rt.localScale.x;
            float scaleY = rt.localScale.y;
            bool disponible  = mesa.Disponible;

            // Creo una nueva mesa con los datos actualizados del editor, conservando el mismo ID de mesa que hay en la BDD
            mesasNuevas.Add(new Mesa(mesa.Id, posX, posY, scaleX, scaleY, disponible, restauranteIdUsuario));
        }

        return mesasNuevas;
    }

    private Restaurante ObtenerRestauranteConNuevasMesasCreadasParaRegistrar()
    {
        List<Mesa> mesasNuevas = new List<Mesa>();

        int restauranteIdUsuario = PlayerPrefs.GetInt("Restaurante_ID Usuario");

        // Recorremos cada hijo del contenedor.
        foreach (Transform child in buttonParent)
        {
            string nombreBoton = child.gameObject.name;

            // Si el �ltimo car�cter no es un n�mero
            if (!char.IsDigit(nombreBoton[nombreBoton.Length - 1]))
            {
                RectTransform rt = child.GetComponent<RectTransform>();
                if (rt != null)
                {
                    float posX = rt.anchoredPosition.x;
                    float posY = rt.anchoredPosition.y;
                    float scaleX = rt.localScale.x;
                    float scaleY = rt.localScale.y;
                    bool disponible = true;

                    mesasNuevas.Add(new Mesa(posX, posY, scaleX, scaleY, disponible, restauranteIdUsuario));
                }
                else
                {
                    Debug.LogWarning("El bot�n " + nombreBoton + " no tiene RectTransform.");
                }
            }
        }

        return new Restaurante(restauranteIdUsuario, "", "", "", mesasNuevas, new List<Trabajador>());

    }

    private bool NombreEsIgualQueEnLaBDD()
    {
        string nombreRestaurante = inputFieldNombreRestaurante.text.Trim();
        nombreRestaurante = Regex.Replace(nombreRestaurante, @"\s+", " "); // Reemplaza m�ltiples espacios por uno

        if (nombreRestaurante.CompareTo(NombreRestaurante) == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool MesasDistintasEnRestaurante()
    {
        // Recorremos cada mesa en la lista del restaurante.
        foreach (Mesa mesa in Mesas)
        {
            // Generamos el nombre que asignamos al bot�n (por ejemplo, "Button1" para la mesa con Id = 1).
            string nombreBoton = "Button" + mesa.Id;

            // Buscamos el bot�n en el contenedor de botones.
            Transform botonTransform = buttonParent.Find(nombreBoton);

            // Si no se encuentra el bot�n, se detecta un cambio.
            if (botonTransform == null)
            {
                Debug.Log("Cambio detectado: El bot�n " + nombreBoton + " no existe, ha sido eliminado.");
                return true;
            }

            // Obtenemos el RectTransform para acceder a la posici�n y escala.
            RectTransform rt = botonTransform.GetComponent<RectTransform>();

            // Creamos los valores esperados basados en las propiedades de la mesa.
            Vector2 posEsperada = new Vector2(mesa.PosX, mesa.PosY);
            Vector3 escalaEsperada = new Vector3(mesa.ScaleX, mesa.ScaleY, 1f);

            // Comparamos la posici�n y la escala actuales del bot�n con las esperadas.
            if (rt.anchoredPosition != posEsperada || rt.localScale != escalaEsperada)
            {
                Debug.Log("Cambio detectado en " + nombreBoton + ": Posici�n o escala difieren.");
                return true;
            }
        }

        // Si se recorre toda la lista sin encontrar diferencias, se devuelve false.
        return false;
     }

    private async Task<bool> NombreRestauranteV�lidoDistintoYNoRepetidoEnLaBDD()
    {
        // Nombre del restaurante cambiado del original.
        if (NombreRestaurante.CompareTo(inputFieldNombreRestaurante.text) != 0)
        {
            string nombreRestaurante = inputFieldNombreRestaurante.text.Trim();
            nombreRestaurante = Regex.Replace(nombreRestaurante, @"\s+", " "); // Reemplaza m�ltiples espacios por uno

            // Si el nombre del restaurante tiene m�s de 2 caracteres, se crea.
            if (nombreRestaurante.Length > 2)
            {
                string cad = await instanceM�todosApiController.GetDataAsync("restaurante/existeConNombre/" + nombreRestaurante);
                // Deserializo la respuesta
                Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

                switch (resultado.Result)
                {
                    case 0: // El nuevo nombre de restaurante no existe en la BDD, se puede actualizar.
                        return true;
                    case 1:
                        if (PlayerPrefs.GetString("TipoIdioma").CompareTo("Espa�ol") == 0 || PlayerPrefs.GetString("TipoIdioma") == null)
                        {
                            Debug.Log("El restaurante ya existe.");
                            //textoErrorRegistro.text = "El restaurante ya existe.";
                            //StartCoroutine(MostrarManoError(2f));
                        }
                        else
                        {
                            Debug.Log("El restaurante ya existe.");
                            //textoErrorRegistro.text = "The restaurant already exists.";
                            //StartCoroutine(MostrarManoError(2f));
                        }
                        return false;
                }
            }
            else
            {
                Debug.Log("Nombre tiene menos de 3 caracteres");
                //textoErrorRegistro.text = "El nombre tiene menos de 3 caracteres.";
                //StartCoroutine(MostrarManoError(2f));
                return false;
            }
        }
        return false;
    }

    private bool HorasDistintasEnRestaurante()
    {
        // Hora apertura
        string[] horaAperturaArray = HoraApertura.Split(":");
        string hora_Apertura = horaAperturaArray[0].Trim();
        string minuto_Apertura = horaAperturaArray[1].Trim();

        // Hora cierre
        string[] horaCierreArray = HoraCierre.Split(":");
        string hora_Cierre = horaCierreArray[0].Trim();
        string minuto_Cierre = horaCierreArray[1].Trim();

        //Hora o minuto de apertura o cierre ha cambiado del original
        if (hora_Apertura.CompareTo(horaApertura.options[horaApertura.value].text) != 0 || minuto_Apertura.CompareTo(minutoApertura.options[minutoApertura.value].text) != 0 || hora_Cierre.CompareTo(horaCierre.options[horaCierre.value].text) != 0 || minuto_Cierre.CompareTo(minutoCierre.options[minutoCierre.value].text) != 0)
        {
            return true;
        }

        return false;
    }

    public void IrALaEscenaMain()
    {
        ComprobarSiHayCambios();        
    }

    private async void ComprobarSiHayCambios()
    {
        bool b = await NombreRestauranteV�lidoDistintoYNoRepetidoEnLaBDD();

        // Nombre cambiado y comprobado que se puede actualizar porque no existe otro restaurante con ese nuevo nombre
        if (b)
        {
            Debug.Log("Hay cambios. �Desea guardar antes de irse?");
            DesactivarBotonesDelCanvas();
            imgHayCambiosSinGuardar.SetActive(true);
        }
        else if (HorasDistintasEnRestaurante() && NombreEsIgualQueEnLaBDD())
        {
            Debug.Log("Hay cambios. �Desea guardar antes de irse?");
            DesactivarBotonesDelCanvas();
            imgHayCambiosSinGuardar.SetActive(true);
        }
        else
        {
            Debug.Log("No hay cambios");
            SceneManager.LoadScene("Main");
        }
    }

    private void DesactivarBotonesDelCanvas()
    {
        buttonVolver.interactable = false;
        buttonGuardar.interactable = false;
        buttonA�adirMesa.interactable = false;
    }

    public void GuardarYSalir()
    {
        StartCoroutine(GuardoYSalgo());
    }

    private IEnumerator GuardoYSalgo()
    {
        Guardar(); //Tengo que esperar a que se guarde antes de cambiar de escena
        imgHayCambiosSinGuardar.SetActive(false);

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("Main");
    }

    public void CancelarYSalir()
    {
        imgHayCambiosSinGuardar.SetActive(false);
        SceneManager.LoadScene("Main");
    }

    
}
