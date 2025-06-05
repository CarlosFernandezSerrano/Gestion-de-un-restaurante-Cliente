using Assets.Scripts.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuscarReservaController : MonoBehaviour
{
    [SerializeField] private GameObject canvasBuscarReserva;
    [SerializeField] private TMP_InputField inputFieldDniCliente;
    [SerializeField] private RectTransform rectTransformContent;
    [SerializeField] private Button botónCancelarReserva;
    [SerializeField] private GameObject bloqueConfirmarCancelarReserva;
    [SerializeField] private Button botónConfirmarCancelarReserva;
    [SerializeField] private Button botónBuscarReservas;
    [SerializeField] private TextMeshProUGUI titulo;

    private List<Button> botonesParaCancelar = new List<Button>();
    private Button botónMesaSeleccionado;
    private int id_ReservaACancelar;
    private string dniEnInputField;

    GestionarMesasController instanceGestionarMesasController;
    MétodosAPIController instanceMétodosAPIController;

    // Start is called before the first frame update
    void Start()
    {
        instanceGestionarMesasController = GestionarMesasController.InstanceGestionarMesasController;
        instanceMétodosAPIController = MétodosAPIController.InstanceMétodosAPIController;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuscarReservasDeCliente()
    {
        if (inputFieldDniCliente.text.Trim().Length.Equals(9))
        {
            dniEnInputField = inputFieldDniCliente.text.Trim();

            BuscarReservasCliente(dniEnInputField);

            inputFieldDniCliente.text = "";
        }        
    }

    private void BuscarReservasCliente(string dniEnInputField)
    {
        List<Reserva> reservasCliente = ObtenerReservasCliente(dniEnInputField);

        // Tengo que eliminar todos los hijos (botones en este caso) de Content antes de poner nuevos (reservas actualizadas)
        EliminarObjetosHijoDeScrollView();

        if (reservasCliente.Count > 0)
        {
            // Una vez obtenidas las reservas del cliente, las coloco ordenadas como botones en un Scroll View
            CrearBotonesDeReservasDeCliente(reservasCliente);
        }
    }

    private void EliminarObjetosHijoDeScrollView()
    {
        foreach (Transform hijo in rectTransformContent)
        {
            Destroy(hijo.gameObject);
        }
    }

    private void CrearBotonesDeReservasDeCliente(List<Reserva> reservasCliente)
    {
        List<Reserva> reservasTerminadasOCanceladas = ObtenerReservasTerminadasOCanceladas(reservasCliente);
        List<Reserva> reservasConfirmadas = ObtenerReservasConfirmadas(reservasCliente);
        List<Reserva> reservasEnUso = ObtenerReservasEnUso(reservasCliente);

        botonesParaCancelar.Clear();

        // Existe una o varias reservas en uso  
        if (reservasEnUso.Count > 0)
        {
            var reservasOrdenadasPorHora = reservasEnUso
            .OrderByDescending(r => DateTime.Parse(r.Fecha)) // primero por fecha descendente (más recientes arriba)
            .ThenBy(r => TimeSpan.Parse(r.Hora))   // luego por hora descendente
            .ToList();


            foreach (Reserva r in reservasOrdenadasPorHora)
            {
                CrearBotónEnScrollView(r, 1);
            }            
        }

        // Existen reservas de la mesa para hoy confirmadas 
        if (reservasConfirmadas.Count > 0)
        {
            var reservasOrdenadasPorHora = reservasConfirmadas
            .OrderByDescending(r => DateTime.Parse(r.Fecha)) // primero por fecha descendente (más recientes arriba)
            .ThenBy(r => TimeSpan.Parse(r.Hora))   // luego por hora descendente
            .ToList();


            foreach (Reserva r in reservasOrdenadasPorHora)
            {
                CrearBotónEnScrollView(r, 2);
            }
        }

        // Existen reservas de la mesa para hoy terminadas 
        if (reservasTerminadasOCanceladas.Count > 0)
        {
            var reservasOrdenadasPorHora = reservasTerminadasOCanceladas
            .OrderByDescending(r => DateTime.Parse(r.Fecha)) // primero por fecha descendente (más recientes arriba)
            .ThenBy(r => TimeSpan.Parse(r.Hora))   // luego por hora descendente
            .ToList();

            foreach (Reserva reserv in reservasOrdenadasPorHora)
            {
                CrearBotónEnScrollView(reserv, 3);
            }
        }

        // Poner listener a los botones que se pueden cancelar
        PonerListenerABotonesDeReservasQueSePuedenCancelar();
    }

    private void PonerListenerABotonesDeReservasQueSePuedenCancelar()
    {
        // Obtenemos todos los componentes Button que sean hijos del contenedor
        foreach (Button button in botonesParaCancelar)
        {
            // Es recomendable capturar la referencia del botón para evitar problemas con clausuras
            Button capturedButton = button;
            capturedButton.onClick.AddListener(() => ActivarBotónCancelarReserva(capturedButton));
        }
    }

    private void ActivarBotónCancelarReserva(Button capturedButton)
    {
        botónMesaSeleccionado = capturedButton; // Obtengo el botón mesa que he pulsado

        botónCancelarReserva.interactable = true;
    }

    private List<Reserva> ObtenerReservasTerminadasOCanceladas(List<Reserva> reservasCliente)
    {
        List<Reserva> reservas = new List<Reserva>();
        foreach (Reserva reserva in reservasCliente)
        {
            if (reserva.Estado.CompareTo(""+EstadoReserva.Terminada) == 0 || reserva.Estado.CompareTo(""+EstadoReserva.Cancelada) == 0)
            {
                reservas.Add(reserva);
            }
        }
        return reservas;
    }

    private List<Reserva> ObtenerReservasConfirmadas(List<Reserva> reservasCliente)
    {
        List<Reserva> reservas = new List<Reserva>();
        foreach (Reserva reserva in reservasCliente)
        {
            if (reserva.Estado.CompareTo("" + EstadoReserva.Confirmada) == 0)
            {
                reservas.Add(reserva);
            }
        }
        return reservas;
    }

    private List<Reserva> ObtenerReservasEnUso(List<Reserva> reservasCliente)
    {
        List<Reserva> reservas = new List<Reserva>();
        foreach (Reserva reserva in reservasCliente)
        {
            if (reserva.Estado.CompareTo("" + EstadoReserva.Pendiente) == 0)
            {
                reservas.Add(reserva);
            }
        }
        return reservas;
    }

    private void CrearBotónEnScrollView(Reserva reserva, int num)
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
                instanceGestionarMesasController.PonerColorCorrectoAImg(imagen, "#6DEC6F");
                //imagen.color = Color.green;
                break;
            case 2:
                instanceGestionarMesasController.PonerColorCorrectoAImg(imagen, "#FDF468");
                break;
            case 3:
                instanceGestionarMesasController.PonerColorCorrectoAImg(imagen, "#EC6C6C");
                break;
        }

        // Agrego un componente Button para que sea interactivo
        botónGO.AddComponent<Button>();

        // Creo un nuevo GameObject hijo, el texto del botón
        CrearTextoDelButton(rt, reserva);

        if (num.Equals(1) || num.Equals(2))
        {
            botonesParaCancelar.Add(botónGO.GetComponent<Button>());
        }
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
        textoBotón.fontSize = 46;
        textoBotón.alignment = TextAlignmentOptions.Left;

        // Obtengo el Id de la mesa en el mapa
        Button botónMesaSelected = instanceGestionarMesasController.padreDeLosBotonesMesa.gameObject.transform.Find("Button-" + reserva.Mesa_Id).GetComponent<Button>();
        int id_Mesa_En_Mapa = int.Parse(instanceGestionarMesasController.ObtenerIDMesaDelMapa(botónMesaSelected));

        PonerTextoBotónDeFormaCorrecta(textoBotón, reserva, id_Mesa_En_Mapa);
        
    }

    private void PonerTextoBotónDeFormaCorrecta(TMP_Text textoBotón, Reserva reserva, int id_Mesa_En_Mapa)
    {
        // El cliente de la reserva tiene un número de teléfono registrado
        if (reserva.Cliente.NumTelefono.Trim().Length > 0)
        {
            if (id_Mesa_En_Mapa.ToString().Length.Equals(2))
            {
                textoBotón.text = " " + reserva.Fecha + "  " + reserva.Hora + "        " + reserva.CantComensales + "            " + id_Mesa_En_Mapa + "      " + reserva.Cliente.Dni + "   " + reserva.Cliente.NumTelefono + "    " + reserva.Cliente.Nombre;
            }
            else
            {
                textoBotón.text = " " + reserva.Fecha + "  " + reserva.Hora + "        " + reserva.CantComensales + "            " + id_Mesa_En_Mapa + "        " + reserva.Cliente.Dni + "   " + reserva.Cliente.NumTelefono + "    " + reserva.Cliente.Nombre;
            }
        }
        else
        {
            if (id_Mesa_En_Mapa.ToString().Length.Equals(2))
            {
                textoBotón.text = " " + reserva.Fecha + "  " + reserva.Hora + "        " + reserva.CantComensales + "            " + id_Mesa_En_Mapa + "      " + reserva.Cliente.Dni + "                       " + reserva.Cliente.Nombre;
            }
            else
            {
                textoBotón.text = " " + reserva.Fecha + "  " + reserva.Hora + "        " + reserva.CantComensales + "            " + id_Mesa_En_Mapa + "        " + reserva.Cliente.Dni + "                       " + reserva.Cliente.Nombre;
            }
        }
    }

    private List<Reserva> ObtenerReservasCliente(string dni)
    {
        List<Mesa> mesasRestaurante = instanceGestionarMesasController.GetMesas();

        List<Reserva> reservasCliente = new List<Reserva>();
        foreach (Mesa mesa in mesasRestaurante)
        {
            foreach (Reserva reserva in mesa.Reservas)
            {
                if (reserva.Cliente.Dni.CompareTo(dni) == 0)
                {
                    reservasCliente.Add(reserva);
                }
            }
        }
        return reservasCliente;
    }

    public void CancelarReserva()
    {
        botónBuscarReservas.interactable = false;

        PonerInteractuablesONoLosBotonesHijoDelScrollView(false);
        botónCancelarReserva.interactable = false;

        Debug.Log("- -Paso por cancelar reserva");

        string[] nombreBotónSeleccionadoArray = botónMesaSeleccionado.gameObject.name.Split("-");
        id_ReservaACancelar = int.Parse(nombreBotónSeleccionadoArray[1]);

        Debug.Log("- -ID reserva: "+ id_ReservaACancelar);

        bloqueConfirmarCancelarReserva.SetActive(true);
    }

    private void PonerInteractuablesONoLosBotonesHijoDelScrollView(bool b)
    {
        foreach (Transform botón in rectTransformContent)
        {
            botón.GetComponent<Button>().interactable = b;
        }
    }

    public async void ConfirmarCancelarReserva()
    {
        botónConfirmarCancelarReserva.interactable = false;

        string cad = await instanceMétodosAPIController.PutDataAsync("reserva/actualizarEstadoReserva", new Reserva(id_ReservaACancelar, "", "", "" + EstadoReserva.Cancelada, 0, 0, 0, new Cliente("", "", "")));

        // Deserializo la respuesta
        Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

        if (resultado.Result.Equals(1))
        {
            Debug.Log("- -Reserva cancelada correctamente");

            // Espero 1.5 segundos a que el código del cliente actualice sus mesas
            await Task.Delay(1500);

            BuscarReservasCliente(dniEnInputField);
            bloqueConfirmarCancelarReserva.SetActive(false);
            botónBuscarReservas.interactable = true;
            PonerInteractuablesONoLosBotonesHijoDelScrollView(true);
        }
        else
        {
            Debug.Log("Error al intentar terminar una reserva");
        }
        
        botónConfirmarCancelarReserva.interactable = true;
    }

    public void DesactivarBloqueConfirmarCancelarReserva()
    {
        botónCancelarReserva.interactable = false;
        bloqueConfirmarCancelarReserva.SetActive(false);
        botónBuscarReservas.interactable = true;
        PonerInteractuablesONoLosBotonesHijoDelScrollView(true);
    }

    public void DesactivarCanvasBuscarReserva()
    {
        canvasBuscarReserva.SetActive(false);

        EliminarObjetosHijoDeScrollView();
        inputFieldDniCliente.text = "";
        botónCancelarReserva.interactable = false;
    }
}
