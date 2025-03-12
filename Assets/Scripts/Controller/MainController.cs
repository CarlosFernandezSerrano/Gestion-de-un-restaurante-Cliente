using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEditor.PackageManager;
using UnityEngine;

using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System;
using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;


public class MainController : MonoBehaviour
{
    [SerializeField] private GameObject canvasLogInUsuario;
    [SerializeField] private GameObject canvasIdiomasLogInYRegistro;
    [SerializeField] private GameObject medioYFinTelon;
    [SerializeField] private TMP_Text textUserNombre;
    [SerializeField] private TMP_Text textUserRol;
    [SerializeField] private TMP_Text textUserRestaurante;
    [SerializeField] private GameObject canvasCrearRestaurante;
    [SerializeField] private Button botónCerrarSesión;
    [SerializeField] private Button botónComprarServicio;
    [SerializeField] private Button botónEditarRestaurante;


    private bool telónMoviéndose = false;
    private bool telónAbajo = false;

    MétodosAPIController instanceMétodosAPIController;
    TrabajadorController instanceTrabajadorController;


    public static MainController InstanceMainController { get; private set; }

    void Awake()
    {
        if (InstanceMainController == null)
        {
            InstanceMainController = this;
        }

        SceneManager.LoadSceneAsync("General Controller", LoadSceneMode.Additive);

    }

    // Start is called before the first frame update
    void Start()
    {
        instanceMétodosAPIController = MétodosAPIController.InstanceMétodosAPIController;
        instanceTrabajadorController = TrabajadorController.InstanceTrabajadorController;

        TrabajadorController.ComprobandoDatosTrabajador = false;
        instanceTrabajadorController.PonerDatosEnPerfilTrabajador(textUserNombre, textUserRol, textUserRestaurante);


        GestiónInicioDelProgramaAsync();

        // Método para prevenir
        QuitarYPonerBotonesSegúnElTrabajador();

        float width = botónCerrarSesión.gameObject.GetComponent<RectTransform>().rect.width;
        float height = botónCerrarSesión.gameObject.GetComponent<RectTransform>().rect.height;
        Debug.Log("Width: " + width + " Y Height: " + height);
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void QuitarYPonerBotonesSegúnElTrabajador()
    {
        QuitarBotónComprarServicio();

        PonerBotónEditarRestaurante();
    }


    // Si el usuario tiene un restaurante_ID superior a 0 (el trabajador ya está asignado a un restaurante), se desactiva el botón de comprar el servicio
    private void QuitarBotónComprarServicio()
    {
        if (PlayerPrefs.GetInt("Restaurante_ID Usuario", 0) > 0)
        {
            botónComprarServicio.gameObject.SetActive(false);
        }
        else // El trabajador no está en ningún restaurante aún y le sale el botón de comprar el servicio
        {// Quizás no es necesario este else
            botónComprarServicio.gameObject.SetActive(true);
        }
    }

    private void PonerBotónEditarRestaurante()
    {
        // Si el trabajador tiene el rol de "Gerente", sale el botón para editar el restaurante
        if (PlayerPrefs.GetInt("Rol_ID Usuario").Equals(2))
        {
            botónEditarRestaurante.gameObject.SetActive(true);
        }
        else
        {
            botónEditarRestaurante.gameObject.SetActive(false);
        }
    }

    private async void GestiónInicioDelProgramaAsync()
    {
        //PlayerPrefs.SetInt("UsuarioRegistrado", 0); // - - - Quitar esta línea cuando deje de hacer pruebas con el registro e inicio de sesión

        int usuarioRegistrado = PlayerPrefs.GetInt("UsuarioRegistrado", 0); // 1 es sí, 0 es no
        //Si el usuario no se ha registrado, le aparece el canvas de iniciar sesión
        if (usuarioRegistrado.Equals(0))
        {
            canvasLogInUsuario.SetActive(true);
            canvasIdiomasLogInYRegistro.SetActive(true);
        }
        else // Si el usuario ya está registrado, compruebo si sigue en la BDD por si lo han eliminado y obtengo su rol_ID actualizado, por si el gerente se lo ha cambiado.
        {
           await ComprueboSiUserExisteAsync();
        }

        
        Debug.Log("ID Usuario: " + PlayerPrefs.GetInt("ID Usuario") + ", Nombre Usuario: " + PlayerPrefs.GetString("Nombre Usuario") + ", Rol_ID Usuario: " + PlayerPrefs.GetInt("Rol_ID Usuario") + ", Restaurante_ID Usuario: " + PlayerPrefs.GetInt("Restaurante_ID Usuario"));
    }

    private async Task ComprueboSiUserExisteAsync()
    {
        string cad = await instanceMétodosAPIController.GetDataAsync("trabajador/existe/" + PlayerPrefs.GetInt("ID Usuario"));
        try
        {
            Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

            // El trabajador no existe. Ha sido eliminado de la BDD y tiene que volver a registrarse.
            if (resultado.Result.Equals(0))
            {
                PlayerPrefs.SetInt("UsuarioRegistrado", 0);
                PlayerPrefs.Save();
                canvasLogInUsuario.SetActive(true);
                canvasIdiomasLogInYRegistro.SetActive(true);
            }
            else // El trabajdor existe y obtengo sus datos por si ha tenido cambios. Ejemplo: le han puesto un rol distinto o le han agregado a un restaurante.
            {
                instanceTrabajadorController.ObtenerDatosTrabajadorPorIdAsync(PlayerPrefs.GetInt("ID Usuario"));
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error: " + ex.Message);
        }
    }


    public void PresionarBotónPerfil()
    {
        RectTransform rt = medioYFinTelon.GetComponent<RectTransform>();

        // Si el telón no se mueve...
        if (!telónMoviéndose)
        {
            telónMoviéndose = true;
            botónCerrarSesión.interactable = false;
            // Y el telón no está abajo, va para abajo
            if (!telónAbajo)
            {
                StartCoroutine(MoverTelónHaciaAbajo(rt));
            }
            else // Y el telón está abajo, va para arriba
            {
                StartCoroutine(MoverTelónHaciaArriba(rt));
            }
        }
        
    }

    private IEnumerator MoverTelónHaciaAbajo(RectTransform rt)
    {
        for (int i = 0; i < 950; i++)
        {
            //Actualizo
            float y = rt.anchoredPosition.y - 1;

            // Pinto
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);

            // Espero al siguiente frame antes de continuar. Más fluido que usar un WaitForSeconds(), ya que el movimiento no se basa en los FPS.
            yield return null;
        }
        telónMoviéndose = false;
        telónAbajo = true;
        botónCerrarSesión.interactable = true;
    }

    private IEnumerator MoverTelónHaciaArriba(RectTransform rt)
    {
        for (int i = 0; i < 950; i++)
        {
            //Actualizo
            float y = rt.anchoredPosition.y + 1;

            // Pinto
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);

            // Espero
            yield return null;
        }
        telónMoviéndose = false;
        telónAbajo = false;
        botónCerrarSesión.interactable = true;
    }

