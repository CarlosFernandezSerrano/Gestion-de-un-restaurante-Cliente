using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class CreaciónRestauranteController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown horaApertura;
    [SerializeField] private TMP_Dropdown minutoApertura;
    [SerializeField] private TMP_Dropdown horaCierre;
    [SerializeField] private TMP_Dropdown minutoCierre;
    [SerializeField] private GameObject canvasCreaciónRestaurante;
    [SerializeField] private TMP_InputField inputFieldNombreRestaurante;
    [SerializeField] private TMP_Text textHoraAperturaRestaurante;
    [SerializeField] private TMP_Text textMinutoAperturaRestaurante;
    [SerializeField] private TMP_Text textHoraCierreRestaurante;
    [SerializeField] private TMP_Text textMinutoCierreRestaurante;
    [SerializeField] private TMP_Text textoErrorRegistro;
    [SerializeField] private GameObject manoError;
    [SerializeField] private GameObject manoOkay;


    private bool manoErrorMoviéndose = false;

    MétodosAPIController instanceMétodosAPIController;
    TrabajadorController instanceTrabajadorController;

    // Start is called before the first frame update
    void Start()
    {
        instanceMétodosAPIController = MétodosAPIController.InstanceMétodosAPIController;
        instanceTrabajadorController = TrabajadorController.InstanceTrabajadorController;

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

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AgregarOpcionesADropdown(TMP_Dropdown dropdown, List<string> opciones)
    {
        dropdown.ClearOptions();  // Limpia opciones anteriores
        dropdown.AddOptions(opciones);
    }

    public void VolverAlCanvasInicioApp()
    {
        canvasCreaciónRestaurante.SetActive(false);
    }

    public async void ConfirmarOpciones()
    {
        Debug.Log("Confirmo las opciones");
        string nombreRestaurante = inputFieldNombreRestaurante.text.Trim();
        Debug.Log("Length InputField Nombre Restaurante: " + nombreRestaurante.Length);
        nombreRestaurante = Regex.Replace(nombreRestaurante, @"\s+", " "); // Reemplaza múltiples espacios por uno
        string horaApertura = textHoraAperturaRestaurante.text + " : " + textMinutoAperturaRestaurante.text;
        string horaCierre = textHoraCierreRestaurante.text + " : " + textMinutoCierreRestaurante.text;

        // Si el nombre del restaurante tiene más de 2 caracteres, se crea. Los inputField en Unity pueden contener un carácter de más invisible.
        if (nombreRestaurante.Length > 2)
        {
            Restaurante restaurante = new Restaurante(nombreRestaurante, horaApertura, horaCierre, new List<Mesa>(), new List<Trabajador>());
            restaurante.mostrar();
            string cad = await instanceMétodosAPIController.PostDataAsync("restaurante/registrarRestaurante", restaurante);
            // Deserializo la respuesta
            Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

            switch (resultado.Result)
            {
                case 0:
                    if (PlayerPrefs.GetString("TipoIdioma").CompareTo("Español") == 0 || PlayerPrefs.GetString("TipoIdioma") == null)
                    {
                        textoErrorRegistro.text = "Error inesperado.";
                        StartCoroutine(MostrarManoError(2f));
                    }
                    else
                    {
                        textoErrorRegistro.text = "Unexpected error.";
                        StartCoroutine(MostrarManoError(2f));
                    }
                    break;
                case 1:
                    textoErrorRegistro.text = "";
                    Debug.Log("Restaurante registrado correctamente");
                    StartCoroutine(MostrarManoOkay(2f));
                    GestionarRegistroExitoso(nombreRestaurante);
                    
                    break;
                case 2:
                    if (PlayerPrefs.GetString("TipoIdioma").CompareTo("Español") == 0 || PlayerPrefs.GetString("TipoIdioma") == null)
                    {
                        textoErrorRegistro.text = "El restaurante ya existe.";
                        StartCoroutine(MostrarManoError(2f));
                    }
                    else
                    {
                        textoErrorRegistro.text = "The restaurant already exists.";
                        StartCoroutine(MostrarManoError(2f));
                    }
                    break;
            }
            resultado.Result = -2;
        }
        else
        {
            Debug.Log("Nombre tiene menos de 3 caracteres");
            textoErrorRegistro.text = "El nombre tiene menos de 3 caracteres.";
            StartCoroutine(MostrarManoError(2f));
        }

        
    }

    private IEnumerator MostrarManoOkay(float duration)
    {
        RectTransform rt = manoOkay.GetComponent<RectTransform>();

        // Espero a que termine de moverse la mano hacia la izquierda.
        yield return StartCoroutine(MoverManoHaciaLaIzquierda(rt, 600));

        // Espero un tiempo para mostrar la mano. 
        yield return new WaitForSeconds(duration);

    }

    private IEnumerator MoverManoHaciaLaIzquierda(RectTransform rt, int distancia)
    {
        for (int i = 0; i < distancia; i++)
        {
            //Actualizo
            float x = rt.anchoredPosition.x - 1;

            // Pinto
            rt.anchoredPosition = new Vector2(x, rt.anchoredPosition.y);

            // Espero al siguiente frame antes de continuar. Más fluido que usar un WaitForSeconds(), ya que el movimiento no se basa en los FPS.
            yield return null;
        }
    }

    private IEnumerator MostrarManoError(float duration)
    {
        RectTransform rt = manoError.GetComponent<RectTransform>();

        // Si el telón no se mueve...
        if (!manoErrorMoviéndose)
        {
            manoErrorMoviéndose = true;
            // Espero a que termine de moverse la mano hacia arriba.
            yield return StartCoroutine(MoverManoErrorHaciaArriba(rt, 350));

            // Espero un tiempo para mostrar la mano con el error 
            yield return new WaitForSeconds(duration);

            // Muevo la mano hacia abajo.
            yield return StartCoroutine(MoverManoErrorHaciaAbajo(rt, 350));

            manoErrorMoviéndose = false;
        }
    }

    private IEnumerator MoverManoErrorHaciaArriba(RectTransform rt, int distancia)
    {
        for (int i = 0; i < distancia; i++)
        {
            //Actualizo
            float y = rt.anchoredPosition.y + 1;

            // Pinto
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);

            // Espero al siguiente frame antes de continuar. Más fluido que usar un WaitForSeconds(), ya que el movimiento no se basa en los FPS.
            yield return null;
        }
    }

    private IEnumerator MoverManoErrorHaciaAbajo(RectTransform rt, int distancia)
    {
        for (int i = 0; i < distancia; i++)
        {
            //Actualizo
            float y = rt.anchoredPosition.y - 1;

            // Pinto
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);

            // Espero al siguiente frame antes de continuar. Más fluido que usar un WaitForSeconds(), ya que el movimiento no se basa en los FPS.
            yield return null;
        }
    }

    // Método llamado cuando el registro es exitoso
    private async void GestionarRegistroExitoso(string nombreRestaurante)
    {
        // Guardo este valor en este PlayerPrefs para usar futuramente.
        PlayerPrefs.SetString("Nombre Restaurante", nombreRestaurante);
        PlayerPrefs.Save();

        // Obtengo el id del restaurante
        string cad2 = await instanceMétodosAPIController.GetDataAsync("restaurante/getRestaurantePorNombre/" + nombreRestaurante);
        // Deserializo
        Restaurante restaurant = JsonConvert.DeserializeObject<Restaurante>(cad2);
        Debug.Log("Restaurante: " + restaurant.mostrar());

        PlayerPrefs.SetInt("Restaurante_ID Usuario", restaurant.Id);
        PlayerPrefs.SetInt("Rol_ID Usuario", 2); // Al comprar el servicio, el rol del usuario cambia
        PlayerPrefs.Save();

        // Actualizo el trabajador con nuevo Rol por comprar el servicio y su nuevo restaurante_ID.
        await instanceTrabajadorController.ActualizarDatosTrabajadorPorIdAsync(new Trabajador(PlayerPrefs.GetInt("ID Usuario"), PlayerPrefs.GetString("Nombre Usuario"), "", 2, PlayerPrefs.GetInt("Restaurante_ID Usuario")));

        StartCoroutine(FinRegistroRestaurante());
    }

    private IEnumerator FinRegistroRestaurante()
    {
        yield return new WaitForSeconds(2f); // Espero a que se muestre bien la mano de OKAY

        textoErrorRegistro.text = "";

        canvasCreaciónRestaurante.SetActive(false);

        //Desactivar el botón de comprar el servicio y mostrar opciones conseguidas: edición del restaurante, etc.
    }
}
