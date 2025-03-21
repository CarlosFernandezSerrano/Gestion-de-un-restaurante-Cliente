using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class GestionarMesasController : MonoBehaviour
{
    [SerializeField] private TMP_Text textNombreRestaurante;
    [SerializeField] private TMP_Text textHoraActual;
    [SerializeField] private GameObject tmpInputFieldPrefab; // Prefab de InputField TMP
    [SerializeField] private TMP_Text textHoraApertura;
    [SerializeField] private TMP_Text textHoraCierre;
    [SerializeField] private GameObject contenedorInfoReservasMesa;


    private List<Mesa> Mesas;

    private int lastIDMesa = 0;
    private string colorHexadecimalVerde = "#00B704";
    private string colorHexadecimalRojo = "#A12121";
    private Button bot�nMesaSeleccionado;


    // Contenedor padre donde se agregar�n los botones
    public RectTransform padreDeLosBotonesMesa;

    // Sprite que cargo desde Resources.
    private Sprite mesaSprite;

    M�todosAPIController instanceM�todosApiController;


    void Awake()
    {
        SceneManager.LoadSceneAsync("General Controller", LoadSceneMode.Additive);
    }

    // Start is called before the first frame update
    void Start()
    {
        instanceM�todosApiController = M�todosAPIController.InstanceM�todosAPIController;

        TrabajadorController.ComprobandoDatosTrabajador = false;

        InvokeRepeating(nameof(ActualizarHora), 0f, 1f); // Llama a ActualizarHora() cada 1 segundo

        ObtenerDatosRestauranteAsync();

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ActualizarHora()
    {
        textHoraActual.text = DateTime.Now.ToString("HH:mm");
    }

    private async void ObtenerDatosRestauranteAsync()
    {
        EliminarObjetosHijoDeFondoDeEdici�n();

        string cad = await instanceM�todosApiController.GetDataAsync("restaurante/getRestaurantePorId/" + PlayerPrefs.GetInt("Restaurante_ID Usuario"));

        // Deserializo la respuesta
        Restaurante restaurante = JsonConvert.DeserializeObject<Restaurante>(cad);

        textHoraApertura.text = restaurante.HoraApertura.Replace(" ", "");
        textHoraCierre.text = restaurante.HoraCierre.Replace(" ", "");
        Mesas = restaurante.Mesas;

        textNombreRestaurante.text = restaurante.Nombre;

        Debug.Log("Hora Apertura: " + restaurante.HoraApertura + "; Hora Cierre: " + restaurante.HoraCierre);

        mesaSprite = Resources.Load<Sprite>("Editar Restaurante/mantelMesa");
        CrearBotonesMesas();

        A�adirListenerABotonesMesaDelMapa();
        
    }

    private void A�adirListenerABotonesMesaDelMapa()
    {
        // Obtenemos todos los componentes Button que sean hijos del contenedor
        Button[] buttons = padreDeLosBotonesMesa.gameObject.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            // Es recomendable capturar la referencia del bot�n para evitar problemas con clausuras
            Button capturedButton = button;
            capturedButton.onClick.AddListener(() => MostrarContenedorInfoReservasMesa(capturedButton));
        }
    }

    private void MostrarContenedorInfoReservasMesa(Button capturedButton)
    {
        contenedorInfoReservasMesa.SetActive(true);
        bot�nMesaSeleccionado = capturedButton;

    }

    public void DesactivarContenedorInfoReservasMesa()
    {
        contenedorInfoReservasMesa.SetActive(false);
    }

    public void PonerNoDisponibleMesa()
    {
        Image img = bot�nMesaSeleccionado.gameObject.transform.Find("Imagen Circle").GetComponent<Image>();
        PonerColorCorrectoAMesa(img, colorHexadecimalRojo);
        contenedorInfoReservasMesa.SetActive(false);
        
        // Indicar al servidor


    }

    public void PonerDisponibleMesa()
    {
        Image img = bot�nMesaSeleccionado.gameObject.transform.Find("Imagen Circle").GetComponent<Image>();
        PonerColorCorrectoAMesa(img, colorHexadecimalVerde);
        contenedorInfoReservasMesa.SetActive(false);

        // Indicar al servidor


    }

    // Elimino todos los botones mesa antes de actualizar el fondo de edici�n, para que no sea un caos y se pongan unos encima de otros, adem�s de su gesti�n luego.
    private void EliminarObjetosHijoDeFondoDeEdici�n()
    {
        foreach (Transform hijo in padreDeLosBotonesMesa)
        {
            Destroy(hijo.gameObject);
        }
    }

    private void CrearBotonesMesas()
    {
        lastIDMesa = 0;

        // El restaurante tiene mesas
        if (Mesas.Count > 0)
        {
            Debug.Log("Hay mesas");
            foreach (var mesa in Mesas)
            {
                CrearBotonMesa(mesa);
            }
        }
        else
        {
            Debug.Log("No hay mesas");
        }
    }

    private void CrearBotonMesa(Mesa mesa)
    {
        // Crear un GameObject para el bot�n y asignarle un nombre �nico.
        GameObject botonGO = new GameObject("Button-" + mesa.Id);

        // Establecer el padre para que se muestre en el UI.
        botonGO.transform.SetParent(padreDeLosBotonesMesa, false);

        // Agregar el componente RectTransform (se agrega autom�ticamente al crear UI, pero lo a�adimos expl�citamente).
        RectTransform rt = botonGO.AddComponent<RectTransform>();
        // Opcional: definir un tama�o por defecto para el bot�n.
        rt.sizeDelta = new Vector2(mesa.Width, mesa.Height);

        // Agregar CanvasRenderer para poder renderizar el UI.
        botonGO.AddComponent<CanvasRenderer>();

        // Agregar el componente Image para mostrar el sprite.
        UnityEngine.UI.Image imagen = botonGO.AddComponent<UnityEngine.UI.Image>();
        if (mesaSprite != null)
        {
            imagen.sprite = mesaSprite;
        }

        // Configurar la posici�n y escala del bot�n bas�ndose en las propiedades de la mesa.
        rt.anchoredPosition = new Vector2(mesa.PosX, mesa.PosY);
        rt.localScale = new Vector3(mesa.ScaleX, mesa.ScaleY, 1f);

        // Agrego un componente Button para que sea interactivo
        botonGO.AddComponent<Button>();

        // Creo nuevos GameObject hijos, las im�genes del bot�n
        CrearImgsDelButton(rt, mesa.Disponible);

        StartCoroutine(CrearUnHijoInputFieldDelBot�nMesa(botonGO, mesa.CantPers));

        // Agrego este script al nuevo bot�n para dotarlo de funcionalidad de arrastre y escala
        //ButtonMesaController bm = botonGO.AddComponent<ButtonMesaController>();
        //bm.containerRect = this.padreDeLosBotonesMesa;  // Asigno el mismo contenedor
        //bm.rectTransform = rt; // Asigno el RectTransform del nuevo bot�n
    }

    public void Salir()
    {
        SceneManager.LoadScene("Main");
    }

    private void CrearImgsDelButton(RectTransform newRect, bool disponible)
    {
        CrearImgCircle(newRect, disponible);
        CrearImgRectangle(newRect);
    }

    private void CrearImgCircle(RectTransform newRect, bool disponible)
    {
        // Creo el objeto
        GameObject imgObject = new GameObject("Imagen Circle");
        // El nuevo bot�n se crear� como hijo del contenedor, NO del Canvas
        imgObject.transform.SetParent(newRect, false);

        // Agrego y configuro el RectTransform: posici�n central y tama�o predeterminado
        RectTransform rectButton = imgObject.AddComponent<RectTransform>();
        rectButton.anchoredPosition = Vector2.zero;
        rectButton.sizeDelta = new Vector2(85, 85); // Tama�o (ancho/alto)

        // Agrego un componente Image
        Image img = imgObject.AddComponent<Image>();
        Sprite newSprite = Resources.Load<Sprite>("Editar Restaurante/circle perfect 1.0");
        if (newSprite != null)
        {
            img.sprite = newSprite;
        }
        else
        {
            Debug.LogWarning("No se encontr� la imagen en Resources: circle perfect 1.0");
        }
        
        // Poner color correcto a mesa seg�n si est� disponible o no. Verde = S� ; Rojo = No
        if (disponible)
        {
            PonerColorCorrectoAMesa(img, colorHexadecimalVerde);
            
        }
        else
        {
            PonerColorCorrectoAMesa(img, colorHexadecimalRojo);
        }
    }

    private void PonerColorCorrectoAMesa(Image img, string hexadecimal)
    {
        Color newColor;
        // Intento convertir el string hexadecimal a Color
        if (ColorUtility.TryParseHtmlString(hexadecimal, out newColor))
        {
            img.color = newColor;
        }
        else
        {
            Debug.LogError("El formato del color hexadecimal es inv�lido.");
        }
    }

    private void CrearImgRectangle(RectTransform newRect)
    {
        // Creo el objeto
        GameObject imgObject = new GameObject("Imagen Rectangle");
        // El nuevo bot�n se crear� como hijo del contenedor, NO del Canvas
        imgObject.transform.SetParent(newRect, false);

        // Agrego y configuro el RectTransform: posici�n central y tama�o predeterminado
        RectTransform rectImg = imgObject.AddComponent<RectTransform>();
        rectImg.anchoredPosition = new Vector2(-67.5f, 35); // x e y
        rectImg.sizeDelta = new Vector2(45, 30); // Tama�o (ancho/alto)

        // Agrego un componente Image
        Image img = imgObject.AddComponent<Image>();
        /*Sprite newSprite = Resources.Load<Sprite>("Editar Restaurante/circle perfect 1.0");
        if (newSprite != null)
        {
            img.sprite = newSprite;
        }
        else
        {
            Debug.LogWarning("No se encontr� la imagen en Resources: circle perfect 1.0");
        }*/

        // Creo un gameObject TMP_Text y lo pongo de hijo en el objeto imagen rect�ngulo
        GameObject textObject = new GameObject("Text");
        // El nuevo bot�n se crear� como hijo del contenedor, NO del Canvas
        textObject.transform.SetParent(rectImg, false);

        // Agrego y configuro el RectTransform: posici�n central y tama�o predeterminado
        RectTransform rectText = textObject.AddComponent<RectTransform>();
        rectText.anchoredPosition = Vector2.zero;
        rectText.sizeDelta = new Vector2(40, 30); // Tama�o (ancho/alto)

        // Agrego un componente TMP_Text
        TMP_Text text = textObject.AddComponent<TextMeshProUGUI>();
        text.alignment = TextAlignmentOptions.Center; // Centro el texto
        lastIDMesa++;
        text.text = "" + lastIDMesa;
        text.fontSize = 32;
        text.fontStyle = FontStyles.Bold;

    }

    private IEnumerator CrearUnHijoInputFieldDelBot�nMesa(GameObject newButtonObj, int cantComensales)
    {
        GameObject inputFieldInstance = Instantiate(tmpInputFieldPrefab, newButtonObj.transform, false);
        inputFieldInstance.name = "InputField";

        TMP_Text textComponent = inputFieldInstance.transform.Find("Text Area/Text").GetComponent<TMP_Text>();
        TMP_Text textPlaceHolder = inputFieldInstance.transform.Find("Text Area/Placeholder").GetComponent<TMP_Text>();

        //inputFieldInstance.GetComponent<TMP_InputField>().interactable = false; // Pongo el inputField en no interactuable
        textComponent.alignment = TextAlignmentOptions.Center; // Centro el texto
        textComponent.fontSize = 56;
        textComponent.fontStyle = FontStyles.Bold;
        textComponent.color = UnityEngine.Color.white;
        RectTransform rtInputField = inputFieldInstance.GetComponent<RectTransform>();
        rtInputField.sizeDelta = new Vector2(100, 55);
        inputFieldInstance.GetComponent<TMP_InputField>().text = "" + cantComensales; // Asigno la cantidad de comensales a la mesa
        inputFieldInstance.GetComponent<Image>().enabled = false; // Quito la imagen del inputField (la pongo en invisible)
        // Espero un frame para que se cree el Caret
        yield return null;

        // Desactivo Raycast Target para que no bloqueen interacci�n con el bot�n
        TMP_SelectionCaret caret = inputFieldInstance.GetComponentInChildren<TMP_SelectionCaret>();
        if (caret != null)
        {
            // Desactivamos raycastTarget del Caret
            caret.raycastTarget = false;
        }
        else
        {
            Debug.Log("Caret no encontrado!!!!!!!!!!!!!!!!!");
        }

        inputFieldInstance.GetComponent<Image>().raycastTarget = false;
        textPlaceHolder.raycastTarget = false;
        textComponent.raycastTarget = false;
    }

    public void IrAlMen�Principal()
    {
        SceneManager.LoadScene("Main");
    }
}