    public void IrAlCanvasCrearRestaurante()
    {
        canvasCrearRestaurante.SetActive(true);
        //Si el telón está abajo
        if (telónAbajo)
            PresionarBotónPerfil();
    }

    public void IrALaEscenaEditarRestaurante()
    {
        SceneManager.LoadScene("Editar Restaurante");
    }

    public void CerrarSesión()
    {
        PlayerPrefs.SetInt("UsuarioRegistrado", 0);
        canvasLogInUsuario.SetActive(true);
        canvasIdiomasLogInYRegistro.SetActive(true);
        PresionarBotónPerfil();
    }

    private void OnEnable()
    {
        Application.wantsToQuit += OnWantsToQuitAsync;
    }

    private void OnDisable()
    {
        Application.wantsToQuit -= OnWantsToQuitAsync;
    }

    // Intercepta el intento de cerrar la aplicación (por ejemplo, con Alt + F4 o clic en el botón de cierre de la ventana).
    private bool OnWantsToQuitAsync() //Hacer que no funcione este método hasta que una parte del programa cargue que es la generación del lobby (creo, no estoy seguro)
    {
        Debug.Log("Interceptando Alt + F4 o cierre manual.");
        
        return true; // Unity cierra la aplicación automáticamente.
    }


    public TMP_Text getTextPerfilUserNombre()
    {
        return textUserNombre;
    }

    public TMP_Text getTextPerfilUserRol()
    {
        return textUserRol;
    }

    public TMP_Text getTextPerfilUserRestaurante()
    {
        return textUserRestaurante;
    }
}
