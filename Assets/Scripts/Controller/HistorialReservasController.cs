using Assets.Scripts.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HistorialReservasController : MonoBehaviour
{
    [SerializeField] private GameObject canvasHistorialReservas;
    [SerializeField] private TMP_InputField inputFieldNombre;
    [SerializeField] private TMP_InputField inputFieldNumMesa;
    [SerializeField] private TMP_InputField inputFieldFecha;
    [SerializeField] private Button buttonBuscar;
    [SerializeField] private RectTransform rtContentHistorialReservas;
    [SerializeField] private RectTransform rtPadreDeLosBotonesMesa;
    [SerializeField] private Scrollbar scrollbarHistorialReservas;
    [SerializeField] private TMP_InputField[] inputFields; // Asigno los InputFields en el orden de tabulación deseado

    GestionarMesasController instanceGestionarMesasController;

    // Start is called before the first frame update
    void Start()
    {
        instanceGestionarMesasController = GestionarMesasController.InstanceGestionarMesasController;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SelectNextInputField();
        }

        ActivarBotónBuscar();
    }

    private void ActivarBotónBuscar()
    {
        string nombre = Regex.Replace(inputFieldNombre.text, @"\s+", "");
        string numMesa = Regex.Replace(inputFieldNumMesa.text, @"\s+", "");
        string fecha = Regex.Replace(inputFieldFecha.text, @"\s+", "");
        
        if (nombre.Length > 0 || numMesa.Length > 0 || fecha.Length > 0)
        {
            if (numMesa.Length > 0)
            {
                try
                {
                    int.Parse(numMesa);
                }
                catch
                {
                    buttonBuscar.interactable = false;
                    Debug.Log("Error, la cadena no es un número entero");
                    return;
                }
            }

            // Valido la fecha
            if (fecha.Length > 0)
            {
                if (!fecha.Length.Equals(8) || !FechaCorrecta(fecha))
                {
                    buttonBuscar.interactable = false;
                    Debug.Log("Error, la fecha no es válida");
                    return;
                }
            }

            buttonBuscar.interactable = true;
        }
        else
        {
            buttonBuscar.interactable = false;
        }
    }

    private bool FechaCorrecta(string fecha)
    {
        int num1 = 0;
        int num2 = 0;
        int num3 = 0;
        int num4 = 0;

        for (int i = 0; i < fecha.Length; i++)
        {
            try
            {
                switch (i)
                {
                    case 0:
                        num1 = int.Parse(""+fecha[i]);
                        break;
                    case 1:
                        num2 = int.Parse("" + fecha[i]);
                        break;
                    case 2:
                        if (fecha[i] != '/')
                        {
                            return false;
                        }
                        break;
                    case 3:
                        num3 = int.Parse("" + fecha[i]);
                        break;
                    case 4:
                        num4 = int.Parse("" + fecha[i]);
                        break;
                    case 5:
                        if (fecha[i] != '/')
                        {
                            return false;
                        }
                        break;
                    case 6:
                        int.Parse("" + fecha[i]);
                        break;
                    case 7:
                        int.Parse("" + fecha[i]);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.Log("Exception: " + ex);
                return false;
            }
        }


        if (num1.Equals(0) && num2.Equals(0) || num3.Equals(0) && num4.Equals(0) || int.Parse(num1+""+num2) > 31 || int.Parse(num3 + "" + num4) > 12)
        {
            return false;
        }

        return true;
    }

    public void ActualizarScrollViewHistorial()
    {
        // Vacío los contenedores
        inputFieldNombre.text = "";
        inputFieldNumMesa.text = "";
        inputFieldFecha.text = "";

        instanceGestionarMesasController.ActivarCanvasHistorialReservas();
    }

    public void BuscarReservasEnHistorialMedianteFiltros()
    {
        string nombre = Regex.Replace(inputFieldNombre.text, @"\s+", "");
        string numMesa = Regex.Replace(inputFieldNumMesa.text, @"\s+", "");
        string fecha = Regex.Replace(inputFieldFecha.text, @"\s+", "");

        CrearBotonesEnHistorialReservas(nombre, numMesa, fecha);

        scrollbarHistorialReservas.value = 1; // Cada vez que muestro el scroll view, pongo el scroll arriba del todo
    }

    private void CrearBotonesEnHistorialReservas(string nombre, string numMesa, string fecha)
    {
        // Vacío los contenedores
        inputFieldNombre.text = "";
        inputFieldNumMesa.text = "";
        inputFieldFecha.text = "";

        instanceGestionarMesasController.EliminarObjetosHijoDeScrollView(rtContentHistorialReservas);

        // Completar fecha. Ejem: Antes - 25 ; Después - 2025
        if (fecha.Length > 0)
        {
            string dosPrimerosDigitosAñoActual = System.DateTime.Now.Year.ToString().Substring(0, 2);
            fecha = fecha.Substring(0,6) + dosPrimerosDigitosAñoActual.Trim() + fecha.Substring(fecha.Length - 2);
        }

        List<Reserva> reservasDeHace1Mes = instanceGestionarMesasController.ObtenerTodasLasReservasDeHace1Mes();
        List<Reserva> reservasTerminadasCanceladasOConfirmadas = instanceGestionarMesasController.ObtenerReservasTerminadasCanceladasOConfirmadas(reservasDeHace1Mes);
        List<Reserva> reservasEnUso = instanceGestionarMesasController.ObtenerReservasEnUso(reservasDeHace1Mes);

        List<Reserva> reservasFiltradasTerminadasCanceladasOConfirmadas = FiltrarReservas(reservasTerminadasCanceladasOConfirmadas, nombre, numMesa, fecha);
        List<Reserva> reservasFiltradasEnUso = FiltrarReservas(reservasEnUso, nombre, numMesa, fecha);

        // Existe una o varias reservas en uso  
        if (reservasFiltradasEnUso.Count > 0)
        {
            var reservasOrdenadasPorHora = reservasFiltradasEnUso
            .OrderByDescending(r => DateTime.Parse(r.Fecha)) // primero por fecha descendente (más recientes arriba)
            .ThenBy(r => TimeSpan.Parse(r.Hora))   // luego por hora descendente
            .ToList();


            foreach (Reserva r in reservasOrdenadasPorHora)
            {
                instanceGestionarMesasController.CrearBotónEnScrollViewHistorialReservas(r);
            }
        }

        // Existen reservas de la mesa para hoy terminadas/canceladas o confirmadas 
        if (reservasFiltradasTerminadasCanceladasOConfirmadas.Count > 0)
        {
            var reservasOrdenadasPorHora = reservasFiltradasTerminadasCanceladasOConfirmadas
            .OrderByDescending(r => DateTime.Parse(r.Fecha)) // primero por fecha descendente (más recientes arriba)
            .ThenBy(r => TimeSpan.Parse(r.Hora))   // luego por hora descendente
            .ToList();

            foreach (Reserva reserv in reservasOrdenadasPorHora)
            {
                instanceGestionarMesasController.CrearBotónEnScrollViewHistorialReservas(reserv);
            }
        }
    }

    private List<Reserva> FiltrarReservas(List<Reserva> reservasSinFiltrar, string nombre, string numMesa, string fecha)
    {
        List<Reserva> reservasFiltradas = new List<Reserva>();
        
        // Si están rellenados todos los campos
        if (nombre.Length > 0 && numMesa.Length > 0 && fecha.Length > 0)
        {
            // Primero se filtra por fecha
            reservasFiltradas = FiltroFecha(reservasSinFiltrar, fecha);

            // Luego se filtra por número de mesa
            reservasFiltradas = FiltroNumMesa(reservasFiltradas, numMesa);

            // Por último, se filtra por nombre del cliente
            reservasFiltradas = FiltroNombre(reservasFiltradas, nombre);

        }

        // Si sólo se ha rellenado el campo nombre
        if (nombre.Length > 0 && numMesa.Length.Equals(0) && fecha.Length.Equals(0))
        {
            reservasFiltradas = FiltroNombre(reservasSinFiltrar, nombre);
        }

        // Si sólo se ha rellenado el campo número de mesa
        if (nombre.Length.Equals(0) && numMesa.Length > 0 && fecha.Length.Equals(0))
        {
            reservasFiltradas = FiltroNumMesa(reservasSinFiltrar, numMesa);
        }

        // Si sólo se ha rellenado el campo fecha
        if (nombre.Length.Equals(0) && numMesa.Length.Equals(0) && fecha.Length > 0)
        {
            reservasFiltradas = FiltroFecha(reservasSinFiltrar, fecha);
        }

        // Si sólo se han rellenado el campo nombre y el campo número de mesa
        if (nombre.Length > 0 && numMesa.Length > 0 && fecha.Length.Equals(0))
        {
            // Primero se filtra por número de mesa
            reservasFiltradas = FiltroNumMesa(reservasSinFiltrar, numMesa);

            // Por último, se filtra por nombre del cliente
            reservasFiltradas = FiltroNombre(reservasFiltradas, nombre);
        }

        // Si sólo se han rellenado el campo nombre y el campo fecha
        if (nombre.Length > 0 && numMesa.Length.Equals(0) && fecha.Length > 0)
        {
            // Primero se filtra por fecha
            reservasFiltradas = FiltroFecha(reservasSinFiltrar, fecha);

            // Luego se filtra por nombre del cliente
            reservasFiltradas = FiltroNombre(reservasFiltradas, nombre);
        }

        // Si sólo se han rellenado el campo numMesa y el campo fecha
        if (nombre.Length.Equals(0) && numMesa.Length > 0 && fecha.Length > 0)
        {
            // Primero se filtra por fecha
            reservasFiltradas = FiltroFecha(reservasSinFiltrar, fecha);

            // Luego se filtra por número de mesa
            reservasFiltradas = FiltroNumMesa(reservasFiltradas, numMesa);
        }

        return reservasFiltradas;       
    }


    // Filtro las reservas por nombre
    private List<Reserva> FiltroNombre(List<Reserva> reservas, string nombre)
    {
        List<Reserva> reservasFiltradas = new List<Reserva>();
        foreach (Reserva reserva in reservas)
        {
            if (reserva.Cliente.Nombre.Contains(nombre))
            {
                reservasFiltradas.Add(reserva);
            }
        }
        return reservasFiltradas;
    }

    // Filtro las reservas por número de mesa
    private List<Reserva> FiltroNumMesa(List<Reserva> reservas, string numMesa)
    {
        List<Reserva> reservasFiltradas = new List<Reserva>();
        foreach (Reserva reserva in reservas)
        {
            // Obtengo el Id de la mesa en el mapa
            Button botónMesaSelected = rtPadreDeLosBotonesMesa.gameObject.transform.Find("Button-" + reserva.Mesa_Id).GetComponent<Button>();
            int id_Mesa_En_Mapa = int.Parse(instanceGestionarMesasController.ObtenerIDMesaDelMapa(botónMesaSelected));

            if (id_Mesa_En_Mapa.Equals(int.Parse(numMesa)))
            {
                reservasFiltradas.Add(reserva);
            }
        }
        return reservasFiltradas;
    }

    // Filtro las reservas por fecha
    private List<Reserva> FiltroFecha(List<Reserva> reservas, string fecha)
    {
        List<Reserva> reservasFiltradas = new List<Reserva>();
        foreach (Reserva reserva in reservas)
        {
            if (reserva.Fecha.Contains(fecha))
            {
                reservasFiltradas.Add(reserva);
            }
        }
        return reservasFiltradas;
    }

    public void DesactivarCanvasHistorialReservas()
    {
        canvasHistorialReservas.SetActive(false);
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
