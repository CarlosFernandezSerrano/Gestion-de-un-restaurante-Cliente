using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Dates;
using UnityEditor;
using UnityEngine;

public class DateFormatInputField : MonoBehaviour
{
    [SerializeField] private TMP_InputField dateInputField;
    [SerializeField] private DatePicker datePicker; // Asigna el DatePicker en el Inspector
    private bool isFormatting = false; // Evita bucles de retroalimentación

    // Start is called before the first frame update
    void Start()
    {
        // Configura el listener para cambios
        dateInputField.onValueChanged.AddListener(FormatDate);
        /*
        dateInputField.onValueChanged.AddListener((rawDate) =>
        {
            if (DateTime.TryParse(rawDate, out DateTime date))
            {
                // Previene bucle infinito al modificar el texto
                if (dateInputField.text != date.ToString("dd/MM/yyyy"))
                {
                    dateInputField.text = date.ToString("dd/MM/yyyy");
                }
            }
        });*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FormatDate(string rawDate)
    {
        if (isFormatting || string.IsNullOrEmpty(rawDate)) return;

        try
        {
            isFormatting = true;

            // Parseo con cultura invariante para evitar errores regionales
            if (DateTime.TryParse(rawDate, System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out DateTime date))
            {
                string formattedDate = date.ToString("dd/MM/yyyy");
                if (dateInputField.text != formattedDate)
                {
                    dateInputField.SetTextWithoutNotify(formattedDate); // Evita retrigger del evento
                }
            }
        }
        finally
        {
            isFormatting = false;
        }
    }

    void OnDestroy()
    {
        dateInputField.onValueChanged.RemoveListener(FormatDate);
    }
}

