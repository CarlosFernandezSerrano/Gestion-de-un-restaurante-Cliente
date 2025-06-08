using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Cambios: Se ha vuelto public el botónMesaSeleccionado para poder pasarlo así a gestionar Pedidos, se ha añadido 2 botones y sus handlers al final, se han añadido los canvas necesarios para pasar a las otras pantallas

public class GestionarMesasController : MonoBehaviour
{
    [SerializeField] private TMP_Text textNombreRestaurante;
    [SerializeField] private TMP_Text textHoraActual;
    [SerializeField] private GameObject tmpInputFieldPrefab; // Prefab de InputField TMP
    [SerializeField] private TMP_Text textHoraApertura;
    [SerializeField] private TMP_Text textHoraCierre;
    [SerializeField] private GameObject contenedorInfoReservasMesa;
    [SerializeField] private Button buttonNoDisponible;
    [SerializeField] private Button buttonDisponible;
    [SerializeField] private GameObject canvasReservasHoyMesa;
    [SerializeField] private Scrollbar scrollbarReservasMesaHoy;
    [SerializeField] private RectTransform rectTransformContent;
    [SerializeField] private TMP_Text textReservasHoyMesa;
    [SerializeField] private GameObject canvasCrearReserva;
    [SerializeField] private GameObject canvasBuscarReserva;
    [SerializeField] private GameObject canvasHistorialReservas;
    [SerializeField] private GameObject canvasPedidos;
    [SerializeField] private GameObject canvasListaPedidos;
    [SerializeField] private Scrollbar scrollbarHistorialReservas; 
    [SerializeField] private RectTransform rectTransformContentHistorialReservas;
    [SerializeField] private Sprite imgCuadradoBordeNegroFino;
    [SerializeField] private GameObject imgConTextoDeBuscarReserva;
    [SerializeField] private GameObject ImgYTextoCrearReserva;
    [SerializeField] private GameObject ImgYTextoListaPedidos;
    [SerializeField] private GameObject ImgYTextoHistorialReservas;
    public GestionarPedidosController instanceGestionarPedidosController;
    public GestionarListaPedidos instanceGestionarListaPedidos;

    private List<Mesa> Mesas;

    private int lastIDMesa = 0;
    private string colorHexadecimalVerde = "#00B704";
    private string colorHexadecimalRojo = "#A12121";
    public Button botónMesaSeleccionado; 

    private int contMostrarBotonesMesa = 1;

    // Contenedor padre donde se agregarán los botones
    public RectTransform padreDeLosBotonesMesa;

    // Sprite que cargo desde Resources.
    private Sprite mesaSprite;

    MétodosAPIController instanceMétodosApiController;
    CrearReservaController instanceCrearReservaController;

    public static GestionarMesasController InstanceGestionarMesasController { get; private set; }

    void Awake()
    {
        if (InstanceGestionarMesasController == null)
        {
            InstanceGestionarMesasController = this;
        }

        SceneManager.LoadSceneAsync("General Controller", LoadSceneMode.Additive);
    }

