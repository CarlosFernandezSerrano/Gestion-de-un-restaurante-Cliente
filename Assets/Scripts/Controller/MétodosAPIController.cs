using Assets.Scripts.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class MétodosAPIController : MonoBehaviour
{

    private bool mostrandoImgNoConexión = false;
    private static string URL = "https://localhost:7233/"; //https://localhost:7233/
    //private static string URL = "https://servidorapirestaurante-production.up.railway.app/";
    GeneralController instanceGeneralController;

    public static MétodosAPIController InstanceMétodosAPIController { get; private set; }

    private void Awake()
    {
        if (InstanceMétodosAPIController == null)
        {
            InstanceMétodosAPIController = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        instanceGeneralController = GeneralController.InstanceGeneralController;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async Task<string> GetDataAsync(string cad)
    {
        string url = URL + cad;

        // Creo la solicitud GET
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // Añado el token en la cabecera Authorization
            request.SetRequestHeader("Authorization", "Bearer " + Usuario.Token);

            // Envio la solicitud
            var operation = request.SendWebRequest();

            // Esperamos a que termine sin bloquear el hilo principal
            while (!operation.isDone)
            {
                await Task.Yield(); // Permite que Unity siga funcionando mientras espera
            }

            // Si hubo un error, lo muestro y devuelvo null
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
                //Debug.LogError("Respuesta completa: " + request.downloadHandler.text);
                if (request.error.Contains("Cannot connect to destination host")) 
                {
                    Debug.Log("Servidor apagado.");
                    GeneralController.NoHayConexion = true;
                    if (!mostrandoImgNoConexión)
                    {
                        mostrandoImgNoConexión = true;
                        StartCoroutine(MostrarImgNoConexión());
                    }                    
                }
                return null;
            }

            // Obtengo la respuesta JSON
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("Respuesta GET en JSON: " + jsonResponse);

            QuitarImgNoConexiónSiLaHubiera();

            // Devuelvo la respuesta JSON
            return jsonResponse;
        }
    }

    private void QuitarImgNoConexiónSiLaHubiera()
    {
        try
        {
            instanceGeneralController.getImgNoConexión().SetActive(false);
        }
        catch (Exception ex)
        {
            Debug.Log("Exception: " + ex);
        }
                
    }

    private IEnumerator MostrarImgNoConexión()
    {
        for (int i = 0; i < 3; i++)
        {
            // Hago aparecer lentamente la imagen de "no hay conexión".
            yield return StartCoroutine(instanceGeneralController.HacerAparecerLentamente(instanceGeneralController.getImgNoConexión(), 1f));

            // La tercera vez que muestro la imagen, ya dejo la imagen puesta y no la quito.
            if (i < 2) 
            {
                if (!GeneralController.NoHayConexion)
                {
                    break;
                }

                // Espero 2 segundos para que se vea bien la imagen
                yield return new WaitForSeconds(2f);

                if (!GeneralController.NoHayConexion)
                {
                    break;
                }

                // Hago desaparecer lentamente la imagen de "no hay conexión".
                yield return StartCoroutine(instanceGeneralController.HacerDesaparecerLentamente(instanceGeneralController.getImgNoConexión(), 1f));
            }
            
        }
        mostrandoImgNoConexión = false;
    }

    public async Task<string> PostDataAsync(string cad, object objeto)
    {
        string url = URL + cad;

        // Convierto el objeto a JSON
        string json = JsonConvert.SerializeObject(objeto);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
        Debug.Log("JSON enviado: " + json);

        // Creo la solicitud POST
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Añado el token en la cabecera Authorization
            request.SetRequestHeader("Authorization", "Bearer " + Usuario.Token);

            // Enviamos la solicitud
            var operation = request.SendWebRequest();

            // Espero sin bloquear el hilo principal
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            // Verifico si hubo un error
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
                Debug.LogError(json);
                return null;
            }

            // Obtengo la respuesta JSON
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("Respuesta POST en JSON: " + jsonResponse);

            return jsonResponse;
        }
    }

    public async Task<string> PutDataAsync(string cad, object objeto)
    {
        string url = URL + cad;

        // Convierto el objeto a JSON
        string json = JsonConvert.SerializeObject(objeto);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
        Debug.Log("JSON enviado: " + json);

        // Creo la solicitud POST
        using (UnityWebRequest request = new UnityWebRequest(url, "PUT"))
        {
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Añado el token en la cabecera Authorization
            request.SetRequestHeader("Authorization", "Bearer " + Usuario.Token);

            // Enviamos la solicitud
            var operation = request.SendWebRequest();

            // Espero sin bloquear el hilo principal
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            // Verifico si hubo un error
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
                return null;
            }

            // Obtengo la respuesta JSON
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("Respuesta PUT en JSON: " + jsonResponse);

            return jsonResponse;
        }
    }

    public async Task<string> DeleteDataAsync(string cad)
    {
        string url = URL + cad;

        // Usamos el método Delete que ya configura la petición DELETE sin cuerpo.
        using (UnityWebRequest request = UnityWebRequest.Delete(url))
        {
            // Asignamos un DownloadHandler para poder recibir respuesta
            request.downloadHandler = new DownloadHandlerBuffer();

            // Añado el token en la cabecera Authorization
            request.SetRequestHeader("Authorization", "Bearer " + Usuario.Token);

            // Enviamos la solicitud de forma asíncrona
            var operation = request.SendWebRequest();

            // Esperamos sin bloquear el hilo principal
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            // Verificamos si hubo error en la petición
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
                return null;
            }

            // Obtenemos la respuesta del servidor
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("Respuesta DELETE en JSON: " + jsonResponse);

            return jsonResponse;
        }
    }
    /*public IEnumerator GetData(string cad)
    {
        string url = "https://localhost:7233/" + cad; // Ajusta la URL según tu configuración
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
                Debug.Log("Respuesta GET en JSON: " + jsonResponse);
                //Cliente cliente = JsonConvert.DeserializeObject<Cliente>(jsonResponse);
                respuestaGET = jsonResponse;
            }
        }
    }

    public IEnumerator PostData(string cad, object objeto)
    {
        string url = "https://localhost:7233/"+cad;

        // Convierto el objeto a JSON
        string json = JsonConvert.SerializeObject(objeto);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
        Debug.Log("JSON: " + json);
        // Configuro la petición
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Envio y espero
            yield return request.SendWebRequest();

            // Manejo la respuesta
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: "+request.error);
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Respuesta POST en JSON: " + jsonResponse);
                respuestaPOST = jsonResponse;                
            }
        }
    }*/
    
}
