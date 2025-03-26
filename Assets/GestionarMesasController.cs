using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



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


    private List<Mesa> Mesas;

    private int lastIDMesa = 0;
    private string colorHexadecimalVerde = "#00B704";
    private string colorHexadecimalRojo = "#A12121";
    private Button botónMesaSeleccionado;

    private int contMostrarBotonesMesa = 1;

    // Contenedor padre donde se agregarán los botones
    public RectTransform padreDeLosBotonesMesa;

    // Sprite que cargo desde Resources.
    private Sprite mesaSprite;

    MétodosAPIController instanceMétodosApiController;


    void Awake()
    {
        SceneManager.LoadSceneAsync("General Controller", LoadSceneMode.Additive);
    }

    // Start is called before the first frame update
    void Start()
    {
        instanceMétodosApiController = MétodosAPIController.InstanceMétodosAPIController;

        TrabajadorController.ComprobandoDatosTrabajador = false;

        InvokeRepeating(nameof(ActualizarHora), 0f, 1f); // Llama a ActualizarHora() cada 1 segundo

        InvokeRepeating(nameof(ObtenerDatosRestauranteAsync), 0f, 1f); // Llama a ObtenerDatosRestauranteAsync() cada 1 segundo

        

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
        string cad = await instanceMétodosApiController.GetDataAsync("restaurante/getRestaurantePorId/" + PlayerPrefs.GetInt("Restaurante_ID Usuario"));

        // Deserializo la respuesta
        Restaurante restaurante = JsonConvert.DeserializeObject<Restaurante>(cad);

        textHoraApertura.text = restaurante.HoraApertura.Replace(" ", "");
        textHoraCierre.text = restaurante.HoraCierre.Replace(" ", "");
        Mesas = restaurante.Mesas;
        


        textNombreRestaurante.text = restaurante.Nombre;

        Debug.Log("Hora Apertura: " + restaurante.HoraApertura + "; Hora Cierre: " + restaurante.HoraCierre);

        // Los botones mesa sólo se pintan una vez
        if (contMostrarBotonesMesa.Equals(1)){
            contMostrarBotonesMesa++;
            mesaSprite = Resources.Load<Sprite>("Editar Restaurante/mantelMesa");
            CrearBotonesMesas();
            ActualizarTodasLasReservasQueYaAcabaronYEstánPendientesAsync(); 
            InvokeRepeating(nameof(ActualizarEstadoReservaYMesaDelDíaDeHoy), 0f, 1f); // Llama a ActualizarEstadoReservaYMesa() cada 1 segundo
        }

        AñadirListenerABotonesMesaDelMapa();
        
    }

    private async void ActualizarTodasLasReservasQueYaAcabaronYEstánPendientesAsync()
    {
        DateTime fechaHoy = DateTime.Today;
        foreach (Mesa mesa in Mesas)
        {

            foreach (Reserva reserva in mesa.Reservas)
            {
                DateTime fechaReserva = DateTime.Parse(reserva.Fecha);

                if (fechaReserva < fechaHoy && reserva.Estado.CompareTo(""+EstadoReserva.Pendiente) == 0)
                {
                    string cad = await instanceMétodosApiController.PutDataAsync("reserva/actualizarEstadoReserva", new Reserva(reserva.Id, "", "", "" + EstadoReserva.Terminada, 0, 0, reserva.Mesa_Id));

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

    // Si una reserva ha acabado, se actualiza la mesa y la reserva. Los colores se actualizan aquí aunque se podrían actualizar aparte
    private async void ActualizarEstadoReservaYMesaDelDíaDeHoy() 
    {                                                       // Hay que hacer bastantes cambios y mejoras aquí __ _ __ _-- _ - - - - -- -_ _ _ _ - -_ _ - _ _ -_
        // Compruebo si ya ha pasado su tiempo límite
        string horaActual = DateTime.Now.ToString("HH:mm");
        TimeSpan horaActualTimeSpan = TimeSpan.Parse(horaActual);

        foreach (Mesa mesa in Mesas)
        {
            // Obtengo las reservas pendientes que tiene la mesa, ya sean de hoy o en adelante
            List<Reserva> reservasMesaPendientes = ObtenerReservasMesaPendientes(mesa.Id);
            // Obtengo las reservas del día de hoy
            List<Reserva> reservasMesaParaHoy = ObtenerReservasMesaParaHoy(reservasMesaPendientes);

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

                // Obtengo las reservas de la mesa que acabaron y con estado "Pendiente" 
                if (horaFinReserva <= horaActualTimeSpan && reserva.Estado.CompareTo("" + EstadoReserva.Pendiente) == 0)
                {
                    string cad = await instanceMétodosApiController.PutDataAsync("reserva/actualizarEstadoReserva", new Reserva(reserva.Id, "", "", "" + EstadoReserva.Terminada, 0, 0, reserva.Mesa_Id));

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
                        PonerColorCorrectoAMesa(img, colorHexadecimalVerde);
                    }
                    else
                    {
                        Debug.Log("Error al intentar actualizar la mesa");
                    }
                }

                PonerReservaConfirmadaEnUsoAsync(reservasMesaParaHoy);

                // Pongo la mesa en no disponible si existe una reserva pendiente ahora
                if (ExisteUnaReservaPendienteAhora(reservasMesaParaHoy, horaActualTimeSpan))
                {
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
                    PonerColorCorrectoAMesa(img, colorHexadecimalRojo);

                }
                else // No hay ninguna reserva ahora mismo en uso, pongo la mesa en disponible
                {
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
                    PonerColorCorrectoAMesa(img, colorHexadecimalVerde);
                }
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

            string[] tiempoPermitido = Restaurante.TiempoPermitidoParaComer.Split(":");
            int horas = int.Parse(tiempoPermitido[0].Trim());
            int minutos = int.Parse(tiempoPermitido[1].Trim());
            Debug.Log("Resultado tiempo permitido - Horas:" + horas + "; Minutos:" + minutos + "*");

            // Sumo el tiempo que puso el gerente en la escena "Editar Restaurante" en tiempo permitido para comer
            TimeSpan sumaTiempo = TimeSpan.FromHours(horas) + TimeSpan.FromMinutes(minutos);
            TimeSpan horaFinReserva = horaReservaTimeSpan.Add(sumaTiempo);

            // Si hay una reserva "Confirmada" que debería estar en uso, se pone en "Pendiente"
            if (horaActualTimeSpan < horaFinReserva && horaActualTimeSpan >= horaReservaTimeSpan && reserva.Estado.CompareTo("" + EstadoReserva.Confirmada) == 0)
            {
                string cad = await instanceMétodosApiController.PutDataAsync("reserva/actualizarEstadoReserva", new Reserva(reserva.Id, "", "", "" + EstadoReserva.Pendiente, 0, 0, reserva.Mesa_Id));

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
    public void PulsarBotónMesaDisponible()
    {
        CancelarReservaActualEnMesaAsync();
    }

    private async void CancelarReservaActualEnMesaAsync()
    {
        Button botónMesaSelected = botónMesaSeleccionado;

        int id_Mesa = ObtenerIDMesaDelNombreDelBotónMesa(botónMesaSelected);

        // Obtengo las reservas pendientes que tiene la mesa, ya sean de hoy o en adelante
        List<Reserva> reservasMesaPendientes = ObtenerReservasMesaPendientes(id_Mesa);

        foreach (Reserva r in reservasMesaPendientes)
        {
            Debug.Log("Reservas mesa pendientes entre hoy y en adelante: " + r.Mostrar());
        }
        Debug.Log("--------------------------------");

        // Obtengo las reservas del día de hoy
        List<Reserva> reservasMesaParaHoy = ObtenerReservasMesaParaHoy(reservasMesaPendientes);

        // Obtengo la reserva actual en uso
        Reserva reserva = ObtenerReservaEnUso(reservasMesaParaHoy);
        
        if (reserva != null)
        {
            Debug.Log("Reserva not null");

            // Una vez obtenida la reserva pendiente, la cancelo en la BDD
            string cad = await instanceMétodosApiController.PutDataAsync("reserva/actualizarEstadoReserva", new Reserva(reserva.Id, "", "", ""+EstadoReserva.Cancelada, 0, 0, id_Mesa));

            // Deserializo la respuesta
            Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

            if (resultado.Result.Equals(1))
            {
                Debug.Log("Reserva cancelada correctamente");
            }
            else
            {
                Debug.Log("Error al intentar cancelar una reserva");
            }

            // También actualizo la mesa en la BDD en "Disponible" = true 
            string cad2 = await instanceMétodosApiController.PutDataAsync("mesa/actualizarCampoDisponible", new Mesa(id_Mesa, 0, 0, 0, 0, 0, 0, 0, true, 0, new List<Reserva>()));

            // Deserializo la respuesta
            Resultado resultado2 = JsonConvert.DeserializeObject<Resultado>(cad2);    

            if (resultado2.Result.Equals(1))
            {
                Debug.Log("Mesa puesta en disponible correctamente");
            }
            else
            {
                Debug.Log("Error al intentar actualizar la mesa");
            }

            buttonDisponible.interactable = false;
            buttonNoDisponible.interactable = true;

            Image img = botónMesaSeleccionado.gameObject.transform.Find("Imagen Circle").GetComponent<Image>();
            PonerColorCorrectoAMesa(img, colorHexadecimalVerde);
            contenedorInfoReservasMesa.SetActive(false);
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
        }
        return null;        
    }

    private List<Reserva> ObtenerReservasMesaParaHoy(List<Reserva> reservasMesaPendientes)
    {
        List<Reserva> reservas = new List<Reserva>();
        DateTime fechaHoy = DateTime.Today;
        foreach (Reserva reserva in reservasMesaPendientes)
        {
            DateTime fechaReserva = DateTime.Parse(reserva.Fecha); 

            // Si la fecha de la reserva es hoy y la reserva está confirmada o pendiente, se obtiene
            if (fechaReserva == fechaHoy && reserva.Estado.CompareTo(""+EstadoReserva.Confirmada) == 0 || reserva.Estado.CompareTo("" + EstadoReserva.Pendiente) == 0)
            {
                reservas.Add(reserva);
            }
        }
        return reservas;
    }

    private List<Reserva> ObtenerReservasMesaPendientes(int id_Mesa)
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
                    Debug.Log("Reservas mesa " + mesa.Id + ": " + reserva.Mostrar());
                    DateTime fechaReserva = DateTime.Parse(reserva.Fecha);
                    Debug.Log("Fecha Hoy: " + fechaHoy + "; Fecha Reserva: " + fechaReserva);
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

    private int ObtenerIDMesaDelNombreDelBotónMesa(Button botónMesaSelected)
    {
        string[] nombreBotónMesaSeparado = botónMesaSelected.name.Trim().Split("-");
        return int.Parse(nombreBotónMesaSeparado[1]);
    }

    private async void PonerReservaAMesaParaAhoraAsync()
    {
        Button botónMesaSelected = botónMesaSeleccionado;

        // Indicar al servidor
        DateTime hoy = DateTime.Today;
        string fechaDeHoy = hoy.ToString("dd/MM/yyyy");

        int id_Mesa = ObtenerIDMesaDelNombreDelBotónMesa(botónMesaSelected);
        TMP_InputField textoCantComensalesMesa = botónMesaSelected.gameObject.transform.Find("InputField").GetComponent<TMP_InputField>();
        int cantComensalesMesa = int.Parse(textoCantComensalesMesa.text.Trim());

        // Intento registrar la reserva de la mesa enviando datos al servidor. Pongo pendiente porque la reserva es para ahora mismo (en uso)
        string cad = await instanceMétodosApiController.PostDataAsync("reserva/crearReserva", new Reserva(0, fechaDeHoy, textHoraActual.text, ""+EstadoReserva.Pendiente, cantComensalesMesa, 0, id_Mesa));

        // Deserializo la respuesta
        Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);
        if (resultado.Result.Equals(1))
        {
            Debug.Log("Reserva registrada correctamente en mesa: "+id_Mesa);
            buttonNoDisponible.interactable = false;
            buttonDisponible.interactable = true;
        }
        else
        {
            Debug.Log("Error al registrar reserva en mesa");
        }

        Image img = botónMesaSelected.gameObject.transform.Find("Imagen Circle").GetComponent<Image>();
        PonerColorCorrectoAMesa(img, colorHexadecimalRojo);
        contenedorInfoReservasMesa.SetActive(false);
    }

    // Elimino todos los botones mesa antes de actualizar el fondo de edición, para que no sea un caos y se pongan unos encima de otros, además de su gestión luego.
    private void EliminarObjetosHijoDeFondoDeEdición()
    {
        foreach (Transform hijo in padreDeLosBotonesMesa)
        {
            Destroy(hijo.gameObject);
        }
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

        // Agrego este script al nuevo botón para dotarlo de funcionalidad de arrastre y escala
        //ButtonMesaController bm = botonGO.AddComponent<ButtonMesaController>();
        //bm.containerRect = this.padreDeLosBotonesMesa;  // Asigno el mismo contenedor
        //bm.rectTransform = rt; // Asigno el RectTransform del nuevo botón
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
            PonerColorCorrectoAMesa(img, colorHexadecimalVerde);
            
        }
        else
        {
            PonerColorCorrectoAMesa(img, colorHexadecimalRojo);
        }
    }

    private void PonerColorCorrectoAMesa(Image img, string hexadecimal)
    {
        Color newColor;
        // Intento convertir el string hexadecimal a Color
        if (ColorUtility.TryParseHtmlString(hexadecimal, out newColor))
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
}
