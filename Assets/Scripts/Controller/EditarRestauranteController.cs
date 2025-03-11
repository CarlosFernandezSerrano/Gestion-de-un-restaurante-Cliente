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
    [SerializeField] private UnityEngine.UI.Button buttonAñadirMesa;



    private string NombreRestaurante;
    private string HoraApertura;
    private string HoraCierre;


    ButtonMesaController instanceButtonMesaController;
    MétodosAPIController instanceMétodosApiController;


    void Awake()
    {
        SceneManager.LoadSceneAsync("General Controller", LoadSceneMode.Additive);
    }

    // Start is called before the first frame update
    void Start()
    {
        instanceButtonMesaController = ButtonMesaController.InstanceButtonMesaController;
        instanceMétodosApiController = MétodosAPIController.InstanceMétodosAPIController;
        
        TrabajadorController.ComprobandoDatosTrabajador = false;

        InicializarValoresDropdowns();

        ObtenerDatosRestaurante();

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

    private async void ObtenerDatosRestaurante()
    {
        string cad = await instanceMétodosApiController.GetDataAsync("restaurante/getRestaurantePorId/" + PlayerPrefs.GetInt("Restaurante_ID Usuario"));

        // Deserializo la respuesta
        Restaurante restaurante = JsonConvert.DeserializeObject<Restaurante>(cad);

        NombreRestaurante =  restaurante.Nombre;
        HoraApertura = restaurante.HoraApertura;
        HoraCierre = restaurante.HoraCierre;

        inputFieldNombreRestaurante.text = NombreRestaurante;

        Debug.Log("Hora Apertura: " + restaurante.HoraApertura);
        Debug.Log("Hora Cierre: " + restaurante.HoraCierre);

        AsignarHorasEnDropdowns();
        


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
        GestionarGuardarDatosRestaurante();
    }

    private async void GestionarGuardarDatosRestaurante()
    {
        Restaurante restaurante = RellenarRestauranteSiActualizara();
        

        bool b = await NombreRestauranteVálidoDistintoYNoRepetidoEnLaBDD();

        // Nombre cambiado y comprobado que se puede actualizar porque no existe otro restaurante con ese nuevo nombre
        if (b)
        {
            Debug.Log("Hay cambios 1 ");
            await ActualizarRestauranteEnBDDAsync(restaurante);
            
            ObtenerDatosRestaurante(); 
        }
        else if (HorasDistintasEnRestaurante() && NombreEsIgualQueEnLaBDD())
        {
            Debug.Log("Hay cambios 2");
            await ActualizarRestauranteEnBDDAsync(restaurante);

            ObtenerDatosRestaurante();
        }
        else
        {
            Debug.Log("No hay cambios");
        }
    }

    private async Task ActualizarRestauranteEnBDDAsync(Restaurante restaurante)
    {
        // Poner método aquí para actualizar datos restaurante en la BDD
        string cad = await instanceMétodosApiController.PutDataAsync("restaurante/actualizarRestaurante/", restaurante);
        // Deserializo la respuesta
        Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

        if (resultado.Result.Equals(1))
        {
            Debug.Log("Actualización exitosa");
        }
        else
        {
            Debug.Log("La actualización salió mal");
        }
    }

    private Restaurante RellenarRestauranteSiActualizara()
    {
        int id = PlayerPrefs.GetInt("Restaurante_ID Usuario");
        string nombreRestaurante = inputFieldNombreRestaurante.text.Trim();
        nombreRestaurante = Regex.Replace(nombreRestaurante, @"\s+", " "); // Reemplaza múltiples espacios por uno

        string hora_Apertura = horaApertura.options[horaApertura.value].text+" : "+ minutoApertura.options[minutoApertura.value].text;
        string hora_Cierre = horaCierre.options[horaCierre.value].text + " : " + minutoCierre.options[minutoCierre.value].text;

        return new Restaurante(id, nombreRestaurante, hora_Apertura, hora_Cierre, new List<Mesa>(), new List<Trabajador>());
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

    private void ComprobarSiHayCambios()
    {
        // Hay cambios sin guardar
        if (HorasDistintasEnRestaurante()) // Cambiar --------- - --- - - -- - -  - - - - - - --  - - -  - - - -  -
        {
            Debug.Log("Hay cambios. ¿Desea guardar antes de irse?");
            DesactivarBotonesDelCanvas();
            imgHayCambiosSinGuardar.SetActive(true);

        }
        else // No hay cambios sin guardar
        {
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
        Guardar(); //Tengo que esperar a que se guarde antes de cambiar de escena
        imgHayCambiosSinGuardar.SetActive(false);
        //Aquí podría esperar un poquito antes de cargar la nueva escena ---------------------------------
        SceneManager.LoadScene("Main");
    }

    public void CancelarYSalir()
    {
        imgHayCambiosSinGuardar.SetActive(false);
        SceneManager.LoadScene("Main");
    }

    
}
