using Assets.Scripts.Model;
using System;
using TMPro;
using UnityEngine;

public class Cambio_Idioma_Escena_Gestionar_Mesas_Controller : MonoBehaviour
{
    // Textos en canvas Gestionar Mesas
    [SerializeField] private TMP_Text textRestauranteGestMesas;
    [SerializeField] private TMP_Text textHoraAperturaGestMesas;
    [SerializeField] private TMP_Text textHoraCierreGestMesas;
    [SerializeField] private TMP_Text textBuscarReservaGestMesas;
    [SerializeField] private TMP_Text textCrearReservaGestMesas;
    [SerializeField] private TMP_Text textHistorialReservasGestMesas;
    [SerializeField] private TMP_Text textBtnVerRegistrosGestMesas;
    [SerializeField] private TMP_Text textBtnNoDisponibleGestMesas;
    [SerializeField] private TMP_Text textBtnDisponibleGestMesas;

    // Textos en canvas Registros Hoy Mesa
    [SerializeField] private TMP_Text textEncabezadoRegistrosMesa;
    [SerializeField] private TMP_Text textIndicaciónEstructuraRegistrosMesa;
    [SerializeField] private TMP_Text textTextoEnUsoRegistrosMesa;
    [SerializeField] private TMP_Text textTextoConfirmadaRegistrosMesa;
    [SerializeField] private TMP_Text textTerminadaCanceladaRegistrosMesa;

    // Textos en canvas Historial Reservas
    [SerializeField] private TMP_Text textEncabezadoHistorialReservas;
    [SerializeField] private TMP_Text textIndicaciónEstructuraHistorialReservas;
    [SerializeField] private TMP_Text textEnUsoHistorialReservas;
    [SerializeField] private TMP_Text textConfirmadaEncabezadoHistorialReservas;
    [SerializeField] private TMP_Text textTerminadaCanceladaHistorialReservas;
    [SerializeField] private TMP_Text textPlaceHolderNombreHistorialReservas;
    [SerializeField] private TMP_Text textPlaceHolderNumMesaHistorialReservas;
    [SerializeField] private TMP_Text textPlaceHolderFechaHistorialReservas;
    [SerializeField] private TMP_Text textBtnBuscarHistorialReservas;
    [SerializeField] private TMP_Text textBtnActualizarHistorialReservas;

    // Textos en canvas Buscar Reserva
    [SerializeField] private TMP_Text textEncabezadoBuscarReserva;
    [SerializeField] private TMP_Text textPlaceHolderInputFieldDniBuscarReserva;
    [SerializeField] private TMP_Text textBtnBuscarEnBuscarReserva;
    [SerializeField] private TMP_Text textIndicaciónEstructuraBuscarReserva;
    [SerializeField] private TMP_Text textEnUsoBuscarReserva;
    [SerializeField] private TMP_Text textConfirmadaBuscarReserva;
    [SerializeField] private TMP_Text textTerminadaCanceladaBuscarReserva;
    [SerializeField] private TMP_Text textBtnCancelarBuscarReserva;
    [SerializeField] private TMP_Text textBtnConfirmarBuscarReserva;
    [SerializeField] private TMP_Text textImgConfirmarCancelarReservaBuscarReserva;

    // Textos en canvas Crear Reserva
    [SerializeField] private TMP_Text textEncabezadoCrearReserva;
    [SerializeField] private TMP_Text text6CeldasEnCrearReserva;
    [SerializeField] private TMP_Text textBtnBuscarMesasDisponiblesCrearReserva;
    [SerializeField] private TMP_Text textMesasDisponiblesCrearReserva;
    [SerializeField] private TMP_Text textNombre14CeldasEnCrearReserva;
    [SerializeField] private TMP_Text textDNI14CeldasEnCrearReserva;
    [SerializeField] private TMP_Text textNumTeléfono14CeldasEnCrearReserva;
    [SerializeField] private TMP_Text textDía14CeldasEnCrearReserva;
    [SerializeField] private TMP_Text textHora14CeldasEnCrearReserva;
    [SerializeField] private TMP_Text textNumCom14CeldasEnCrearReserva;
    [SerializeField] private TMP_Text textNumMesa14CeldasEnCrearReserva;
    [SerializeField] private TMP_Text textPlaceHolderInputFieldNombre14CeldasEnCrearReserva;
    [SerializeField] private TMP_Text textPlaceHolderInputFieldDNI14CeldasEnCrearReserva;
    [SerializeField] private TMP_Text textPlaceHolderInputFieldNumTeléfono14CeldasEnCrearReserva;
    [SerializeField] private TMP_Text textPlaceHolderInputFieldMesa14CeldasEnCrearReserva;
    [SerializeField] private TMP_Text textBtnCrearEnCrearReserva;
    [SerializeField] private TMP_Text textPlaceHolderFechaEnCrearReserva;

