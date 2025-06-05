using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Newtonsoft.Json.Linq;
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
    [SerializeField] private GameObject canvasIdiomasLogInYRegistro;
    [SerializeField] private TMP_InputField[] inputFields; // Asigno los InputFields en el orden de tabulación deseado
    [SerializeField] private RectTransform rtBotónIdiomaSpanish;
    [SerializeField] private RectTransform rtBotónIdiomaEnglish;
    [SerializeField] private RectTransform rtTextoRegistrarse;
    [SerializeField] private GameObject canvasInformaciónApp;

    //private bool mensajeAPIPlayFabDevuelto = false;

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

        if (botónAcceder.IsInteractable())
        {
            // Detectar Enter principal
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("Se ha pulsado Enter");
                ConfirmarIniciarSesión();
            }

            // Detectar Enter del teclado numérico
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                Debug.Log("Se ha pulsado Enter del teclado numérico");
                ConfirmarIniciarSesión();
            }
        }
        
    }


    // Método para ser llamado por el botón de inicio de sesión
    public void ConfirmarIniciarSesión()
    {
        textoErrorLogin.text = "";
        StartCoroutine(DesactivarPorUnTiempoLosBotonesYLuegoActivarCuandoHayaRespuestaDeLaAPIdePlayFab());

        string username = inputFieldNombreLogin.text.Trim();
        string password = inputFieldPasswordLogin.text.Trim();

        // Validaciones básicas antes de intentar iniciar sesión
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            textoErrorLogin.text = "Por favor, ingresa tu nombre de usuario y contraseña.";
            return;
        }
        Debug.Log("El usuario trata de iniciar sesión");

        LoginUserAsync(username, password);
    }

    private IEnumerator DesactivarPorUnTiempoLosBotonesYLuegoActivarCuandoHayaRespuestaDeLaAPIdePlayFab()
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
    public async void LoginUserAsync(string nombre, string password)
    {
        // Creo la solicitud de inicio de sesión
        Trabajador t = new Trabajador(nombre, password, 0, 0);
        string cad = await instanceMétodosAPIController.PostDataAsync("trabajador/logIn/", t);

        // Deserializo la respuesta
        JObject jsonObject = JObject.Parse(cad);
        int resultValue = jsonObject["result"].Value<int>();
        

        //Resultado data = JsonConvert.DeserializeObject<Resultado>(cad);
        switch (resultValue)
        {
            case 1:
                string tokenValue = jsonObject["token"].Value<string>();
                Usuario.Token = tokenValue;
                Debug.Log("Token: " + Usuario.Token);
                FicheroController.GestionarEncriptarFicheroUserInfo(Usuario.ID, Usuario.Restaurante_ID, Usuario.Idioma, tokenValue); // Guardo el token en el fichero

                if (Usuario.Idioma.CompareTo("Español") == 0 || Usuario.Idioma == null)
                {
                    textoExitoLogin.text = "Inicio de sesión correcto";
                }
                else
                {
                    textoExitoLogin.text = "Successful login";
                }
                GestionarLogInExitoso();
                instanceTrabajadorController.ObtenerDatosTrabajadorPorNombreAsync(new Trabajador(nombre, "", 0, 0));
                break;
            case 0:
                if (Usuario.Idioma.CompareTo("Español") == 0 || Usuario.Idioma == null)
                {
                    textoErrorLogin.text = "Contraseña incorrecta";
                }
                else
                {
                    textoErrorLogin.text = "Incorrect password";
                }
                break;
            case -1:
                if (Usuario.Idioma.CompareTo("Español") == 0 || Usuario.Idioma == null)
                {
                    textoErrorLogin.text = "El trabajador " + nombre + " no existe";
                }
                else
                {
                    textoErrorLogin.text = "The worker " + nombre + " does not exist";
                }
                break;
        }
    }

    private void GestionarLogInExitoso()
    {
        StartCoroutine(FinInicioSesion());

        string nombreUsuario = inputFieldNombreLogin.text.Trim();

        Usuario.Nombre = nombreUsuario;
    }

    // Coroutine para finalizar el proceso de inicio de sesión
    private IEnumerator FinInicioSesion()
    {
        yield return new WaitForSeconds(1f); // Espera 1 segundo

        textoExitoLogin.text = "";
        textoErrorLogin.text = "";
        
        canvasIdiomasLogInYRegistro.SetActive(false);
        canvasInicioSesiónUsuario.SetActive(false);

        //Dejo vacíos los campos por si se vuelve a ver este canvas
        inputFieldNombreLogin.text = "";
        inputFieldPasswordLogin.text = "";

        PlayerPrefs.SetInt("UsuarioRegistrado", 1);
        PlayerPrefs.Save();
    }

    

    public void IrAlCanvasInformationApp()
    {
        canvasInicioSesiónUsuario.SetActive(false);
        canvasIdiomasLogInYRegistro.SetActive(false);
        canvasInformaciónApp.SetActive(true);

        textoErrorLogin.text = "";

        //Dejo vacíos los campos por si se vuelve a ver este canvas
        inputFieldNombreLogin.text = "";
        inputFieldPasswordLogin.text = "";
    }

    public void ActivarCanvasIniciarSesión()
    {
        canvasInformaciónApp.SetActive(false);
        canvasInicioSesiónUsuario.SetActive(true);
        canvasIdiomasLogInYRegistro.SetActive(true);
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
