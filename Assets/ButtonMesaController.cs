using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonMesaController : MonoBehaviour, IPointerDownHandler, IDragHandler, IScrollHandler
{
    public static ButtonMesaController buttonSeleccionadoParaBorrar;

    [Header("Referencias")]
    // Imagen contenedora que limitará los botones (debe tener RectTransform)
    public RectTransform containerRect;

    // Nombre de la imagen en Resources que se usará en el botón
    private string ImageName = "mantelMesa";

    public RectTransform rectTransform; // RectTransform del botón generado
    private bool isDragging = false;

    EditarRestauranteController instanceEditarRestauranteController;

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
        instanceEditarRestauranteController = EditarRestauranteController.InstanceEditarRestauranteController;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnButton()
    {
        // Defino la posición de spawn (centro del contenedor)
        Vector2 spawnPos = Vector2.zero;

        // Definir el tamaño predeterminado del botón (en este ejemplo 100x50)
        Vector2 defaultSize = new Vector2(180, 100);

        // Creo un Rect que representaría el área del nuevo botón
        Rect newButtonRect = new Rect(spawnPos - defaultSize / 2f, defaultSize);

        // Compruebo si en la posición de spawn ya hay un botón (que se solape)
        ButtonMesaController[] buttons = containerRect.GetComponentsInChildren<ButtonMesaController>();

        // Revisar si ya hay un botón en la zona central
        foreach (ButtonMesaController btn in buttons)
        {
            if (btn.rectTransform == null)
                continue;

            Vector2 otherPos = btn.rectTransform.anchoredPosition;
            Vector2 otherSize = btn.rectTransform.rect.size * btn.rectTransform.localScale;

            //if (Mathf.Abs(otherPos.x - spawnPos.x) < marginX && Mathf.Abs(otherPos.y - spawnPos.y) < marginY)
            if (Mathf.Abs(otherPos.x - spawnPos.x) < (otherSize.x / 2f + defaultSize.x / 2f) && Mathf.Abs(otherPos.y - spawnPos.y) < (otherSize.y / 2f + defaultSize.y / 2f))
            {
                Debug.Log("Ya existe un botón en el centro. No se creará uno nuevo.");

                string cad = "Ya existe un botón en el centro.";
                StartCoroutine(instanceEditarRestauranteController.MovimientoCartelDeMadera(2f, cad, 0f, 12f));
                return;
            }
        }

        instanceEditarRestauranteController.GetButtonAñadirMesa().interactable = false;
        Debug.Log("Es null?: " + instanceEditarRestauranteController.GetContenedorAsignarComensales());
        instanceEditarRestauranteController.GetContenedorAsignarComensales().SetActive(true);
        // Gestionar cantidad de comensales a la mesa
        //instanceEditarRestauranteController.GestionarCrearNuevoBotón();
        /*
        // Crear un nuevo GameObject para el botón
        GameObject newButtonObj = new GameObject("Button");
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
        */
    }

    // Detecta cuando se presiona el botón (inicia el arrastre)
    public void OnPointerDown(PointerEventData eventData)
    {
        // Verificar si este objeto es hijo del contenedorPadre
        if (transform.parent != containerRect.transform)
        {
            Debug.Log("Este botón no es hijo del contenedor permitido.");
            return; // Salir si no es hijo
        }

        // Si se pulsa con clic derecho, se marca este botón
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Actualizamos la referencia del botón seleccionado
            buttonSeleccionadoParaBorrar = this;
            Debug.Log("Botón marcado: " + gameObject.name);
            
            instanceEditarRestauranteController.ActivarPapelera();
            // (Opcional) Aquí puedes cambiar el color o aplicar alguna animación para indicar selección.
            isDragging = false; // No queremos que inicie un arrastre con clic derecho.
            return;
        }
        // Si se pulsa con clic izquierdo
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Si el clic izquierdo es en algo que no es el botón papelera, se desmarca
            // Aquí asumimos que al botón papelera le asignarás un Tag, por ejemplo "TrashButton"
            if (!gameObject.CompareTag("TrashButton"))
            {
                // Si se ha marcado algún botón previamente y se hace clic en otro elemento,
                // se deselecciona el botón marcado.
                buttonSeleccionadoParaBorrar = null;
                instanceEditarRestauranteController.DesactivarPapelera();
                Debug.Log("Selección desmarcada");
            }
            isDragging = true;
        }
    }

    // Mientras se arrastra, actualiza la posición y la limita al contenedor
    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging && rectTransform != null)
        {
            Vector2 newPos = rectTransform.anchoredPosition + eventData.delta;
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

    public void GestionarPapelera()
    {
        Debug.Log("He pulsado papelera; Botón marcado: " + buttonSeleccionadoParaBorrar.gameObject.name);

        string nombreBotón = buttonSeleccionadoParaBorrar.gameObject.name;
        // Si el botón seleccionado para ser borrado no está registrado en la BDD, lo eliino sólo en la memoria.
        if (nombreBotón.CompareTo("Button") == 0)
        {
            Destroy(buttonSeleccionadoParaBorrar.gameObject);
        }
        else // El botón seleccionado para ser eliminado/borrado está registrado en la BDD 
        {
            Debug.Log("Button con número");

            string[] array = nombreBotón.Split("-");
            Debug.Log("Número:" + array[1] + "*");
            int num = int.Parse(array[1]);
            instanceEditarRestauranteController.GestionarEliminarMesaEnBDDAsync(num);
        }

        // Una vez usada la papelera, la desactivo
        instanceEditarRestauranteController.DesactivarPapelera();
    }

    
}
