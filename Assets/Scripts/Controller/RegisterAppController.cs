using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using Assets.Scripts.Model;


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


    M�todosAPIController instanceM�todosAPIController;

    // Start is called before the first frame update
    void Start()
    {
        instanceM�todosAPIController = M�todosAPIController.instanceM�todosAPIController;
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
                                StartCoroutine(RegisterUser(textoNombre, textoPassword));     
                            }
                            else
                            {
                                Debug.Log("El nombre no puede tener espacios entre letras");
                                textoErrorRegistro.text = "El nombre no puede tener espacios entre letras.";
                            }
                        }
                        else
                        {
                            Debug.Log("Nombre con �");
                            textoErrorRegistro.text = "El nombre no debe tener �.";
                        }
                    }
                    else
                    {
                        Debug.Log("Nombre con menos de 3 caracteres");
                        textoErrorRegistro.text = "Nombre con menos de 3 caracteres.";
                    }
                }
                else
                {
                    Debug.Log("Contrase�a con menos de 6 caracteres");
                    textoErrorRegistro.text = "Contrase�a con menos de 6 caracteres.";
                }
            }
            else
            {
                Debug.Log("Contrase�a diferente en ambos lados");
                textoErrorRegistro.text = "Contrase�a diferente en ambos lados.";
            }
        }
        else
        {
            Debug.Log("El usuario no se puede registrar.");
            textoErrorRegistro.text = "Rellene todos los campos por favor.";
        }
    }

    private IEnumerator desactivarPorUnTiempoLosBotonesYLuegoActivarCuandoHayaRespuestaDeLaAPIdePlayFab()
    {

        bot�nIniciarSesi�n.interactable = false;
        bot�nConfirmar.interactable = false;

        //yield return new WaitUntil(() => mensajeAPIPlayFabDevuelto);
        yield return new WaitForSeconds(2f);

        bot�nIniciarSesi�n.interactable = true;
        bot�nConfirmar.interactable = true;
    }


    // M�todo para registrar al usuario
    public IEnumerator RegisterUser(string username, string password)
    {
        // Crear la solicitud de registro
        Debug.Log("El usuario trata de registrarse");
        Trabajador t = new Trabajador(username, password, 0, 0);
        yield return StartCoroutine(instanceM�todosAPIController.PostData("trabajador/registrarUser", t));
        Debug.Log("Respuesta register user: " + instanceM�todosAPIController.respuestaPOST);

        // Deserializo la respuesta
        Resultado data = JsonConvert.DeserializeObject<Resultado>(instanceM�todosAPIController.respuestaPOST);
        Debug.Log("El valor de result es: " + data.Result);
        
        switch (data.Result)
        {
            case 0:
                textoErrorRegistro.text = "Error inesperado";
                break;
            case 1:
                textoErrorRegistro.text = "";
                texto�xitoRegistro.text = "Trabajador registrado correctamente";
                GestionarRegistroExitoso();
                break;
            case 2:
                textoErrorRegistro.text = "El usuario " + username + " ya existe";
                break;            
        }
    }

    // M�todo llamado cuando el registro es exitoso
    private void GestionarRegistroExitoso()
    {
        StartCoroutine(finRegistroUsuario());

        string nombreUsuario = inputTextoNombre.text.Trim();

        //Guardo estos valores en estos PlayerPrefs para usar futuramente
        PlayerPrefs.SetString("Nombre Usuario", nombreUsuario);
        PlayerPrefs.SetInt("Rol Usuario", 1);
        Debug.Log("Nombre Usuario: " + PlayerPrefs.GetString("Nombre Usuario") + ", Rol Usuario: " + PlayerPrefs.GetInt("Rol Usuario"));
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

        PlayerPrefs.SetInt("UsuarioRegistrado", 1);
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
