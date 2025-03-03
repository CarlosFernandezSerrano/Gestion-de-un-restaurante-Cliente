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


public class MainController : MonoBehaviour, IProtocolo
{
    [SerializeField] private GameObject canvasLogInUsuario;

    

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("UsuarioRegistrado", 0); //Quitar esta l�nea cuando deje de hacer pruebas con el registro e inicio de sesi�n

        int usuarioRegistrado = PlayerPrefs.GetInt("UsuarioRegistrado", 0); // 1 es s�, 0 es no
        //Si el usuario no se ha registrado, le aparece el canvas de iniciar sesi�n
        if (usuarioRegistrado.Equals(0))
        {
            canvasLogInUsuario.SetActive(true);
        }
        
        Debug.Log("ID Usuario: " + PlayerPrefs.GetInt("ID Usuario") + ", Nombre Usuario: " + PlayerPrefs.GetString("Nombre Usuario") + ", Rol Usuario: " + PlayerPrefs.GetInt("Rol Usuario"));
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
