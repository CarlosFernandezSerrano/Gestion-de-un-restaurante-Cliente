using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] private UnityEngine.UI.Button buttonAñadirMesa;
    [SerializeField] private RectTransform imgCartel;
    [SerializeField] private TMP_Text textError;
    [SerializeField] private UnityEngine.UI.Button botónPapelera;
    [SerializeField] private RectTransform rtManosAdvertencia;
    [SerializeField] private RectTransform rtImgObjetoSello;
    [SerializeField] private RawImage imgSelloTintaEN;
    [SerializeField] private RawImage imgSelloTintaES;

    private string NombreRestaurante;
    private string HoraApertura;
    private string HoraCierre;
    private List<Mesa> Mesas;

    private int idMesaAEliminar;

    // Contenedor padre donde se agregarán los botones
    public RectTransform buttonParent;

    // Sprite que cargo desde Resources.
    private Sprite mesaSprite;

    ButtonMesaController instanceButtonMesaController;
    MétodosAPIController instanceMétodosApiController;

    public static EditarRestauranteController InstanceEditarRestauranteController { get; private set; }


    void Awake()
    {
        if (InstanceEditarRestauranteController == null)
        {
            InstanceEditarRestauranteController = this;
        }

        SceneManager.LoadSceneAsync("General Controller", LoadSceneMode.Additive);
    }

    // Start is called before the first frame update
    void Start()
    {
        instanceButtonMesaController = ButtonMesaController.InstanceButtonMesaController;
        instanceMétodosApiController = MétodosAPIController.InstanceMétodosAPIController;
        
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
        EliminarObjetosHijoDeFondoDeEdición();

        string cad = await instanceMétodosApiController.GetDataAsync("restaurante/getRestaurantePorId/" + PlayerPrefs.GetInt("Restaurante_ID Usuario"));

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

    // Elimino todos los botones mesa antes de actualizar el fondo de edición, para que no sea un caos y se pongan unos encima de otros, además de su gestión luego.
    private void EliminarObjetosHijoDeFondoDeEdición()
    {
        foreach (Transform hijo in buttonParent)
        {
            Destroy(hijo.gameObject);
        }
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
        // Crear un GameObject para el botón y asignarle un nombre único.
        GameObject botonGO = new GameObject("Button-" + mesa.Id);

        // Establecer el padre para que se muestre en el UI.
        botonGO.transform.SetParent(buttonParent, false);

        // Agregar el componente RectTransform (se agrega automáticamente al crear UI, pero lo añadimos explícitamente).
        RectTransform rt = botonGO.AddComponent<RectTransform>();
        // Opcional: definir un tamaño por defecto para el botón.
        rt.sizeDelta = new Vector2(mesa.Width, mesa.Height);

        // Agregar CanvasRenderer para poder renderizar el UI.
        botonGO.AddComponent<CanvasRenderer>();

        // Agregar el componente Image para mostrar el sprite.
        UnityEngine.UI.Image imagen = botonGO.AddComponent<UnityEngine.UI.Image>();
        if (mesaSprite != null)
        {
            imagen.sprite = mesaSprite;
        }

        // Configurar la posición y escala del botón basándose en las propiedades de la mesa.
        rt.anchoredPosition = new Vector2(mesa.PosX, mesa.PosY);
        rt.localScale = new Vector3(mesa.ScaleX, mesa.ScaleY, 1f);

        // Agrego un componente Button para que sea interactivo
        botonGO.AddComponent<UnityEngine.UI.Button>();

        // Agrego este script al nuevo botón para dotarlo de funcionalidad de arrastre y escala
        ButtonMesaController bm = botonGO.AddComponent<ButtonMesaController>();
        bm.containerRect = this.buttonParent;  // Asigno el mismo contenedor
        bm.rectTransform = rt; // Asigno el RectTransform del nuevo botón
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
        // Busco el índice de ese valor en el Dropdown
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
        

        bool b = await NombreRestauranteVálidoDistintoYNoRepetidoEnLaBDD();

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
            inputFieldNombreRestaurante.text = NombreRestaurante;
        }
    }

    private async Task ActualizarRestauranteEnBDDAsync(Restaurante restaurante)
    {
        string cad = await instanceMétodosApiController.PutDataAsync("restaurante/actualizarRestaurante/", restaurante);
        // Deserializo la respuesta
        Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

        if (resultado.Result.Equals(1))
        {
            Debug.Log("Actualización exitosa");
            StartCoroutine(MostrarObjetoSello());
        }
        else
        {
            Debug.Log("La actualización salió mal");
        }
    }

    private async Task RegistrarMesasNuevasEnBDDAsync(Restaurante rest)
    {
        string cad = await instanceMétodosApiController.PostDataAsync("restaurante/registrarMesas/", rest);

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
        nombreRestaurante = Regex.Replace(nombreRestaurante, @"\s+", " "); // Reemplaza múltiples espacios por uno

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
            // Generamos el nombre que asignamos al botón (por ejemplo, "Button-1" para la mesa con Id = 1).
            string nombreBoton = "Button-" + mesa.Id;

            // Buscamos el botón en el contenedor de botones.
            Transform botonTransform = buttonParent.Find(nombreBoton);

            // Si no se encuentra el botón, se detecta un cambio.
            if (botonTransform == null)
            {
                Debug.Log("Cambio detectado: El botón " + nombreBoton + " no existe, ha sido eliminado.");
                continue;
            }

            // Obtengo el RectTransform para acceder a la posición y escala.
            RectTransform rt = botonTransform.GetComponent<RectTransform>();

            float posX = rt.anchoredPosition.x;
            float posY = rt.anchoredPosition.y;
            float width = rt.rect.width;
            float height = rt.rect.height;
            float scaleX = rt.localScale.x;
            float scaleY = rt.localScale.y;
            bool disponible  = mesa.Disponible;

            // Creo una nueva mesa con los datos actualizados del editor, conservando el mismo ID de mesa que hay en la BDD
            mesasNuevas.Add(new Mesa(mesa.Id, posX, posY, width, height, scaleX, scaleY, disponible, restauranteIdUsuario));
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

            // Si el último carácter no es un número
            if (!char.IsDigit(nombreBoton[nombreBoton.Length - 1]))
            {
                RectTransform rt = child.GetComponent<RectTransform>();
                if (rt != null)
                {
                    float posX = rt.anchoredPosition.x;
                    float posY = rt.anchoredPosition.y;
                    float width = rt.rect.width;
                    float height = rt.rect.height;
                    float scaleX = rt.localScale.x;
                    float scaleY = rt.localScale.y;
                    bool disponible = true;

                    //float width = botónCerrarSesión.gameObject.GetComponent<RectTransform>().rect.width;
                    //float height = botónCerrarSesión.gameObject.GetComponent<RectTransform>().rect.height;
                    Debug.Log("Width: " + width + " Y Height: " + height);

                    mesasNuevas.Add(new Mesa(posX, posY, width, height, scaleX, scaleY, disponible, restauranteIdUsuario));
                }
                else
                {
                    Debug.LogWarning("El botón " + nombreBoton + " no tiene RectTransform.");
                }
            }
        }

        return new Restaurante(restauranteIdUsuario, "", "", "", mesasNuevas, new List<Trabajador>());

    }

    private bool NombreEsIgualQueEnLaBDD()
    {
        string nombreRestaurante = inputFieldNombreRestaurante.text.Trim();
        nombreRestaurante = Regex.Replace(nombreRestaurante, @"\s+", " "); // Reemplaza múltiples espacios por uno

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
        if (Mesas.Count > 0)
        {
            // Recorremos cada mesa en la lista del restaurante.
            foreach (Mesa mesa in Mesas)
            {
                // Generamos el nombre que asignamos al botón (por ejemplo, "Button1" para la mesa con Id = 1).
                string nombreBoton = "Button-" + mesa.Id;

                // Buscamos el botón en el contenedor de botones.
                Transform botonTransform = buttonParent.Find(nombreBoton);

                // Si no se encuentra el botón, se detecta un cambio, pero no lo guardo porque cuando elimino se guarda ese cambio en la BDD.
                if (botonTransform == null)
                {
                    Debug.Log("Cambio detectado: El botón " + nombreBoton + " no existe, ha sido eliminado.");
                    continue;
                }

                // Obtenemos el RectTransform para acceder a la posición y escala.
                RectTransform rt = botonTransform.GetComponent<RectTransform>();

                // Creamos los valores esperados basados en las propiedades de la mesa.
                Vector2 posEsperada = new Vector2(mesa.PosX, mesa.PosY);
                Vector3 escalaEsperada = new Vector3(mesa.ScaleX, mesa.ScaleY, 1f);

                // Comparamos la posición y la escala actuales del botón con las esperadas.
                if (rt.anchoredPosition != posEsperada || rt.localScale != escalaEsperada)
                {
                    Debug.Log("Cambio detectado en " + nombreBoton + ": Posición o escala difieren.");
                    return true;
                }
            }

            // Si se recorre toda la lista sin encontrar diferencias, se devuelve false.
            //return false;
        }

        
            // Buscamos el botón en el contenedor de botones.
            Transform botonTransform2 = buttonParent.Find("Button");

            // Si no se encuentra el botón, no hay botones nuevos creados
            if (botonTransform2 == null)
            {

                return false;
            }
            else
            {
                return true;
            }
            /*if (buttonParent.childCount > 0)
            {
                Debug.Log("El GameObject padre tiene al menos un hijo.");
                return true;
            }
            else
            {
                Debug.Log("El GameObject padre no tiene hijos.");
                return false;
            }*/
        
     }

    private async Task<bool> NombreRestauranteVálidoDistintoYNoRepetidoEnLaBDD()
    {
        // Nombre del restaurante cambiado del original.
        if (NombreRestaurante.CompareTo(inputFieldNombreRestaurante.text) != 0)
        {
            string nombreRestaurante = inputFieldNombreRestaurante.text.Trim();
            nombreRestaurante = Regex.Replace(nombreRestaurante, @"\s+", " "); // Reemplaza múltiples espacios por uno

            // Si el nombre del restaurante tiene más de 2 caracteres, se crea.
            if (nombreRestaurante.Length > 2)
            {
                string cad = await instanceMétodosApiController.GetDataAsync("restaurante/existeConNombre/" + nombreRestaurante);
                // Deserializo la respuesta
                Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

                switch (resultado.Result)
                {
                    case 0: // El nuevo nombre de restaurante no existe en la BDD, se puede actualizar.
                        return true;
                    case 1:
                        if (PlayerPrefs.GetString("TipoIdioma").CompareTo("Español") == 0 || PlayerPrefs.GetString("TipoIdioma") == null)
                        {
                            Debug.Log("El restaurante ya existe.");
                            string cad2 = "No se puede cambiar el nombre, ya existe";
                            StartCoroutine(MovimientoCartelDeMadera(2f, cad2, 8.4f, 12f));
                        }
                        else
                        {
                            Debug.Log("El restaurante ya existe.");
                            string cad3 = "The name cannot be changed, it already exists.";
                            StartCoroutine(MovimientoCartelDeMadera(2f, cad3, 6f, 11f));
                        }
                        return false;
                }
            }
            else
            {
                string cad4 = "Nombre tiene menos de 3 caracteres";
                StartCoroutine(MovimientoCartelDeMadera(2f, cad4, 0f, 12f));
                Debug.Log("Nombre tiene menos de 3 caracteres");
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
        bool b = await NombreRestauranteVálidoDistintoYNoRepetidoEnLaBDD();

        // Nombre cambiado y comprobado que se puede actualizar porque no existe otro restaurante con ese nuevo nombre
        if (b)
        {
            Debug.Log("Hay cambios. ¿Desea guardar antes de irse?");
            DesactivarBotonesDelCanvas();
            imgHayCambiosSinGuardar.SetActive(true);
        }
        else if (HorasDistintasEnRestaurante() && NombreEsIgualQueEnLaBDD() || NombreEsIgualQueEnLaBDD() && MesasDistintasEnRestaurante())
        {
            Debug.Log("Hay cambios. ¿Desea guardar antes de irse?");
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
        buttonAñadirMesa.interactable = false;
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

    public IEnumerator MovimientoCartelDeMadera(float tiempoDeEspera, string cad, float posYTexto, float tamañoLetra)
    {
        buttonGuardar.interactable = false;

        RectTransform rt = textError.gameObject.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, posYTexto); // Cambio la "y" del texto para que se vea bien en el cartel

        textError.fontSize = tamañoLetra; // Pongo el tamaño de letra que quiero
        textError.text = cad;

        // Subo el cartel
        for (int i = 0; i < 180; i++)
        {
            // Actualizo
            float j = imgCartel.localEulerAngles.z - 1;

            // Pinto 
            imgCartel.localEulerAngles = new Vector3(0, 0, j);

            // Espero
            yield return new WaitForSeconds(0.001f);
        }

        // Espero X segundos
        yield return new WaitForSeconds(tiempoDeEspera);

        // Bajo el cartel
        for (int i = 0; i < 180; i++)
        {
            // Actualizo
            float k = imgCartel.localEulerAngles.z - 1;

            // Pinto 
            imgCartel.localEulerAngles = new Vector3(0, 0, k);

            // Espero
            yield return new WaitForSeconds(0.001f);
        }

        buttonGuardar.interactable = true;
    }

    public void ActivarPapelera()
    {
        botónPapelera.interactable = true;
    }

    public void DesactivarPapelera()
    {
        botónPapelera.interactable = false;
    }

    public IEnumerator MostrarManosAdvertencia(float tiempoDeEspera, string cad1, string cad2)
    {
        float y = rtManosAdvertencia.anchoredPosition.y;
        Debug.Log("Y: " + y);
        while (y < -188)
        {
            // Actualizo 
            y = rtManosAdvertencia.anchoredPosition.y + 2;

            // Pinto
            rtManosAdvertencia.anchoredPosition = new Vector2(rtManosAdvertencia.anchoredPosition.x, y);

            // Espero
            yield return null;
        }
        
        /*for (int i = 0; i < 710; i++)
        {
        }*/
    }

    public async void GestionarEliminarMesaEnBDDAsync(int idMesa)
    {
        idMesaAEliminar = idMesa;
        Debug.Log("Número obtenido del botón que se quiere eliminar:" + idMesa + "*");
        //Compruebo si tiene reservas esa mesa
        string cad = await instanceMétodosApiController.GetDataAsync("reserva/existe/"+idMesa);

        // Deserializo la respuesta
        Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

        // La mesa tiene una o varias reservas en la BDD
        if (resultado.Result.Equals(1))
        {
            StartCoroutine(MostrarManosAdvertencia(1f, "", ""));
        }
        else // La mesa no tiene ninguna reserva, se puede eliminar de la BDD sin problemas
        {
            EliminarMesaEnBDDAsync(idMesa);
        }
    }

    private IEnumerator MostrarObjetoSello()
    {
        rtImgObjetoSello.gameObject.SetActive(true);

        // Muevo el sello hacia la izquierda
        while (rtImgObjetoSello.anchoredPosition.x > 0)
        {
            // Actualizo 
            float x = rtImgObjetoSello.anchoredPosition.x - 2;

            // Pinto
            rtImgObjetoSello.anchoredPosition = new Vector2(x, rtImgObjetoSello.anchoredPosition.y);

            // Espero
            yield return null;
        }

        // Reducir la escala del sello
        for (int i = 0; i < 20; i++)
        {
            // Actualizo
            float x = rtImgObjetoSello.localScale.x - 0.005f;
            float y = rtImgObjetoSello.localScale.y - 0.005f;

            // Pinto
            rtImgObjetoSello.localScale = new Vector3(x, y, rtImgObjetoSello.localScale.z);

            // Espero
            yield return new WaitForSeconds(0.003f);
        }

        if (PlayerPrefs.GetString("TipoIdioma", "Español").CompareTo("Español") == 0)
        {
            imgSelloTintaES.gameObject.SetActive(true);
            StartCoroutine(HacerDesaparecerLentamenteElTextoAAdivinar(1, imgSelloTintaES));
        }
        else
        {
            imgSelloTintaEN.gameObject.SetActive(true);
            StartCoroutine(HacerDesaparecerLentamenteElTextoAAdivinar(1, imgSelloTintaEN));
        }

        // Aumentar la escala del sello
        for (int i = 0; i < 20; i++)
        {
            // Actualizo
            float x = rtImgObjetoSello.localScale.x + 0.005f;
            float y = rtImgObjetoSello.localScale.y + 0.005f;

            // Pinto
            rtImgObjetoSello.localScale = new Vector3(x, y, rtImgObjetoSello.localScale.z);

            // Espero
            yield return new WaitForSeconds(0.003f);
        }

        // Muevo el sello hacia la derecha
        while (rtImgObjetoSello.anchoredPosition.x < 1295)
        {
            // Actualizo 
            float x = rtImgObjetoSello.anchoredPosition.x + 2;

            // Pinto
            rtImgObjetoSello.anchoredPosition = new Vector2(x, rtImgObjetoSello.anchoredPosition.y);

            // Espero
            yield return null;
        }

        rtImgObjetoSello.gameObject.SetActive(false);
    }

    private IEnumerator HacerDesaparecerLentamenteElTextoAAdivinar(int duration, RawImage rawImage)
    {
        // Hago la imagen visible primero poniendo el valor a = "Alpha" en 1
        rawImage.color = new UnityEngine.Color(rawImage.color.r, rawImage.color.g, rawImage.color.b, 1f);
        
        yield return new WaitForSeconds(0.5f); // Espero x segundos para que se vea bien la imagen antes de hacerla desaparecer.

        // Muestro la tinta del sello lentamente
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration); // Interpola de 1 a 0 (totalmente visible a invisible)

            // Aplico el valor alfa interpolado a cada imagen
            rawImage.color = new UnityEngine.Color(rawImage.color.r, rawImage.color.g, rawImage.color.b, alpha);

            yield return null; // Espera un frame antes de continuar el bucle
        }
        rawImage.gameObject.SetActive(false);
    }

    public void CancelarEliminarMesaConReservas()
    {
        EsconderManosAdvertencia();
    }

    private void EsconderManosAdvertencia()
    {
        rtManosAdvertencia.gameObject.SetActive(false);
        rtManosAdvertencia.anchoredPosition = new Vector2(rtManosAdvertencia.anchoredPosition.x, -898f);
        rtManosAdvertencia.gameObject.SetActive(true);
    }

    public void EliminarMesaConReservas()
    {
        Debug.Log("Ver si pillo correctamente el nombre del id que quiero eliminar: " + idMesaAEliminar);
        EliminarMesaEnBDDAsync(idMesaAEliminar);
    }

    private async void EliminarMesaEnBDDAsync(int idMesa)
    {
        string cad2 = await instanceMétodosApiController.DeleteDataAsync("mesa/borrarxid/" + idMesaAEliminar);

        // Deserializo la respuesta
        Resultado resultado2 = JsonConvert.DeserializeObject<Resultado>(cad2);

        // Mesa eliminada correctamente
        if (resultado2.Result.Equals(1))
        {
            Debug.Log("Mesa " + idMesa + " eliminada correctamente en la BDD.");

            //Elimino también la mesa en el editor del restaurante
            // Busco el botón en el contenedor de botones.
            Transform botonTransform = buttonParent.Find("Button-" + idMesa);
            Destroy(botonTransform.gameObject);
        }
        else
        {
            Debug.Log("Fallo al intentar eliminar mesa: " + idMesa + " en la BDD.");
        }

        EsconderManosAdvertencia(); // Por si las manos de advertencia se están mostrando
    }
}
