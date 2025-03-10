using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonMesaController : MonoBehaviour, IPointerDownHandler, IDragHandler, IScrollHandler
{
    [Header("Referencias")]
    // Imagen contenedora que limitará los botones (debe tener RectTransform)
    public RectTransform containerRect;

    [Header("Configuración")]
    // Nombre de la imagen en Resources que se usará en el botón
    private string ImageName = "mantelMesa";

    private RectTransform rectTransform; // RectTransform del botón generado
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
        // Crear un nuevo GameObject para el botón
        GameObject newButtonObj = new GameObject("BotónGenerado");
        // El nuevo botón se creará como hijo del contenedor, NO del Canvas
        newButtonObj.transform.SetParent(containerRect, false);

        // Agregar y configurar el RectTransform: posición central y tamaño predeterminado
        RectTransform newRect = newButtonObj.AddComponent<RectTransform>();
        newRect.anchoredPosition = Vector2.zero;
        newRect.sizeDelta = new Vector2(100, 50); // Ejemplo de tamaño

        // Agregar un Image y cargar la imagen desde Resources (incluyendo la subcarpeta si es necesario)
        Image img = newButtonObj.AddComponent<Image>();
        // Si la imagen está en "Assets/Resources/Editar Restaurante/Mantel Mesa.png", se carga así:
        Sprite newSprite = Resources.Load<Sprite>("Editar Restaurante/" + ImageName);
        if (newSprite != null)
        {
            img.sprite = newSprite;
        }
        else
        {
            Debug.LogWarning("No se encontró la imagen en Resources: " + ImageName);
        }

        // Agrego un componente Button para que sea interactivo
        newButtonObj.AddComponent<Button>();

        // Agrego este script al nuevo botón para dotarlo de funcionalidad de arrastre y escala
        ButtonMesaController bm = newButtonObj.AddComponent<ButtonMesaController>();
        bm.containerRect = this.containerRect;  // Asigna el mismo contenedor
        bm.rectTransform = newRect;              // Asigna el RectTransform del nuevo botón
    }

    // Detecta cuando se presiona el botón (inicia el arrastre)
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
    }

    // Mientras se arrastra, actualiza la posición y la limita al contenedor
    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging && rectTransform != null)
        {
            Vector2 newPos = rectTransform.anchoredPosition + eventData.delta;
            rectTransform.anchoredPosition = ClampToContainer(newPos);
        }
    }

    // Cambia el tamaño del botón al usar la rueda del mouse
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

    // Método para clamping: limita la posición del botón dentro de los límites del contenedor
    private Vector2 ClampToContainer(Vector2 pos)
    {
        if (containerRect == null || rectTransform == null)
            return pos;

        // Tamaño del contenedor (área disponible)
        Vector2 containerSize = containerRect.rect.size;
        // Tamaño del botón (considerando la escala actual)
        Vector2 buttonSize = rectTransform.rect.size * rectTransform.localScale;

        // Suponiendo que el pivote es (0.5, 0.5) para ambos, se calculan los márgenes:
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
