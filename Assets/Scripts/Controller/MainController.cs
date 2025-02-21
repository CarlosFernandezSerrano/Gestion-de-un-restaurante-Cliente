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

    const string IP_SERVER = "127.0.0.1";
    const int PUERTO = 50001;

    private bool conectadoAlServer = false;
    private bool conectandoAlServer = false;
    private bool finCliente = false;

    TcpClient client;
    NetworkStream stream;

    

    // Start is called before the first frame update
    void Start()
    {
        int usuarioRegistrado = PlayerPrefs.GetInt("UsuarioRegistrado", 0); // 1 es sí, 0 es no
        //Si el usuario no se ha registrado, le aparece el canvas de iniciar sesión
        if (usuarioRegistrado.Equals(0))
        {
            canvasLogInUsuario.SetActive(true);
        }

        GestionarCliente();
        
        


        
        //recibirUnMensajeDelServidorAsync();


        //client.Close();

    }

    private async void GestionarCliente()
    {
        do
        {
            if (client != null && stream != null)
            {
                Debug.Log("A");
                string mensaje = ""+await LeerMensajeDelServidor();
                //StartCoroutine(esperarARecibirElMensajeYGestionarlo());
                Debug.Log("Mensaje SERVER: " + mensaje);
                if (mensaje.Trim() != "" && mensaje != null)
                {
                    GestionarMensajeDelServidor(mensaje);
                }
                
            }
            else
            {
                if (!conectandoAlServer && !conectadoAlServer)
                {
                    conectandoAlServer = true;
                    Debug.Log("B");
                    ConectarAlServidorAsync();
                }
                
            }
            await Task.Delay(1000); // Espera x milisegundos
        } while (!finCliente);
        
    }

    private async void ConectarAlServidorAsync()
    {
        // Creo el TcpClient
        client = new TcpClient();
        do
        {
            try
            {
                // El cliente intenta conectar de forma asíncrona al servidor (IP y puerto)
                await client.ConnectAsync(IP_SERVER, PUERTO);

                // Verificamos si la conexión fue exitosa
                if (client.Connected)
                {
                    // Obtenemos el NetworkStream para la comunicación
                    stream = client.GetStream(); //"stream" es el canal a través del cual se envían y reciben datos por la red. Es decir, una vez que el cliente 
                                                 //se conecta a un servidor, el NetworkStream permite leer y escribir información en esa conexión.
                    Debug.Log("Conexión al servidor exitosa.");
                    conectadoAlServer = true;
                    conectandoAlServer = false; enviarUnMensajeAlServidorAsync("1:Hola server, soy el cliente1");
                    break; // Salimos del bucle si la conexión es exitosa
                }
                else
                {
                    Debug.Log("No se pudo conectar al servidor.");
                }
            }
            catch (Exception ex)
            {
                // Capturamos cualquier excepción y la mostramos
                Debug.Log("Error al intentar conectar al servidor: " + ex.Message);
            }
        } while (!conectadoAlServer);
        
    }



    // Update is called once per frame
    void Update()
    {
        
    }


    private void GestionarMensajeDelServidor(string mensaje)
    {
        string[] cadServer = mensaje.Trim().Split(":");
        Debug.Log("Mensaje del server: " + mensaje);
        int numMensaje = int.Parse(cadServer[0]);

        switch (numMensaje)
        {
            case IProtocolo.BIENVENIDA:
                Debug.Log("Llega aquí");
                //enviarUnMensajeAlServidorAsync(""+IProtocolo.BIENVENIDA + ":" + "Hola servidor, ¿qué tal?");
                //enviarUnMensajeAlServidorAsync("1:Hola server, soy el cliente2");
                Debug.Log("Consigue llegar aquí");
                break;
            case 3:

                break;
            default:
                break;
        }
    }
    

    private async void enviarUnMensajeAlServidorAsync(string cad)
    {
        // Enviar un mensaje al servidor
        string message = cad;
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        await stream.WriteAsync(messageBytes, 0, messageBytes.Length);
    }

    private async Task<string> LeerMensajeDelServidor()
    {
        // Leer la respuesta del servidor
        byte[] buffer = new byte[1024];
        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Debug.Log("Respuesta del servidor: " + response);
        return response;
    }

}
