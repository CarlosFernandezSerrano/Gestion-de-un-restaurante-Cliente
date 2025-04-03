using Assets.Scripts.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class CrearReservaController : MonoBehaviour
{
    [SerializeField] private GameObject canvasCrearReserva;
    [SerializeField] private TMP_Dropdown dropDownDías;
    [SerializeField] private TMP_Dropdown dropDownMeses;
    [SerializeField] private TMP_Dropdown dropDownAños;
    [SerializeField] private TMP_Dropdown dropDownHoras;
    [SerializeField] private TMP_Dropdown dropDownMinutos;
    [SerializeField] private TMP_Dropdown dropDownCantComensales;
    [SerializeField] private TMP_Text textResultadoMesasDisponibles;
    [SerializeField] private TMP_InputField inputFieldNombre;
    [SerializeField] private TMP_InputField inputFieldDni;
    [SerializeField] private TMP_InputField inputFieldNumTeléfono;
    [SerializeField] private TMP_InputField inputFieldNumMesa;
    [SerializeField] private TMP_Text textValorDíaEnCrear;
    [SerializeField] private TMP_Text textValorHoraEnCrear;
    [SerializeField] private TMP_Text textValorNumComensalesEnCrear;


    GestionarMesasController instanceGestionarMesasController;

    public static CrearReservaController InstanceCrearReservaController { get; private set; }

    private void Awake()
    {
        if (InstanceCrearReservaController == null)
        {
            InstanceCrearReservaController = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        instanceGestionarMesasController = GestionarMesasController.InstanceGestionarMesasController;

        InicializarValoresDropdowns();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InicializarValoresDropdowns()
    {
        
        List<string> opcionesDías = new List<string> { "01", "02", "03", "04", "05", "06", "07", "08", "09",
                                                          "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
                                                          "20", "21", "22", "23", "24", "25", "26", "27", "28", "29",
                                                          "30", "31"};
        List<string> opcionesMeses = new List<string> { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };
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

    public void AsignarValoresConcretosEnDropdowns()
    {
        string fechaActual = DateTime.Now.ToString("yyyy-MM-dd");   // Ejemplo: "2025-04-01"
        string horaActual = DateTime.Now.ToString("HH:mm");      // Ejemplo: "14:35"

        // Fecha actual
        string[] fechaActualArray = fechaActual.Split("-");
        string año = fechaActualArray[0].Trim();
        string mes = fechaActualArray[1].Trim();
        string día = fechaActualArray[2].Trim();

        // Hora actual
        string[] horaActualArray = horaActual.Split(":");
        string hora = horaActualArray[0].Trim();
        string minuto = horaActualArray[1].Trim();

        BuscoElIndiceYLoPongoSiLoEncuentro(dropDownDías, día);
        BuscoElIndiceYLoPongoSiLoEncuentro(dropDownMeses, mes);
        BuscoElIndiceYLoPongoSiLoEncuentro(dropDownAños, año);
        BuscoElIndiceYLoPongoSiLoEncuentro(dropDownHoras, hora);
        BuscoElIndiceYLoPongoSiLoEncuentro(dropDownMinutos, minuto);
    }

    private void BuscoElIndiceYLoPongoSiLoEncuentro(TMP_Dropdown horaOMinuto, string hora_O_Minuto)
    {
        // Busco el índice de ese valor en el Dropdown
        int index = horaOMinuto.options.FindIndex(option => option.text == hora_O_Minuto);

        // Si lo encuentra, establecerlo como el seleccionado
        if (index != -1)
        {
            horaOMinuto.value = index;
            horaOMinuto.RefreshShownValue(); // Refresca la UI para mostrar el valor seleccionado.
        }
        else
        {
            Debug.Log("Valor no encontrado");
        }
    }

    public void ObtenerMesasDisponibles()
    {
        textResultadoMesasDisponibles.text = "";

        string fecha_Actual = DateTime.Now.ToString("dd/MM/yyyy");   // Ejemplo: "01-04-2025"
        string hora_Actual = DateTime.Now.ToString("HH:mm");      // Ejemplo: "14:35"
        
        string fecha_Reserva = dropDownDías.options[dropDownDías.value].text + "/" + dropDownMeses.options[dropDownMeses.value].text + "/" + dropDownAños.options[dropDownAños.value].text;
        string hora_Reserva = dropDownHoras.options[dropDownHoras.value].text + ":" + dropDownMinutos.options[dropDownMinutos.value].text;
        string cantComensales_Reserva = dropDownCantComensales.options[dropDownCantComensales.value].text;
        int cantComensales = int.Parse(cantComensales_Reserva.Trim());
        Debug.Log("+Fecha: " + fecha_Reserva + "; Hora: " + hora_Reserva);
        DateTime fechaReserva = DateTime.Parse("30/04/2024"); // Asigno este dato en el caso de que de error en el try
        try
        {
            fechaReserva = DateTime.Parse(fecha_Reserva);
        }
        catch 
        {
            Debug.Log("++Error, fecha incorrecta");
        }
        
        DateTime fechaActual = DateTime.Parse(fecha_Actual);
        TimeSpan horaActual = TimeSpan.Parse(hora_Actual);
        TimeSpan horaReserva = TimeSpan.Parse(hora_Reserva);


        // Si la fecha de la reserva a crear es menor a la fecha actual, ERROR
        if (fechaReserva < fechaActual)
        {
            Debug.Log("++Error, no se puede crear una reserva en esa fecha porque ya ha pasado.");
            return;
        }
        else // Fecha puesta para crear la reserva, CORRECTA
        {
            Debug.Log("++Fecha correcta");
            
            // Si la fecha de la reserva es la misma que la fecha actual, pero la hora ya ha pasado, ERROR
            if (fechaActual == fechaReserva && horaReserva < horaActual)
            {
                Debug.Log("++Error, fecha correcta, pero hora pasada");
                textResultadoMesasDisponibles.text = " Hora pasada.";
                return;
            }

            List<Mesa> mesasRestaurante = instanceGestionarMesasController.GetMesas();
            if (mesasRestaurante.Count > 0)
            {
                List<Mesa> mesasDisponiblesAUnaFechaHoraYComensalesDeterminados = new List<Mesa>();

                foreach (Mesa mesa in mesasRestaurante)
                {
                    Debug.Log("++Mesa: " + mesa.Mostrar());

                    if (ReservaNuevaNoObstaculizaElResto(mesa.Id, fechaReserva, horaReserva))
                    {
                        mesasDisponiblesAUnaFechaHoraYComensalesDeterminados.Add(mesa);
                    }
                }

                // Si la lista no está vacía, se muestran las mesas disponibles
                if (mesasDisponiblesAUnaFechaHoraYComensalesDeterminados.Count > 0)
                {
                    var mesasOrdenadas = mesasDisponiblesAUnaFechaHoraYComensalesDeterminados.OrderBy(m => m.CantPers).ToList();
                    string cad = " ";
                    int cont = 0;
                    for (int i = 0; i < mesasOrdenadas.Count; i++)
                    {
                        if (mesasOrdenadas[i].CantPers >= cantComensales && cantComensales <= mesasOrdenadas[i].CantPers + 5)
                        {
                            cont++;
                            Button botónMesaSelected = instanceGestionarMesasController.padreDeLosBotonesMesa.gameObject.transform.Find("Button-" + mesasOrdenadas[i].Id).GetComponent<Button>();
                            int id_Mesa_En_Mapa = int.Parse(instanceGestionarMesasController.ObtenerIDMesaDelMapa(botónMesaSelected));
                            if (i < mesasOrdenadas.Count - 1)
                            {
                                cad += "" + id_Mesa_En_Mapa + "(" + mesasOrdenadas[i].CantPers + "), ";
                            }
                            else
                            {
                                cad += "" + id_Mesa_En_Mapa + "(" + mesasOrdenadas[i].CantPers + ")";
                            }
                            PonerValoresEnLasOpcionesDeCrear(fecha_Reserva, hora_Reserva, cantComensales_Reserva);
                        }
                    }
                    // Si hay alguna mesa disponible "con la cantidad de comensales que pide el usuario"
                    if (cont > 0)
                    {
                        textResultadoMesasDisponibles.text = cad; // Pinto en TMP_Text en Scroll View las mesas disponibles con su capacidad de personas
                    }
                    else
                    {
                        textResultadoMesasDisponibles.text = " Ninguna";
                    }
                }
                else // Ninguna mesa disponible en esa fecha a esa hora
                {
                    textResultadoMesasDisponibles.text = " Ninguna";
                    PonerValoresEnLasOpcionesDeCrear("", "", "");
                }
                
            }
            else
            {
                Debug.Log("++No hay mesas");
            }

        }
    }

    // Valores: día, fecha, cant comensales
    public void PonerValoresEnLasOpcionesDeCrear(string fecha_Reserva, string hora_Reserva, string cantComensales_Reserva)
    {
        textValorDíaEnCrear.text = fecha_Reserva;
        textValorHoraEnCrear.text = hora_Reserva;
        try
        {
            textValorNumComensalesEnCrear.text = "" + int.Parse(cantComensales_Reserva);
        }
        catch
        {
            textValorNumComensalesEnCrear.text = cantComensales_Reserva;
        }
        
    }

    private bool ReservaNuevaNoObstaculizaElResto(int id_Mesa, DateTime fechaReservaFutura, TimeSpan horaReservaFutura)
    {
        // Obtengo las reservas pendientes que tiene la mesa, ya sean de hoy o en adelante
        List<Reserva> reservasMesaDeHoyEnAdelante = instanceGestionarMesasController.ObtenerReservasMesaDeHoyEnAdelante(id_Mesa);

        //string horaActual = DateTime.Now.ToString("HH:mm");
        //TimeSpan horaActualTimeSpan = TimeSpan.Parse(horaActual);

        string[] tiempoPermitido = Restaurante.TiempoPermitidoParaComer.Split(":");
        int horas = int.Parse(tiempoPermitido[0].Trim());
        int minutos = int.Parse(tiempoPermitido[1].Trim());
        Debug.Log("Resultado tiempo permitido - Horas:" + horas + "; Minutos:" + minutos + "*");

        // Sumo el tiempo que puso el gerente en la escena "Editar Restaurante" en tiempo permitido para comer
        TimeSpan sumaTiempo = TimeSpan.FromHours(horas) + TimeSpan.FromMinutes(minutos);
        TimeSpan horaFinReservaFutura = horaReservaFutura.Add(sumaTiempo);

        foreach (Reserva reserva in reservasMesaDeHoyEnAdelante)
        {
            string hora_Reserva = reserva.Hora;
            TimeSpan horaReservaExistente = TimeSpan.Parse(hora_Reserva);
            
            Debug.Log("Resultado tiempo permitido - Horas:" + horas + "; Minutos:" + minutos + "*");
            DateTime fechaReservaExistente = DateTime.Parse(reserva.Fecha);

            // Sumo el tiempo que puso el gerente en la escena "Editar Restaurante" en tiempo permitido para comer
            TimeSpan sumaTiempoo = TimeSpan.FromHours(horas) + TimeSpan.FromMinutes(minutos);
            TimeSpan horaFinReservaExistente = horaReservaExistente.Add(sumaTiempoo);
            Debug.Log("HoraFINReservaFutura: " + horaFinReservaFutura + "; HoraInicioReservaExistente: " + horaReservaExistente + " -  HoraFinReservaExistente: " + horaFinReservaExistente + "; Estado reserva existente = " + reserva.Estado);
            if (fechaReservaFutura == fechaReservaExistente && horaFinReservaFutura <= horaFinReservaExistente && horaFinReservaFutura >= horaReservaExistente && (reserva.Estado.CompareTo("" + EstadoReserva.Confirmada) == 0 || reserva.Estado.CompareTo("" + EstadoReserva.Pendiente) == 0) || fechaReservaFutura == fechaReservaExistente && horaReservaFutura >= horaReservaExistente && horaReservaFutura < horaFinReservaExistente && (reserva.Estado.CompareTo("" + EstadoReserva.Confirmada) == 0 || reserva.Estado.CompareTo("" + EstadoReserva.Pendiente) == 0))
            {
                Debug.Log("++--------------- RESERVA NUEVA OBSTACULIZA");
                return false;
            }
            Debug.Log("++Estado reserva:" + reserva.Estado);
        }
        Debug.Log("++--------------- RESERVA NUEVA NO OBSTACULIZA");
        return true;

    }

    public void CrearReserva()
    {
        string nombre = inputFieldNombre.text;
        string dni = inputFieldDni.text;
        string teléfono = inputFieldNumTeléfono.text;
        string num_Mesa = inputFieldNumMesa.text;

        Debug.Log("++Crear Reserva Presionado: " + nombre + ", " + dni + ", " + teléfono+", "+num_Mesa);


    }

    public void DesactivarCanvasCrearReserva()
    {
        canvasCrearReserva.SetActive(false);
        textResultadoMesasDisponibles.text = "";
    }
}
