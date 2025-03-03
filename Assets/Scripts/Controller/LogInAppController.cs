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


public class LogInAppController : MonoBehaviour
{
    [SerializeField] private GameObject canvasRegistroUsuario;
    [SerializeField] private GameObject canvasInicioSesi�nUsuario;
    [SerializeField] private TMP_Text textoErrorLogin;
    [SerializeField] private TMP_Text textoExitoLogin;
    [SerializeField] private TMP_InputField inputFieldNombreLogin;
    [SerializeField] private TMP_InputField inputFieldPasswordLogin;
    [SerializeField] private Button bot�nRegistrarse;
    [SerializeField] private Button bot�nAcceder;
    [SerializeField] private TMP_InputField[] inputFields; // Asigno los InputFields en el orden de tabulaci�n deseado

    //private bool mensajeAPIPlayFabDevuelto = false;

    M�todosAPIController instanceM�todosAPIController;


    // Start is called before the first frame update
    void Start()
    {
        instanceM�todosAPIController = M�todosAPIController.instanceM�todosAPIController;

                
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


    // M�todo para ser llamado por el bot�n de inicio de sesi�n
    public void gestionarIniciarSesi�n()
    {
        textoErrorLogin.text = "";
        StartCoroutine(desactivarPorUnTiempoLosBotonesYLuegoActivarCuandoHayaRespuestaDeLaAPIdePlayFab());

        string username = inputFieldNombreLogin.text.Trim();
        string password = inputFieldPasswordLogin.text.Trim();

        // Validaciones b�sicas antes de intentar iniciar sesi�n
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            textoErrorLogin.text = "Por favor, ingresa tu nombre de usuario y contrase�a.";
            return;
        }
        Debug.Log("El usuario trata de iniciar sesi�n");

        StartCoroutine(LoginUser(username, password));
    }

    private IEnumerator desactivarPorUnTiempoLosBotonesYLuegoActivarCuandoHayaRespuestaDeLaAPIdePlayFab()
    {
        //mensajeAPIPlayFabDevuelto = false;

        bot�nRegistrarse.interactable = false;
        bot�nAcceder.interactable = false;

        //yield return new WaitUntil(() => mensajeAPIPlayFabDevuelto);
        yield return new WaitForSeconds(2f);

        bot�nRegistrarse.interactable = true;
        bot�nAcceder.interactable = true;
    }

    // M�todo para iniciar sesi�n al usuario
    public IEnumerator LoginUser(string nombre, string password)
    {
        // Crear la solicitud de inicio de sesi�n

        Trabajador t = new Trabajador(nombre, password, 0, 0);
        yield return StartCoroutine(instanceM�todosAPIController.PostData("trabajador/logIn/", t));
        Debug.Log("Respuesta login user: "+instanceM�todosAPIController.respuestaGET);

        // Deserializo la respuesta
        Resultado data = JsonConvert.DeserializeObject<Resultado>(instanceM�todosAPIController.respuestaGET);
        if (data.Result.Equals(1)){
            textoErrorLogin.text = "El trabajador " + nombre + " ya existe";
        }
        data.Result = -1;
        Trabajador t2 = new Trabajador(nombre,password,1,1);
        //Cliente c = new Cliente(nombre, "024124124f", "335423252");
        yield return StartCoroutine(instanceM�todosAPIController.PostData("trabajador/guardar", t2));
        Debug.Log("Respueta despu�s de POST: " + instanceM�todosAPIController.respuestaPOST);
        // Si quieres deserializar:
        Trabajador trabajador2 = JsonConvert.DeserializeObject<Trabajador>(instanceM�todosAPIController.respuestaPOST);
        //Cliente cliente2 = JsonConvert.DeserializeObject<Cliente>(instanceM�todosAPIController.respuestaPOST);
        Debug.Log("Deserializado JSON en POST: " + trabajador2.mostrar());
        
    }

    // Coroutine para finalizar el proceso de inicio de sesi�n
    private IEnumerator FinInicioSesion()
    {
        yield return new WaitForSeconds(1f); // Espera 1 segundo

        textoExitoLogin.text = "";
        textoErrorLogin.text = "";
        canvasInicioSesi�nUsuario.SetActive(false);
        //Dejo vac�os los campos por si se vuelve a ver este canvas
        inputFieldNombreLogin.text = "";
        inputFieldPasswordLogin.text = "";

        //mensajeAPIPlayFabDevuelto = true;

        PlayerPrefs.SetInt("UsuarioRegistrado", 1);
    }

    

    public void irAlCanvasRegistrarse()
    {
        canvasInicioSesi�nUsuario.SetActive(false);
        canvasRegistroUsuario.SetActive(true);

        textoErrorLogin.text = "";

        //Dejo vac�os los campos por si se vuelve a ver este canvas
        inputFieldNombreLogin.text = "";
        inputFieldPasswordLogin.text = "";
    }

    /// <summary>
    /// M�todo para cambiar de componente con TAB en la interfaz gr�fica.
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