    // Textos en canvas Info mesas disponibles
    [SerializeField] private TMP_Text textEncabezadoInfoMesasDisponibles;
    [SerializeField] private TMP_Text textNumMesaInfoMesasDisponibles;
    [SerializeField] private TMP_Text textNumComensalesInfoMesasDisponibles;

    //Textos en canvas Pedidos
    [SerializeField] private TMP_Text imagenTextoPedidos;
    [SerializeField] private TMP_Text botonEntrarPedido;
    [SerializeField] private TMP_Text tituloPedidos;
    [SerializeField] private TMP_Text encabezadoArticulos;
    [SerializeField] private TMP_Text textoCat1;
    [SerializeField] private TMP_Text textoCat2;
    [SerializeField] private TMP_Text textoCat3;
    [SerializeField] private TMP_Text textoCat4;
    [SerializeField] private TMP_Text botonFactura;
    [SerializeField] private TMP_Text botonFinPedido;

    //Textos en canvas ListaPedidos
    [SerializeField] private TMP_Text tituloListaPedidos;
    [SerializeField] private TMP_Text buscarEstado;
    [SerializeField] private TMP_Text buscarMesa;

    //Texto en canvas factura
    [SerializeField] private TMP_Text tituloFactura;
    [SerializeField] private TMP_Text botonPago;
    [SerializeField] private TMP_Text botonImprimir;
    [SerializeField] private TMP_Text totalFactura;
    [SerializeField] private TMP_Text IDFactura;
    [SerializeField] private TMP_Text descripcionFactura;
    [SerializeField] private TMP_Text cantidadFactura;
    [SerializeField] private TMP_Text importeArticulo;
    [SerializeField] private TMP_Text nombreRestauranteFactura;


