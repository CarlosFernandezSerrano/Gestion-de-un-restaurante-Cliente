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
    [SerializeField] private Button bot�nCerrarSesi�n;


    private bool tel�nMovi�ndose = false;
    private bool tel�nAbajo = false;

    M�todosAPIController instanceM�todosAPIController;
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
        instanceM�todosAPIController = M�todosAPIController.InstanceM�todosAPIController;
        instanceTrabajadorController = TrabajadorController.InstanceTrabajadorController;


        Gesti�nInicioDelProgramaAsync();

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private async void Gesti�nInicioDelProgramaAsync()
    {
        //PlayerPrefs.SetInt("UsuarioRegistrado", 0); // - - - Quitar esta l�nea cuando deje de hacer pruebas con el registro e inicio de sesi�n

        int usuarioRegistrado = PlayerPrefs.GetInt("UsuarioRegistrado", 0); // 1 es s�, 0 es no
        //Si el usuario no se ha registrado, le aparece el canvas de iniciar sesi�n
        if (usuarioRegistrado.Equals(0))
        {
            canvasLogInUsuario.SetActive(true);
            canvasIdiomasLogInYRegistro.SetActive(true);
        }
        else // Si el usuario ya est� registrado, compruebo si sigue en la BDD por si lo han eliminado y obtengo su rol_ID actualizado, por si el gerente se lo ha cambiado.
        {
           await ComprueboSiUserExisteAsync();
        }

        
        Debug.Log("ID Usuario: " + PlayerPrefs.GetInt("ID Usuario") + ", Nombre Usuario: " + PlayerPrefs.GetString("Nombre Usuario") + ", Rol_ID Usuario: " + PlayerPrefs.GetInt("Rol_ID Usuario") + ", Restaurante_ID Usuario: " + PlayerPrefs.GetInt("Restaurante_ID Usuario"));
    }

    private async Task ComprueboSiUserExisteAsync()
    {
        string cad = await instanceM�todosAPIController.GetDataAsync("trabajador/existe/" + PlayerPrefs.GetInt("ID Usuario"));
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


    public void PresionarBot�nPerfil()
    {
        RectTransform rt = medioYFinTelon.GetComponent<RectTransform>();

        // Si el tel�n no se mueve...
        if (!tel�nMovi�ndose)
        {
            tel�nMovi�ndose = true;
            bot�nCerrarSesi�n.interactable = false;
            // Y el tel�n no est� abajo, va para abajo
            if (!tel�nAbajo)
            {
                StartCoroutine(MoverTel�nHaciaAbajo(rt));
            }
            else // Y el tel�n est� abajo, va para arriba
            {
                StartCoroutine(MoverTel�nHaciaArriba(rt));
            }
        }
        
    }

    private IEnumerator MoverTel�nHaciaAbajo(RectTransform rt)
    {
        for (int i = 0; i < 950; i++)
        {
            //Actualizo
            float y = rt.anchoredPosition.y - 1;

            // Pinto
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);

            // Espero al siguiente frame antes de continuar. M�s fluido que usar un WaitForSeconds(), ya que el movimiento no se basa en los FPS.
            yield return null;
        }
        tel�nMovi�ndose = false;
        tel�nAbajo = true;
        bot�nCerrarSesi�n.interactable = true;
    }

    private IEnumerator MoverTel�nHaciaArriba(RectTransform rt)
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
        tel�nMovi�ndose = false;
        tel�nAbajo = false;
        bot�nCerrarSesi�n.interactable = true;
    }

    public void IrAlCanvasCrearRestaurante()
    {
        canvasCrearRestaurante.SetActive(true);
        PresionarBot�nPerfil();
    }

    public void CerrarSesi�n()
    {
        PlayerPrefs.SetInt("UsuarioRegistrado", 0);
        canvasLogInUsuario.SetActive(true);
        canvasIdiomasLogInYRegistro.SetActive(true);
        PresionarBot�nPerfil();
    }

    private void OnEnable()
    {
        Application.wantsToQuit += OnWantsToQuitAsync;
    }

    private void OnDisable()
    {
        Application.wantsToQuit -= OnWantsToQuitAsync;
    }

    // Intercepta el intento de cerrar la aplicaci�n (por ejemplo, con Alt + F4 o clic en el bot�n de cierre de la ventana).
    private bool OnWantsToQuitAsync() //Hacer que no funcione este m�todo hasta que una parte del programa cargue que es la generaci�n del lobby (creo, no estoy seguro)
    {
        Debug.Log("Interceptando Alt + F4 o cierre manual.");
        
        return true; // Unity cierra la aplicaci�n autom�ticamente.
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
