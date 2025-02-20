using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class RegisterAppController : MonoBehaviour
{
    [SerializeField] private GameObject canvasRegistroUsuario;
    [SerializeField] private GameObject canvasIniciarSesiónUsuario;
    [SerializeField] private TMP_Text textoErrorRegistro;
    [SerializeField] private TMP_Text textoÉxitoRegistro;
    [SerializeField] private TMP_InputField inputTextoNombre;
    [SerializeField] private TMP_InputField inputTextoContraseña;
    [SerializeField] private TMP_InputField inputTextoContraseñaRepetida;
    [SerializeField] private Button botónConfirmar;
    [SerializeField] private Button botónIniciarSesión;
    [SerializeField] private TMP_InputField[] inputFields; // Asigno los InputFields en el orden de tabulación deseado


    private bool esHombre = false;
    private bool mensajeAPIPlayFabDevuelto = false;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SelectNextInputField();
        }
    }

    
    public void cambiarACanvasIniciarSesión()
    {
        canvasRegistroUsuario.SetActive(false);
        canvasIniciarSesiónUsuario.SetActive(true);

        textoErrorRegistro.text = "";

        //Dejo vacíos los campos por si se vuelve a ver este canvas
        inputTextoNombre.text = "";
        inputTextoContraseña.text = "";
        inputTextoContraseñaRepetida.text = "";
    }

    public void confirmarRegistroUsuario()
    {
        textoErrorRegistro.text = "";
        StartCoroutine(desactivarPorUnTiempoLosBotonesYLuegoActivarCuandoHayaRespuestaDeLaAPIdePlayFab());

        string textoNombre = inputTextoNombre.text.Trim();
        string textoPassword = inputTextoContraseña.text.Trim();
        string textoRepeatedPassword = inputTextoContraseñaRepetida.text.Trim();

        //Si se han rellenado todos los huecos (InputFields) pasa por aquí
        if (!string.IsNullOrEmpty(textoNombre) && !string.IsNullOrEmpty(textoPassword) && !string.IsNullOrEmpty(textoRepeatedPassword))
        {
            Debug.Log("El usuario se puede registrar");

            //La contraseña repetida es igual a la contraseña 
            if (textoPassword.CompareTo(textoRepeatedPassword) == 0)
            {
                Debug.Log("Contraseña igual en ambos lados.");

                //La contraseña tiene más de 5 caracteres
                if (textoPassword.Length > 5)
                {
                    Debug.Log("Contraseña con buena cantidad de caracteres");

                    //El nombre tiene más de 2 caracteres
                    if (textoNombre.Length > 2)
                    {
                        Debug.Log("Nombre con buena cantidad de caracteres");

                        // El nombre no tiene la ñ
                        if (!textoNombre.Contains("ñ") && !textoNombre.Contains("Ñ"))
                        {
                            Debug.Log("Nombre sin ñ");

                            //El nombre no tiene espacios entre letras
                            if (!textoNombre.Contains(" "))
                            {
                                Debug.Log("El nombre no tiene espacios entre letras");                                
                                RegisterUser(textoNombre, textoPassword);                                
                            }
                            else
                            {
                                Debug.Log("El nombre no puede tener espacios entre letras");
                                textoErrorRegistro.text = "El nombre no puede tener espacios entre letras.";

                                mensajeAPIPlayFabDevuelto = true; //No accede a la API en este punto, lo pongo como si accediese. Pero necesito activar esta variable aquí para que funcione bien el resto del código, en vez de crear 2 variables
                            }

                        }
                        else
                        {
                            Debug.Log("Nombre con ñ");
                            textoErrorRegistro.text = "El nombre no debe tener ñ.";

                            mensajeAPIPlayFabDevuelto = true; //No accede a la API en este punto, lo pongo como si accediese. Pero necesito activar esta variable aquí para que funcione bien el resto del código, en vez de crear 2 variables
                        }

                    }
                    else
                    {
                        Debug.Log("Nombre con menos de 3 caracteres");
                        textoErrorRegistro.text = "Nombre con menos de 3 caracteres.";

                        mensajeAPIPlayFabDevuelto = true; //No accede a la API en este punto, lo pongo como si accediese. Pero necesito activar esta variable aquí para que funcione bien el resto del código, en vez de crear 2 variables
                    }

                }
                else
                {
                    Debug.Log("Contraseña con menos de 6 caracteres");
                    textoErrorRegistro.text = "Contraseña con menos de 6 caracteres.";

                    mensajeAPIPlayFabDevuelto = true; //No accede a la API en este punto, lo pongo como si accediese. Pero necesito activar esta variable aquí para que funcione bien el resto del código, en vez de crear 2 variables
                }

            }
            else
            {
                Debug.Log("Contraseña diferente en ambos lados");
                textoErrorRegistro.text = "Contraseña diferente en ambos lados.";

                mensajeAPIPlayFabDevuelto = true; //No accede a la API en este punto, lo pongo como si accediese. Pero necesito activar esta variable aquí para que funcione bien el resto del código, en vez de crear 2 variables
            }
        }
        else
        {
            Debug.Log("El usuario no se puede registrar.");
            textoErrorRegistro.text = "Rellene todos los campos por favor.";

            mensajeAPIPlayFabDevuelto = true; //No accede a la API en este punto, lo pongo como si accediese. Pero necesito activar esta variable aquí para que funcione bien el resto del código, en vez de crear 2 variables
        }
    }

    private IEnumerator desactivarPorUnTiempoLosBotonesYLuegoActivarCuandoHayaRespuestaDeLaAPIdePlayFab()
    {
        mensajeAPIPlayFabDevuelto = false;

        botónIniciarSesión.interactable = false;
        botónConfirmar.interactable = false;

        //yield return new WaitUntil(() => mensajeAPIPlayFabDevuelto);
        yield return new WaitForSeconds(2f);

        botónIniciarSesión.interactable = true;
        botónConfirmar.interactable = true;
    }


    // Método para registrar al usuario
    public void RegisterUser(string username, string password)
    {
        // Crear la solicitud de registro
        Debug.Log("El usuario trata de registrarse");
        
    }

    // Método llamado cuando el registro es exitoso
    private void OnRegisterSuccess()
    {
        Debug.Log("Usuario registrado exitosamente");

        // Aquí puedes proceder al siguiente paso, como iniciar sesión automáticamente o cambiar de escena
        textoErrorRegistro.text = "";
        textoÉxitoRegistro.text = "Usuario registrado con éxito.";
        StartCoroutine(finRegistroUsuario());
        //PlayerPrefs.SetInt("UsuarioRegistrado", 1); //Ya está registrado el usuario

        string nombreUsuario = inputTextoNombre.text.Trim();
        string contraseñaUsuario = inputTextoContraseña.text.Trim();

        //Guardo estas strings en estos PlayerPrefs para usar futuramente en el modo multijugador
        PlayerPrefs.SetString("Nombre Usuario", nombreUsuario);
        //No creo que haya peligro que se guarde en este PlayerPrefs la contraseña del usuario, ya que sólo podría guardarse en la App de la persona donde inició sesión.
        PlayerPrefs.SetString("Contraseña Usuario", contraseñaUsuario);

    }

    private IEnumerator finRegistroUsuario()
    {
        yield return new WaitForSeconds(1f); //1f = duración que espera antes de ocultar el canvas y así mostrar el mensaje de "Usuario registrado con éxito" un poco.

        textoÉxitoRegistro.text = "";
        textoErrorRegistro.text = "";
        canvasRegistroUsuario.SetActive(false);

        //Dejo vacíos los campos por si se vuelve a ver este canvas
        inputTextoNombre.text = "";
        inputTextoContraseña.text = "";
        inputTextoContraseñaRepetida.text = "";

        mensajeAPIPlayFabDevuelto = true;

        PlayerPrefs.SetInt("UsuarioRegistrado", 1);
    }

    // Método llamado cuando el registro falla
    private void OnRegisterFailure(string error)
    {
        Debug.LogError("Error al registrar el usuario: " + error);

        // Verificar el tipo de error
        if (error == "")
        {
            textoErrorRegistro.text = "El nombre de usuario ya está en uso. Por favor, elija otro nombre.";
            mensajeAPIPlayFabDevuelto = true;
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