    // Start is called before the first frame update
    void Start()
    {
        PonerTextosEnIdiomaCorrecto();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PonerTextosEnIdiomaCorrecto()
    {
        switch (Usuario.Idioma)
        {
            case "Español":
                // Textos en canvas Gestionar Mesas
                textRestauranteGestMesas.text = "Restaurante:";
                textHoraAperturaGestMesas.text = "Hora Apertura:";
                textHoraCierreGestMesas.text = "Hora Cierre:";
                textBuscarReservaGestMesas.text = "Buscar Reserva";
                textCrearReservaGestMesas.text = "Crear Reserva";
                textHistorialReservasGestMesas.text = "Historial Reservas";
                textBtnVerRegistrosGestMesas.text = "Ver registros mesa de hoy";
                textBtnNoDisponibleGestMesas.text = "No disponible";
                textBtnDisponibleGestMesas.text = "Disponible";

                // Textos en canvas Registros Hoy Mesa
                textEncabezadoRegistrosMesa.text = "Registros Hoy Mesa";
                textIndicaciónEstructuraRegistrosMesa.text = "   Fecha     Hora   Nº Com.  Teléfono   Cliente";
                textTextoEnUsoRegistrosMesa.text = "En uso.";
                textTextoConfirmadaRegistrosMesa.text = "Confirmada.";
                textTerminadaCanceladaRegistrosMesa.text = "Terminada / Cancelada.";

                // Textos en canvas Historial Reservas
                textEncabezadoHistorialReservas.text = "Historial Reservas";
                textIndicaciónEstructuraHistorialReservas.text = "     Fecha      Hora    Nº Com.  Nº Mesa        DNI         Nº Teléfono   Nombre";
                textEnUsoHistorialReservas.text = "En uso.";
                textConfirmadaEncabezadoHistorialReservas.text = "Confirmada.";
                textTerminadaCanceladaHistorialReservas.text = "Terminada / Cancelada.";
                textPlaceHolderNombreHistorialReservas.text = "Nombre...";
                textPlaceHolderNumMesaHistorialReservas.text = "Nº Mesa...";
                textPlaceHolderFechaHistorialReservas.text = "Fecha ("+ DateTime.Now.ToString("dd/MM/yy")+")";
                textBtnBuscarHistorialReservas.text = "Buscar";
                textBtnActualizarHistorialReservas.text = "Actualizar";

                // Textos en canvas Buscar Reserva
                textEncabezadoBuscarReserva.text = "BUSCAR RESERVAS DE CLIENTES";
                textPlaceHolderInputFieldDniBuscarReserva.text = " Poner DNI cliente...";
                textBtnBuscarEnBuscarReserva.text = "Buscar";
                textIndicaciónEstructuraBuscarReserva.text = "     Fecha      Hora    Nº Com.  Nº Mesa       DNI         Nº Teléfono   Nombre";
                textEnUsoBuscarReserva.text = "En uso.";
                textConfirmadaBuscarReserva.text = "Confirmada.";
                textTerminadaCanceladaBuscarReserva.text = "Terminada / Cancelada.";
                textBtnCancelarBuscarReserva.text = "Cancelar";
                textBtnConfirmarBuscarReserva.text = "Confirmar";
                textImgConfirmarCancelarReservaBuscarReserva.text = "Confirmar cancelar reserva";

                // Textos en canvas Crear Reserva
                textEncabezadoCrearReserva.fontSize = 150;
                textEncabezadoCrearReserva.text = "Crear Reserva";
                text6CeldasEnCrearReserva.text = "       Fecha                Hora         Nº Comensales";
                textBtnBuscarMesasDisponiblesCrearReserva.text = "Buscar Mesas Disponibles";
                textMesasDisponiblesCrearReserva.text = "Mesas disponibles:";
                textNombre14CeldasEnCrearReserva.text = "Nombre";
                textDNI14CeldasEnCrearReserva.text = "DNI";
                textNumTeléfono14CeldasEnCrearReserva.fontSize = 48;
                textNumTeléfono14CeldasEnCrearReserva.text = "Nº\nTeléfono";
                textDía14CeldasEnCrearReserva.text = "Fecha";
                textHora14CeldasEnCrearReserva.text = "Hora";
                textNumCom14CeldasEnCrearReserva.fontSize = 48;
                textNumCom14CeldasEnCrearReserva.text = "Nº\nCom.";
                textNumMesa14CeldasEnCrearReserva.text = "Nº\nMesa";
                textPlaceHolderInputFieldNombre14CeldasEnCrearReserva.text = "Max 7 carac...";
                textPlaceHolderInputFieldDNI14CeldasEnCrearReserva.text = "Max 9 carac...";
                textPlaceHolderInputFieldNumTeléfono14CeldasEnCrearReserva.text = "Max 9 carac...";
                textPlaceHolderInputFieldMesa14CeldasEnCrearReserva.text = "Mesa...";
                textBtnCrearEnCrearReserva.text = "Crear";
                textPlaceHolderFechaEnCrearReserva.text = "Selecciona una fecha";

                // Textos en canvas Info mesas disponibles
                textEncabezadoInfoMesasDisponibles.text = "INFORMACIÓN MESAS DISPONIBLES";
                textNumMesaInfoMesasDisponibles.text = "Número de mesa";
                textNumComensalesInfoMesasDisponibles.text = "Número de comensales";

                //Textos en canvas Pedidos
                imagenTextoPedidos.text = "Lista de pedidos";
                botonEntrarPedido.text = "Realizar pedido";
                tituloPedidos.text="Pedidos para la mesa ";
                encabezadoArticulos.text="Artículos";
                textoCat1.text = "Entrantes";
                textoCat2.text = "Platos";
                textoCat3.text = "Bebidas";
                textoCat4.text = "Postres";
                botonFactura.text = "Pasar a factura";
                botonFinPedido.text = "Finalizar pedido";

                //Textos en canvas ListaPedidos
                tituloListaPedidos.text = "Lista de pedidos";
                buscarEstado.text = "Buscar por estado";
                buscarMesa.text = "Buscar por mesa";

                //Texto en canvas factura
                tituloFactura.text = "Factura";
                botonPago.text = "Realizar pago";
                botonImprimir.text = "Imprimir factura";
                totalFactura.text = "TOTAL: ";
                IDFactura.text = "ID";
                descripcionFactura.text = "Descripción";
                cantidadFactura.text = "Cantidad";
                importeArticulo.text = "Importe";
                nombreRestauranteFactura.text = "Restaurante:";


                break;

            case "English":
                // Textos en canvas Gestionar Mesas
                textRestauranteGestMesas.text = "Restaurant:";
                textHoraAperturaGestMesas.text = "Opening Time:";
                textHoraCierreGestMesas.text = "Closing Time:";
                textBuscarReservaGestMesas.text = "Search Reservation";
                textCrearReservaGestMesas.text = "Create Reservation";
                textHistorialReservasGestMesas.text = "Reservation History";
                textBtnVerRegistrosGestMesas.text = "View today's table records";
                textBtnNoDisponibleGestMesas.text = "Not available";
                textBtnDisponibleGestMesas.text = "Available";

                // Textos en canvas Registros Hoy Mesa
                textEncabezadoRegistrosMesa.text = "Records Today Table";
                textIndicaciónEstructuraRegistrosMesa.text = "    Date      Hour   Guests  Telephone  Customer";
                textTextoEnUsoRegistrosMesa.text = "In use.";
                textTextoConfirmadaRegistrosMesa.text = "Confirmed.";
                textTerminadaCanceladaRegistrosMesa.text = "Completed / Cancelled.";

                // Textos en canvas Historial Reservas
                textEncabezadoHistorialReservas.text = "Reservation History";
                textIndicaciónEstructuraHistorialReservas.text = "      Date       Hour     Guests   Table Nº.       DNI         Telephone     Name";
                textEnUsoHistorialReservas.text = "In use.";
                textConfirmadaEncabezadoHistorialReservas.text = "Confirmed.";
                textTerminadaCanceladaHistorialReservas.text = "Completed / Cancelled.";
                textPlaceHolderNombreHistorialReservas.text = "Name...";
                textPlaceHolderNumMesaHistorialReservas.text = "Table Nº...";
                textPlaceHolderFechaHistorialReservas.text = "Date (" + DateTime.Now.ToString("dd/MM/yy") + ")";
                textBtnBuscarHistorialReservas.text = "Search";
                textBtnActualizarHistorialReservas.text = "Update";

                // Textos en canvas Buscar Reserva
                textEncabezadoBuscarReserva.text = "SEARCH CUSTOMER RESERVATIONS";
                textPlaceHolderInputFieldDniBuscarReserva.text = " Enter customer DNI...";
                textBtnBuscarEnBuscarReserva.text = "Search";
                textIndicaciónEstructuraBuscarReserva.text = "      Date       Hour     Guests   Table Nº.       DNI        Telephone     Name";
                textEnUsoBuscarReserva.text = "In use.";
                textConfirmadaBuscarReserva.text = "Confirmed.";
                textTerminadaCanceladaBuscarReserva.text = "Completed / Cancelled.";
                textBtnCancelarBuscarReserva.text = "Cancel";
                textBtnConfirmarBuscarReserva.text = "Confirm";
                textImgConfirmarCancelarReservaBuscarReserva.text = "Confirm cancel reservation";

                // Textos en canvas Crear Reserva
                textEncabezadoCrearReserva.fontSize = 138;
                textEncabezadoCrearReserva.text = "Create Reservation"; 
                text6CeldasEnCrearReserva.text = "        Date                 Hour               Guests";
                textBtnBuscarMesasDisponiblesCrearReserva.text = "Search for Available Tables";
                textMesasDisponiblesCrearReserva.text = "Available tables:";
                textNombre14CeldasEnCrearReserva.text = "Name";
                textDNI14CeldasEnCrearReserva.text = "DNI";
                textNumTeléfono14CeldasEnCrearReserva.fontSize = 46;
                textNumTeléfono14CeldasEnCrearReserva.text = "Telephone";
                textDía14CeldasEnCrearReserva.text = "Date";
                textHora14CeldasEnCrearReserva.text = "Hour";
                textNumCom14CeldasEnCrearReserva.fontSize = 41;
                textNumCom14CeldasEnCrearReserva.text = "Guests";
                textNumMesa14CeldasEnCrearReserva.text = "Table Nº";
                textPlaceHolderInputFieldNombre14CeldasEnCrearReserva.text = "Max 7 chara...";
                textPlaceHolderInputFieldDNI14CeldasEnCrearReserva.text = "Max 9 chara...";
                textPlaceHolderInputFieldNumTeléfono14CeldasEnCrearReserva.text = "Max 9 chara...";
                textPlaceHolderInputFieldMesa14CeldasEnCrearReserva.text = "Table...";
                textBtnCrearEnCrearReserva.text = "Create";
                textPlaceHolderFechaEnCrearReserva.text = "Select a date";

                // Textos en canvas Info mesas disponibles
                textEncabezadoInfoMesasDisponibles.text = "INFORMATION ON AVAILABLE TABLES";
                textNumMesaInfoMesasDisponibles.text = "Table number";
                textNumComensalesInfoMesasDisponibles.text = "Number of guests";

                //Textos en canvas Pedidos
                imagenTextoPedidos.text = "List of orders";
                botonEntrarPedido.text = "Make an order";
                tituloPedidos.text = "Order for table ";
                encabezadoArticulos.text = "Items";
                textoCat1.text = "Appetizer";
                textoCat2.text = "Main dish";
                textoCat3.text = "Drinks";
                textoCat4.text = "Desserts";
                botonFactura.text = "Go to bill";
                botonFinPedido.text = "Finish order";


                //Textos en canvas ListaPedidos
                tituloListaPedidos.text = "Orders";
                buscarEstado.text = "Search by state";
                buscarMesa.text = "Search by table";

                //Texto en canvas factura
                tituloFactura.text = "Bill";
                botonPago.text = "Proceed to payment";
                botonImprimir.text = "Print bill";
                totalFactura.text = "TOTAL: ";
                IDFactura.text = "ID";
                descripcionFactura.text = "Description";
                cantidadFactura.text = "Amount";
                importeArticulo.text = "Price";
                nombreRestauranteFactura.text = "Restaurant:";
                break;
        }

    }
}
