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
        // Definir la posición de spawn (centro del contenedor)
        Vector2 spawnPos = Vector2.zero;
        // Definir el tamaño predeterminado del botón (en este ejemplo 100x50)
        Vector2 defaultSize = new Vector2(100, 50);
        // Creamos un Rect que representaría el área del nuevo botón
        Rect newButtonRect = new Rect(spawnPos - defaultSize / 2f, defaultSize);

        // Comprobamos si en la posición de spawn ya hay un botón (que se solape)
        ButtonMesaController[] buttons = containerRect.GetComponentsInChildren<ButtonMesaController>();
        // Revisar si ya hay un botón en la zona central
        //ButtonMesaController[] buttons = containerRect.GetComponentsInChildren<ButtonMesaController>();
        foreach (ButtonMesaController btn in buttons)
        {
            if (btn.rectTransform == null)
                continue;

            Vector2 otherPos = btn.rectTransform.anchoredPosition;
            Vector2 otherSize = btn.rectTransform.rect.size * btn.rectTransform.localScale;

            float marginX = 190f; // Margen en el eje X
            float marginY = 120f; // Margen en el eje Y

            if (Mathf.Abs(otherPos.x - spawnPos.x) < marginX && Mathf.Abs(otherPos.y - spawnPos.y) < marginY)
            {
                Debug.Log("Ya existe un botón en el centro. No se creará uno nuevo.");
                return;
            }
        }
        foreach (ButtonMesaController btn in buttons)
        {
            if (btn.rectTransform == null)
                continue;

            Vector2 otherPos = btn.rectTransform.anchoredPosition;
            Vector2 otherSize = btn.rectTransform.rect.size * btn.rectTransform.localScale;
            Rect otherRect = new Rect(otherPos - otherSize / 2f, otherSize);

            if (newButtonRect.Overlaps(otherRect))
            {

                Debug.Log("No se puede crear un nuevo botón porque se superpondría con uno existente.");
                return;
            }
        }

        // Crear un nuevo GameObject para el botón
        GameObject newButtonObj = new GameObject("BotónGenerado");
        // El nuevo botón se creará como hijo del contenedor, NO del Canvas
        newButtonObj.transform.SetParent(containerRect, false);

        // Agregar y configurar el RectTransform: posición central y tamaño predeterminado
        RectTransform newRect = newButtonObj.AddComponent<RectTransform>();
        newRect.anchoredPosition = Vector2.zero;
        newRect.sizeDelta = new Vector2(180, 100); // Ejemplo de tamaño

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
            //rectTransform.anchoredPosition = ClampToContainer(newPos);
            newPos = ClampToContainer(newPos);
            if (!WillOverlap(newPos))
            {
                rectTransform.anchoredPosition = newPos;
            }
        }
    }

    // Cambia el tamaño del botón al usar la rueda del mouse
    public void OnScroll(PointerEventData eventData)
    {
        /*if (rectTransform != null)
        {
            float scaleChange = eventData.scrollDelta.y * 0.1f;
            Vector3 newScale = rectTransform.localScale + new Vector3(scaleChange, scaleChange, 0);
            newScale.x = Mathf.Clamp(newScale.x, 0.5f, 3f);
            newScale.y = Mathf.Clamp(newScale.y, 0.5f, 3f);
            newScale.z = 1;
            rectTransform.localScale = newScale;
        }*/
        /*if (rectTransform != null && containerRect != null)
        {
            // Ajusto la sensibilidad del scroll
            float scaleChange = eventData.scrollDelta.y * 0.1f;
            Vector3 newScale = rectTransform.localScale + new Vector3(scaleChange, scaleChange, 0);

            // Defino los límites de escala:
            newScale.x = Mathf.Clamp(newScale.x, 1f, 3f);
            newScale.y = Mathf.Clamp(newScale.y, 1f, 3f);
            newScale.z = 1;

            // Calculamos el tamaño del botón con la nueva escala
            Vector2 newSize = rectTransform.rect.size * newScale;

            // Verificamos si cabe dentro del contenedor
            Vector2 halfContainer = containerRect.rect.size * 0.5f;
            Vector2 halfButton = newSize * 0.5f;
            Vector2 pos = rectTransform.anchoredPosition;

            bool fitsInside =
                pos.x - halfButton.x >= -halfContainer.x &&
                pos.x + halfButton.x <= halfContainer.x &&
                pos.y - halfButton.y >= -halfContainer.y &&
                pos.y + halfButton.y <= halfContainer.y;

            // Si cabe, aplicamos la nueva escala
            if (fitsInside)
            {
                rectTransform.localScale = newScale;
            }
        }*/
        if(rectTransform != null && containerRect != null)
        {
            float scaleChange = eventData.scrollDelta.y * 0.1f;
            Vector3 newScale = rectTransform.localScale + new Vector3(scaleChange, scaleChange, 0);

            // Definir los límites de escala
            newScale.x = Mathf.Clamp(newScale.x, 1f, 3f);
            newScale.y = Mathf.Clamp(newScale.y, 1f, 3f);
            newScale.z = 1;

            // Calculamos el tamaño del botón con la nueva escala
            Vector2 newSize = rectTransform.rect.size * newScale;
            Vector2 newPos = rectTransform.anchoredPosition;

            // Verificamos si cabe dentro del contenedor
            Vector2 halfContainer = containerRect.rect.size * 0.5f;
            Vector2 halfButton = newSize * 0.5f;

            bool fitsInside =
                newPos.x - halfButton.x >= -halfContainer.x &&
                newPos.x + halfButton.x <= halfContainer.x &&
                newPos.y - halfButton.y >= -halfContainer.y &&
                newPos.y + halfButton.y <= halfContainer.y;

            // Verificar si el nuevo tamaño causaría superposición con otro botón
            bool overlapsWithOther = WillOverlapWithSize(newSize);

            // Si cabe dentro del contenedor y no se superpone con otro botón, aplicar la nueva escala
            if (fitsInside && !overlapsWithOther)
            {
                rectTransform.localScale = newScale;
            }
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

    // Comprueba si, en la nueva posición, este botón se superpondría con otro
    private bool WillOverlap(Vector2 newPos)
    {
        if (rectTransform == null)
            return false;

        // Calcula el rectángulo del botón en la posición nueva
        Vector2 mySize = rectTransform.rect.size * rectTransform.localScale;
        Rect myRect = new Rect(newPos - mySize / 2f, mySize);

        // Obtener todos los ButtonMesaController hijos del contenedor
        ButtonMesaController[] buttons = containerRect.GetComponentsInChildren<ButtonMesaController>();
        foreach (ButtonMesaController btn in buttons)
        {
            // Se ignora el botón actual
            if (btn == this || btn.rectTransform == null)
                continue;

            Vector2 otherSize = btn.rectTransform.rect.size * btn.rectTransform.localScale;
            Rect otherRect = new Rect(btn.rectTransform.anchoredPosition - otherSize / 2f, otherSize);

            // Si se superponen, se retorna true
            if (myRect.Overlaps(otherRect))
                return true;
        }
        return false;
    }

    private bool WillOverlapWithSize(Vector2 newSize)
    {
        if (rectTransform == null)
            return false;

        // Calcula el rectángulo del botón con el nuevo tamaño
        Vector2 myPosition = rectTransform.anchoredPosition;
        Rect myRect = new Rect(myPosition - newSize / 2f, newSize);

        // Obtener todos los botones dentro del contenedor
        ButtonMesaController[] buttons = containerRect.GetComponentsInChildren<ButtonMesaController>();
        foreach (ButtonMesaController btn in buttons)
        {
            if (btn == this || btn.rectTransform == null)
                continue;

            Vector2 otherSize = btn.rectTransform.rect.size * btn.rectTransform.localScale;
            Rect otherRect = new Rect(btn.rectTransform.anchoredPosition - otherSize / 2f, otherSize);

            // Si el nuevo tamaño se superpone con otro botón, retorna true
            if (myRect.Overlaps(otherRect))
                return true;
        }
        return false;
    }
}
