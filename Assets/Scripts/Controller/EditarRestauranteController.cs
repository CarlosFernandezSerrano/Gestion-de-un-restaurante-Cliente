using Assets.Scripts.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EditarRestauranteController : MonoBehaviour, IPointerDownHandler, IDragHandler, IScrollHandler
{
    [SerializeField] private TMP_Dropdown horaApertura;
    [SerializeField] private TMP_Dropdown minutoApertura;
    [SerializeField] private TMP_Dropdown horaCierre;
    [SerializeField] private TMP_Dropdown minutoCierre;

    [Header("Referencias")]
    public Canvas canvas;  // Referencia al Canvas donde se crearán los botones.

    [Header("Configuración")]
    private string ImageName = "Mantel Mesa";  // Nombre de la imagen en Resources.

    private RectTransform rectTransform;
    private bool isDragging = false;



    void Awake()
    {
        SceneManager.LoadSceneAsync("General Controller", LoadSceneMode.Additive);
    }

    // Start is called before the first frame update
    void Start()
    {
        TrabajadorController.ComprobandoDatosTrabajador = false;

        List<string> opcionesHoras = new List<string> { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24" };
        List<string> opcionesMinutos = new List<string> { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09",
                                                          "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
                                                          "20", "21", "22", "23", "24", "25", "26", "27", "28", "29",
                                                          "30", "31", "32", "33", "34", "35", "36", "37", "38", "39",
                                                          "40", "41", "42", "43", "44", "45", "46", "47", "48", "49",
                                                          "50", "51", "52", "53", "54", "55", "56", "57", "58", "59" };
        AgregarOpcionesADropdown(horaApertura, opcionesHoras);
        AgregarOpcionesADropdown(minutoApertura, opcionesMinutos);
        AgregarOpcionesADropdown(horaCierre, opcionesHoras);
        AgregarOpcionesADropdown(minutoCierre, opcionesMinutos);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AgregarOpcionesADropdown(TMP_Dropdown dropdown, List<string> opciones)
    {
        dropdown.ClearOptions();  // Limpia opciones anteriores
        dropdown.AddOptions(opciones);
    }


    public void AgregarMesa()
    {
        Debug.Log("A");
        SpawnButton();
    }

    public void SpawnButton()
    {
        // Crear un nuevo GameObject para el botón
        GameObject newButtonObj = new GameObject("BotónGenerado");
        newButtonObj.transform.SetParent(canvas.transform, false);

        // Agregar un RectTransform y configurar la posición en el centro
        rectTransform = newButtonObj.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.zero;

        // Agregar un Image y cargar la imagen desde Resources
        Image img = newButtonObj.AddComponent<Image>();
        Sprite newSprite = Resources.Load<Sprite>("Editar Restaurante/"+ImageName);
        if (newSprite != null)
        {
            img.sprite = newSprite;
        }
        else
        {
            Debug.LogWarning("No se encontró la imagen en Resources: " + ImageName);
        }

        // Agregar un botón para que sea interactivo
        Button newButton = newButtonObj.AddComponent<Button>();

        // Agregar este script al botón para que tenga funcionalidad de movimiento y escala
        //newButtonObj.AddComponent<ButtonManager>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            rectTransform.anchoredPosition += eventData.delta;
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        float scaleChange = eventData.scrollDelta.y * 0.1f;
        rectTransform.localScale += new Vector3(scaleChange, scaleChange, 0);
        rectTransform.localScale = new Vector3(
            Mathf.Clamp(rectTransform.localScale.x, 0.5f, 3f),
            Mathf.Clamp(rectTransform.localScale.y, 0.5f, 3f),
            1
        );
    }



    public void IrALaEscenaMain()
    {
        ComprobarSiHayCambios();

        SceneManager.LoadScene("Main");
    }

    private void ComprobarSiHayCambios()
    {
        
    }
}