    // Start is called before the first frame update
    void Start()
    {
        instanceMétodosApiController = MétodosAPIController.InstanceMétodosAPIController;
        instanceCrearReservaController = CrearReservaController.InstanceCrearReservaController;

        TrabajadorController.ComprobandoDatosTrabajador = false;

        InvokeRepeating(nameof(ActualizarHora), 0f, 1f); // Llama a ActualizarHora() cada 1 segundo

        InvokeRepeating(nameof(ObtenerDatosRestauranteAsync), 0f, 0.3f); // Llama a ObtenerDatosRestauranteAsync() cada 1 segundo

        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ActualizarHora()
    {
        textHoraActual.text = DateTime.Now.ToString("HH:mm");
    }

    private async void ObtenerDatosRestauranteAsync()
    {
        string cad = await instanceMétodosApiController.GetDataAsync("restaurante/getRestaurantePorId/" + Usuario.Restaurante_ID);

        // Deserializo la respuesta
        Restaurante restaurante = JsonConvert.DeserializeObject<Restaurante>(cad);



        textHoraApertura.text = restaurante.HoraApertura;
        textHoraCierre.text = restaurante.HoraCierre;
        Restaurante.TiempoPermitidoParaComer = restaurante.TiempoParaComer;
        Mesas = restaurante.Mesas;



        string nombreRest = restaurante.Nombre;

        switch (nombreRest.Length)
        {
            case <= 10:
                textNombreRestaurante.fontSize = 56;
                break;
            case <= 12:
                textNombreRestaurante.fontSize = 50;
                break;
            case <= 15:
                textNombreRestaurante.fontSize = 43;
                break;
            case 16:
                textNombreRestaurante.fontSize = 38;
                break;
            case 17:
                textNombreRestaurante.fontSize = 35;
                break;
        }
        textNombreRestaurante.text = nombreRest;

        Debug.Log("Hora Apertura: " + restaurante.HoraApertura + "; Hora Cierre: " + restaurante.HoraCierre);

        ActualizarEstadoReservaYMesaDelDíaDeHoyAsync();

        // Los botones mesa sólo se pintan una vez
        if (contMostrarBotonesMesa.Equals(1)){
            contMostrarBotonesMesa++;
            mesaSprite = Resources.Load<Sprite>("Editar Restaurante/mantelMesa");
            CrearBotonesMesas();
            await ActualizarTodasLasReservasQueYaAcabaronYEstánPendientesAsync();
            ActualizoLasMesasQueNoTienenReservasPendientesYEstánOcupadasAsync();
            AñadirListenerABotonesMesaDelMapa();
        }
        
    }

    private async void ActualizoLasMesasQueNoTienenReservasPendientesYEstánOcupadasAsync()
    {
        foreach (Mesa mesa in Mesas)
        {
            // Actualizo la mesa si no tiene ninguna reserva en uso y está puesta en ocupada
            if (LaMesaNoTieneNingunaReservaPendienteHoy(mesa.Id) && mesa.Disponible == false)
            {
                string cad2 = await instanceMétodosApiController.PutDataAsync("mesa/actualizarCampoDisponible", new Mesa(mesa.Id, 0, 0, 0, 0, 0, 0, 0, true, 0, new List<Reserva>()));

                // Deserializo la respuesta
                Resultado resultado2 = JsonConvert.DeserializeObject<Resultado>(cad2);

                if (resultado2.Result.Equals(1))
                {
                    Debug.Log("Mesa puesta en disponible correctamente");

                    Image img = padreDeLosBotonesMesa.gameObject.transform.Find("Button-" + mesa.Id + "/Imagen Circle").GetComponent<Image>();
                    PonerColorCorrectoAImg(img, colorHexadecimalVerde);
                }
                else
                {
                    Debug.Log("Error al intentar actualizar la mesa");
                }
            }
        }
    }

    private async Task ActualizarTodasLasReservasQueYaAcabaronYEstánPendientesAsync()
    {
        DateTime fechaHoy = DateTime.Today;
        foreach (Mesa mesa in Mesas)
        {
            foreach (Reserva reserva in mesa.Reservas)
            {
                DateTime fechaReserva = DateTime.Parse(reserva.Fecha);

                if (fechaReserva < fechaHoy && reserva.Estado.CompareTo(""+EstadoReserva.Pendiente) == 0)
                {
                    string cad = await instanceMétodosApiController.PutDataAsync("reserva/actualizarEstadoReserva", new Reserva(reserva.Id, "", "", "" + EstadoReserva.Terminada, 0, 0, reserva.Mesa_Id, reserva.Cliente));

                    // Deserializo la respuesta
                    Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

                    if (resultado.Result.Equals(1))
                    {
                        Debug.Log("Reserva terminada correctamente");
                    }
                    else
                    {
                        Debug.Log("Error al intentar terminar una reserva");
                    }
                }
            }
        }
    }

    private bool LaMesaNoTieneNingunaReservaPendienteHoy(int mesa_Id)
    {
        // Obtengo las reservas pendientes que tiene la mesa, ya sean de hoy o en adelante
        List<Reserva> reservasMesaPendientes = ObtenerReservasMesaDeHoyEnAdelante(mesa_Id);
        // Obtengo las reservas del día de hoy
        List<Reserva> reservasMesaParaHoy = ObtenerReservasMesaParaHoy(reservasMesaPendientes);
        int cont = 0;
        foreach (Reserva reserva in reservasMesaParaHoy)
        {
            // Si encuentra una reserva de hoy pendiente, sumo el contador
            if (reserva.Estado.CompareTo(""+EstadoReserva.Pendiente) == 0)
            {
                cont++;
            }
        }
        // Existe ahora mismo una mesa pendiente
        if (cont > 0)
        {
            return false;
        }
        else
        {
            return true;
        }

    }

    // Si una reserva ha acabado, se actualiza la mesa y la reserva. Los colores se actualizan aquí aunque se podrían actualizar aparte
    private async void ActualizarEstadoReservaYMesaDelDíaDeHoyAsync() 
    {                                                       
        // Compruebo si ya ha pasado su tiempo límite
        string horaActual = DateTime.Now.ToString("HH:mm");
        TimeSpan horaActualTimeSpan = TimeSpan.Parse(horaActual);

        foreach (Mesa mesa in Mesas)
        {
            // Obtengo las reservas pendientes que tiene la mesa, ya sean de hoy o en adelante
            List<Reserva> reservasMesaHoyEnAdelante = ObtenerReservasMesaDeHoyEnAdelante(mesa.Id);
            // Obtengo las reservas del día de hoy
            List<Reserva> reservasMesaParaHoy = ObtenerReservasMesaParaHoy(reservasMesaHoyEnAdelante);

            List<Reserva> reservasPendientesEnUnaMesaHoy = new List<Reserva>();
            foreach (Reserva reserva in reservasMesaParaHoy)
            {
                string horaReserva = reserva.Hora;
                TimeSpan horaReservaTimeSpan = TimeSpan.Parse(horaReserva);

                string[] tiempoPermitido = Restaurante.TiempoPermitidoParaComer.Split(":");
                int horas = int.Parse(tiempoPermitido[0].Trim());
                int minutos = int.Parse(tiempoPermitido[1].Trim());
                //Debug.Log("Resultado tiempo permitido - Horas:" + horas + "; Minutos:" + minutos + "*");

                // Sumo el tiempo que puso el gerente en la escena "Editar Restaurante" en tiempo permitido para comer
                TimeSpan sumaTiempo = TimeSpan.FromHours(horas) + TimeSpan.FromMinutes(minutos);
                TimeSpan horaFinReserva = horaReservaTimeSpan.Add(sumaTiempo);

                // Termino todas las reservas pendientes menos la última
                if (Restaurante.TiempoPermitidoParaComer.CompareTo("00:00") == 0 && reserva.Estado.CompareTo("" + EstadoReserva.Pendiente) == 0)
                {
                    reservasPendientesEnUnaMesaHoy.Add(reserva);
                }

                // Si el tiempo límite es distinto de 00:00
                if (Restaurante.TiempoPermitidoParaComer.CompareTo("00:00") != 0)
                {
                    // Obtengo las reservas de la mesa que acabaron hoy y con estado "Pendiente" o "Confirmada" para finalizarlas
                    if (horaFinReserva <= horaActualTimeSpan && reserva.Estado.CompareTo("" + EstadoReserva.Pendiente) == 0 || horaFinReserva <= horaActualTimeSpan && reserva.Estado.CompareTo("" + EstadoReserva.Confirmada) == 0)
                    {
                        string cad = await instanceMétodosApiController.PutDataAsync("reserva/actualizarEstadoReserva", new Reserva(reserva.Id, "", "", "" + EstadoReserva.Terminada, 0, 0, reserva.Mesa_Id, reserva.Cliente));

                        // Deserializo la respuesta
                        Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

                        if (resultado.Result.Equals(1))
                        {
                            Debug.Log("Reserva terminada correctamente");
                        }
                        else
                        {
                            Debug.Log("Error al intentar terminar una reserva");
                        }

                        string cad2 = await instanceMétodosApiController.PutDataAsync("mesa/actualizarCampoDisponible", new Mesa(reserva.Mesa_Id, 0, 0, 0, 0, 0, 0, 0, true, 0, new List<Reserva>()));

                        // Deserializo la respuesta
                        Resultado resultado2 = JsonConvert.DeserializeObject<Resultado>(cad2);

                        if (resultado2.Result.Equals(1))
                        {
                            Debug.Log("Mesa puesta en disponible correctamente");

                            Image img = padreDeLosBotonesMesa.gameObject.transform.Find("Button-" + reserva.Mesa_Id + "/Imagen Circle").GetComponent<Image>();
                            PonerColorCorrectoAImg(img, colorHexadecimalVerde);
                        }
                        else
                        {
                            Debug.Log("Error al intentar actualizar la mesa");
                        }
                    }
                }
                

                PonerReservaConfirmadaEnUsoAsync(reservasMesaParaHoy);

                // Si el tiempo límite es distinto de 00:00
                if (Restaurante.TiempoPermitidoParaComer.CompareTo("00:00") != 0)
                {
                    // Pongo la mesa en no disponible si existe una reserva pendiente ahora
                    if (ExisteUnaReservaPendienteAhora(reservasMesaParaHoy, horaActualTimeSpan))
                    {
                        Debug.Log("- -Pasa por if");
                        // Actualizo la mesa en la BDD en "Disponible" = false 
                        string cad = await instanceMétodosApiController.PutDataAsync("mesa/actualizarCampoDisponible", new Mesa(reserva.Mesa_Id, 0, 0, 0, 0, 0, 0, 0, false, 0, new List<Reserva>()));

                        // Deserializo la respuesta
                        Resultado resultado2 = JsonConvert.DeserializeObject<Resultado>(cad);

                        if (resultado2.Result.Equals(1))
                        {
                            Debug.Log("Mesa puesta en no disponible correctamente");
                        }
                        else
                        {
                            Debug.Log("Error al intentar actualizar la mesa");
                        }

                        try
                        {
                            Image img = padreDeLosBotonesMesa.gameObject.transform.Find("Button-" + reserva.Mesa_Id + "/Imagen Circle").GetComponent<Image>();
                            PonerColorCorrectoAImg(img, colorHexadecimalRojo);
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("Exception: " + ex);
                        }
                    }
                    else // No hay ninguna reserva ahora mismo en uso, pongo la mesa en disponible
                    {
                        Debug.Log("- -Pasa por else");
                        // Actualizo la mesa en la BDD en "Disponible" = true 
                        string cad = await instanceMétodosApiController.PutDataAsync("mesa/actualizarCampoDisponible", new Mesa(reserva.Mesa_Id, 0, 0, 0, 0, 0, 0, 0, true, 0, new List<Reserva>()));

                        // Deserializo la respuesta
                        Resultado resultado2 = JsonConvert.DeserializeObject<Resultado>(cad);

                        if (resultado2.Result.Equals(1))
                        {
                            Debug.Log("Mesa puesta en disponible correctamente");
                        }
                        else
                        {
                            Debug.Log("Error al intentar actualizar la mesa");
                        }

                        try
                        {
                            Image img = padreDeLosBotonesMesa.gameObject.transform.Find("Button-" + reserva.Mesa_Id + "/Imagen Circle").GetComponent<Image>();
                            PonerColorCorrectoAImg(img, colorHexadecimalVerde);
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("Exception: " + ex);
                        }
                        
                    }
                }
                else // Si límite de tiempo es = 0
                {
                    // Pongo la mesa en no disponible si existe una reserva pendiente ahora
                    if (ExisteUnaReservaPendienteAhoraParaUnaMesaSinTiempoLímite(reservasMesaParaHoy, horaActualTimeSpan))
                    {
                        Debug.Log("- -Pasa por if");
                        // Actualizo la mesa en la BDD en "Disponible" = false 
                        string cad = await instanceMétodosApiController.PutDataAsync("mesa/actualizarCampoDisponible", new Mesa(reserva.Mesa_Id, 0, 0, 0, 0, 0, 0, 0, false, 0, new List<Reserva>()));

                        // Deserializo la respuesta
                        Resultado resultado2 = JsonConvert.DeserializeObject<Resultado>(cad);

                        if (resultado2.Result.Equals(1))
                        {
                            Debug.Log("Mesa puesta en no disponible correctamente");
                        }
                        else
                        {
                            Debug.Log("Error al intentar actualizar la mesa");
                        }

                        Image img = padreDeLosBotonesMesa.gameObject.transform.Find("Button-" + reserva.Mesa_Id + "/Imagen Circle").GetComponent<Image>();
                        PonerColorCorrectoAImg(img, colorHexadecimalRojo);

                    }
                    else // No hay ninguna reserva ahora mismo en uso, pongo la mesa en disponible
                    {
                        Debug.Log("- -Pasa por else");
                        // Actualizo la mesa en la BDD en "Disponible" = true 
                        string cad = await instanceMétodosApiController.PutDataAsync("mesa/actualizarCampoDisponible", new Mesa(reserva.Mesa_Id, 0, 0, 0, 0, 0, 0, 0, true, 0, new List<Reserva>()));

                        // Deserializo la respuesta
                        Resultado resultado2 = JsonConvert.DeserializeObject<Resultado>(cad);

                        if (resultado2.Result.Equals(1))
                        {
                            Debug.Log("Mesa puesta en disponible correctamente");
                        }
                        else
                        {
                            Debug.Log("Error al intentar actualizar la mesa");
                        }

                        Image img = padreDeLosBotonesMesa.gameObject.transform.Find("Button-" + reserva.Mesa_Id + "/Imagen Circle").GetComponent<Image>();
                        PonerColorCorrectoAImg(img, colorHexadecimalVerde);
                    }
                }
            }
            // Si hay más de una reserva con estado "Pendiente" en una mesa
            if (reservasPendientesEnUnaMesaHoy.Count > 1)
            {
                await TerminoTodasLasReservasPendientesMenosLaÚltima(reservasPendientesEnUnaMesaHoy);
            }
        }
    }

    private bool ExisteUnaReservaPendienteAhoraParaUnaMesaSinTiempoLímite(List<Reserva> reservasMesaParaHoy, TimeSpan horaActualTimeSpan)
    {
        foreach (Reserva reserva in reservasMesaParaHoy)
        {
            string horaReserva = reserva.Hora;
            TimeSpan horaReservaTimeSpan = TimeSpan.Parse(horaReserva);

            // Si hay una reserva pendiente en uso, actualizo sólo la mesa a "No Disponible" porque la reserva ya está creada
            if (horaActualTimeSpan >= horaReservaTimeSpan && reserva.Estado.CompareTo("" + EstadoReserva.Pendiente) == 0)
            {
                return true;
            }
        }
        return false;
    }

    private async Task TerminoTodasLasReservasPendientesMenosLaÚltima(List<Reserva> reservasPendientesEnUnaMesaHoy)
    {
        var reservasOrdenadas = reservasPendientesEnUnaMesaHoy.OrderBy(r => TimeSpan.Parse(r.Hora)).ToList();

        Debug.Log("Muestro las reservas pendientes ordenadas según la hora:");
        foreach (Reserva reserva in reservasOrdenadas)
        {
            Debug.Log("Muestro las reservas pendientes ordenadas según la hora:"+reserva.Mostrar());
        }

        for (int i = 0; i < reservasPendientesEnUnaMesaHoy.Count - 1; i++)
        {
            Debug.Log("Reserva en lista: "+reservasPendientesEnUnaMesaHoy[i].Mostrar());
            string cad = await instanceMétodosApiController.PutDataAsync("reserva/actualizarEstadoReserva", new Reserva(reservasPendientesEnUnaMesaHoy[i].Id, "", "", "" + EstadoReserva.Terminada, 0, 0, reservasPendientesEnUnaMesaHoy[i].Mesa_Id, reservasPendientesEnUnaMesaHoy[i].Cliente));

            // Deserializo la respuesta
            Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

            if (resultado.Result.Equals(1))
            {
                Debug.Log("Reserva terminada correctamente");
            }
            else
            {
                Debug.Log("Error al intentar terminar una reserva");
            }
        }
    }

    // Cambio el estado de la reserva de "Confirmada" a "Pendiente" (en uso)
    private async void PonerReservaConfirmadaEnUsoAsync(List<Reserva> reservasMesaParaHoy) 
    {
        string horaActual = DateTime.Now.ToString("HH:mm");
        TimeSpan horaActualTimeSpan = TimeSpan.Parse(horaActual);

        foreach (Reserva reserva in reservasMesaParaHoy)
        {
            string horaReserva = reserva.Hora;
            TimeSpan horaReservaTimeSpan = TimeSpan.Parse(horaReserva);

            if (Restaurante.TiempoPermitidoParaComer.CompareTo("00:00") == 0 && horaActualTimeSpan >= horaReservaTimeSpan && reserva.Estado.CompareTo("" + EstadoReserva.Confirmada) == 0)
            {
                string cad = await instanceMétodosApiController.PutDataAsync("reserva/actualizarEstadoReserva", new Reserva(reserva.Id, "", "", "" + EstadoReserva.Pendiente, 0, 0, reserva.Mesa_Id, reserva.Cliente));

                // Deserializo la respuesta
                Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

                if (resultado.Result.Equals(1))
                {
                    Debug.Log("Reserva actualizada de confirmada a pendiente correctamente");
                }
                else
                {
                    Debug.Log("Error al intentar poner en uso (pendiente) una reserva");
                }

                continue;
            }

            string[] tiempoPermitido = Restaurante.TiempoPermitidoParaComer.Split(":");
            int horas = int.Parse(tiempoPermitido[0].Trim());
            int minutos = int.Parse(tiempoPermitido[1].Trim());
            //Debug.Log("Resultado tiempo permitido - Horas:" + horas + "; Minutos:" + minutos + "*");

            // Sumo el tiempo que puso el gerente en la escena "Editar Restaurante" en tiempo permitido para comer
            TimeSpan sumaTiempo = TimeSpan.FromHours(horas) + TimeSpan.FromMinutes(minutos);
            TimeSpan horaFinReserva = horaReservaTimeSpan.Add(sumaTiempo);

            // Si hay una reserva "Confirmada" que debería estar en uso, se pone en "Pendiente"
            if (horaActualTimeSpan < horaFinReserva && horaActualTimeSpan >= horaReservaTimeSpan && reserva.Estado.CompareTo("" + EstadoReserva.Confirmada) == 0)
            {
                string cad = await instanceMétodosApiController.PutDataAsync("reserva/actualizarEstadoReserva", new Reserva(reserva.Id, "", "", "" + EstadoReserva.Pendiente, 0, 0, reserva.Mesa_Id, reserva.Cliente));

                // Deserializo la respuesta
                Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

                if (resultado.Result.Equals(1))
                {
                    Debug.Log("Reserva actualizada de confirmada a pendiente correctamente");
                }
                else
                {
                    Debug.Log("Error al intentar poner en uso (pendiente) una reserva");
                }
            }
        }
    }

    private bool ExisteUnaReservaPendienteAhora(List<Reserva> reservasMesaParaHoy, TimeSpan horaActualTimeSpan)
    {        
        foreach (Reserva reserva in reservasMesaParaHoy)
        {
            string horaReserva = reserva.Hora;
            TimeSpan horaReservaTimeSpan = TimeSpan.Parse(horaReserva);

            string[] tiempoPermitido = Restaurante.TiempoPermitidoParaComer.Split(":");
            int horas = int.Parse(tiempoPermitido[0].Trim());
            int minutos = int.Parse(tiempoPermitido[1].Trim());
            Debug.Log("Resultado tiempo permitido - Horas:" + horas + "; Minutos:" + minutos + "*");

            // Sumo el tiempo que puso el gerente en la escena "Editar Restaurante" en tiempo permitido para comer
            TimeSpan sumaTiempo = TimeSpan.FromHours(horas) + TimeSpan.FromMinutes(minutos);
            TimeSpan horaFinReserva = horaReservaTimeSpan.Add(sumaTiempo);

            // Si hay una reserva pendiente en uso, actualizo sólo la mesa a "No Disponible" porque la reserva ya está creada
            if (horaActualTimeSpan < horaFinReserva && horaActualTimeSpan >= horaReservaTimeSpan && reserva.Estado.CompareTo(""+EstadoReserva.Pendiente) == 0)
            {
                return true;
            }
        }
        return false;
    }

    private void AñadirListenerABotonesMesaDelMapa()
    {
        // Obtenemos todos los componentes Button que sean hijos del contenedor
        Button[] buttons = padreDeLosBotonesMesa.gameObject.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            // Es recomendable capturar la referencia del botón para evitar problemas con clausuras
            Button capturedButton = button;
            capturedButton.onClick.AddListener(() => MostrarContenedorInfoReservasMesa(capturedButton));
        }
    }

