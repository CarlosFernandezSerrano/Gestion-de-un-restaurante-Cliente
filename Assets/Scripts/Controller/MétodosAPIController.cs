using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class MétodosAPIController : MonoBehaviour
{

    public static MétodosAPIController instanceMétodosAPIController { get; private set; }

    private void Awake()
    {
        if (instanceMétodosAPIController == null)
        {
            instanceMétodosAPIController = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async Task<string> GetDataAsync(string cad)
    {
        string url = "https://localhost:7233/" + cad;

        // Creo la solicitud GET
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
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
                return null;
            }

            // Obtengo la respuesta JSON
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("Respuesta GET en JSON: " + jsonResponse);

            // Devuelvo la respuesta JSON
            return jsonResponse;
        }
    }

    public async Task<string> PostDataAsync(string cad, object objeto)
    {
        string url = "https://localhost:7233/" + cad;

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
            Debug.Log("Respuesta POST en JSON: " + jsonResponse);

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
