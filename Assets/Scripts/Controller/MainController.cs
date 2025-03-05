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


public class MainController : MonoBehaviour, IProtocolo
{
    [SerializeField] private GameObject canvasLogInUsuario;
    [SerializeField] private GameObject canvasIdiomasLogInYRegistro;
    [SerializeField] private GameObject medioYFinTelon;
 

    MétodosAPIController instanceMétodosAPIController;
    TrabajadorController instanceTrabajadorController;

    void Awake()
    {
        SceneManager.LoadSceneAsync("General Controller", LoadSceneMode.Additive);
    }

    // Start is called before the first frame update
    void Start()
    {
        instanceMétodosAPIController = MétodosAPIController.instanceMétodosAPIController;
        instanceTrabajadorController = TrabajadorController.instanceTrabajadorController;

        //PlayerPrefs.SetInt("UsuarioRegistrado", 0); //Quitar esta línea cuando deje de hacer pruebas con el registro e inicio de sesión

        int usuarioRegistrado = PlayerPrefs.GetInt("UsuarioRegistrado", 0); // 1 es sí, 0 es no
        //Si el usuario no se ha registrado, le aparece el canvas de iniciar sesión
        if (usuarioRegistrado.Equals(0))
        {
            canvasIdiomasLogInYRegistro.SetActive(true);
            canvasLogInUsuario.SetActive(true);
        }
        else // Si el usuario ya está registrado, compruebo si sigue en la BDD por si lo han eliminado y obtengo su rol_ID actualizado, por si el gerente se lo ha cambiado.
        {
            ComprueboSiUserExiste();
        }

        Debug.Log("ID Usuario: " + PlayerPrefs.GetInt("ID Usuario") + ", Nombre Usuario: " + PlayerPrefs.GetString("Nombre Usuario") + ", Rol_ID Usuario: " + PlayerPrefs.GetInt("Rol_ID Usuario") + ", Restaurante_ID Usuario: " + PlayerPrefs.GetInt("Restaurante_ID Usuario"));
    }

    private async void ComprueboSiUserExiste()
    {
        string cad = await instanceMétodosAPIController.GetDataAsync("trabajador/existe/" + PlayerPrefs.GetInt("ID Usuario"));
        Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);
        // El trabajador no existe. Ha sido eliminado de la BDD y tiene que volver a registrarse.
        if (resultado.Result.Equals(0)) 
        {
            PlayerPrefs.SetInt("UsuarioRegistrado", 0);
            PlayerPrefs.Save();
            canvasIdiomasLogInYRegistro.SetActive(true);
            canvasLogInUsuario.SetActive(true);
        }
        else // El trabajdor existe y obtengo sus datos por si ha tenido cambios. Ejemplo: le han puesto un rol distinto o le han agregado a un restaurante.
        {
            instanceTrabajadorController.ObtenerDatosTrabajadorPorIdAsync(PlayerPrefs.GetInt("ID Usuario"));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }    



    public void PresionarBotónPerfil()
    {
        RectTransform rt = medioYFinTelon.GetComponent<RectTransform>();

        StartCoroutine(MoverTelónHaciaAbajo(rt));

        //StartCoroutine(MoverTelónHaciaArriba(rt));
        

        
    }

    private IEnumerator MoverTelónHaciaArriba(RectTransform rt)
    {
        throw new NotImplementedException();
    }

    private IEnumerator MoverTelónHaciaAbajo(RectTransform rt)
    {
        for (int i = 0; i < 900; i++)
        {
            //Actualizo
            float y = rt.anchoredPosition.y - 1;

            // Pinto
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);

            // Espero
            yield return new WaitForSeconds(0.005f);
        }
        
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
}