    private void MostrarContenedorInfoReservasMesa(Button capturedButton)
    {
        botónMesaSeleccionado = capturedButton; // Obtengo el botón mesa que he pulsado

        // Configuro los botones del contenedor info Mesa según si está disponible o no la mesa
        int id_Mesa = ObtenerIDMesaDelNombreDelBotónMesa(botónMesaSeleccionado);
        bool mesaDisponible = BuscarSiLaMesaEstáDisponible(id_Mesa);
        if (mesaDisponible)
        {
            buttonNoDisponible.interactable = true;
            buttonDisponible.interactable = false;
        }
        else
        {
            buttonNoDisponible.interactable = false;
            buttonDisponible.interactable = true;
        }

        // Activo el contendor Info Reservas Mesa
        contenedorInfoReservasMesa.SetActive(true);
    }

    private bool BuscarSiLaMesaEstáDisponible(int idMesa)
    {
        DateTime fechaHoy = DateTime.Today; // Día de hoy

        foreach (Mesa mesa in Mesas)
        {
            // Si se encuentra la mesa que estoy buscando, se obtiene su valor del atributo "Disponible"
            if (mesa.Id.Equals(idMesa))
            {
                return mesa.Disponible;
            }
        }
        return false;
    }

    public void DesactivarContenedorInfoReservasMesa()
    {
        contenedorInfoReservasMesa.SetActive(false);
    }

