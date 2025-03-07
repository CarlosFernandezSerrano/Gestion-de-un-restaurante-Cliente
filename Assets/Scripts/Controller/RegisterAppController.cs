using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using Assets.Scripts.Model;
using UnityEditor.Experimental.GraphView;
using System;
using Assets.Scripts.Controller;


public class RegisterAppController : MonoBehaviour
{
    [SerializeField] private GameObject canvasRegistroUsuario;
    [SerializeField] private GameObject canvasIniciarSesiónUsuario;
    [SerializeField] private TMP_Text textoErrorRegistro;
    [SerializeField] private TMP_Text textoÉxitoRegistro;
    [SerializeField] private TMP_InputField inputTextoNombre;
    [SerializeField] private TMP_InputField inputTextoContraseña;
    [SerializeField] private TMP_InputField inputTextoContraseñaRepetida;
    [SerializeField] private Button botónConfirmar;
    [SerializeField] private Button botónIniciarSesión;
    [SerializeField] private GameObject canvasIdiomasLogInYRegistro;
    [SerializeField] private TMP_InputField[] inputFields; // Asigno los InputFields en el orden de tabulación deseado


    MétodosAPIController instanceMétodosAPIController;
    TrabajadorController instanceTrabajadorController;

    // Start is called before the first frame update
    void Start()
    {
        instanceMétodosAPIController = MétodosAPIController.InstanceMétodosAPIController;
        instanceTrabajadorController = TrabajadorController.InstanceTrabajadorController;

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SelectNextInputField();
        }
    }

    
    public void CambiarACanvasIniciarSesión()
    {
        canvasRegistroUsuario.SetActive(false);
        canvasIniciarSesiónUsuario.SetActive(true);

        textoErrorRegistro.text = "";

        //Dejo vacíos los campos por si se vuelve a ver este canvas
        inputTextoNombre.text = "";
        inputTextoContraseña.text = "";
        inputTextoContraseñaRepetida.text = "";
    }

    public void ConfirmarRegistroUsuario()
    {
        textoErrorRegistro.text = "";
        StartCoroutine(DesactivarPorUnTiempoLosBotonesYLuegoActivarCuandoHayaRespuestaDeLaAPIdePlayFab());

        string textoNombre = inputTextoNombre.text.Trim();
        string textoPassword = inputTextoContraseña.text.Trim();
        string textoRepeatedPassword = inputTextoContraseñaRepetida.text.Trim();

        //Si se han rellenado todos los huecos (InputFields) pasa por aquí
        if (!string.IsNullOrEmpty(textoNombre) && !string.IsNullOrEmpty(textoPassword) && !string.IsNullOrEmpty(textoRepeatedPassword))
        {
            // La contraseña es igual en ambos lados. 
            if (textoPassword.CompareTo(textoRepeatedPassword) == 0)
            {
                // La contraseña tiene más de 5 caracteres
                if (textoPassword.Length > 5)
                {
                    // El nombre tiene más de 2 caracteres
                    if (textoNombre.Length > 2)
                    {
                        // El nombre no tiene la ñ
                        //if (!textoNombre.Contains("ñ") && !textoNombre.Contains("Ñ"))
                        //{
                            Debug.Log("Nombre sin ñ");

                            // El nombre no tiene espacios entre letras
                            if (!textoNombre.Contains(" "))
                            {
                               RegisterUserAsync(textoNombre, textoPassword);     
                            }
                            else
                            {
                                Debug.Log("El nombre no puede tener espacios entre letras");
                                textoErrorRegistro.text = "El nombre no puede tener espacios entre letras.";
                            }
                        /*}
                        else
                        {
                            Debug.Log("Nombre con ñ");
                            textoErrorRegistro.text = "El nombre no debe tener ñ.";
                        }*/
                    }
                    else
                    {
                        Debug.Log("Nombre con menos de 3 caracteres");
                        textoErrorRegistro.text = "Nombre con menos de 3 caracteres.";
                    }
                }
                else
                {
                    Debug.Log("Contraseña con menos de 6 caracteres");
                    textoErrorRegistro.text = "Contraseña con menos de 6 caracteres.";
                }
            }
            else
            {
                Debug.Log("Contraseña diferente en ambos lados");
                textoErrorRegistro.text = "Contraseña diferente en ambos lados.";
            }
        }
        else
        {
            Debug.Log("El usuario no se puede registrar.");
            textoErrorRegistro.text = "Rellene todos los campos por favor.";
        }
    }

    private IEnumerator DesactivarPorUnTiempoLosBotonesYLuegoActivarCuandoHayaRespuestaDeLaAPIdePlayFab()
    {

        botónIniciarSesión.interactable = false;
        botónConfirmar.interactable = false;

        //yield return new WaitUntil(() => mensajeAPIPlayFabDevuelto);
        yield return new WaitForSeconds(1.5f);

        botónIniciarSesión.interactable = true;
        botónConfirmar.interactable = true;
    }


    // Método para registrar al usuario
    public async void RegisterUserAsync(string username, string password)
    {
        // Creo la solicitud de registro
        Debug.Log("El usuario trata de registrarse");
        Trabajador t = new Trabajador(username, password, 0, 0);
        string cad = await instanceMétodosAPIController.PostDataAsync("trabajador/registrarUser", t);

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
                if (PlayerPrefs.GetString("TipoIdioma").CompareTo("Español") == 0 || PlayerPrefs.GetString("TipoIdioma") == null)
                {
                    textoÉxitoRegistro.text = "Trabajador registrado correctamente";
                }
                else
                {
                    textoÉxitoRegistro.text = "Worker registered correctly";
                }
                GestionarRegistroExitoso();
                instanceTrabajadorController.ObtenerDatosTrabajadorPorNombreAsync(new Trabajador(username, "", 0, 0));
                break;
            case 2:
                if (PlayerPrefs.GetString("TipoIdioma").CompareTo("Español") == 0 || PlayerPrefs.GetString("TipoIdioma") == null)
                {
                    textoErrorRegistro.text = "El usuario " + username + " ya existe";
                }
                else
                {
                    textoErrorRegistro.text = "The user " + username + " already exists";
                }
                break;            
        }
        resultado.Result = -2;        
    }

    

    // Método llamado cuando el registro es exitoso
    private void GestionarRegistroExitoso()
    {
        StartCoroutine(FinRegistroUsuario());

        string nombreUsuario = inputTextoNombre.text.Trim();

        //Guardo este valor en este PlayerPrefs para usar futuramente.
        PlayerPrefs.SetString("Nombre Usuario", nombreUsuario);
        PlayerPrefs.Save();
    }

    private IEnumerator FinRegistroUsuario()
    {
        yield return new WaitForSeconds(1f); // 1f = duración que espera antes de ocultar el canvas y así mostrar el mensaje de "Usuario registrado con éxito" un poco.

        textoÉxitoRegistro.text = "";
        textoErrorRegistro.text = "";
        
        canvasIdiomasLogInYRegistro.SetActive(false);
        canvasRegistroUsuario.SetActive(false);

        // Dejo vacíos los campos por si se vuelve a ver este canvas
        inputTextoNombre.text = "";
        inputTextoContraseña.text = "";
        inputTextoContraseñaRepetida.text = "";

        PlayerPrefs.SetInt("UsuarioRegistrado", 1);
        PlayerPrefs.Save();
    }


    /// <summary>
    ///  Método para cambiar de componente con TAB en la interfaz gráfica.
    /// </summary>
    private void SelectNextInputField()
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;

        for (int i = 0; i < inputFields.Length; i++)
        {
            if (inputFields[i].gameObject == current)
            {
                int nextIndex = (i + 1) % inputFields.Length;
                inputFields[nextIndex].Select();
                break;
            }
        }
    }
}
