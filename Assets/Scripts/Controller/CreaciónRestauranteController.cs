using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
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
    [SerializeField] private Button buttonConfirmarOpciones;


    private bool manoErrorMoviéndose = false;

    MétodosAPIController instanceMétodosAPIController;
    TrabajadorController instanceTrabajadorController;

    // Start is called before the first frame update
    void Start()
    {
        instanceMétodosAPIController = MétodosAPIController.InstanceMétodosAPIController;
        instanceTrabajadorController = TrabajadorController.InstanceTrabajadorController;

        List<string> opcionesHoras = new List<string> { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23" };
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

    public async void ConfirmarOpcionesAsync()
    {
        buttonConfirmarOpciones.interactable = false;

        Debug.Log("Confirmo las opciones");
        string nombreRestaurante = inputFieldNombreRestaurante.text.Trim();
        Debug.Log("Length InputField Nombre Restaurante: " + nombreRestaurante.Length);
        nombreRestaurante = Regex.Replace(nombreRestaurante, @"\s+", " "); // Reemplaza múltiples espacios por uno
        string horaApertura = textHoraAperturaRestaurante.text + ":" + textMinutoAperturaRestaurante.text;
        string horaCierre = textHoraCierreRestaurante.text + ":" + textMinutoCierreRestaurante.text;

        // Si el nombre del restaurante tiene más de 2 caracteres, se crea. Los inputField en Unity pueden contener un carácter de más invisible.
        if (nombreRestaurante.Length > 2)
        {
            Restaurante restaurante = new Restaurante(nombreRestaurante, horaApertura, horaCierre, "00:00", new List<Mesa>(), new List<Trabajador>());
            restaurante.mostrar();
            string cad = await instanceMétodosAPIController.PostDataAsync("restaurante/registrarRestaurante", restaurante);
            // Deserializo la respuesta
            Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

            switch (resultado.Result)
            {
                case 0:
                    if (Usuario.Idioma.CompareTo("Español") == 0 || Usuario.Idioma == null)
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
                case > 0:
                    textoErrorRegistro.text = "";
                    Debug.Log("Restaurante registrado correctamente");
                    Usuario.Restaurante_ID = resultado.Result;
                    FicheroController.GestionarEncriptarFicheroUserInfo(Usuario.ID, Usuario.Restaurante_ID, Usuario.Idioma, Usuario.Token);
                    StartCoroutine(MostrarManoOkay(2f));
                    GestionarRegistroExitoso(nombreRestaurante);
                    
                    break;
                /*case 2:
                    if (Usuario.Idioma.CompareTo("Español") == 0 || Usuario.Idioma == null)
                    {
                        textoErrorRegistro.text = "El restaurante ya existe.";
                        StartCoroutine(MostrarManoError(2f));
                    }
                    else
                    {
                        textoErrorRegistro.text = "The restaurant already exists.";
                        StartCoroutine(MostrarManoError(2f));
                    }
                    break;*/
            }
            resultado.Result = -2;
        }
        else
        {
            Debug.Log("Nombre tiene menos de 3 caracteres");
            if (Usuario.Idioma.CompareTo("Español") == 0 || Usuario.Idioma == null)
            {
                textoErrorRegistro.text = "El nombre tiene menos de 3 caracteres.";
            }
            else
            {
                textoErrorRegistro.text = "The name has less than 3 characters.";
            }
                
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
        float velocidad = 1000f; 
        while (rt.anchoredPosition.x >= 716)
        {
            //Actualizo
            float x = rt.anchoredPosition.x - velocidad * Time.deltaTime;

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
            buttonConfirmarOpciones.interactable = true;
        }
    }

    private IEnumerator MoverManoErrorHaciaArriba(RectTransform rt, int distancia)
    {
        float velocidad = 1000f;
        while (rt.anchoredPosition.y <= -425)
        {
            //Actualizo
            float y = rt.anchoredPosition.y + velocidad * Time.deltaTime;

            // Pinto
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);

            // Espero al siguiente frame antes de continuar. Más fluido que usar un WaitForSeconds(), ya que el movimiento no se basa en los FPS.
            yield return null;

        }
    }

    private IEnumerator MoverManoErrorHaciaAbajo(RectTransform rt, int distancia)
    {
        float velocidad = 1000f;
        while (rt.anchoredPosition.y >= -836)
        {
            //Actualizo
            float y = rt.anchoredPosition.y - velocidad * Time.deltaTime;

            // Pinto
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);

            // Espero al siguiente frame antes de continuar. Más fluido que usar un WaitForSeconds(), ya que el movimiento no se basa en los FPS.
            yield return null;
        }
    }

    // Método llamado cuando el registro es exitoso
    private async void GestionarRegistroExitoso(string nombreRestaurante)
    {
        /*
        // Obtengo el id del restaurante
        string cad2 = await instanceMétodosAPIController.GetDataAsync("restaurante/getRestaurantePorNombre/" + nombreRestaurante);
        // Deserializo
        Restaurante restaurant = JsonConvert.DeserializeObject<Restaurante>(cad2);
        Debug.Log("Restaurante: " + restaurant.mostrar());

        Usuario.Restaurante_ID = restaurant.Id;
        */
        Usuario.Rol_ID = 2; // Al comprar el servicio, el rol del usuario cambia

        // Actualizo el trabajador con nuevo Rol por comprar el servicio y su nuevo restaurante_ID.
        await instanceTrabajadorController.ActualizarDatosTrabajadorPorIdAsync(new Trabajador(Usuario.ID, Usuario.Nombre, "", 2, Usuario.Restaurante_ID));

        StartCoroutine(FinRegistroRestaurante());
    }

    private IEnumerator FinRegistroRestaurante()
    {
        yield return new WaitForSeconds(2f); // Espero a que se muestre bien la mano de OKAY

        textoErrorRegistro.text = "";

        canvasCreaciónRestaurante.SetActive(false);

        buttonConfirmarOpciones.interactable = true;
    }
}
