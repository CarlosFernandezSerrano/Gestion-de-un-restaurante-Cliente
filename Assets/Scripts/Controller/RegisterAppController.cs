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
    [SerializeField] private GameObject canvasIniciarSesi�nUsuario;
    [SerializeField] private TMP_Text textoErrorRegistro;
    [SerializeField] private TMP_Text texto�xitoRegistro;
    [SerializeField] private TMP_InputField inputTextoNombre;
    [SerializeField] private TMP_InputField inputTextoContrase�a;
    [SerializeField] private TMP_InputField inputTextoContrase�aRepetida;
    [SerializeField] private Button bot�nConfirmar;
    [SerializeField] private Button bot�nIniciarSesi�n;
    [SerializeField] private GameObject canvasIdiomasLogInYRegistro;
    [SerializeField] private TMP_InputField[] inputFields; // Asigno los InputFields en el orden de tabulaci�n deseado


    M�todosAPIController instanceM�todosAPIController;
    TrabajadorController instanceTrabajadorController;

    // Start is called before the first frame update
    void Start()
    {
        instanceM�todosAPIController = M�todosAPIController.InstanceM�todosAPIController;
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

    
    public void CambiarACanvasIniciarSesi�n()
    {
        canvasRegistroUsuario.SetActive(false);
        canvasIniciarSesi�nUsuario.SetActive(true);

        textoErrorRegistro.text = "";

        //Dejo vac�os los campos por si se vuelve a ver este canvas
        inputTextoNombre.text = "";
        inputTextoContrase�a.text = "";
        inputTextoContrase�aRepetida.text = "";
    }

    public void ConfirmarRegistroUsuario()
    {
        textoErrorRegistro.text = "";
        StartCoroutine(DesactivarPorUnTiempoLosBotonesYLuegoActivarCuandoHayaRespuestaDeLaAPIdePlayFab());

        string textoNombre = inputTextoNombre.text.Trim();
        string textoPassword = inputTextoContrase�a.text.Trim();
        string textoRepeatedPassword = inputTextoContrase�aRepetida.text.Trim();

        //Si se han rellenado todos los huecos (InputFields) pasa por aqu�
        if (!string.IsNullOrEmpty(textoNombre) && !string.IsNullOrEmpty(textoPassword) && !string.IsNullOrEmpty(textoRepeatedPassword))
        {
            // La contrase�a es igual en ambos lados. 
            if (textoPassword.CompareTo(textoRepeatedPassword) == 0)
            {
                // La contrase�a tiene m�s de 5 caracteres
                if (textoPassword.Length > 5)
                {
                    // El nombre tiene m�s de 2 caracteres
                    if (textoNombre.Length > 2)
                    {
                        // El nombre no tiene la �
                        //if (!textoNombre.Contains("�") && !textoNombre.Contains("�"))
                        //{
                            Debug.Log("Nombre sin �");

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
                            Debug.Log("Nombre con �");
                            textoErrorRegistro.text = "El nombre no debe tener �.";
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
                    Debug.Log("Contrase�a con menos de 6 caracteres");
                    textoErrorRegistro.text = "Contrase�a con menos de 6 caracteres.";
                }
            }
            else
            {
                Debug.Log("Contrase�a diferente en ambos lados");
                textoErrorRegistro.text = "Contrase�a diferente en ambos lados.";
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

        bot�nIniciarSesi�n.interactable = false;
        bot�nConfirmar.interactable = false;

        //yield return new WaitUntil(() => mensajeAPIPlayFabDevuelto);
        yield return new WaitForSeconds(1.5f);

        bot�nIniciarSesi�n.interactable = true;
        bot�nConfirmar.interactable = true;
    }


    // M�todo para registrar al usuario
    public async void RegisterUserAsync(string username, string password)
    {
        // Creo la solicitud de registro
        Debug.Log("El usuario trata de registrarse");
        Trabajador t = new Trabajador(username, password, 0, 0);
        string cad = await instanceM�todosAPIController.PostDataAsync("trabajador/registrarUser", t);

        // Deserializo la respuesta
        Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);
        //Debug.Log("El valor de result es: " + data.Result);
        
        switch (resultado.Result)
        {
            case 0:
                if (PlayerPrefs.GetString("TipoIdioma").CompareTo("Espa�ol") == 0 || PlayerPrefs.GetString("TipoIdioma") == null)
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
                if (PlayerPrefs.GetString("TipoIdioma").CompareTo("Espa�ol") == 0 || PlayerPrefs.GetString("TipoIdioma") == null)
                {
                    texto�xitoRegistro.text = "Trabajador registrado correctamente";
                }
                else
                {
                    texto�xitoRegistro.text = "Worker registered correctly";
                }
                GestionarRegistroExitoso();
                instanceTrabajadorController.ObtenerDatosTrabajadorPorNombreAsync(new Trabajador(username, "", 0, 0));
                break;
            case 2:
                if (PlayerPrefs.GetString("TipoIdioma").CompareTo("Espa�ol") == 0 || PlayerPrefs.GetString("TipoIdioma") == null)
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

    

    // M�todo llamado cuando el registro es exitoso
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
        yield return new WaitForSeconds(1f); // 1f = duraci�n que espera antes de ocultar el canvas y as� mostrar el mensaje de "Usuario registrado con �xito" un poco.

        texto�xitoRegistro.text = "";
        textoErrorRegistro.text = "";
        
        canvasIdiomasLogInYRegistro.SetActive(false);
        canvasRegistroUsuario.SetActive(false);

        // Dejo vac�os los campos por si se vuelve a ver este canvas
        inputTextoNombre.text = "";
        inputTextoContrase�a.text = "";
        inputTextoContrase�aRepetida.text = "";

        PlayerPrefs.SetInt("UsuarioRegistrado", 1);
        PlayerPrefs.Save();
    }


    /// <summary>
    ///  M�todo para cambiar de componente con TAB en la interfaz gr�fica.
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
