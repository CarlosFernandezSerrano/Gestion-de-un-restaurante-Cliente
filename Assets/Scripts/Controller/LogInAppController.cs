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

    Cliente instanceCliente;

    // Start is called before the first frame update
    void Start()
    {
        instanceCliente = Cliente.instanceCiente;

        Debug.Log("A");
        StartCoroutine(GetData("listar"));
        Cliente cliente = new Cliente("2","Carlos Fernandez","30", "juan@example.com");
        StartCoroutine(PostData(cliente));
        StartCoroutine(DeleteData());
        Debug.Log("B");
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

        LoginUser(username, password);
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
    public void LoginUser(string username, string password)
    {
        // Crear la solicitud de inicio de sesión
        

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

    public IEnumerator GetData(string cad)
    {
        string url = "https://localhost:7233/cliente/"+cad; // Ajusta la URL según tu configuración
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error: " + request.error);
            }
            else
            {
                // Procesa la respuesta, que generalmente es JSON
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Respuesta: " + jsonResponse);
                // Deserializa el JSON a un objeto Persona
                //Cliente cliente = JsonConvert.DeserializeObject<Cliente>(jsonResponse);
                //Debug.Log("Cliente: " + cliente.mostrar());
                // Puedes deserializar con JsonUtility o alguna otra librería JSON
            }
        }
    }

    public IEnumerator PostData(Cliente cliente)
    {
        string url = "https://localhost:7233/cliente/guardar"; // Ajusta la URL según tu configuración

        // 2. Crear el objeto cliente
        /*Cliente cliente = new Cliente
        {
            nombre = "Ana García",
            edad = "28",
            correo = "ana@ejemplo.com"
        };*/

        // 3. Convertir a JSON
        //string json = cliente.toJSONString();
        string json = JsonConvert.SerializeObject(cliente);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
        Debug.Log("Cliente: " + cliente.mostrar());
        Debug.Log("JSON: " + json);
        // 4. Configurar la petición
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 5. Enviar y esperar
            yield return request.SendWebRequest();

            // 6. Manejar respuesta
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {request.error}");
            }
            else
            {
                Debug.Log($"Respuesta: {request.downloadHandler.text}");
                // Si quieres deserializar:

            }
        }
    }

    public IEnumerator DeleteData()
    {
        string url = "https://localhost:7233/cliente/borrarxid/3"; // Ajusta la URL según tu configuración
        using (UnityWebRequest request = UnityWebRequest.Delete(url))
        {
            // Asigna un downloadHandler para poder capturar la respuesta del servidor
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error: " + request.error);
            }
            else
            {
                // Procesa la respuesta, que generalmente es JSON
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Respuesta: " + jsonResponse);
                // Puedes deserializar con JsonUtility o alguna otra librería JSON
            }
        }
    }


    
}
