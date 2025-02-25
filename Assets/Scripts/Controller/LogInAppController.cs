using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using System.Text;
using UnityEditor.PackageManager.UI;
using Newtonsoft.Json;
using UnityEditor.PackageManager;


public class LogInAppController : MonoBehaviour
{
    [SerializeField] private GameObject canvasRegistroUsuario;
    [SerializeField] private GameObject canvasInicioSesiónUsuario;
    [SerializeField] private TMP_Text textoErrorLogin;
    [SerializeField] private TMP_Text textoExitoLogin;
    [SerializeField] private TMP_InputField inputFieldNombreLogin;
    [SerializeField] private TMP_InputField inputFieldPasswordLogin;
    [SerializeField] private Button botónRegistrarse;
    [SerializeField] private Button botónAcceder;
    [SerializeField] private TMP_InputField[] inputFields; // Asigno los InputFields en el orden de tabulación deseado

    //private bool mensajeAPIPlayFabDevuelto = false;

    MétodosAPIController instanceMétodosAPIController;


    // Start is called before the first frame update
    void Start()
    {
        instanceMétodosAPIController = MétodosAPIController.instanceMétodosAPIController;

        //StartCoroutine(GetData("listar"));
        StartCoroutine(instanceMétodosAPIController.GetData("cliente/listar"));
        
        
        
        //StartCoroutine(DeleteData());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SelectNextInputField();
        }
    }


    // Método para ser llamado por el botón de inicio de sesión
    public void gestionarIniciarSesión()
    {
        textoErrorLogin.text = "";
        StartCoroutine(desactivarPorUnTiempoLosBotonesYLuegoActivarCuandoHayaRespuestaDeLaAPIdePlayFab());

        string username = inputFieldNombreLogin.text.Trim();
        string password = inputFieldPasswordLogin.text.Trim();

        // Validaciones básicas antes de intentar iniciar sesión
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            textoErrorLogin.text = "Por favor, ingresa tu nombre de usuario y contraseña.";
            //mensajeAPIPlayFabDevuelto = true; //No accede a la API en este punto, lo pongo como si accediese. Pero necesito activar esta variable aquí para que funcione bien el resto del código, en vez de crear 2 variables
            return;
        }
        Debug.Log("El usuario trata de iniciar sesión");

        StartCoroutine(LoginUser(username, password));
    }

    private IEnumerator desactivarPorUnTiempoLosBotonesYLuegoActivarCuandoHayaRespuestaDeLaAPIdePlayFab()
    {
        //mensajeAPIPlayFabDevuelto = false;

        botónRegistrarse.interactable = false;
        botónAcceder.interactable = false;

        //yield return new WaitUntil(() => mensajeAPIPlayFabDevuelto);
        yield return new WaitForSeconds(2f);

        botónRegistrarse.interactable = true;
        botónAcceder.interactable = true;
    }

    // Método para iniciar sesión al usuario
    public IEnumerator LoginUser(string nombre, string password)
    {
        // Crear la solicitud de inicio de sesión
        
        //Cliente cliente = new Cliente("2", "Carlos Fernandez", "30", "juan@example.com");
        yield return StartCoroutine(instanceMétodosAPIController.GetData("cliente/existe/"+nombre));

        //yield return StartCoroutine(instanceMétodosAPIController.PostData("cliente/guardar", cliente));
    }

    // Coroutine para finalizar el proceso de inicio de sesión
    private IEnumerator FinInicioSesion()
    {
        yield return new WaitForSeconds(1f); // Espera 1 segundo

        textoExitoLogin.text = "";
        textoErrorLogin.text = "";
        canvasInicioSesiónUsuario.SetActive(false);
        //Dejo vacíos los campos por si se vuelve a ver este canvas
        inputFieldNombreLogin.text = "";
        inputFieldPasswordLogin.text = "";

        //mensajeAPIPlayFabDevuelto = true;

        PlayerPrefs.SetInt("UsuarioRegistrado", 1);
    }

    

    public void irAlCanvasRegistrarse()
    {
        canvasInicioSesiónUsuario.SetActive(false);
        canvasRegistroUsuario.SetActive(true);

        textoErrorLogin.text = "";

        //Dejo vacíos los campos por si se vuelve a ver este canvas
        inputFieldNombreLogin.text = "";
        inputFieldPasswordLogin.text = "";
    }

    /// <summary>
    /// Método para cambiar de componente con TAB en la interfaz gráfica.
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
