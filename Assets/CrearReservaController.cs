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
        AgregarOpcionesADropdown(dropDownDías, opcionesDías);
        AgregarOpcionesADropdown(dropDownMeses, opcionesMeses);
        AgregarOpcionesADropdown(dropDownAños, opcionesAños);
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
