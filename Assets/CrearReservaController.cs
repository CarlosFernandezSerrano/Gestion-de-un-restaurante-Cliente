using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CrearReservaController : MonoBehaviour
{
    [SerializeField] private GameObject canvasCrearReserva;
    [SerializeField] private TMP_Dropdown dropDownDías;
    [SerializeField] private TMP_Dropdown dropDownMeses;
    [SerializeField] private TMP_Dropdown dropDownAños;
    [SerializeField] private TMP_Dropdown dropDownHoras;
    [SerializeField] private TMP_Dropdown dropDownMinutos;
    [SerializeField] private TMP_Dropdown dropDownCantComensales;


    // Start is called before the first frame update
    void Start()
    {
        InicializarValoresDropdowns();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InicializarValoresDropdowns()
    {
        
        List<string> opcionesDías = new List<string> { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09",
                                                          "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
                                                          "20", "21", "22", "23", "24", "25", "26", "27", "28", "29",
                                                          "30", "31"};
        List<string> opcionesMeses = new List<string> { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };
        List<string> opcionesAños = new List<string> { "2025", "2026", "2027", "2028", "2029", "2030", "2031", "2032", "2033", "2034", "2035", "2036", "2037", "2038", "2039", "2040" };
        List<string> opcionesHoras = new List<string> { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23" };
        List<string> opcionesMinutos = new List<string> { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09",
                                                          "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
                                                          "20", "21", "22", "23", "24", "25", "26", "27", "28", "29",
                                                          "30", "31", "32", "33", "34", "35", "36", "37", "38", "39",
                                                          "40", "41", "42", "43", "44", "45", "46", "47", "48", "49",
                                                          "50", "51", "52", "53", "54", "55", "56", "57", "58", "59" };
        List<string> opcionesCantComensales = new List<string> { "01", "02", "03", "04", "05", "06", "07", "08", "09",
                                                          "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
                                                          "20", "21", "22", "23", "24", "25", "26", "27", "28", "29",
                                                          "30", "31", "32", "33", "34", "35", "36", "37", "38", "39",
                                                          "40", "41", "42", "43", "44", "45", "46", "47", "48", "49",
                                                          "50" };

        AgregarOpcionesADropdown(dropDownDías, opcionesDías);
        AgregarOpcionesADropdown(dropDownMeses, opcionesMeses);
        AgregarOpcionesADropdown(dropDownAños, opcionesAños);

        AgregarOpcionesADropdown(dropDownHoras, opcionesHoras);
        AgregarOpcionesADropdown(dropDownMinutos, opcionesMinutos);

        AgregarOpcionesADropdown(dropDownCantComensales, opcionesCantComensales);
    }

    private void AgregarOpcionesADropdown(TMP_Dropdown dropdown, List<string> opciones)
    {
        dropdown.ClearOptions();  // Limpia opciones anteriores
        dropdown.AddOptions(opciones);
    }


    public void DesactivarCanvasCrearReserva()
    {
        canvasCrearReserva.SetActive(false);
    }
}
