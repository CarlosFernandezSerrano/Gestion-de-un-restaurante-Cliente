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
    private Button bot�nMesaSeleccionado;

    private int contMostrarBotonesMesa = 1;

    // Contenedor padre donde se agregar�n los botones
    public RectTransform padreDeLosBotonesMesa;

    // Sprite que cargo desde Resources.
    private Sprite mesaSprite;

    M�todosAPIController instanceM�todosApiController;


    void Awake()
    {
        SceneManager.LoadSceneAsync("General Controller", LoadSceneMode.Additive);
    }

    // Start is called before the first frame update
    void Start()
    {
        instanceM�todosApiController = M�todosAPIController.InstanceM�todosAPIController;

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
        string cad = await instanceM�todosApiController.GetDataAsync("restaurante/getRestaurantePorId/" + PlayerPrefs.GetInt("Restaurante_ID Usuario"));

        // Deserializo la respuesta
        Restaurante restaurante = JsonConvert.DeserializeObject<Restaurante>(cad);

        textHoraApertura.text = restaurante.HoraApertura.Replace(" ", "");
        textHoraCierre.text = restaurante.HoraCierre.Replace(" ", "");
        Mesas = restaurante.Mesas;
        


        textNombreRestaurante.text = restaurante.Nombre;

        Debug.Log("Hora Apertura: " + restaurante.HoraApertura + "; Hora Cierre: " + restaurante.HoraCierre);

        // Los botones mesa s�lo se pintan una vez
        if (contMostrarBotonesMesa.Equals(1)){
            contMostrarBotonesMesa++;
            mesaSprite = Resources.Load<Sprite>("Editar Restaurante/mantelMesa");
            CrearBotonesMesas();
            ActualizarTodasLasReservasQueYaAcabaronYEst�nPendientesAsync(); 
            InvokeRepeating(nameof(ActualizarEstadoReservaYMesaDelD�aDeHoy), 0f, 1f); // Llama a ActualizarEstadoReservaYMesa() cada 1 segundo
        }

        A�adirListenerABotonesMesaDelMapa();
        
    }

    private async void ActualizarTodasLasReservasQueYaAcabaronYEst�nPendientesAsync()
    {
        DateTime fechaHoy = DateTime.Today;
        foreach (Mesa mesa in Mesas)
        {

            foreach (Reserva reserva in mesa.Reservas)
            {
                DateTime fechaReserva = DateTime.Parse(reserva.Fecha);

                if (fechaReserva < fechaHoy && reserva.Estado.CompareTo(""+EstadoReserva.Pendiente) == 0)
                {
                    string cad = await instanceM�todosApiController.PutDataAsync("reserva/actualizarEstadoReserva", new Reserva(reserva.Id, "", "", "" + EstadoReserva.Terminada, 0, 0, reserva.Mesa_Id));

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

    // Si una reserva ha acabado, se actualiza la mesa y la reserva. Los colores se actualizan aqu� aunque se podr�an actualizar aparte
    private async void ActualizarEstadoReservaYMesaDelD�aDeHoy() 
    {                                                       // Hay que hacer bastantes cambios y mejoras aqu� __ _ __ _-- _ - - - - -- -_ _ _ _ - -_ _ - _ _ -_
        // Compruebo si ya ha pasado su tiempo l�mite
        string horaActual = DateTime.Now.ToString("HH:mm");
        TimeSpan horaActualTimeSpan = TimeSpan.Parse(horaActual);

        foreach (Mesa mesa in Mesas)
        {
            // Obtengo las reservas pendientes que tiene la mesa, ya sean de hoy o en adelante
            List<Reserva> reservasMesaPendientes = ObtenerReservasMesaPendientes(mesa.Id);
            // Obtengo las reservas del d�a de hoy
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
                    string cad = await instanceM�todosApiController.PutDataAsync("reserva/actualizarEstadoReserva", new Reserva(reserva.Id, "", "", "" + EstadoReserva.Terminada, 0, 0, reserva.Mesa_Id));

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

                    string cad2 = await instanceM�todosApiController.PutDataAsync("mesa/actualizarCampoDisponible", new Mesa(reserva.Mesa_Id, 0, 0, 0, 0, 0, 0, 0, true, 0, new List<Reserva>()));

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
                    string cad = await instanceM�todosApiController.PutDataAsync("mesa/actualizarCampoDisponible", new Mesa(reserva.Mesa_Id, 0, 0, 0, 0, 0, 0, 0, false, 0, new List<Reserva>()));

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
                    string cad = await instanceM�todosApiController.PutDataAsync("mesa/actualizarCampoDisponible", new Mesa(reserva.Mesa_Id, 0, 0, 0, 0, 0, 0, 0, true, 0, new List<Reserva>()));

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

            // Si hay una reserva "Confirmada" que deber�a estar en uso, se pone en "Pendiente"
            if (horaActualTimeSpan < horaFinReserva && horaActualTimeSpan >= horaReservaTimeSpan && reserva.Estado.CompareTo("" + EstadoReserva.Confirmada) == 0)
            {
                string cad = await instanceM�todosApiController.PutDataAsync("reserva/actualizarEstadoReserva", new Reserva(reserva.Id, "", "", "" + EstadoReserva.Pendiente, 0, 0, reserva.Mesa_Id));

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

            // Si hay una reserva pendiente en uso, actualizo s�lo la mesa a "No Disponible" porque la reserva ya est� creada
            if (horaActualTimeSpan < horaFinReserva && horaActualTimeSpan >= horaReservaTimeSpan && reserva.Estado.CompareTo(""+EstadoReserva.Pendiente) == 0)
            {
                return true;
            }
        }
        return false;
    }

    private void A�adirListenerABotonesMesaDelMapa()
    {
        // Obtenemos todos los componentes Button que sean hijos del contenedor
        Button[] buttons = padreDeLosBotonesMesa.gameObject.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            // Es recomendable capturar la referencia del bot�n para evitar problemas con clausuras
            Button capturedButton = button;
            capturedButton.onClick.AddListener(() => MostrarContenedorInfoReservasMesa(capturedButton));
        }
    }

    private void MostrarContenedorInfoReservasMesa(Button capturedButton)
    {
        bot�nMesaSeleccionado = capturedButton; // Obtengo el bot�n mesa que he pulsado

        // Configuro los botones del contenedor info Mesa seg�n si est� disponible o no la mesa
        int id_Mesa = ObtenerIDMesaDelNombreDelBot�nMesa(bot�nMesaSeleccionado);
        bool mesaDisponible = BuscarSiLaMesaEst�Disponible(id_Mesa);
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

    private bool BuscarSiLaMesaEst�Disponible(int idMesa)
    {
        DateTime fechaHoy = DateTime.Today; // D�a de hoy

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

    // Si pulso el bot�n "Disponible" quiere decir que hay una reserva en curso y quiero cancelarla
    public void PulsarBot�nMesaDisponible()
    {
        CancelarReservaActualEnMesaAsync();
    }

    private async void CancelarReservaActualEnMesaAsync()
    {
        Button bot�nMesaSelected = bot�nMesaSeleccionado;

        int id_Mesa = ObtenerIDMesaDelNombreDelBot�nMesa(bot�nMesaSelected);

        // Obtengo las reservas pendientes que tiene la mesa, ya sean de hoy o en adelante
        List<Reserva> reservasMesaPendientes = ObtenerReservasMesaPendientes(id_Mesa);

        foreach (Reserva r in reservasMesaPendientes)
        {
            Debug.Log("Reservas mesa pendientes entre hoy y en adelante: " + r.Mostrar());
        }
        Debug.Log("--------------------------------");

        // Obtengo las reservas del d�a de hoy
        List<Reserva> reservasMesaParaHoy = ObtenerReservasMesaParaHoy(reservasMesaPendientes);

        // Obtengo la reserva actual en uso
        Reserva reserva = ObtenerReservaEnUso(reservasMesaParaHoy);
        
        if (reserva != null)
        {
            Debug.Log("Reserva not null");

            // Una vez obtenida la reserva pendiente, la cancelo en la BDD
            string cad = await instanceM�todosApiController.PutDataAsync("reserva/actualizarEstadoReserva", new Reserva(reserva.Id, "", "", ""+EstadoReserva.Cancelada, 0, 0, id_Mesa));

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

            // Tambi�n actualizo la mesa en la BDD en "Disponible" = true 
            string cad2 = await instanceM�todosApiController.PutDataAsync("mesa/actualizarCampoDisponible", new Mesa(id_Mesa, 0, 0, 0, 0, 0, 0, 0, true, 0, new List<Reserva>()));

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

            Image img = bot�nMesaSeleccionado.gameObject.transform.Find("Imagen Circle").GetComponent<Image>();
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

            // Si la fecha de la reserva es hoy y la reserva est� confirmada o pendiente, se obtiene
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

    private int ObtenerIDMesaDelNombreDelBot�nMesa(Button bot�nMesaSelected)
    {
        string[] nombreBot�nMesaSeparado = bot�nMesaSelected.name.Trim().Split("-");
        return int.Parse(nombreBot�nMesaSeparado[1]);
    }

    private async void PonerReservaAMesaParaAhoraAsync()
    {
        Button bot�nMesaSelected = bot�nMesaSeleccionado;

        // Indicar al servidor
        DateTime hoy = DateTime.Today;
        string fechaDeHoy = hoy.ToString("dd/MM/yyyy");

        int id_Mesa = ObtenerIDMesaDelNombreDelBot�nMesa(bot�nMesaSelected);
        TMP_InputField textoCantComensalesMesa = bot�nMesaSelected.gameObject.transform.Find("InputField").GetComponent<TMP_InputField>();
        int cantComensalesMesa = int.Parse(textoCantComensalesMesa.text.Trim());

        // Intento registrar la reserva de la mesa enviando datos al servidor. Pongo pendiente porque la reserva es para ahora mismo (en uso)
        string cad = await instanceM�todosApiController.PostDataAsync("reserva/crearReserva", new Reserva(0, fechaDeHoy, textHoraActual.text, ""+EstadoReserva.Pendiente, cantComensalesMesa, 0, id_Mesa));

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

        Image img = bot�nMesaSelected.gameObject.transform.Find("Imagen Circle").GetComponent<Image>();
        PonerColorCorrectoAMesa(img, colorHexadecimalRojo);
        contenedorInfoReservasMesa.SetActive(false);
    }

    // Elimino todos los botones mesa antes de actualizar el fondo de edici�n, para que no sea un caos y se pongan unos encima de otros, adem�s de su gesti�n luego.
    private void EliminarObjetosHijoDeFondoDeEdici�n()
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
        // Crear un GameObject para el bot�n y asignarle un nombre �nico.
        GameObject botonGO = new GameObject("Button-" + mesa.Id);

        // Establecer el padre para que se muestre en el UI.
        botonGO.transform.SetParent(padreDeLosBotonesMesa, false);

        // Agregar el componente RectTransform (se agrega autom�ticamente al crear UI, pero lo a�adimos expl�citamente).
        RectTransform rt = botonGO.AddComponent<RectTransform>();
        // Opcional: definir un tama�o por defecto para el bot�n.
        rt.sizeDelta = new Vector2(mesa.Width, mesa.Height);

        // Agregar CanvasRenderer para poder renderizar el UI.
        botonGO.AddComponent<CanvasRenderer>();

        // Agregar el componente Image para mostrar el sprite.
        UnityEngine.UI.Image imagen = botonGO.AddComponent<UnityEngine.UI.Image>();
        if (mesaSprite != null)
        {
            imagen.sprite = mesaSprite;
        }

        // Configurar la posici�n y escala del bot�n bas�ndose en las propiedades de la mesa.
        rt.anchoredPosition = new Vector2(mesa.PosX, mesa.PosY);
        rt.localScale = new Vector3(mesa.ScaleX, mesa.ScaleY, 1f);

        // Agrego un componente Button para que sea interactivo
        botonGO.AddComponent<Button>();

        // Creo nuevos GameObject hijos, las im�genes del bot�n
        CrearImgsDelButton(rt, mesa.Disponible);

        StartCoroutine(CrearUnHijoInputFieldDelBot�nMesa(botonGO, mesa.CantPers));

        // Agrego este script al nuevo bot�n para dotarlo de funcionalidad de arrastre y escala
        //ButtonMesaController bm = botonGO.AddComponent<ButtonMesaController>();
        //bm.containerRect = this.padreDeLosBotonesMesa;  // Asigno el mismo contenedor
        //bm.rectTransform = rt; // Asigno el RectTransform del nuevo bot�n
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
        // El nuevo bot�n se crear� como hijo del contenedor, NO del Canvas
        imgObject.transform.SetParent(newRect, false);

        // Agrego y configuro el RectTransform: posici�n central y tama�o predeterminado
        RectTransform rectButton = imgObject.AddComponent<RectTransform>();
        rectButton.anchoredPosition = Vector2.zero;
        rectButton.sizeDelta = new Vector2(85, 85); // Tama�o (ancho/alto)

        // Agrego un componente Image
        Image img = imgObject.AddComponent<Image>();
        Sprite newSprite = Resources.Load<Sprite>("Editar Restaurante/circle perfect 1.0");
        if (newSprite != null)
        {
            img.sprite = newSprite;
        }
        else
        {
            Debug.LogWarning("No se encontr� la imagen en Resources: circle perfect 1.0");
        }
        
        // Poner color correcto a mesa seg�n si est� disponible o no. Verde = S� ; Rojo = No
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
            Debug.LogError("El formato del color hexadecimal es inv�lido.");
        }
    }

    private void CrearImgRectangle(RectTransform newRect)
    {
        // Creo el objeto
        GameObject imgObject = new GameObject("Imagen Rectangle");
        // El nuevo bot�n se crear� como hijo del contenedor, NO del Canvas
        imgObject.transform.SetParent(newRect, false);

        // Agrego y configuro el RectTransform: posici�n central y tama�o predeterminado
        RectTransform rectImg = imgObject.AddComponent<RectTransform>();
        rectImg.anchoredPosition = new Vector2(-67.5f, 35); // x e y
        rectImg.sizeDelta = new Vector2(45, 30); // Tama�o (ancho/alto)

        // Agrego un componente Image
        Image img = imgObject.AddComponent<Image>();
        /*Sprite newSprite = Resources.Load<Sprite>("Editar Restaurante/circle perfect 1.0");
        if (newSprite != null)
        {
            img.sprite = newSprite;
        }
        else
        {
            Debug.LogWarning("No se encontr� la imagen en Resources: circle perfect 1.0");
        }*/

        // Creo un gameObject TMP_Text y lo pongo de hijo en el objeto imagen rect�ngulo
        GameObject textObject = new GameObject("Text");
        // El nuevo bot�n se crear� como hijo del contenedor, NO del Canvas
        textObject.transform.SetParent(rectImg, false);

        // Agrego y configuro el RectTransform: posici�n central y tama�o predeterminado
        RectTransform rectText = textObject.AddComponent<RectTransform>();
        rectText.anchoredPosition = Vector2.zero;
        rectText.sizeDelta = new Vector2(40, 30); // Tama�o (ancho/alto)

        // Agrego un componente TMP_Text
        TMP_Text text = textObject.AddComponent<TextMeshProUGUI>();
        text.alignment = TextAlignmentOptions.Center; // Centro el texto
        lastIDMesa++;
        text.text = "" + lastIDMesa;
        text.fontSize = 32;
        text.fontStyle = FontStyles.Bold;

    }

    private IEnumerator CrearUnHijoInputFieldDelBot�nMesa(GameObject newButtonObj, int cantComensales)
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

        // Desactivo Raycast Target para que no bloqueen interacci�n con el bot�n
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

    public void IrAlMen�Principal()
    {
        SceneManager.LoadScene("Main");
    }
}
