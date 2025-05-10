using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;
using Newtonsoft.Json.Linq;


public class RegisterAppController : MonoBehaviour
{
    [SerializeField] private GameObject canvasRegistroUsuario;
    [SerializeField] private TMP_Text textoErrorRegistro;
    [SerializeField] private TMP_Text textoÉxitoRegistro;
    [SerializeField] private TMP_InputField inputTextoNombre;
    [SerializeField] private TMP_InputField inputTextoContraseña;
    [SerializeField] private TMP_InputField inputTextoContraseñaRepetida;
    [SerializeField] private Button botónConfirmar;
    [SerializeField] private TMP_InputField[] inputFields; // Asigno los InputFields en el orden de tabulación deseado


    MétodosAPIController instanceMétodosAPIController;
    TrabajadorController instanceTrabajadorController;
    GestionarTrabajadoresController instanceGestionarTrabajadoresController;

    // Start is called before the first frame update
    void Start()
    {
        instanceMétodosAPIController = MétodosAPIController.InstanceMétodosAPIController;
        instanceTrabajadorController = TrabajadorController.InstanceTrabajadorController;
        instanceGestionarTrabajadoresController = GestionarTrabajadoresController.InstanceGestionarTrabajadoresController;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SelectNextInputField();
        }
    }

    
    public void DesactivarCanvasRegistrarUsuario()
    {
        canvasRegistroUsuario.SetActive(false);

        textoErrorRegistro.text = "";
        textoÉxitoRegistro.text = "";

        //Dejo vacíos los campos por si se vuelve a ver este canvas
        inputTextoNombre.text = "";
        inputTextoContraseña.text = "";
        inputTextoContraseñaRepetida.text = "";
    }

    public void ConfirmarRegistrarUsuario()
    {
        textoErrorRegistro.text = "";
        StartCoroutine(DesactivarPorUnTiempoLosBotonesYLuegoActivarCuandoHayaRespuestaDeLaAPIdePlayFab());

        string textoNombre = inputTextoNombre.text.Trim();
        string textoPassword = inputTextoContraseña.text.Trim();
        string textoRepeatedPassword = inputTextoContraseñaRepetida.text.Trim();

        //Si se han rellenado todos los huecos (InputFields) pasa por aquí
        if (!string.IsNullOrEmpty(textoNombre) && !string.IsNullOrEmpty(textoPassword) && !string.IsNullOrEmpty(textoRepeatedPassword))
        {
            // La contraseña es igual en ambos lados. 
            if (textoPassword.CompareTo(textoRepeatedPassword) == 0)
            {
                // La contraseña tiene más de 5 caracteres
                if (textoPassword.Length > 5)
                {
                    // El nombre tiene más de 2 caracteres
                    if (textoNombre.Length > 2)
                    {
                        // El nombre no tiene la ñ
                        //if (!textoNombre.Contains("ñ") && !textoNombre.Contains("Ñ"))
                        //{
                            Debug.Log("Nombre sin ñ");

                            // El nombre no tiene espacios entre letras
                            if (!textoNombre.Contains(" "))
                            {
                               RegisterUserAsync(textoNombre, textoPassword);     
                            }
                            else
                            {
                                Debug.Log("El nombre no puede tener espacios entre letras");
                                textoErrorRegistro.text = "El nombre no puede tener espacios entre letras.";
                            }
                        /*}
                        else
                        {
                            Debug.Log("Nombre con ñ");
                            textoErrorRegistro.text = "El nombre no debe tener ñ.";
                        }*/
                    }
                    else
                    {
                        Debug.Log("Nombre con menos de 3 caracteres");
                        textoErrorRegistro.text = "Nombre con menos de 3 caracteres.";
                    }
                }
                else
                {
                    Debug.Log("Contraseña con menos de 6 caracteres");
                    textoErrorRegistro.text = "Contraseña con menos de 6 caracteres.";
                }
            }
            else
            {
                Debug.Log("Contraseña diferente en ambos lados");
                textoErrorRegistro.text = "Contraseña diferente en ambos lados.";
            }
        }
        else
        {
            Debug.Log("El usuario no se puede registrar.");
            textoErrorRegistro.text = "Rellene todos los campos por favor.";
        }
    }

    private IEnumerator DesactivarPorUnTiempoLosBotonesYLuegoActivarCuandoHayaRespuestaDeLaAPIdePlayFab()
    {
        botónConfirmar.interactable = false;

        //yield return new WaitUntil(() => mensajeAPIPlayFabDevuelto);
        yield return new WaitForSeconds(1.5f);

        botónConfirmar.interactable = true;
    }


    // Método para registrar al usuario
    public async void RegisterUserAsync(string username, string password)
    {
        // Creo la solicitud de registro
        Debug.Log("El usuario trata de registrarse");
        Trabajador t = new Trabajador(username, password, 0, 0);
        string cad = await instanceMétodosAPIController.PostDataAsync("trabajador/registrarUser", t);

        // Deserializo la respuesta
        JObject jsonObject = JObject.Parse(cad);
        int resultValue = jsonObject["result"].Value<int>();

        switch (resultValue)
        {
            case 0:
                if (Usuario.Idioma.CompareTo("Español") == 0 || Usuario.Idioma == null)
                {
                    textoErrorRegistro.text = "Error inesperado";
                }
                else
                {
                    textoErrorRegistro.text = "Unexpected error";
                }
                break;
            case 1:
                textoErrorRegistro.text = "";

                if (Usuario.Idioma.CompareTo("Español") == 0 || Usuario.Idioma == null)
                {
                    textoÉxitoRegistro.text = "Trabajador registrado y añadido correctamente al restaurante";
                    instanceGestionarTrabajadoresController.AñadirTrabajadorARestaurante(username);
                }
                else
                {
                    textoÉxitoRegistro.text = "Worker registered correctly";
                }
                DejarVacíosLosCampos();
                break;
            case 2:
                if (Usuario.Idioma.CompareTo("Español") == 0 || Usuario.Idioma == null)
                {
                    textoErrorRegistro.text = "El usuario " + username + " ya existe";
                }
                else
                {
                    textoErrorRegistro.text = "The user " + username + " already exists";
                }
                break;            
        }
    }

    
    private void DejarVacíosLosCampos()
    {
        //textoÉxitoRegistro.text = "";
        textoErrorRegistro.text = "";
        
        // Dejo vacíos los campos
        inputTextoNombre.text = "";
        inputTextoContraseña.text = "";
        inputTextoContraseñaRepetida.text = "";
    }


    /// <summary>
    ///  Método para cambiar de componente con TAB en la interfaz gráfica.
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
}
