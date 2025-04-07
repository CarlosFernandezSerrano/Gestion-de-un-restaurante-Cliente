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
            CrearBot�nEnScrollView(reserva, 1);
        }

        // Existen reservas de la mesa para hoy confirmadas 
        if (reservasConfirmadas.Count > 0)
        {
            var reservasOrdenadasPorHora = reservasConfirmadas
            .OrderByDescending(r => DateTime.Parse(r.Fecha)) // primero por fecha descendente (m�s recientes arriba)
            .ThenBy(r => TimeSpan.Parse(r.Hora))   // luego por hora descendente
            .ToList();


            foreach (Reserva r in reservasOrdenadasPorHora)
            {
                CrearBot�nEnScrollView(r, 2);
            }
        }

        // Existen reservas de la mesa para hoy terminadas 
        if (reservasTerminadasOCanceladas.Count > 0)
        {
            var reservasOrdenadasPorHora = reservasTerminadasOCanceladas
            .OrderByDescending(r => DateTime.Parse(r.Fecha)) // primero por fecha descendente (m�s recientes arriba)
            .ThenBy(r => TimeSpan.Parse(r.Hora))   // luego por hora descendente
            .ToList();

            foreach (Reserva reserv in reservasOrdenadasPorHora)
            {
                CrearBot�nEnScrollView(reserv, 3);
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

    private void CrearBot�nEnScrollView(Reserva reserva, int num)
    {
        // Creo un GameObject para el bot�n y le asigno un nombre �nico.
        GameObject bot�nGO = new GameObject("Button-" + reserva.Id);

        // Establezco el padre para que se muestre en el UI.
        bot�nGO.transform.SetParent(rectTransformContent, false);

        // Agrego el componente RectTransform (se agrega autom�ticamente al crear UI, pero lo a�ado expl�citamente).
        RectTransform rt = bot�nGO.AddComponent<RectTransform>();
        // Defino un tama�o por defecto para el bot�n.
        rt.sizeDelta = new Vector2(1530.9f, 138f);

        // Agrego CanvasRenderer para poder renderizar el UI.
        bot�nGO.AddComponent<CanvasRenderer>();

        // Agrego el componente Image para mostrar el sprite.
        Image imagen = bot�nGO.AddComponent<Image>();

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
        bot�nGO.AddComponent<Button>();

        // Creo un nuevo GameObject hijo, el texto del bot�n
        CrearTextoDelButton(rt, reserva);
    }

    private void CrearTextoDelButton(RectTransform rt, Reserva reserva)
    {
        // Creo un GameObject para el bot�n y le asigno un nombre �nico.
        GameObject textGO = new GameObject("TMP_Text");

        // Establezco el padre para que se muestre en el UI.
        textGO.transform.SetParent(rt, false);

        // Agrego el componente RectTransform (se agrega autom�ticamente al crear UI, pero lo a�ado expl�citamente).
        RectTransform rtText = textGO.AddComponent<RectTransform>();
        // Anclas estiradas (stretch) en ambas direcciones
        rtText.anchorMin = new Vector2(0, 0);
        rtText.anchorMax = new Vector2(1, 1);

        // M�rgenes todos a 0 (equivale a Left, Right, Top, Bottom en el inspector)
        rtText.offsetMin = Vector2.zero;
        rtText.offsetMax = Vector2.zero;

        // Centrado por si acaso (aunque no influye mucho cuando est� estirado)
        rtText.anchoredPosition = Vector2.zero;

        // Agrego CanvasRenderer para poder renderizar el UI.
        textGO.AddComponent<CanvasRenderer>();

        // Agrego el componente TMP_Text para mostrar el sprite.
        TMP_Text textoBot�n = textGO.AddComponent<TextMeshProUGUI>();
        textoBot�n.fontStyle = FontStyles.Bold;
        textoBot�n.fontSize = 56;
        textoBot�n.alignment = TextAlignmentOptions.Left;

        textoBot�n.text = "  " + reserva.Fecha + "   " + reserva.Hora + "          " + reserva.CantComensales + "           " + reserva.Cliente.NumTelefono + "    " + reserva.Cliente.Nombre;
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
