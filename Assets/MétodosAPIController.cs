using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class MétodosAPIController : MonoBehaviour
{

    public string respuestaGET { get; set; }
    public string respuestaPOST { get; set; }
    public string respuestaPUT { get; set; }
    public string respuestaDELETE { get; set; }

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

    public IEnumerator GetData(string cad)
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
                Debug.Log("Respuesta 1: " + jsonResponse);
                //Cliente cliente = JsonConvert.DeserializeObject<Cliente>(jsonResponse);
                Debug.Log("Deserializado 1: ");
                // Deserializa el JSON a una lista de objetos Cliente
                List<Cliente> listaClientes = JsonConvert.DeserializeObject<List<Cliente>>(jsonResponse);
                Debug.Log("Clientes:");
                foreach (Cliente cliente in listaClientes)
                {
                    Debug.Log(cliente.mostrar());
                }
            }
        }
    }

    public IEnumerator PostData(string cad, Cliente cliente)
    {
        string url = "https://localhost:7233/"+cad; // Ajusta la URL según tu configuración

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
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Respuesta POST en JSON: " + jsonResponse);
                Debug.Log($"Respuesta: {request.downloadHandler.text}");
                respuestaPOST = jsonResponse;
                // Si quieres deserializar:
                Cliente cliente2 = JsonConvert.DeserializeObject<Cliente>(jsonResponse);
                Debug.Log("Deserializado JSON en POST: " + cliente2.mostrar());

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
