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
    [SerializeField] private TMP_Text textNombreRestaurante;
    [SerializeField] private TMP_Text textHoraAperturaRestaurante;
    [SerializeField] private TMP_Text textMinutoAperturaRestaurante;
    [SerializeField] private TMP_Text textHoraCierreRestaurante;
    [SerializeField] private TMP_Text textMinutoCierreRestaurante;
    [SerializeField] private TMP_Text textoErrorRegistro;


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
        Debug.Log("Nombre restaurante Antes: " + nombreRestaurante);
        nombreRestaurante = Regex.Replace(nombreRestaurante, @"\s+", " "); // Reemplaza múltiples espacios por uno
        Debug.Log("Nombre restaurante Después: " + nombreRestaurante);
        string horaApertura = textHoraAperturaRestaurante.text + " : " + textMinutoAperturaRestaurante.text;
        string horaCierre = textHoraCierreRestaurante.text + " : " + textMinutoCierreRestaurante.text;

        // Si el nombre del restaurante tiene más de 2 caracteres, se crea
        if (nombreRestaurante.Length > 2)
        {
            Restaurante restaurante = new Restaurante(nombreRestaurante, horaApertura, horaCierre, new List<Mesa>(), new List<Trabajador>());
            restaurante.mostrar();
            string cad = await instanceMétodosAPIController.PostDataAsync("restaurante/registrarRestaurante", restaurante);
            // Deserializo la respuesta
            Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);
            //Debug.Log("El valor de result es: " + data.Result);

            switch (resultado.Result)
            {
                case 0:
                    if (PlayerPrefs.GetString("TipoIdioma").CompareTo("Español") == 0 || PlayerPrefs.GetString("TipoIdioma") == null)
                    {
                        textoErrorRegistro.text = "Error inesperado";
                    }
                    else
                    {
                        textoErrorRegistro.text = "Unexpected error";
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
                    }
                    else
                    {
                        textoErrorRegistro.text = "The restaurant already exists";
                    }
                    break;
            }
            resultado.Result = -2;
        }
        else
        {
            Debug.Log("Nombre tiene menos de 3 caracteres");
            textoErrorRegistro.text = "El nombre tiene menos de 3 caracteres";
        }

        
    }

}
