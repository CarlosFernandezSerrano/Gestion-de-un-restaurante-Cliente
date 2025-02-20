using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class RegisterAppController : MonoBehaviour
{
    [SerializeField] private GameObject canvasRegistroUsuario;
    [SerializeField] private GameObject canvasIniciarSesi�nUsuario;
    [SerializeField] private TMP_Text textoErrorRegistro;
    [SerializeField] private TMP_Text texto�xitoRegistro;
    [SerializeField] private TMP_InputField inputTextoNombre;
    [SerializeField] private TMP_InputField inputTextoContrase�a;
    [SerializeField] private TMP_InputField inputTextoContrase�aRepetida;
    [SerializeField] private Button bot�nConfirmar;
    [SerializeField] private Button bot�nIniciarSesi�n;
    [SerializeField] private TMP_InputField[] inputFields; // Asigno los InputFields en el orden de tabulaci�n deseado


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

    
    public void cambiarACanvasIniciarSesi�n()
    {
        canvasRegistroUsuario.SetActive(false);
        canvasIniciarSesi�nUsuario.SetActive(true);

        textoErrorRegistro.text = "";

        //Dejo vac�os los campos por si se vuelve a ver este canvas
        inputTextoNombre.text = "";
        inputTextoContrase�a.text = "";
        inputTextoContrase�aRepetida.text = "";
    }

    public void confirmarRegistroUsuario()
    {
        textoErrorRegistro.text = "";
        StartCoroutine(desactivarPorUnTiempoLosBotonesYLuegoActivarCuandoHayaRespuestaDeLaAPIdePlayFab());

        string textoNombre = inputTextoNombre.text.Trim();
        string textoPassword = inputTextoContrase�a.text.Trim();
        string textoRepeatedPassword = inputTextoContrase�aRepetida.text.Trim();

        //Si se han rellenado todos los huecos (InputFields) pasa por aqu�
        if (!string.IsNullOrEmpty(textoNombre) && !string.IsNullOrEmpty(textoPassword) && !string.IsNullOrEmpty(textoRepeatedPassword))
        {
            Debug.Log("El usuario se puede registrar");

            //La contrase�a repetida es igual a la contrase�a 
            if (textoPassword.CompareTo(textoRepeatedPassword) == 0)
            {
                Debug.Log("Contrase�a igual en ambos lados.");

                //La contrase�a tiene m�s de 5 caracteres
                if (textoPassword.Length > 5)
                {
                    Debug.Log("Contrase�a con buena cantidad de caracteres");

                    //El nombre tiene m�s de 2 caracteres
                    if (textoNombre.Length > 2)
                    {
                        Debug.Log("Nombre con buena cantidad de caracteres");

                        // El nombre no tiene la �
                        if (!textoNombre.Contains("�") && !textoNombre.Contains("�"))
                        {
                            Debug.Log("Nombre sin �");

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

                                mensajeAPIPlayFabDevuelto = true; //No accede a la API en este punto, lo pongo como si accediese. Pero necesito activar esta variable aqu� para que funcione bien el resto del c�digo, en vez de crear 2 variables
                            }

                        }
                        else
                        {
                            Debug.Log("Nombre con �");
                            textoErrorRegistro.text = "El nombre no debe tener �.";

                            mensajeAPIPlayFabDevuelto = true; //No accede a la API en este punto, lo pongo como si accediese. Pero necesito activar esta variable aqu� para que funcione bien el resto del c�digo, en vez de crear 2 variables
                        }

                    }
                    else
                    {
                        Debug.Log("Nombre con menos de 3 caracteres");
                        textoErrorRegistro.text = "Nombre con menos de 3 caracteres.";

                        mensajeAPIPlayFabDevuelto = true; //No accede a la API en este punto, lo pongo como si accediese. Pero necesito activar esta variable aqu� para que funcione bien el resto del c�digo, en vez de crear 2 variables
                    }

                }
                else
                {
                    Debug.Log("Contrase�a con menos de 6 caracteres");
                    textoErrorRegistro.text = "Contrase�a con menos de 6 caracteres.";

                    mensajeAPIPlayFabDevuelto = true; //No accede a la API en este punto, lo pongo como si accediese. Pero necesito activar esta variable aqu� para que funcione bien el resto del c�digo, en vez de crear 2 variables
                }

            }
            else
            {
                Debug.Log("Contrase�a diferente en ambos lados");
                textoErrorRegistro.text = "Contrase�a diferente en ambos lados.";

                mensajeAPIPlayFabDevuelto = true; //No accede a la API en este punto, lo pongo como si accediese. Pero necesito activar esta variable aqu� para que funcione bien el resto del c�digo, en vez de crear 2 variables
            }
        }
        else
        {
            Debug.Log("El usuario no se puede registrar.");
            textoErrorRegistro.text = "Rellene todos los campos por favor.";

            mensajeAPIPlayFabDevuelto = true; //No accede a la API en este punto, lo pongo como si accediese. Pero necesito activar esta variable aqu� para que funcione bien el resto del c�digo, en vez de crear 2 variables
        }
    }

    private IEnumerator desactivarPorUnTiempoLosBotonesYLuegoActivarCuandoHayaRespuestaDeLaAPIdePlayFab()
    {
        mensajeAPIPlayFabDevuelto = false;

        bot�nIniciarSesi�n.interactable = false;
        bot�nConfirmar.interactable = false;

        //yield return new WaitUntil(() => mensajeAPIPlayFabDevuelto);
        yield return new WaitForSeconds(2f);

        bot�nIniciarSesi�n.interactable = true;
        bot�nConfirmar.interactable = true;
    }


    // M�todo para registrar al usuario
    public void RegisterUser(string username, string password)
    {
        // Crear la solicitud de registro
        Debug.Log("El usuario trata de registrarse");
        
    }

    // M�todo llamado cuando el registro es exitoso
    private void OnRegisterSuccess()
    {
        Debug.Log("Usuario registrado exitosamente");

        // Aqu� puedes proceder al siguiente paso, como iniciar sesi�n autom�ticamente o cambiar de escena
        textoErrorRegistro.text = "";
        texto�xitoRegistro.text = "Usuario registrado con �xito.";
        StartCoroutine(finRegistroUsuario());
        //PlayerPrefs.SetInt("UsuarioRegistrado", 1); //Ya est� registrado el usuario

        string nombreUsuario = inputTextoNombre.text.Trim();
        string contrase�aUsuario = inputTextoContrase�a.text.Trim();

        //Guardo estas strings en estos PlayerPrefs para usar futuramente en el modo multijugador
        PlayerPrefs.SetString("Nombre Usuario", nombreUsuario);
        //No creo que haya peligro que se guarde en este PlayerPrefs la contrase�a del usuario, ya que s�lo podr�a guardarse en la App de la persona donde inici� sesi�n.
        PlayerPrefs.SetString("Contrase�a Usuario", contrase�aUsuario);

    }

    private IEnumerator finRegistroUsuario()
    {
        yield return new WaitForSeconds(1f); //1f = duraci�n que espera antes de ocultar el canvas y as� mostrar el mensaje de "Usuario registrado con �xito" un poco.

        texto�xitoRegistro.text = "";
        textoErrorRegistro.text = "";
        canvasRegistroUsuario.SetActive(false);

        //Dejo vac�os los campos por si se vuelve a ver este canvas
        inputTextoNombre.text = "";
        inputTextoContrase�a.text = "";
        inputTextoContrase�aRepetida.text = "";

        mensajeAPIPlayFabDevuelto = true;

        PlayerPrefs.SetInt("UsuarioRegistrado", 1);
    }

    // M�todo llamado cuando el registro falla
    private void OnRegisterFailure(string error)
    {
        Debug.LogError("Error al registrar el usuario: " + error);

        // Verificar el tipo de error
        if (error == "")
        {
            textoErrorRegistro.text = "El nombre de usuario ya est� en uso. Por favor, elija otro nombre.";
            mensajeAPIPlayFabDevuelto = true;
        }
    }

    /// <summary>
    ///  M�todo para cambiar de componente con TAB en la interfaz gr�fica.
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