    public void PonerNoDisponibleMesa()
    {
        PonerReservaAMesaParaAhoraAsync();
    }

    // Si pulso el botón "Disponible" quiere decir que hay una reserva en curso y quiero cancelarla
    public void PonerDisponibleMesa()
    {
        CancelarReservaActualEnMesaAsync();
    }

    private async void CancelarReservaActualEnMesaAsync()
    {
        Debug.Log("--------------------------------");

        Button botónMesaSelected = botónMesaSeleccionado;

        int id_Mesa = ObtenerIDMesaDelNombreDelBotónMesa(botónMesaSelected);

        // Obtengo las reservas pendientes que tiene la mesa, ya sean de hoy o en adelante
        List<Reserva> reservasMesaPendientes = ObtenerReservasMesaDeHoyEnAdelante(id_Mesa);

        foreach (Reserva r in reservasMesaPendientes)
        {
            Debug.Log("Reservas mesa pendientes entre hoy y en adelante: " + r.Mostrar());
        }

        // Obtengo las reservas del día de hoy
        List<Reserva> reservasMesaParaHoy = ObtenerReservasMesaParaHoy(reservasMesaPendientes);

        // Obtengo la reserva actual en uso
        Reserva reserva = ObtenerReservaEnUso(reservasMesaParaHoy);
        
        if (reserva != null)
        {
            Debug.Log("Reserva not null");

            // Una vez obtenida la reserva pendiente, la cancelo en la BDD
            string cad = await instanceMétodosApiController.PutDataAsync("reserva/actualizarEstadoReserva", new Reserva(reserva.Id, "", "", ""+EstadoReserva.Cancelada, 0, 0, id_Mesa, reserva.Cliente));

            // Deserializo la respuesta
            Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

            if (resultado.Result.Equals(1))
            {
                Debug.Log("Reserva cancelada correctamente");
                // También actualizo la mesa en la BDD en "Disponible" = true 
                string cad2 = await instanceMétodosApiController.PutDataAsync("mesa/actualizarCampoDisponible", new Mesa(id_Mesa, 0, 0, 0, 0, 0, 0, 0, true, 0, new List<Reserva>()));

                // Deserializo la respuesta
                Resultado resultado2 = JsonConvert.DeserializeObject<Resultado>(cad2);

                if (resultado2.Result.Equals(1))
                {
                    Debug.Log("Mesa puesta en disponible correctamente");
                    Image img = botónMesaSeleccionado.gameObject.transform.Find("Imagen Circle").GetComponent<Image>();
                    PonerColorCorrectoAImg(img, colorHexadecimalVerde);
                    contenedorInfoReservasMesa.SetActive(false);
                }
                else
                {
                    Debug.Log("Error al intentar actualizar la mesa");
                }
            }
            else
            {
                Debug.Log("Error al intentar cancelar una reserva");
            }
        }
        else
        {
            Debug.Log("Reserva null");
        }                
    }

    private Reserva ObtenerReservaEnUso(List<Reserva> reservasMesaHoyPendientes)
    {
        string horaActual = DateTime.Now.ToString("HH:mm");
        TimeSpan horaActualTimeSpan = TimeSpan.Parse(horaActual);

        foreach (Reserva reserva in reservasMesaHoyPendientes)
        {
            string horaReserva = reserva.Hora;
            TimeSpan horaReservaTimeSpan = TimeSpan.Parse(horaReserva);

            string[] tiempoPermitido = Restaurante.TiempoPermitidoParaComer.Split(":");
            int horas = int.Parse(tiempoPermitido[0].Trim());
            int minutos = int.Parse(tiempoPermitido[1].Trim());
            Debug.Log("Resultado tiempo permitido - Horas:" + horas + "; Minutos:" + minutos + "*");

            // Sumo el tiempo que puso el gerente en la escena "Editar Restaurante" en tiempo permitido para comer
            TimeSpan sumaTiempo = TimeSpan.FromHours(horas) + TimeSpan.FromMinutes(minutos);
            TimeSpan horaFinReserva = horaReservaTimeSpan.Add(sumaTiempo);

            if (horaActualTimeSpan < horaFinReserva && horaActualTimeSpan >= horaReservaTimeSpan && reserva.Estado.CompareTo("" + EstadoReserva.Pendiente) == 0)
            {
                return reserva;
            }
            if (Restaurante.TiempoPermitidoParaComer.CompareTo("00:00") == 0 && reserva.Estado.CompareTo("" + EstadoReserva.Pendiente) == 0)
            {
                return reserva;
            }
        }
        return null;        
    }

    // Obtengo las reservas del día de hoy (pendiente y confirmadas)
    private List<Reserva> ObtenerReservasMesaParaHoy(List<Reserva> reservasMesaPendientes)
    {
        List<Reserva> reservas = new List<Reserva>();
        DateTime fechaHoy = DateTime.Today;
        foreach (Reserva reserva in reservasMesaPendientes)
        {
            DateTime fechaReserva = DateTime.Parse(reserva.Fecha); 

            // Si la fecha de la reserva es hoy y la reserva está confirmada o pendiente, se obtiene
            if (fechaReserva == fechaHoy && reserva.Estado.CompareTo(""+EstadoReserva.Confirmada) == 0 || fechaReserva == fechaHoy && reserva.Estado.CompareTo("" + EstadoReserva.Pendiente) == 0 || fechaReserva == fechaHoy && reserva.Estado.CompareTo("" + EstadoReserva.Cancelada) == 0)
            {
                reservas.Add(reserva);
            }
        }
        return reservas;
    }

    // Obtengo las reservas pendientes que tiene la mesa, ya sean de hoy o en adelante sin importar el estado
    public List<Reserva> ObtenerReservasMesaDeHoyEnAdelante(int id_Mesa)
    {
        List<Reserva> reservas = new List<Reserva>();
        DateTime fechaHoy = DateTime.Today;

        // Recorro todas las mesas del restaurante
        foreach (var mesa in Mesas)
        {
            // Si encuentro la mesa que estoy buscando, obtengo sus reservas
            if (mesa.Id.Equals(id_Mesa))
            {
                // Muestro las reservas que tiene la mesa
                foreach (var reserva in mesa.Reservas)
                {
                    //Debug.Log("Reservas mesa " + mesa.Id + ": " + reserva.Mostrar());
                    DateTime fechaReserva = DateTime.Parse(reserva.Fecha);
                    //Debug.Log("Fecha Hoy: " + fechaHoy + "; Fecha Reserva: " + fechaReserva);
                    if (fechaReserva >= fechaHoy)
                    {
                        reservas.Add(reserva);
                    }
                }
                return reservas;                
            }            
        }
        return reservas;
    }

    public int ObtenerIDMesaDelNombreDelBotónMesa(Button botónMesaSelected)
    {
        string[] nombreBotónMesaSeparado = botónMesaSelected.name.Trim().Split("-");
        return int.Parse(nombreBotónMesaSeparado[1]);
    }

