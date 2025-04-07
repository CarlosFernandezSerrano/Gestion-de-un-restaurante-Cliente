using Assets.Scripts.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BuscarReservaController : MonoBehaviour
{
    [SerializeField] private GameObject canvasBuscarReserva;
    [SerializeField] private TMP_InputField inputFieldDniCliente;
    [SerializeField] private RectTransform rectTransformContent;

    GestionarMesasController instanceGestionarMesasController;

    // Start is called before the first frame update
    void Start()
    {
        instanceGestionarMesasController = GestionarMesasController.InstanceGestionarMesasController;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuscarReservasDeCliente()
    {
        if (inputFieldDniCliente.text.Trim().Length > 5)
        {
            List<Reserva> reservasCliente = ObtenerReservasCliente(inputFieldDniCliente.text.Trim());

            // Tengo que eliminar todos los hijos (botones en este caso) de Content antes de poner nuevos (reservas actualizadas)
            EliminarObjetosHijoDeScrollView();

            if (reservasCliente.Count > 0)
            {
                // Una vez obtenidas las reservas del cliente, las coloco ordenadas como botones en un Scroll View
                CrearBotonesDeReservasDeCliente(reservasCliente);
            }
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
        Reserva reserva = ObtenerReservaEnUso(reservasCliente);

        // Existe una reserva en uso  
        if (reserva != null)
        {
            CrearBotónEnScrollView(reserva, 1);
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

    private Reserva ObtenerReservaEnUso(List<Reserva> reservasCliente)
    {
        foreach (Reserva reserva in reservasCliente)
        {
            if (reserva.Estado.CompareTo("" + EstadoReserva.Pendiente) == 0)
            {
                return reserva;
            }
        }
        return null;
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

        textoBotón.text = "  " + reserva.Fecha + "   " + reserva.Hora + "          " + reserva.CantComensales + "           " + reserva.Cliente.NumTelefono + "    " + reserva.Cliente.Nombre;
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

    public void DesactivarCanvasBuscarReserva()
    {
        canvasBuscarReserva.SetActive(false);
    }
}
