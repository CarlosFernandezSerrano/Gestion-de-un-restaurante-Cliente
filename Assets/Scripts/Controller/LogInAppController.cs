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
using Assets.Scripts.Model;
using Assets.Scripts.Controller;


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
    TrabajadorController instanceTrabajadorController;


    // Start is called before the first frame update
    void Start()
    {
        instanceMétodosAPIController = MétodosAPIController.instanceMétodosAPIController;
        instanceTrabajadorController = TrabajadorController.instanceTrabajadorController;
                
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
    public void ConfirmarIniciarSesión()
    {
        textoErrorLogin.text = "";
        StartCoroutine(desactivarPorUnTiempoLosBotonesYLuegoActivarCuandoHayaRespuestaDeLaAPIdePlayFab());

        string username = inputFieldNombreLogin.text.Trim();
        string password = inputFieldPasswordLogin.text.Trim();

        // Validaciones básicas antes de intentar iniciar sesión
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            textoErrorLogin.text = "Por favor, ingresa tu nombre de usuario y contraseña.";
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
        // Creo la solicitud de inicio de sesión
        Trabajador t = new Trabajador(nombre, password, 0, 0);
        yield return StartCoroutine(instanceMétodosAPIController.PostData("trabajador/logIn/", t));

        // Deserializo la respuesta
        Resultado data = JsonConvert.DeserializeObject<Resultado>(instanceMétodosAPIController.respuestaPOST);
        switch (data.Result)
        {
            case 1:
                textoExitoLogin.text = "Inicio de sesión correcto";
                GestionarLogInExitoso();
                yield return StartCoroutine(instanceTrabajadorController.ObtenerDatosTrabajador(t));
                break;
            case 0:
                textoErrorLogin.text = "Contraseña incorrecta";
                break;
            case -1:
                textoErrorLogin.text = "El trabajador " + nombre + " no existe";
                break;
        }
        data.Result = -2;
    }

    private void GestionarLogInExitoso()
    {
        StartCoroutine(FinInicioSesion());

        string nombreUsuario = inputFieldNombreLogin.text.Trim();

        //Guardo estos valores en estos PlayerPrefs para usar futuramente.
        PlayerPrefs.SetString("Nombre Usuario", nombreUsuario);
        PlayerPrefs.SetInt("Rol Usuario", 1);
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
