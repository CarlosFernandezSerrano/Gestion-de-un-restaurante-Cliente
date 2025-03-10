using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonMesaController : MonoBehaviour, IPointerDownHandler, IDragHandler, IScrollHandler
{
    [Header("Referencias")]
    // Imagen contenedora que limitar� los botones (debe tener RectTransform)
    public RectTransform containerRect;

    [Header("Configuraci�n")]
    // Nombre de la imagen en Resources que se usar� en el bot�n
    private string ImageName = "mantelMesa";

    private RectTransform rectTransform; // RectTransform del bot�n generado
    private bool isDragging = false;

    public static ButtonMesaController InstanceButtonMesaController { get; private set; }

    void Awake()
    {
        if (InstanceButtonMesaController == null)
        {
            InstanceButtonMesaController = this;
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

    public void SpawnButton()
    {
        // Crear un nuevo GameObject para el bot�n
        GameObject newButtonObj = new GameObject("Bot�nGenerado");
        // El nuevo bot�n se crear� como hijo del contenedor, NO del Canvas
        newButtonObj.transform.SetParent(containerRect, false);

        // Agregar y configurar el RectTransform: posici�n central y tama�o predeterminado
        RectTransform newRect = newButtonObj.AddComponent<RectTransform>();
        newRect.anchoredPosition = Vector2.zero;
        newRect.sizeDelta = new Vector2(100, 50); // Ejemplo de tama�o

        // Agregar un Image y cargar la imagen desde Resources (incluyendo la subcarpeta si es necesario)
        Image img = newButtonObj.AddComponent<Image>();
        // Si la imagen est� en "Assets/Resources/Editar Restaurante/Mantel Mesa.png", se carga as�:
        Sprite newSprite = Resources.Load<Sprite>("Editar Restaurante/" + ImageName);
        if (newSprite != null)
        {
            img.sprite = newSprite;
        }
        else
        {
            Debug.LogWarning("No se encontr� la imagen en Resources: " + ImageName);
        }

        // Agrego un componente Button para que sea interactivo
        newButtonObj.AddComponent<Button>();

        // Agrego este script al nuevo bot�n para dotarlo de funcionalidad de arrastre y escala
        ButtonMesaController bm = newButtonObj.AddComponent<ButtonMesaController>();
        bm.containerRect = this.containerRect;  // Asigna el mismo contenedor
        bm.rectTransform = newRect;              // Asigna el RectTransform del nuevo bot�n
    }

    // Detecta cuando se presiona el bot�n (inicia el arrastre)
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
    }

    // Mientras se arrastra, actualiza la posici�n y la limita al contenedor
    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging && rectTransform != null)
        {
            Vector2 newPos = rectTransform.anchoredPosition + eventData.delta;
            rectTransform.anchoredPosition = ClampToContainer(newPos);
        }
    }

    // Cambia el tama�o del bot�n al usar la rueda del mouse
    public void OnScroll(PointerEventData eventData)
    {
        if (rectTransform != null)
        {
            float scaleChange = eventData.scrollDelta.y * 0.1f;
            Vector3 newScale = rectTransform.localScale + new Vector3(scaleChange, scaleChange, 0);
            newScale.x = Mathf.Clamp(newScale.x, 0.5f, 3f);
            newScale.y = Mathf.Clamp(newScale.y, 0.5f, 3f);
            newScale.z = 1;
            rectTransform.localScale = newScale;
        }
    }

    // M�todo para clamping: limita la posici�n del bot�n dentro de los l�mites del contenedor
    private Vector2 ClampToContainer(Vector2 pos)
    {
        if (containerRect == null || rectTransform == null)
            return pos;

        // Tama�o del contenedor (�rea disponible)
        Vector2 containerSize = containerRect.rect.size;
        // Tama�o del bot�n (considerando la escala actual)
        Vector2 buttonSize = rectTransform.rect.size * rectTransform.localScale;

        // Suponiendo que el pivote es (0.5, 0.5) para ambos, se calculan los m�rgenes:
        Vector2 halfContainer = containerSize * 0.5f;
        Vector2 halfButton = buttonSize * 0.5f;

        float minX = -halfContainer.x + halfButton.x;
        float maxX = halfContainer.x - halfButton.x;
        float minY = -halfContainer.y + halfButton.y;
        float maxY = halfContainer.y - halfButton.y;

        float clampedX = Mathf.Clamp(pos.x, minX, maxX);
        float clampedY = Mathf.Clamp(pos.y, minY, maxY);

        return new Vector2(clampedX, clampedY);
    }
}