    private async void PonerReservaAMesaParaAhoraAsync() // En un momento aquí no se actualiza la mesa a disponible = false
    {
        Debug.Log("############# RESERVAR AL INSTANTE ##################");

        Button botónMesaSelected = botónMesaSeleccionado;

        // Indicar al servidor
        DateTime hoy = DateTime.Today;
        string fechaDeHoy = hoy.ToString("dd/MM/yyyy");

        int id_Mesa = ObtenerIDMesaDelNombreDelBotónMesa(botónMesaSelected);
        TMP_InputField textoCantComensalesMesa = botónMesaSelected.gameObject.transform.Find("InputField").GetComponent<TMP_InputField>();
        int cantComensalesMesa = int.Parse(textoCantComensalesMesa.text.Trim());

        if (ReservaNuevaNoObstaculizaElResto(id_Mesa))
        {
            // Intento registrar la reserva de la mesa enviando datos al servidor. Pongo pendiente porque la reserva es para ahora mismo (en uso)
            string cad = await instanceMétodosApiController.PostDataAsync("reserva/crearReserva", new Reserva(0, fechaDeHoy, textHoraActual.text, "" + EstadoReserva.Pendiente, cantComensalesMesa, 0, id_Mesa, new Cliente("", "", "")));

            // Deserializo la respuesta
            Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);
            if (resultado.Result.Equals(1))
            {
                await instanceMétodosApiController.PutDataAsync("reserva/actualizarBoolCreandoReserva", new Resultado(1));
                Debug.Log("Reserva registrada correctamente en mesa: " + id_Mesa);
                // Pongo mesa en No Disponible
                string cad2 = await instanceMétodosApiController.PutDataAsync("mesa/actualizarCampoDisponible", new Mesa(id_Mesa, 0, 0, 0, 0, 0, 0, 0, false, 0, new List<Reserva>()));

                // Deserializo la respuesta
                Resultado resultado2 = JsonConvert.DeserializeObject<Resultado>(cad2);

                if (resultado2.Result.Equals(1))
                {
                    Debug.Log("Mesa puesta en disponible correctamente");

                    Image img = botónMesaSelected.gameObject.transform.Find("Imagen Circle").GetComponent<Image>();
                    PonerColorCorrectoAImg(img, colorHexadecimalRojo);
                    contenedorInfoReservasMesa.SetActive(false);
                }
                else
                {
                    Debug.Log("Error al intentar actualizar la mesa");
                }
            }
            else
            {
                Debug.Log("Error al registrar reserva en mesa");
            }
        }
        else
        {
            Debug.Log("No se puede poner la reserva porque ya hay una reserva cercana para esa mesa");
        }
    }

    private bool ReservaNuevaNoObstaculizaElResto(int id_Mesa)
    {
        // Obtengo las reservas pendientes que tiene la mesa, ya sean de hoy o en adelante
        List<Reserva> reservasMesaPendientes = ObtenerReservasMesaDeHoyEnAdelante(id_Mesa);
        // Obtengo las reservas del día de hoy
        List<Reserva> reservasMesaParaHoy = ObtenerReservasMesaParaHoy(reservasMesaPendientes);

        string horaActual = DateTime.Now.ToString("HH:mm");
        TimeSpan horaActualTimeSpan = TimeSpan.Parse(horaActual);

        string[] tiempoPermitido = Restaurante.TiempoPermitidoParaComer.Split(":");
        int horas = int.Parse(tiempoPermitido[0].Trim());
        int minutos = int.Parse(tiempoPermitido[1].Trim());
        Debug.Log("Resultado tiempo permitido - Horas:" + horas + "; Minutos:" + minutos + "*");

        // Sumo el tiempo que puso el gerente en la escena "Editar Restaurante" en tiempo permitido para comer
        TimeSpan sumaTiempo = TimeSpan.FromHours(horas) + TimeSpan.FromMinutes(minutos);
        TimeSpan horaFinReservaFutura = horaActualTimeSpan.Add(sumaTiempo);


        foreach (Reserva reserva in reservasMesaParaHoy)
        {
            string horaReserva = reserva.Hora;
            TimeSpan horaReservaTimeSpan = TimeSpan.Parse(horaReserva);

            string[] tiempoPermitidoo = Restaurante.TiempoPermitidoParaComer.Split(":");
            int horass = int.Parse(tiempoPermitidoo[0].Trim());
            int minutoss = int.Parse(tiempoPermitidoo[1].Trim());
            Debug.Log("Resultado tiempo permitido - Horas:" + horass + "; Minutos:" + minutoss + "*");

            // Sumo el tiempo que puso el gerente en la escena "Editar Restaurante" en tiempo permitido para comer
            TimeSpan sumaTiempoo = TimeSpan.FromHours(horass) + TimeSpan.FromMinutes(minutoss);
            TimeSpan horaFinReservaExistente = horaReservaTimeSpan.Add(sumaTiempoo);
            Debug.Log("HoraFINReservaFutura: " + horaFinReservaFutura + "; HoraInicioReservaExistente: "+ horaReservaTimeSpan + " -  HoraFinReservaExistente: " + horaFinReservaExistente + "; Estado reserva existente = " + reserva.Estado);
            if (horaFinReservaFutura < horaFinReservaExistente && horaFinReservaFutura >= horaReservaTimeSpan && reserva.Estado.CompareTo("" + EstadoReserva.Confirmada) == 0)
            {
                Debug.Log("--------------- RESERVA NUEVA OBSTACULIZA");
                return false;
            }
        }
        Debug.Log("--------------- RESERVA NUEVA NO OBSTACULIZA");
        return true;

    }

    private void CrearBotonesMesas()
    {
        lastIDMesa = 0;

        // El restaurante tiene mesas
        if (Mesas.Count > 0)
        {
            Debug.Log("Hay mesas");
            foreach (var mesa in Mesas)
            {
                CrearBotonMesa(mesa);
            }
        }
        else
        {
            Debug.Log("No hay mesas");
        }
    }

    private void CrearBotonMesa(Mesa mesa)
    {
        // Crear un GameObject para el botón y asignarle un nombre único.
        GameObject botonGO = new GameObject("Button-" + mesa.Id);

        // Establecer el padre para que se muestre en el UI.
        botonGO.transform.SetParent(padreDeLosBotonesMesa, false);

        // Agregar el componente RectTransform (se agrega automáticamente al crear UI, pero lo añadimos explícitamente).
        RectTransform rt = botonGO.AddComponent<RectTransform>();
        // Opcional: definir un tamaño por defecto para el botón.
        rt.sizeDelta = new Vector2(mesa.Width, mesa.Height);

        // Agregar CanvasRenderer para poder renderizar el UI.
        botonGO.AddComponent<CanvasRenderer>();

        // Agregar el componente Image para mostrar el sprite.
        UnityEngine.UI.Image imagen = botonGO.AddComponent<UnityEngine.UI.Image>();
        if (mesaSprite != null)
        {
            imagen.sprite = mesaSprite;
        }

        // Configurar la posición y escala del botón basándose en las propiedades de la mesa.
        rt.anchoredPosition = new Vector2(mesa.PosX, mesa.PosY);
        rt.localScale = new Vector3(mesa.ScaleX, mesa.ScaleY, 1f);

        // Agrego un componente Button para que sea interactivo
        botonGO.AddComponent<Button>();

        // Creo nuevos GameObject hijos, las imágenes del botón
        CrearImgsDelButton(rt, mesa.Disponible);

        StartCoroutine(CrearUnHijoInputFieldDelBotónMesa(botonGO, mesa.CantPers));
    }

    public void Salir()
    {
        SceneManager.LoadScene("Main");
    }

    private void CrearImgsDelButton(RectTransform newRect, bool disponible)
    {
        CrearImgCircle(newRect, disponible);
        CrearImgRectangle(newRect);
    }

    private void CrearImgCircle(RectTransform newRect, bool disponible)
    {
        // Creo el objeto
        GameObject imgObject = new GameObject("Imagen Circle");
        // El nuevo botón se creará como hijo del contenedor, NO del Canvas
        imgObject.transform.SetParent(newRect, false);

        // Agrego y configuro el RectTransform: posición central y tamaño predeterminado
        RectTransform rectButton = imgObject.AddComponent<RectTransform>();
        rectButton.anchoredPosition = Vector2.zero;
        rectButton.sizeDelta = new Vector2(85, 85); // Tamaño (ancho/alto)

        // Agrego un componente Image
        Image img = imgObject.AddComponent<Image>();
        Sprite newSprite = Resources.Load<Sprite>("Editar Restaurante/circle perfect 1.0");
        if (newSprite != null)
        {
            img.sprite = newSprite;
        }
        else
        {
            Debug.LogWarning("No se encontró la imagen en Resources: circle perfect 1.0");
        }
        
        // Poner color correcto a mesa según si está disponible o no. Verde = Sí ; Rojo = No
        if (disponible)
        {
            PonerColorCorrectoAImg(img, colorHexadecimalVerde);
            
        }
        else
        {
            PonerColorCorrectoAImg(img, colorHexadecimalRojo);
        }
    }

    public void PonerColorCorrectoAImg(Image img, string hexadecimal)
    {
        Color newColor;
        // Intento convertir el string hexadecimal a Color
        if (UnityEngine.ColorUtility.TryParseHtmlString(hexadecimal, out newColor))
        {
            img.color = newColor;
        }
        else
        {
            Debug.LogError("El formato del color hexadecimal es inválido.");
        }
    }

    private void CrearImgRectangle(RectTransform newRect)
    {
        // Creo el objeto
        GameObject imgObject = new GameObject("Imagen Rectangle");
        // El nuevo botón se creará como hijo del contenedor, NO del Canvas
        imgObject.transform.SetParent(newRect, false);

        // Agrego y configuro el RectTransform: posición central y tamaño predeterminado
        RectTransform rectImg = imgObject.AddComponent<RectTransform>();
        rectImg.anchoredPosition = new Vector2(-67.5f, 35); // x e y
        rectImg.sizeDelta = new Vector2(45, 30); // Tamaño (ancho/alto)

        // Agrego un componente Image
        Image img = imgObject.AddComponent<Image>();
        img.sprite = imgCuadradoBordeNegroFino;
        /*Sprite newSprite = Resources.Load<Sprite>("Editar Restaurante/circle perfect 1.0");
        if (newSprite != null)
        {
            img.sprite = newSprite;
        }
        else
        {
            Debug.LogWarning("No se encontró la imagen en Resources: circle perfect 1.0");
        }*/

        // Creo un gameObject TMP_Text y lo pongo de hijo en el objeto imagen rectángulo
        GameObject textObject = new GameObject("Text");
        // El nuevo botón se creará como hijo del contenedor, NO del Canvas
        textObject.transform.SetParent(rectImg, false);

        // Agrego y configuro el RectTransform: posición central y tamaño predeterminado
        RectTransform rectText = textObject.AddComponent<RectTransform>();
        rectText.anchoredPosition = Vector2.zero;
        rectText.sizeDelta = new Vector2(40, 30); // Tamaño (ancho/alto)

        // Agrego un componente TMP_Text
        TMP_Text text = textObject.AddComponent<TextMeshProUGUI>();
        text.alignment = TextAlignmentOptions.Center; // Centro el texto
        lastIDMesa++;
        text.text = "" + lastIDMesa;
        text.fontSize = 32;
        text.fontStyle = FontStyles.Bold;

    }

    private IEnumerator CrearUnHijoInputFieldDelBotónMesa(GameObject newButtonObj, int cantComensales)
    {
        GameObject inputFieldInstance = Instantiate(tmpInputFieldPrefab, newButtonObj.transform, false);
        inputFieldInstance.name = "InputField";

        TMP_Text textComponent = inputFieldInstance.transform.Find("Text Area/Text").GetComponent<TMP_Text>();
        TMP_Text textPlaceHolder = inputFieldInstance.transform.Find("Text Area/Placeholder").GetComponent<TMP_Text>();

        //inputFieldInstance.GetComponent<TMP_InputField>().interactable = false; // Pongo el inputField en no interactuable
        textComponent.alignment = TextAlignmentOptions.Center; // Centro el texto
        textComponent.fontSize = 56;
        textComponent.fontStyle = FontStyles.Bold;
        textComponent.color = UnityEngine.Color.white;
        RectTransform rtInputField = inputFieldInstance.GetComponent<RectTransform>();
        rtInputField.sizeDelta = new Vector2(100, 55);
        inputFieldInstance.GetComponent<TMP_InputField>().text = "" + cantComensales; // Asigno la cantidad de comensales a la mesa
        inputFieldInstance.GetComponent<Image>().enabled = false; // Quito la imagen del inputField (la pongo en invisible)
        // Espero un frame para que se cree el Caret
        yield return null;

        // Desactivo Raycast Target para que no bloqueen interacción con el botón
        TMP_SelectionCaret caret = inputFieldInstance.GetComponentInChildren<TMP_SelectionCaret>();
        if (caret != null)
        {
            // Desactivamos raycastTarget del Caret
            caret.raycastTarget = false;
        }
        else
        {
            Debug.Log("Caret no encontrado!!!!!!!!!!!!!!!!!");
        }

        inputFieldInstance.GetComponent<Image>().raycastTarget = false;
        textPlaceHolder.raycastTarget = false;
        textComponent.raycastTarget = false;
    }

    public void IrAlMenúPrincipal()
    {
        SceneManager.LoadScene("Main");
    }

    public void GestionarVerReservasMesas()
    {
        // Una vez obtenidas las reservas según su estado, las coloco ordenadas como botones en un Scroll View
        CrearBotonesDeReservasEnMesaHoy();

        canvasReservasHoyMesa.SetActive(true);
        scrollbarReservasMesaHoy.value = 1; // Cada vez que muestro el scroll view, pongo el scroll arriba del todo
    }

    private void CrearBotonesDeReservasEnMesaHoy()
    {
        Button botónMesaSelected = botónMesaSeleccionado;
        int id_Mesa = ObtenerIDMesaDelNombreDelBotónMesa(botónMesaSelected);

        if (Usuario.Idioma.CompareTo("Español") == 0)
        {
            textReservasHoyMesa.text = "Registros Hoy Mesa " + ObtenerIDMesaDelMapa(botónMesaSelected);
        }
        else
        {
            textReservasHoyMesa.text = "Registrations Today Table " + ObtenerIDMesaDelMapa(botónMesaSelected);
        }

        // Obtengo las reservas pendientes que tiene la mesa, ya sean de hoy o en adelante
        List<Reserva> reservasMesaPendientes = ObtenerReservasMesaDeHoyEnAdelante(id_Mesa);
        // Obtengo las reservas del día de hoy terminadas
        List<Reserva> reservasMesaParaHoyTerminadasYCanceladas = ObtenerReservasMesaParaHoyTerminadasYCanceladas(reservasMesaPendientes);
        // Obtengo las reservas del día de hoy (pendiente y confirmadas)
        List<Reserva> reservasMesaParaHoy = ObtenerReservasMesaParaHoy(reservasMesaPendientes);
        // Obtengo las reservas del día de hoy confirmadas
        List<Reserva> reservasMesaParaHoyConfirmadas = ObtenerReservasMesaHoyConfirmadas(reservasMesaParaHoy);
        // Obtengo la reserva actual en uso
        Reserva reserva = ObtenerReservaEnUso(reservasMesaParaHoy);

        // Tengo que eliminar todos los hijos (botones en este caso) de Content antes de poner nuevos (reservas actualizadas)
        EliminarObjetosHijoDeScrollView(rectTransformContent);

        // Existe una reserva en uso  
        if (reserva != null)
        {
            CrearBotónEnScrollViewReservasMesaHoy(reserva, 1); 
        }

        // Existen reservas de la mesa para hoy confirmadas 
        if (reservasMesaParaHoyConfirmadas.Count > 0)
        {
            var reservasOrdenadasPorHora = reservasMesaParaHoyConfirmadas.OrderBy(r => TimeSpan.Parse(r.Hora)).ToList();

            foreach (Reserva r in reservasOrdenadasPorHora)
            {
                CrearBotónEnScrollViewReservasMesaHoy(r, 2);
            }
        }

        // Existen reservas de la mesa para hoy terminadas 
        if (reservasMesaParaHoyTerminadasYCanceladas.Count > 0)
        {
            var reservasOrdenadasPorHora = reservasMesaParaHoyTerminadasYCanceladas.OrderBy(r => TimeSpan.Parse(r.Hora)).ToList();

            foreach (Reserva reserv in reservasOrdenadasPorHora)
            {
                CrearBotónEnScrollViewReservasMesaHoy(reserv, 3);
            }
        }
    }

    public string ObtenerIDMesaDelMapa(Button botónMesaSelected)
    {
        return "" + botónMesaSelected.gameObject.transform.Find("Imagen Rectangle/Text").GetComponent<TextMeshProUGUI>().text.Trim();
    }

    public void EliminarObjetosHijoDeScrollView(RectTransform rectTransformContent)
    {
        foreach (Transform hijo in rectTransformContent)
        {
            Destroy(hijo.gameObject);
        }
    }

    private List<Reserva> ObtenerReservasMesaParaHoyTerminadasYCanceladas(List<Reserva> reservasMesaPendientes)
    {
        List<Reserva> reservas = new List<Reserva>();
        DateTime fechaHoy = DateTime.Today;
        foreach (Reserva reserva in reservasMesaPendientes)
        {
            DateTime fechaReserva = DateTime.Parse(reserva.Fecha);

            // Si la fecha de la reserva es hoy y la reserva está terminada, se obtiene
            if (fechaReserva == fechaHoy && reserva.Estado.CompareTo("" + EstadoReserva.Terminada) == 0 || fechaReserva == fechaHoy && reserva.Estado.CompareTo("" + EstadoReserva.Cancelada) == 0)
            {
                reservas.Add(reserva);
            }
        }
        return reservas;
    }

    private List<Reserva> ObtenerReservasMesaHoyConfirmadas(List<Reserva> reservasMesaParaHoy)
    {
        List<Reserva> reservas = new List<Reserva>();
        foreach (Reserva reserva in reservasMesaParaHoy)
        {
            // Si la fecha de la reserva es hoy y la reserva está terminada, se obtiene
            if (reserva.Estado.CompareTo("" + EstadoReserva.Confirmada) == 0)
            {
                reservas.Add(reserva);
            }
        }
        return reservas;
    }

    private void CrearBotónEnScrollViewReservasMesaHoy(Reserva reserva, int num)
    {
        // Creo un GameObject para el botón y le asigno un nombre único.
        GameObject botónGO = new GameObject("Button-" + reserva.Id);

        // Establezco el padre para que se muestre en el UI.
        botónGO.transform.SetParent(rectTransformContent, false);

        // Agrego el componente RectTransform (se agrega automáticamente al crear UI, pero lo añado explícitamente).
        RectTransform rt = botónGO.AddComponent<RectTransform>();
        // Defino un tamaño por defecto para el botón.
        rt.sizeDelta = new Vector2(1530.9f, 138f);

        // Agrego CanvasRenderer para poder renderizar el UI.
        botónGO.AddComponent<CanvasRenderer>();

        // Agrego el componente Image para mostrar el sprite.
        Image imagen = botónGO.AddComponent<Image>();

        switch (num)
        {
            case 1:
                PonerColorCorrectoAImg(imagen, "#6DEC6F");
                //imagen.color = Color.green;
                break;
            case 2:
                PonerColorCorrectoAImg(imagen, "#FDF468");
                break;
            case 3:
                PonerColorCorrectoAImg(imagen, "#EC6C6C");
                break;
        }

        // Agrego un componente Button para que sea interactivo
        Button b=botónGO.AddComponent<Button>();

        int idMesa = Int32.Parse(botónMesaSeleccionado.gameObject.name.Split("-")[1]);
        b.onClick.AddListener(() => cambiarAListaPedidos(idMesa));
         //Creo un nuevo GameObject hijo, el texto del botón
        CrearTextoDelButton(rt, reserva);
    }

    private void CrearTextoDelButton(RectTransform rt, Reserva reserva)
    {
        // Creo un GameObject para el botón y le asigno un nombre único.
        GameObject textGO = new GameObject("TMP_Text");

        // Establezco el padre para que se muestre en el UI.
        textGO.transform.SetParent(rt, false);

        // Agrego el componente RectTransform (se agrega automáticamente al crear UI, pero lo añado explícitamente).
        RectTransform rtText = textGO.AddComponent<RectTransform>();
        // Anclas estiradas (stretch) en ambas direcciones
        rtText.anchorMin = new Vector2(0, 0);
        rtText.anchorMax = new Vector2(1, 1);

        // Márgenes todos a 0 (equivale a Left, Right, Top, Bottom en el inspector)
        rtText.offsetMin = Vector2.zero;
        rtText.offsetMax = Vector2.zero;

        // Centrado por si acaso (aunque no influye mucho cuando está estirado)
        rtText.anchoredPosition = Vector2.zero;

        // Agrego CanvasRenderer para poder renderizar el UI.
        textGO.AddComponent<CanvasRenderer>();

        // Agrego el componente TMP_Text para mostrar el sprite.
        TMP_Text textoBotón = textGO.AddComponent<TextMeshProUGUI>();
        textoBotón.fontStyle = FontStyles.Bold;
        textoBotón.fontSize = 56;
        textoBotón.alignment = TextAlignmentOptions.Left;

        if (Usuario.Idioma.CompareTo("Español") == 0)
        {
            // Si el cliente tiene un número de teléfono registrado en la BDD
            if (reserva.Cliente.NumTelefono.Trim().Length > 0)
            {
                textoBotón.text = "  " + reserva.Fecha + "    " + reserva.Hora + "           " + reserva.CantComensales + "          " + reserva.Cliente.NumTelefono + "    " + reserva.Cliente.Nombre;
            }
            else // El cliente no tiene ningún número de teléfono registrado
            {
                textoBotón.text = "  " + reserva.Fecha + "    " + reserva.Hora + "           " + reserva.CantComensales + "                              " + reserva.Cliente.Nombre;
            }
        }
        else
        {
            // Si el cliente tiene un número de teléfono registrado en la BDD
            if (reserva.Cliente.NumTelefono.Trim().Length > 0)
            {
                textoBotón.text = "  " + reserva.Fecha + "    " + reserva.Hora + "          " + reserva.CantComensales + "         " + reserva.Cliente.NumTelefono + "     " + reserva.Cliente.Nombre;
            }
            else // El cliente no tiene ningún número de teléfono registrado
            {
                textoBotón.text = "  " + reserva.Fecha + "    " + reserva.Hora + "          " + reserva.CantComensales + "                              " + reserva.Cliente.Nombre;
            }
        }        
    }

    public void DesactivarCanvasReservasMesaHoy()
    {
        contenedorInfoReservasMesa.SetActive(false);
        canvasReservasHoyMesa.SetActive(false);
    }

    public void ActivarCanvasCrearReserva()
    {
        instanceCrearReservaController.InicializarValoresDropdowns();
        instanceCrearReservaController.AsignarValoresConcretosEnDropdowns();
        instanceCrearReservaController.PonerValoresEnLasOpcionesDeCrear("","","");
        instanceCrearReservaController.Poner4ValoresEnCrearVacíos();

        canvasCrearReserva.SetActive(true);
    }

    public void ActivarCanvasBuscarReserva()
    {
        canvasBuscarReserva.SetActive(true);
    }

    public void ActivarCanvasHistorialReservas()
    {
        // Una vez obtenidas las reservas según su estado, las coloco ordenadas como botones en un Scroll View
        CrearBotonesEnHistorialReservas();

        canvasHistorialReservas.SetActive(true);

        scrollbarHistorialReservas.value = 1; // Cada vez que muestro el scroll view, pongo el scroll arriba del todo
    }

    private void CrearBotonesEnHistorialReservas()
    {
        // Tengo que eliminar todos los hijos (botones en este caso) de Content antes de poner nuevos (reservas actualizadas)
        EliminarObjetosHijoDeScrollView(rectTransformContentHistorialReservas);

        List<Reserva> reservasDeHace1Mes = ObtenerTodasLasReservasDeHace1Mes();
        List<Reserva> reservasTerminadasCanceladasOConfirmadas = ObtenerReservasTerminadasCanceladasOConfirmadas(reservasDeHace1Mes);
        List<Reserva> reservasEnUso = ObtenerReservasEnUso(reservasDeHace1Mes);


        // Existe una o varias reservas en uso  
        if (reservasEnUso.Count > 0)
        {
            var reservasOrdenadasPorHora = reservasEnUso
            .OrderByDescending(r => DateTime.Parse(r.Fecha)) // primero por fecha descendente (más recientes arriba)
            .ThenBy(r => TimeSpan.Parse(r.Hora))   // luego por hora descendente
            .ToList();


            foreach (Reserva r in reservasOrdenadasPorHora)
            {
                CrearBotónEnScrollViewHistorialReservas(r);
            }
        }

        // Existen reservas de la mesa para hoy terminadas/canceladas o confirmadas 
        if (reservasTerminadasCanceladasOConfirmadas.Count > 0)
        {
            var reservasOrdenadasPorHora = reservasTerminadasCanceladasOConfirmadas
            .OrderByDescending(r => DateTime.Parse(r.Fecha)) // primero por fecha descendente (más recientes arriba)
            .ThenBy(r => TimeSpan.Parse(r.Hora))   // luego por hora descendente
            .ToList();

            foreach (Reserva reserv in reservasOrdenadasPorHora)
            {
                CrearBotónEnScrollViewHistorialReservas(reserv);
            }
        }
    }

    public void CrearBotónEnScrollViewHistorialReservas(Reserva reserva)
    {
        // Creo un GameObject para el botón y le asigno un nombre único.
        GameObject botónGO = new GameObject("Button-" + reserva.Id);

        // Establezco el padre para que se muestre en el UI.
        botónGO.transform.SetParent(rectTransformContentHistorialReservas, false);

        // Agrego el componente RectTransform (se agrega automáticamente al crear UI, pero lo añado explícitamente).
        RectTransform rt = botónGO.AddComponent<RectTransform>();
        // Defino un tamaño por defecto para el botón.
        rt.sizeDelta = new Vector2(1530.9f, 138f);

        // Agrego CanvasRenderer para poder renderizar el UI.
        botónGO.AddComponent<CanvasRenderer>();

        // Agrego el componente Image para mostrar el sprite.
        Image imagen = botónGO.AddComponent<Image>();

        switch (reserva.Estado)
        {
            case "Pendiente":
                PonerColorCorrectoAImg(imagen, "#6DEC6F");
                //imagen.color = Color.green;
                break;
            case "Confirmada":
                PonerColorCorrectoAImg(imagen, "#FDF468");
                break;
            case "Cancelada":
                PonerColorCorrectoAImg(imagen, "#EC6C6C");
                break;
            case "Terminada":
                PonerColorCorrectoAImg(imagen, "#EC6C6C");
                break;
        }

        // Agrego un componente Button para que sea interactivo
        botónGO.AddComponent<Button>();

        // Creo un nuevo GameObject hijo, el texto del botón
        CrearTextoDelButtonEnHistorialReservas(rt, reserva);
    }

    private void CrearTextoDelButtonEnHistorialReservas(RectTransform rt, Reserva reserva)
    {
        // Creo un GameObject para el botón y le asigno un nombre único.
        GameObject textGO = new GameObject("TMP_Text");

        // Establezco el padre para que se muestre en el UI.
        textGO.transform.SetParent(rt, false);

        // Agrego el componente RectTransform (se agrega automáticamente al crear UI, pero lo añado explícitamente).
        RectTransform rtText = textGO.AddComponent<RectTransform>();
        // Anclas estiradas (stretch) en ambas direcciones
        rtText.anchorMin = new Vector2(0, 0);
        rtText.anchorMax = new Vector2(1, 1);

        // Márgenes todos a 0 (equivale a Left, Right, Top, Bottom en el inspector)
        rtText.offsetMin = Vector2.zero;
        rtText.offsetMax = Vector2.zero;

        // Centrado por si acaso (aunque no influye mucho cuando está estirado)
        rtText.anchoredPosition = Vector2.zero;

        // Agrego CanvasRenderer para poder renderizar el UI.
        textGO.AddComponent<CanvasRenderer>();

        // Agrego el componente TMP_Text para mostrar el sprite.
        TMP_Text textoBotón = textGO.AddComponent<TextMeshProUGUI>();
        textoBotón.fontStyle = FontStyles.Bold;
        textoBotón.fontSize = 46;
        textoBotón.alignment = TextAlignmentOptions.Left;

        // Obtengo el Id de la mesa en el mapa
        Button botónMesaSelected = padreDeLosBotonesMesa.gameObject.transform.Find("Button-" + reserva.Mesa_Id).GetComponent<Button>();
        int id_Mesa_En_Mapa = int.Parse(ObtenerIDMesaDelMapa(botónMesaSelected));

        // Si el cliente tiene un número de teléfono registrado en la BDD
        if (reserva.Cliente.NumTelefono.Trim().Length > 0)
        {
            if (id_Mesa_En_Mapa.ToString().Length.Equals(2))
            {
                textoBotón.text = " " + reserva.Fecha + "  " + reserva.Hora + "        " + reserva.CantComensales + "            " + id_Mesa_En_Mapa + "       " + reserva.Cliente.Dni + "   " + reserva.Cliente.NumTelefono + "    " + reserva.Cliente.Nombre;
            }
            else
            {
                textoBotón.text = " " + reserva.Fecha + "  " + reserva.Hora + "        " + reserva.CantComensales + "            " + id_Mesa_En_Mapa + "         " + reserva.Cliente.Dni + "   " + reserva.Cliente.NumTelefono + "    " + reserva.Cliente.Nombre;
            }
            
        }
        else // El cliente no tiene ningún número de teléfono registrado
        {
            if (id_Mesa_En_Mapa.ToString().Length.Equals(2))
            {
                textoBotón.text = " " + reserva.Fecha + "  " + reserva.Hora + "        " + reserva.CantComensales + "            " + id_Mesa_En_Mapa + "       " + reserva.Cliente.Dni + "                       " + reserva.Cliente.Nombre;
            }
            else
            {
                textoBotón.text = " " + reserva.Fecha + "  " + reserva.Hora + "        " + reserva.CantComensales + "            " + id_Mesa_En_Mapa + "         " + reserva.Cliente.Dni + "                       " + reserva.Cliente.Nombre;
            }
            
        }
    }

    public List<Reserva> ObtenerReservasTerminadasCanceladasOConfirmadas(List<Reserva> reservasDeHace1Mes)
    {
        List<Reserva> reservas = new List<Reserva>();
        foreach (Reserva reserva in reservasDeHace1Mes)
        {
            if (reserva.Estado.CompareTo(""+EstadoReserva.Cancelada) == 0 || reserva.Estado.CompareTo(""+EstadoReserva.Terminada) == 0 || reserva.Estado.CompareTo("" + EstadoReserva.Confirmada) == 0)
            {
                reservas.Add(reserva);
            }
        }
        return reservas;
    }

    public List<Reserva> ObtenerReservasEnUso(List<Reserva> reservasDeHace1Mes)
    {
        List<Reserva> reservas = new List<Reserva>();
        foreach (Reserva reserva in reservasDeHace1Mes)
        {
            if (reserva.Estado.CompareTo("" + EstadoReserva.Pendiente) == 0)
            {
                reservas.Add(reserva);
            }
        }
        return reservas;
    }

    public List<Reserva> ObtenerTodasLasReservasDeHace1Mes()
    {
        DateTime fechaActual = DateTime.Today;
        DateTime fechaHaceUnMes = fechaActual.AddMonths(-1); // Le resto un mes a la fecha actual 
        TimeSpan horaActual = TimeSpan.Parse(DateTime.Now.ToString("HH:mm"));
        List<Reserva> reservas = new List<Reserva>();
        foreach (Mesa mesa in Mesas)
        {
            foreach (Reserva reserva in mesa.Reservas)
            {
                DateTime fechaReserva = DateTime.Parse(reserva.Fecha);
                TimeSpan horaReserva = TimeSpan.Parse(reserva.Hora);

                if (fechaReserva <= fechaActual && fechaReserva >= fechaHaceUnMes || fechaReserva == fechaActual &&  horaReserva <= horaActual)
                {
                    reservas.Add(reserva);
                }
            }
        }
        return reservas;
    }

    public void MostrarImgConTextoDeBuscarReserva()
    {
        Debug.Log("Activando imagen desde EventTrigger");
        imgConTextoDeBuscarReserva.SetActive(true);
    }

    public void OcultarImgConTextoDeBuscarReserva()
    {
        Debug.Log("Ocultando imagen desde EventTrigger");
        imgConTextoDeBuscarReserva.SetActive(false);
    }

    public void MostrarImgYTextoDeCrearReserva()
    {
        Debug.Log("Activando imagen desde EventTrigger");
        ImgYTextoCrearReserva.SetActive(true);
    }

    public void OcultarImgYTextoDeCrearReserva()
    {
        Debug.Log("Ocultando imagen desde EventTrigger");
        ImgYTextoCrearReserva.SetActive(false);
    }

    public void MostrarImgYTextoDeHistorialReservas()
    {
        Debug.Log("Activando imagen desde EventTrigger");
        ImgYTextoHistorialReservas.SetActive(true);
    }

    public void OcultarImgYTextoDeHistorialReservas()
    {
        Debug.Log("Ocultando imagen desde EventTrigger");
        ImgYTextoHistorialReservas.SetActive(false);
    }
    public void MostrarImgConTextoDeListaPedidos()
    {
        Debug.Log("Activando imagen desde EventTrigger");
        ImgYTextoListaPedidos.SetActive(true);
    }
    public void OcultarImgYTextoDeListaPedidos()
    {
        Debug.Log("Ocultando imagen desde EventTrigger");
        ImgYTextoListaPedidos.SetActive(false);
    }
    public List<Mesa> GetMesas()
    {
        return Mesas;
    }

    public string GetHoraAperturaRestaurante()
    {
        return textHoraApertura.text;
    }

    public string GetHoraCierreRestaurante()
    {
        return textHoraCierre.text;
    }
    // PONER AQUÍ LAS FUNCIONES PARA GENERAR CANVAS PEDIDOS
    public async void cambiarAPedidos()
    {
        if (botónMesaSeleccionado != null)
        {
            instanceGestionarPedidosController = GestionarPedidosController.instanceGestionarPedidosController;
            instanceGestionarPedidosController.pedido=new Pedido(-1,"",-1,"",-1);
            instanceGestionarPedidosController.actualizarArticulos();
            instanceGestionarPedidosController.entrarPedido(Int32.Parse(botónMesaSeleccionado.gameObject.name.Split("-")[1]));
            Debug.Log("BOTÓN SELECCIONADO: " + botónMesaSeleccionado.gameObject.name);
        }
        //Se debería mostrar un mensaje de error si no se tiene una mesa seleccionada
        else Debug.Log("No se ha seleccionado ningún botón");
    }
    public void cambiarAListaPedidos(int mesa)
    {
        Debug.Log("PRUEBA LISTA");
        instanceGestionarListaPedidos.entrarLista(mesa);
        Debug.Log("FIN PRUEBA");
    }

    public int getNumMesa(int idMesa)
    {
        TextMeshProUGUI texto=padreDeLosBotonesMesa.gameObject.transform.Find("Button-" + idMesa + "/Imagen Rectangle/Text").GetComponent<TextMeshProUGUI>();
        Debug.Log("TEXTO:" + texto.text);
        return Int32.Parse(texto.text);
    }
}
