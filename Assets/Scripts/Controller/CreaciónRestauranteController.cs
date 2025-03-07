using Assets.Scripts.Model;
using Newtonsoft.Json;
using System;
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
    [SerializeField] private TMP_Text textNombreRestaurante;
    [SerializeField] private TMP_Text textHoraAperturaRestaurante;
    [SerializeField] private TMP_Text textMinutoAperturaRestaurante;
    [SerializeField] private TMP_Text textHoraCierreRestaurante;
    [SerializeField] private TMP_Text textMinutoCierreRestaurante;
    [SerializeField] private TMP_Text textoErrorRegistro;
    [SerializeField] private GameObject manoError;


    private bool manoErrorMoviéndose = false;

    MétodosAPIController instanceMétodosAPIController;

    // Start is called before the first frame update
    void Start()
    {
        instanceMétodosAPIController = MétodosAPIController.InstanceMétodosAPIController;

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
        string nombreRestaurante = textNombreRestaurante.text.Trim();
        Debug.Log("Length 1: " + nombreRestaurante.Length);
        Debug.Log("Nombre restaurante ANTES:"+nombreRestaurante+"*");
        nombreRestaurante = Regex.Replace(nombreRestaurante, @"\s+", " "); // Reemplaza múltiples espacios por uno
        Debug.Log("Nombre restaurante DESPUÉS: " + nombreRestaurante);
        string horaApertura = textHoraAperturaRestaurante.text + " : " + textMinutoAperturaRestaurante.text;
        string horaCierre = textHoraCierreRestaurante.text + " : " + textMinutoCierreRestaurante.text;

        // Si el nombre del restaurante tiene más de 2 caracteres, se crea
        if (nombreRestaurante.Length > 2)
        {
            Debug.Log("Length 2: " + nombreRestaurante.Trim().Length);
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
                        textoErrorRegistro.text = "Error inesperado";
                        StartCoroutine(MostrarManoError(2f));
                    }
                    else
                    {
                        textoErrorRegistro.text = "Unexpected error";
                        StartCoroutine(MostrarManoError(2f));
                    }
                    break;
                case 1:
                    textoErrorRegistro.text = "";
                    Debug.Log("Restaurante registrado correctamente");
                    
                    //GestionarRegistroExitoso();
                    //instanceTrabajadorController.ActualizarDatosTrabajadorPorNombreAsync(new Trabajador(username, "", 0, 0));
                    break;
                case 2:
                    if (PlayerPrefs.GetString("TipoIdioma").CompareTo("Español") == 0 || PlayerPrefs.GetString("TipoIdioma") == null)
                    {
                        textoErrorRegistro.text = "El restaurante ya existe";
                        StartCoroutine(MostrarManoError(2f));
                    }
                    else
                    {
                        textoErrorRegistro.text = "The restaurant already exists";
                        StartCoroutine(MostrarManoError(2f));
                    }
                    break;
            }
            resultado.Result = -2;
        }
        else
        {
            Debug.Log("Nombre tiene menos de 3 caracteres");
            textoErrorRegistro.text = "El nombre tiene menos de 3 caracteres";
            StartCoroutine(MostrarManoError(2f));
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
            yield return StartCoroutine(MoverManoHaciaArriba(rt));

            // Espero un tiempo para mostrar la mano con el error 
            yield return new WaitForSeconds(duration);

            // Muevo la mano hacia abajo.
            yield return StartCoroutine(MoverManoHaciaAbajo(rt));

            manoErrorMoviéndose = false;
        }
    }

    private IEnumerator MoverManoHaciaArriba(RectTransform rt)
    {
        for (int i = 0; i < 250; i++)
        {
            //Actualizo
            float y = rt.anchoredPosition.y + 1;

            // Pinto
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);

            // Espero al siguiente frame antes de continuar. Más fluido que usar un WaitForSeconds(), ya que el movimiento no se basa en los FPS.
            yield return null;
        }
    }

    private IEnumerator MoverManoHaciaAbajo(RectTransform rt)
    {
        for (int i = 0; i < 250; i++)
        {
            //Actualizo
            float y = rt.anchoredPosition.y - 1;

            // Pinto
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);

            // Espero al siguiente frame antes de continuar. Más fluido que usar un WaitForSeconds(), ya que el movimiento no se basa en los FPS.
            yield return null;
        }
    }

    
}
