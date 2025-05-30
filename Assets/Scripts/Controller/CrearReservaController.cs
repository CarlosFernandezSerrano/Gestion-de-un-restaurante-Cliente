using Assets.Scripts.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UI.Dates;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    [SerializeField] private Button buttonCrear;
    [SerializeField] private GameObject canvasInfoMesasDisponibles;
    [SerializeField] private TMP_InputField inputFieldFecha;
    [SerializeField] private DatePicker datePicker;
    [SerializeField] private TMP_InputField[] inputFields; // Asigno los InputFields en el orden de tabulación deseado


    private List<int> mesasDisponiblesEnMapa = new List<int>();
    private bool posibleCrearReserva = false;

    GestionarMesasController instanceGestionarMesasController;
    MétodosAPIController instanceMétodosApiController;

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
        instanceMétodosApiController = MétodosAPIController.InstanceMétodosAPIController;

        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SelectNextInputField();
        }

        ActualizarBotónCrear();

        //ActualizarHoraReserva();
        
    }
    

    private void ActualizarBotónCrear()
    {
        if (Hay7De7CamposRellenos())
        {
            buttonCrear.interactable = true;
        }
        else
        {
            buttonCrear.interactable = false;
        }
    }

    // Si la hora de reserva que se pone es menor a la hora de apertura o es mayor a la hora de cierre, se pone la hora actual
    private void ActualizarHoraReserva()
    {
        if (canvasCrearReserva.activeSelf)
        {
            TimeSpan hora_Reserva = TimeSpan.Parse(dropDownHoras.options[dropDownHoras.value].text + ":" + dropDownMinutos.options[dropDownMinutos.value].text);
            
            string fecha_Actual = DateTime.Now.ToString("dd/MM/yyyy");   // Ejemplo: "01-04-2025"
            DateTime fechaActual = DateTime.Parse(fecha_Actual);
            DateTime fechaReserva = DateTime.Parse(dropDownDías.options[dropDownDías.value].text + "/" + dropDownMeses.options[dropDownMeses.value].text + "/" + dropDownAños.options[dropDownAños.value].text);
            
            string hora_Actual = DateTime.Now.ToString("HH:mm");      // Ejemplo: "14:35"
            TimeSpan horaActual = TimeSpan.Parse(hora_Actual);
            /*
            // Si la fecha de la reserva es la misma que la fecha actual y la hora de la reserva es menor que la hora actual, se pone la hora actual
            if ( fechaReserva == fechaActual && hora_Reserva < horaActual)
            {
                // Hora actual
                string[] horaActualArray = hora_Actual.Split(":");
                string hora = horaActualArray[0].Trim();
                string minuto = horaActualArray[1].Trim();

                BuscoElIndiceYLoPongoSiLoEncuentro(dropDownHoras, hora);
                BuscoElIndiceYLoPongoSiLoEncuentro(dropDownMinutos, minuto);

                if (Usuario.Idioma.CompareTo("Español") == 0)
                {
                    textResultadoMesasDisponibles.text = " Hora pasada.";
                }
                else
                {
                    textResultadoMesasDisponibles.text = " Past hour.";
                }
                Espero1SegundoYQuitoElTextoDeMesasDisponiblesAsync();
            }*/

            /*
            // Si la fecha de la reserva es menor que la fecha actual, se pone la fecha actual en los dropdowns
            if (fechaReserva < fechaActual)
            {
                // Fecha actual
                string[] fechaActualArray = fecha_Actual.Split("/");
                string día = fechaActualArray[0].Trim();
                string mes = fechaActualArray[1].Trim();
                string año = fechaActualArray[2].Trim();

                BuscoElIndiceYLoPongoSiLoEncuentro(dropDownDías, día);
                BuscoElIndiceYLoPongoSiLoEncuentro(dropDownMeses, mes);
                BuscoElIndiceYLoPongoSiLoEncuentro(dropDownAños, año);

                if (Usuario.Idioma.CompareTo("Español") == 0)
                {
                    textResultadoMesasDisponibles.text = " Fecha pasada.";
                }
                else
                {
                    textResultadoMesasDisponibles.text = " Past date.";
                }
                Espero1SegundoYQuitoElTextoDeMesasDisponiblesAsync();
            }*/
        }
        
    }

    private async void Espero1SegundoYQuitoElTextoDeMesasDisponiblesAsync()
    {
        string texto = textResultadoMesasDisponibles.text.Trim();

        await Task.Delay(1500); // Espero 1 segundo sin bloquear

        if (texto.Contains("Fecha pasada") || texto.Contains("Past date") || texto.Contains("Ninguna") || texto.Contains("None") || texto.Contains("Reserva registrada") || texto.Contains("Reservation registered") || texto.Contains("Hora incorrecta") || texto.Contains("Wrong hour") || texto.Contains("No hay una fecha puesta") || texto.Contains("There is no date set") || texto.Contains("Error,") || texto.Contains("Hora pasada, son las") || texto.Contains("Past hour, it's"))
        {
            textResultadoMesasDisponibles.text = "";
        }
    }

    private bool Hay7De7CamposRellenos()
    {
        string nombre = inputFieldNombre.text.Trim();
        string dni = inputFieldDni.text.Trim();
        string fecha = textValorDíaEnCrear.text.Trim();
        string hora = textValorHoraEnCrear.text.Trim();
        string num_Comensales = textValorNumComensalesEnCrear.text.Trim();
        string num_Mesa = inputFieldNumMesa.text.Trim();

        string[] campos = { nombre, dni, num_Mesa, fecha, hora, num_Comensales };

        // Si ninguno de los campos está vacío, devuelve true. Todos los campos están llenos.
        if (!campos.Any(string.IsNullOrWhiteSpace) && NumMesaCorrecto(num_Mesa) && LosInputFieldCumplenElTamaño(nombre, dni))
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(dni, @"^\d{8}[A-Za-z]$") && DNIValido(dni))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public static bool DNIValido(string dni)
    {
        if (string.IsNullOrWhiteSpace(dni) || dni.Length != 9)
            return false;

        string letras = "TRWAGMYFPDXBNJZSQVHLCKE";
        string numeros = dni.Substring(0, 8);
        char letra = char.ToUpper(dni[8]);

        if (!int.TryParse(numeros, out int numeroDni))
            return false;

        int indice = numeroDni % 23;
        char letraEsperada = letras[indice];

        return letra == letraEsperada;
    }

    private bool LosInputFieldCumplenElTamaño(string nombreInputField, string dniInputField)
    {
        string nombre = Regex.Replace(nombreInputField, @"\s+", "");
        string dni = Regex.Replace(dniInputField, @"\s+", "");
        string num_Teléfono = Regex.Replace(inputFieldNumTeléfono.text.Trim(), @"\s+", ""); 

        // Si se pone un número de teléfono
        if (num_Teléfono.Length > 0)
        {
            if (nombre.Length <= 7 && dni.Length.Equals(9) && num_Teléfono.Length.Equals(9))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else // Si no se pone un número de teléfono
        {
            if (nombre.Length <= 7 && dni.Length.Equals(9))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        
    }

    private bool NumMesaCorrecto(string num_Mesa)
    {
        try
        {
            int numMesa = int.Parse(num_Mesa);

            foreach (int numMesaEnMapa in mesasDisponiblesEnMapa)
            {
                if (numMesaEnMapa.Equals(numMesa))
                {
                    return true;
                }
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public void InicializarValoresDropdowns()
    {
        List<string> opcionesDías = new List<string> { "01", "02", "03", "04", "05", "06", "07", "08", "09",
                                                          "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
                                                          "20", "21", "22", "23", "24", "25", "26", "27", "28", "29",
                                                          "30", "31"};
        List<string> opcionesMeses = new List<string> { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };

        
        // Calculo el año actual y al dropdown le agrego 9 años más
        string fechaActual = DateTime.Now.ToString("yyyy-MM-dd");
        string[] fechaArray = fechaActual.Split("-");
        int añoActual = int.Parse(fechaArray[0]);

        List<string> opcionesAños = new List<string>();
        for (int a = añoActual; a < añoActual + 10 + 1; a++)
        {
            opcionesAños.Add("" + a);
        }

        // Hora apertura restaurante
        string[] horaAperturaArray = instanceGestionarMesasController.GetHoraAperturaRestaurante().Split(":");
        int hora_a = int.Parse(horaAperturaArray[0].Trim());
        // Hora cierre restaurante
        string[] horaCierreArray = instanceGestionarMesasController.GetHoraCierreRestaurante().Split(":");
        int hora_c = int.Parse(horaCierreArray[0].Trim());

        List<string> opcionesHoras = new List<string>();
        
        // Si la hora de apertura es igual a la hora de cierre
        if (hora_a.Equals(hora_c))
        {
            opcionesHoras = new List<string> { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09",
                                                          "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
                                                          "20", "21", "22", "23" };
        }
        else
        { 
            // Si la hora de apertura es mayor que la hora de cierre. Ejemplo: 05:00 - 02:00
            if (hora_a > hora_c)
            {
                int i = hora_a;
                while (i < 24)
                {
                    if (i < 10)
                    {
                        opcionesHoras.Add("0" + i);
                    }
                    else
                    {
                        opcionesHoras.Add("" + i);
                    }
                    i++;
                }
                for (int j = 0; j < hora_c + 1; j++)
                {
                    if (j < 10)
                    {
                        opcionesHoras.Add("0" + j);
                    }
                    else
                    {
                        opcionesHoras.Add("" + j);
                    }
                }
            }
            else // Ejemplo: 17:00 - 22:00
            {
                for (int i = hora_a; i < hora_c + 1; i++)
                {
                    if (i < 10)
                    {
                        opcionesHoras.Add("0" + i);
                    }
                    else
                    {
                        opcionesHoras.Add("" + i);
                    }
                }
            }
            

            
        }

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
        posibleCrearReserva = false;
        textResultadoMesasDisponibles.text = "";
        mesasDisponiblesEnMapa.Clear();

        
        string fecha_Actual = DateTime.Now.ToString("dd/MM/yyyy");   // Ejemplo: "01-04-2025"
        string hora_Actual = DateTime.Now.ToString("HH:mm");      // Ejemplo: "14:35"

        string fecha_Reserva = inputFieldFecha.text.Trim();

        // Si no hay ninguna fecha puesta
        if (fecha_Reserva.Length < 10)
        {
            if (Usuario.Idioma.CompareTo("Español") == 0)
            {
                textResultadoMesasDisponibles.text = " No hay una fecha puesta.";
            }
            else
            {
                textResultadoMesasDisponibles.text = " There is no date set.";
            }
            Espero1SegundoYQuitoElTextoDeMesasDisponiblesAsync();
            return;
        }



        //string fecha_Reserva = dropDownDías.options[dropDownDías.value].text + "/" + dropDownMeses.options[dropDownMeses.value].text + "/" + dropDownAños.options[dropDownAños.value].text;
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
            if (Usuario.Idioma.CompareTo("Español") == 0)
            {
                textResultadoMesasDisponibles.text = "Error, esa fecha ha pasado.";
            }
            else
            {
                textResultadoMesasDisponibles.text = "Error, that date has passed.";
            }
            Espero1SegundoYQuitoElTextoDeMesasDisponiblesAsync();

            PonerEnDatePickerLaFechaActual();
            return;
        }
        else // Fecha puesta para crear la reserva, CORRECTA
        {
            Debug.Log("++Fecha correcta");
            
            // Si la fecha de la reserva es la misma que la fecha actual, pero la hora ya ha pasado, ERROR
            if (fechaActual == fechaReserva && horaReserva < horaActual)
            {
                Debug.Log("++Error, fecha correcta, pero hora pasada");
                if (Usuario.Idioma.CompareTo("Español") == 0)
                {
                    textResultadoMesasDisponibles.text = "Hora pasada, son las " + DateTime.Now.ToString("HH:mm") + ".";
                }
                else
                {
                    textResultadoMesasDisponibles.text = "Past hour, it's " + DateTime.Now.ToString("HH:mm") + ".";
                }
                Espero1SegundoYQuitoElTextoDeMesasDisponiblesAsync();
                PonerValoresEnLasOpcionesDeCrear("", "", "");
                inputFieldNumMesa.text = "";
                PonerLaHoraActual(hora_Actual);
                return;
            }

            TimeSpan hora_Apertura = TimeSpan.Parse(instanceGestionarMesasController.GetHoraAperturaRestaurante());
            TimeSpan hora_Cierre = TimeSpan.Parse(instanceGestionarMesasController.GetHoraCierreRestaurante());
            // Si la hora de apertura es igual a la hora de cierre
            if (hora_Apertura == hora_Cierre)
            {
                Debug.Log("Resultado hora cierre antes: "+hora_Cierre);
                // Resto 1 minuto a la hora cierre para que calcule bien
                hora_Cierre = hora_Cierre - TimeSpan.FromMinutes(1);
                if (hora_Cierre < TimeSpan.Zero)
                    hora_Cierre += TimeSpan.FromHours(24);
                Debug.Log("Resultado hora cierre: " + hora_Cierre + "; 1 minuto:" + TimeSpan.FromMinutes(1));
            }
            // Si se pone una hora cuando el restaurante no está de servicio, sale error
            if (hora_Apertura > hora_Cierre)
            {
                if (horaReserva > hora_Cierre && horaReserva < hora_Apertura)
                {
                    if (Usuario.Idioma.CompareTo("Español") == 0)
                    {
                        textResultadoMesasDisponibles.text = "Hora incorrecta";
                    }
                    else
                    {
                        textResultadoMesasDisponibles.text = "Wrong hour";
                    }
                    Espero1SegundoYQuitoElTextoDeMesasDisponiblesAsync();
                    PonerLaHoraActual(hora_Actual);
                    return;
                }
            }
            else
            {
                if (horaReserva < hora_Apertura && horaReserva > hora_Cierre || horaReserva > hora_Cierre && horaReserva > hora_Apertura || horaReserva < hora_Cierre && horaReserva < hora_Apertura)
                {
                    if (Usuario.Idioma.CompareTo("Español") == 0)
                    {
                        textResultadoMesasDisponibles.text = "Hora incorrecta";
                    }
                    else
                    {
                        textResultadoMesasDisponibles.text = "Wrong hour";
                    }
                    Espero1SegundoYQuitoElTextoDeMesasDisponiblesAsync();
                    PonerLaHoraActual(hora_Actual);
                    return;
                }
            }
            

            

            List<Mesa> mesasRestaurante = instanceGestionarMesasController.GetMesas();
            if (mesasRestaurante.Count > 0)
            {
                List<Mesa> mesasDisponiblesAUnaFechaYHoraDeterminadas = new List<Mesa>();

                foreach (Mesa mesa in mesasRestaurante)
                {
                    Debug.Log("++Mesa: " + mesa.Mostrar());

                    if (ReservaNuevaNoObstaculizaElResto(mesa.Id, fechaReserva, horaReserva))
                    {
                        mesasDisponiblesAUnaFechaYHoraDeterminadas.Add(mesa);
                    }
                }

                // Si la lista no está vacía, se muestran las mesas disponibles
                if (mesasDisponiblesAUnaFechaYHoraDeterminadas.Count > 0)
                {
                    var mesasOrdenadas = mesasDisponiblesAUnaFechaYHoraDeterminadas.OrderBy(m => m.CantPers).ToList();
                    string cad = " ";
                    int cont = 0;
                    for (int i = 0; i < mesasOrdenadas.Count; i++)
                    {
                        // Si se encuentra una mesa con x número de capacidad de comensales de las disponibles
                        if (mesasOrdenadas[i].CantPers >= cantComensales && cantComensales <= mesasOrdenadas[i].CantPers + 5)
                        {
                            posibleCrearReserva = true;
                            cont++;
                            Button botónMesaSelected = instanceGestionarMesasController.padreDeLosBotonesMesa.gameObject.transform.Find("Button-" + mesasOrdenadas[i].Id).GetComponent<Button>();
                            int id_Mesa_En_Mapa = int.Parse(instanceGestionarMesasController.ObtenerIDMesaDelMapa(botónMesaSelected));
                            mesasDisponiblesEnMapa.Add(id_Mesa_En_Mapa);
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
                        if (Usuario.Idioma.CompareTo("Español") == 0)
                        {
                            textResultadoMesasDisponibles.text = " Ninguna";
                        }
                        else
                        {
                            textResultadoMesasDisponibles.text = " None";
                        }
                        Espero1SegundoYQuitoElTextoDeMesasDisponiblesAsync();
                        PonerValoresEnLasOpcionesDeCrear("", "", "");
                        PonerEnDatePickerLaFechaActual();
                    }
                }
                else // Ninguna mesa disponible en esa fecha a esa hora
                {
                    if (Usuario.Idioma.CompareTo("Español") == 0)
                    {
                        textResultadoMesasDisponibles.text = " Ninguna";
                    }
                    else
                    {
                        textResultadoMesasDisponibles.text = " None";
                    }
                    Espero1SegundoYQuitoElTextoDeMesasDisponiblesAsync();
                    PonerValoresEnLasOpcionesDeCrear("", "", "");
                    PonerEnDatePickerLaFechaActual();
                }
            }
            else
            {
                Debug.Log("++No hay mesas");
            }

        }
    }

    private void PonerLaHoraActual(string hora_Actual)
    {
        // Pongo la hora actual
        string[] horaActualArray = hora_Actual.Split(":");
        string hora = horaActualArray[0].Trim();
        string minuto = horaActualArray[1].Trim();

        BuscoElIndiceYLoPongoSiLoEncuentro(dropDownHoras, hora);
        BuscoElIndiceYLoPongoSiLoEncuentro(dropDownMinutos, minuto);
    }

    private void PonerEnDatePickerLaFechaActual()
    {
        string fechaFormatoGlobal = DateTime.Now.ToString("yyyy-MM-dd");
        datePicker.SelectedDate = DateTime.Parse(fechaFormatoGlobal.Trim());
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
        string fecha = textValorDíaEnCrear.text.Trim();
        string hora = textValorHoraEnCrear.text.Trim();
        string cant_Comensales = textValorNumComensalesEnCrear.text.Trim();
        string num_Mesa = inputFieldNumMesa.text;

        Button botónMesaSelected = ObtenerBotónMesaAPartirDeSuIDdelMapa(num_Mesa);
        int mesa_ID = instanceGestionarMesasController.ObtenerIDMesaDelNombreDelBotónMesa(botónMesaSelected);
        
        Debug.Log("++Crear Reserva Presionado: " + nombre + ", " + dni + ", " + teléfono+", "+num_Mesa+", ID_MESA en BDD: "+mesa_ID);

        Cliente cliente = new Cliente(nombre, dni, teléfono);
        Reserva reserva = new Reserva(fecha, hora, ""+EstadoReserva.Confirmada, int.Parse(cant_Comensales), 0, mesa_ID, cliente);

        ObtenerMesasDisponibles();

        if (posibleCrearReserva && Hay7De7CamposRellenos())
        {
            Debug.Log("+ +Se crea la reserva con los datos del cliente");
            GestionarCrearReserva(reserva); // Se crea la reserva con los datos del cliente
        }
        else
        {
            Debug.Log("+ +Se cancela la creación de la reserva porque no cumple ciertas características");
        }
        
        
    }

    private Button ObtenerBotónMesaAPartirDeSuIDdelMapa(string num_Mesa)
    {
        foreach (Transform hijo in instanceGestionarMesasController.padreDeLosBotonesMesa)
        {
            // Suponemos que cada hijo es un botón
            TMP_Text textRectangleButton = hijo.Find("Imagen Rectangle/Text").GetComponent<TMP_Text>();

            if (textRectangleButton.text.CompareTo(num_Mesa) == 0)
            {
                return hijo.GetComponent<Button>();
            }
        }
        return null;
    }

    private async void GestionarCrearReserva(Reserva reserva)
    {
        // Intento registrar la reserva de la mesa enviando datos al servidor. Pongo pendiente porque la reserva es para ahora mismo (en uso)
        string cad = await instanceMétodosApiController.PostDataAsync("reserva/crearReserva", reserva);

        // Deserializo la respuesta
        Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);
        if (resultado.Result.Equals(1))
        {
            await instanceMétodosApiController.PutDataAsync("reserva/actualizarBoolCreandoReserva", new Resultado(1));
            Debug.Log("+ +Reserva registrada correctamente en mesa: " + reserva.Mesa_Id);
            if (Usuario.Idioma.CompareTo("Español") == 0)
            {
                textResultadoMesasDisponibles.text = "Reserva registrada";// correctamente";
            }
            else
            {
                textResultadoMesasDisponibles.text = "Reservation registered";// successfully registered";
            }
            Espero1SegundoYQuitoElTextoDeMesasDisponiblesAsync();
            PonerEnDatePickerLaFechaActual();
            Poner4ValoresEnCrearVacíos();
            PonerValoresEnLasOpcionesDeCrear("", "", "");
            AsignarValoresConcretosEnDropdowns();
        }
        else
        {
            Debug.Log("+ +Error al registrar reserva en mesa");
        }
    }

    public void Poner4ValoresEnCrearVacíos()
    {
        inputFieldNombre.text = "";
        inputFieldDni.text = "";
        inputFieldNumTeléfono.text = "";
        inputFieldNumMesa.text = "";
    }

    public void DesactivarCanvasCrearReserva()
    {
        canvasCrearReserva.SetActive(false);
        textResultadoMesasDisponibles.text = "";
        // Si hay una fecha puesta, se actualiza a la actual.
        if (inputFieldFecha.text.Trim().Length > 5)
        {
            PonerEnDatePickerLaFechaActual();
        }
    }

    public void ActivarCanvasInfoMesasDisponibles()
    {
        canvasInfoMesasDisponibles.SetActive(true);
    }

    public void DesactivarCanvasInfoMesasDisponibles()
    {
        canvasInfoMesasDisponibles.SetActive(false);
    }

    /// <summary>
    /// Método para cambiar de componente con TAB en la interfaz gráfica.
    /// </summary>
    private void SelectNextInputField()
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;

        for (int i = 0; i < inputFields.Length; i++)
        {
            if (inputFields[i].gameObject == current)
            {
                int nextIndex = (i + 1) % inputFields.Length;
                inputFields[nextIndex].Select();
                break;
            }
        }
    }
}
