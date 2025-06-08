using System.Collections;
using UnityEngine;

using System.Threading.Tasks;
using System;
using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Video;


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
    [SerializeField] private Button botónGestionarMesas;
    [SerializeField] private Button botónGestionarTrabajadores;
    [SerializeField] private RectTransform rtBotónIdiomaSpanish;
    [SerializeField] private RectTransform rtBotónIdiomaEnglish;
    [SerializeField] private RectTransform rtTextoIniciarSesión;
    [SerializeField] private GameObject canvasVídeoTutorialApp;
    [SerializeField] private VideoClip videoClipEmpleado;
    [SerializeField] private VideoClip videoClipGerente;
    [SerializeField] private Image imgBtnOnYPausa;
    [SerializeField] private Sprite imgPausa;
    [SerializeField] private Sprite imgPlay;
    [SerializeField] private VideoPlayer videoPlayer;

    private bool telónMoviéndose = false;
    private bool telónAbajo = false;

    MétodosAPIController instanceMétodosAPIController;
    TrabajadorController instanceTrabajadorController;
    FicheroController instanceFicheroController;

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
        instanceFicheroController = FicheroController.InstanceFicheroController;

        instanceFicheroController.GestionarFicheros();

        TrabajadorController.ComprobandoDatosTrabajador = false;
        instanceTrabajadorController.PonerDatosEnPerfilTrabajador(textUserNombre, textUserRol, textUserRestaurante);


        GestiónInicioDelProgramaAsync();

        // Método para prevenir
        QuitarYPonerBotonesSegúnElTrabajador();
        
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void QuitarYPonerBotonesSegúnElTrabajador()
    {
        QuitarBotónComprarServicio();

        PonerBotonesFuncionalidadesProgramaSegunElRol();
    }


    // Si el usuario tiene un restaurante_ID superior a 0 (el trabajador ya está asignado a un restaurante), se desactiva el botón de comprar el servicio
    private void QuitarBotónComprarServicio()
    {
        if (!GeneralController.NoHayConexion)
        {
            if (Usuario.Restaurante_ID > 0)
            {
                botónComprarServicio.gameObject.SetActive(false);
            }
            else // El trabajador no está en ningún restaurante aún y le sale el botón de comprar el servicio
            {// Quizás no es necesario este else
                botónComprarServicio.gameObject.SetActive(true);
            }
        }
    }

    private void PonerBotonesFuncionalidadesProgramaSegunElRol()
    {
        // Si el trabajador está en un restaurante, se comprueba el rol
        if (Usuario.Restaurante_ID > 0)
        {
            switch (Usuario.Rol_ID)
            {
                case 1:
                    botónEditarRestaurante.gameObject.SetActive(false);
                    botónGestionarMesas.gameObject.SetActive(true);
                    botónGestionarTrabajadores.gameObject.SetActive(false);
                    break;
                case 2: // Si el trabajador tiene el rol de "Gerente", se muestran los botones específicos para editar el restaurante y para gestionar los trabajadores
                    botónEditarRestaurante.gameObject.SetActive(true);
                    botónGestionarMesas.gameObject.SetActive(true);
                    botónGestionarTrabajadores.gameObject.SetActive(true);
                    break;
            }
        }
        else // El trabajador no está en ningún restaurante
        {
            botónEditarRestaurante.gameObject.SetActive(false);
            botónGestionarMesas.gameObject.SetActive(false);
            botónGestionarTrabajadores.gameObject.SetActive(false);
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

        Debug.Log("ID Usuario: " + Usuario.ID + ", Nombre Usuario: " + Usuario.Nombre + ", Rol_ID Usuario: " + Usuario.Rol_ID + ", Restaurante_ID Usuario: " + Usuario.Restaurante_ID);
    }

    private async Task ComprueboSiUserExisteAsync()
    {
        string cad = await instanceMétodosAPIController.GetDataAsync("trabajador/existe/" + Usuario.ID);
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
                instanceTrabajadorController.ObtenerDatosTrabajadorPorIdAsync(Usuario.ID);
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
        float velocidad = 1200f;
        while (rt.anchoredPosition.y > 193)
        {
            //Actualizo
            float yTelon = rt.anchoredPosition.y - velocidad * Time.deltaTime;

            // Pinto
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, yTelon);

            // Espero al siguiente frame antes de continuar. Más fluido que usar un WaitForSeconds(), ya que el movimiento no se basa en los FPS.
            yield return null;
        }
        
        telónMoviéndose = false;
        telónAbajo = true;
        botónCerrarSesión.interactable = true;
    }

    private IEnumerator MoverTelónHaciaArriba(RectTransform rt)
    {
        float velocidad = 1200f;
        while (rt.anchoredPosition.y < 1143)
        {
            //Actualizo
            float y = rt.anchoredPosition.y + velocidad * Time.deltaTime;

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

    public void IrALaEscenaGestionarMesas()
    {
        SceneManager.LoadScene("Gestionar Mesas");
    }

    public void IrALaEscenaGestionarTrabajadores()
    {
        SceneManager.LoadScene("Gestionar Trabajadores");
    }

    public void CerrarSesión()
    {
        PlayerPrefs.SetInt("UsuarioRegistrado", 0);
        
        // Pongo los botones de idiomas con una "Y" específica para el canvas de iniciar sesión
        rtBotónIdiomaSpanish.anchoredPosition = new Vector2(rtBotónIdiomaSpanish.anchoredPosition.x, rtTextoIniciarSesión.anchoredPosition.y);
        rtBotónIdiomaEnglish.anchoredPosition = new Vector2(rtBotónIdiomaEnglish.anchoredPosition.x, rtTextoIniciarSesión.anchoredPosition.y);

        canvasLogInUsuario.SetActive(true);
        canvasIdiomasLogInYRegistro.SetActive(true);
        PresionarBotónPerfil();
    }

    public void SalirDeLaApp()
    {
        Application.Quit();
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
    private bool OnWantsToQuitAsync() // Hacer que no funcione este método hasta que una parte del programa cargue que es la generación del lobby (creo, no estoy seguro)
    {
        Debug.Log("Interceptando Alt + F4 o cierre manual.");
        
        return true; // Unity cierra la aplicación automáticamente.
    }

    public void ActivarCanvasVídeoTutorial()
    {
        VideoPlayer vp = canvasVídeoTutorialApp.GetComponent<VideoPlayer>();


        if (textUserRol.text.Trim().Length > 0)
        {
            if (textUserRol.text.Contains("Empleado") || textUserRol.text.Contains("Employee"))
            {
                vp.clip = videoClipEmpleado;
            }
            else
            {
                vp.clip = videoClipGerente;
            }
        }
        
        canvasVídeoTutorialApp.SetActive(true);
        
        vp.Play();
    }

    public void DesactivarCanvasTutorialVídeo()
    {
        VideoPlayer vp = canvasVídeoTutorialApp.GetComponent<VideoPlayer>();
        vp.Stop();
        canvasVídeoTutorialApp.SetActive(false);

        imgBtnOnYPausa.sprite = imgPausa;
    }

    public void PausarODespausarVídeo()
    {
        VideoPlayer vp =  canvasVídeoTutorialApp.GetComponent<VideoPlayer>();
        if (imgBtnOnYPausa.sprite == imgPausa)
        {
            vp.Pause();
            imgBtnOnYPausa.sprite = imgPlay;
            return;
        }

        if (imgBtnOnYPausa.sprite == imgPlay)
        {
            vp.Play();
            imgBtnOnYPausa.sprite = imgPausa;
            return;
        }

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
