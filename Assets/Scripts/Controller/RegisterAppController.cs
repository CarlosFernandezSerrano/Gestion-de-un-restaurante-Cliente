using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;


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


    private bool esperandoEnError = false;
    private bool esperandoEnExito = false;

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

        if (botónConfirmar.IsInteractable())
        {
            // Detectar Enter principal
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ConfirmarRegistrarUsuario();
            }

            // Detectar Enter del teclado numérico
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                ConfirmarRegistrarUsuario();
            }
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
                                if (Usuario.Idioma.CompareTo("Español") == 0)
                                {
                                    textoErrorRegistro.text = "El nombre no puede tener espacios entre letras.";
                                }
                                else
                                {
                                    textoErrorRegistro.text = "The name cannot have spaces between letters.";
                                }
                                
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
                        if (Usuario.Idioma.CompareTo("Español") == 0)
                        {
                            textoErrorRegistro.text = "Nombre con menos de 3 caracteres.";
                        }
                        else
                        {
                            textoErrorRegistro.text = "Name with less than 3 characters.";
                        }
                    }
                }
                else
                {
                    Debug.Log("Contraseña con menos de 6 caracteres");
                    if (Usuario.Idioma.CompareTo("Español") == 0)
                    {
                        textoErrorRegistro.text = "Contraseña con menos de 6 caracteres.";
                    }
                    else
                    {
                        textoErrorRegistro.text = "Password with less than 6 characters.";
                    }
                }
            }
            else
            {
                Debug.Log("Contraseña diferente en ambos lados");
                if (Usuario.Idioma.CompareTo("Español") == 0)
                {
                    textoErrorRegistro.text = "Contraseña diferente en ambos lados.";
                }
                else
                {
                    textoErrorRegistro.text = "Different password on both sides.";
                }
            }
        }
        else
        {
            Debug.Log("El usuario no se puede registrar.");
            if (Usuario.Idioma.CompareTo("Español") == 0)
            {
                textoErrorRegistro.text = "Rellene todos los campos por favor.";
            }
            else
            {
                textoErrorRegistro.text = "Please fill in all fields.";
            }
        }
        Espero1SegundoYQuitoElTextoDeÉxitoOErrorAsync();
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
                    textoÉxitoRegistro.text = "Worker registered and successfully added to the restaurant";
                    instanceGestionarTrabajadoresController.AñadirTrabajadorARestaurante(username);
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
        Espero1SegundoYQuitoElTextoDeÉxitoOErrorAsync();
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

    private async void Espero1SegundoYQuitoElTextoDeÉxitoOErrorAsync()
    {
        if (textoErrorRegistro.text.Trim().Length > 0)
        {
            if (!esperandoEnError)
            {
                esperandoEnError = true;
                await Task.Delay(1500); // Espero 1 segundo sin bloquear

                textoErrorRegistro.text = "";

                esperandoEnError = false;
                return;
            }
            
        }

        if (textoÉxitoRegistro.text.Trim().Length > 0)
        {
            if (!esperandoEnExito)
            {
                esperandoEnExito = true;
                await Task.Delay(1500); // Espero 1 segundo sin bloquear

                textoÉxitoRegistro.text = "";

                esperandoEnExito = false;
                return;
            }
        }
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
