using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class GestionarMesasController : MonoBehaviour
{
    [SerializeField] private TMP_Text inputFieldNombreRestaurante;
    [SerializeField] private GameObject imgHayCambiosSinGuardar;
    [SerializeField] private UnityEngine.UI.Button buttonGuardar;
    [SerializeField] private UnityEngine.UI.Button buttonVolver;
    [SerializeField] private UnityEngine.UI.Button buttonA�adirMesa;
    [SerializeField] private RectTransform imgCartel;
    [SerializeField] private TMP_Text textError;
    [SerializeField] private UnityEngine.UI.Button bot�nPapelera;
    [SerializeField] private RectTransform rtManosAdvertencia;
    [SerializeField] private RectTransform rtImgObjetoSello;
    [SerializeField] private RawImage imgSelloTintaEN;
    [SerializeField] private RawImage imgSelloTintaES;
    [SerializeField] private GameObject contenedorAsignarComensalesAMesa;
    [SerializeField] private TMP_InputField inputFieldCantComensales;
    [SerializeField] private GameObject textErrorComensales;
    [SerializeField] private GameObject tmpInputFieldPrefab; // Prefab de InputField TMP

    private string NombreRestaurante;
    private string HoraApertura;
    private string HoraCierre;
    private List<Mesa> Mesas;

    private int idMesaAEliminar;
    private int lastIDMesa = 0;

    // Contenedor padre donde se agregar�n los botones
    public RectTransform padreDeLosBotonesMesa;

    // Sprite que cargo desde Resources.
    private Sprite mesaSprite;

    ButtonMesaController instanceButtonMesaController;
    M�todosAPIController instanceM�todosApiController;


    void Awake()
    {
        SceneManager.LoadSceneAsync("General Controller", LoadSceneMode.Additive);
    }

    // Start is called before the first frame update
    void Start()
    {
        instanceButtonMesaController = ButtonMesaController.InstanceButtonMesaController;
        instanceM�todosApiController = M�todosAPIController.InstanceM�todosAPIController;

        TrabajadorController.ComprobandoDatosTrabajador = false;

        ObtenerDatosRestauranteAsync();

    }

    // Update is called once per frame
    void Update()
    {

    }
    

    private async void ObtenerDatosRestauranteAsync()
    {
        EliminarObjetosHijoDeFondoDeEdici�n();

        string cad = await instanceM�todosApiController.GetDataAsync("restaurante/getRestaurantePorId/" + PlayerPrefs.GetInt("Restaurante_ID Usuario"));

        // Deserializo la respuesta
        Restaurante restaurante = JsonConvert.DeserializeObject<Restaurante>(cad);

        NombreRestaurante = restaurante.Nombre;
        HoraApertura = restaurante.HoraApertura;
        HoraCierre = restaurante.HoraCierre;
        Mesas = restaurante.Mesas;

        inputFieldNombreRestaurante.text = NombreRestaurante;

        Debug.Log("Hora Apertura: " + restaurante.HoraApertura + "; Hora Cierre: " + restaurante.HoraCierre);

        mesaSprite = Resources.Load<Sprite>("Editar Restaurante/mantelMesa");
        CrearBotonesMesas();
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
        botonGO.AddComponent<UnityEngine.UI.Button>();

        // Creo nuevos GameObject hijos, las im�genes del bot�n
        CrearImgsDelButton(rt);

        StartCoroutine(CrearUnHijoInputFieldDelBot�nMesa(botonGO, mesa.CantPers));

        // Agrego este script al nuevo bot�n para dotarlo de funcionalidad de arrastre y escala
        ButtonMesaController bm = botonGO.AddComponent<ButtonMesaController>();
        bm.containerRect = this.padreDeLosBotonesMesa;  // Asigno el mismo contenedor
        bm.rectTransform = rt; // Asigno el RectTransform del nuevo bot�n
    }

    public void Salir()
    {
        SceneManager.LoadScene("Main");
    }

    public void GestionarCrearNuevoBot�n(int cantComensales)
    {
        // Crear un nuevo GameObject para el bot�n
        GameObject newButtonObj = new GameObject("Button");
        // El nuevo bot�n se crear� como hijo del contenedor, NO del Canvas
        newButtonObj.transform.SetParent(padreDeLosBotonesMesa, false);

        // Agregar y configurar el RectTransform: posici�n central y tama�o predeterminado
        RectTransform rectButton = newButtonObj.AddComponent<RectTransform>();
        rectButton.anchoredPosition = Vector2.zero;
        rectButton.sizeDelta = new Vector2(180, 100); // Tama�o (ancho/alto)

        // Agregar un Image y cargar la imagen desde Resources (incluyendo la subcarpeta si es necesario)
        UnityEngine.UI.Image img = newButtonObj.AddComponent<UnityEngine.UI.Image>();
        Sprite newSprite = Resources.Load<Sprite>("Editar Restaurante/mantelMesa");
        if (newSprite != null)
        {
            img.sprite = newSprite;
        }
        else
        {
            Debug.LogWarning("No se encontr� la imagen en Resources: mantelMesa");
        }


        // Agrego un componente Button para que sea interactivo
        newButtonObj.AddComponent<UnityEngine.UI.Button>();

        // Creo nuevos GameObject hijos, las im�genes del bot�n
        CrearImgsDelButton(rectButton);

        StartCoroutine(CrearUnHijoInputFieldDelBot�nMesa(newButtonObj, cantComensales));

        // 3 l�neas esenciales
        // Agrego este script al nuevo bot�n para dotarlo de funcionalidad de arrastre y escala
        ButtonMesaController bm = newButtonObj.AddComponent<ButtonMesaController>();
        bm.containerRect = this.padreDeLosBotonesMesa;  // Asigna el mismo contenedor
        bm.rectTransform = rectButton;              // Asigna el RectTransform del nuevo bot�n
    }

    private void CrearImgsDelButton(RectTransform newRect)
    {
        CrearImgCircle(newRect);
        CrearImgRectangle(newRect);
    }

    private void CrearImgCircle(RectTransform newRect)
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

    public void CancelarCrearBot�nMesa()
    {
        contenedorAsignarComensalesAMesa.SetActive(false);
        buttonA�adirMesa.interactable = true;
        textErrorComensales.SetActive(false);
    }
}
