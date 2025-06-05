using Assets.Scripts.Model;
using TMPro;
using UnityEngine;

public class Cambio_Idioma_Escena_Editar_Restaurante_Controller : MonoBehaviour
{
    // Textos en canvas Editar Restaurante
    [SerializeField] private TMP_Text textNombreRestEdit;
    [SerializeField] private TMP_Text textHoraAperturaEdit;
    [SerializeField] private TMP_Text textHoraCierreEdit;
    [SerializeField] private TMP_Text textTiempoPermitidoEdit;
    [SerializeField] private TMP_Text textBtnSinTiempoEdit;
    [SerializeField] private TMP_Text textGuardarEdit;
    [SerializeField] private TMP_Text textAñadirMesaEdit;
    [SerializeField] private TMP_Text textPapeleraEdit;
    [SerializeField] private TMP_Text textCuidadoEdit;
    [SerializeField] private TMP_Text textAdvertencia1Edit;
    [SerializeField] private TMP_Text textAdvertencia2Edit;
    [SerializeField] private TMP_Text textBtnCancelarEdit;
    [SerializeField] private TMP_Text textBtnEliminarEdit;
    [SerializeField] private TMP_Text textHayCambiosSinGuardarEdit; 
    [SerializeField] private TMP_Text textNoGuardarEdit;
    [SerializeField] private TMP_Text textGuardarEnHayCambiosEdit;
    [SerializeField] private TMP_Text textCantDeComensalesEdit;
    [SerializeField] private TMP_Text textBtnCancelarCantComensalesEdit;
    [SerializeField] private TMP_Text textBtnConfirmarCantComensalesEdit;


    // Textos en canvas Info Manejo Mesas
    [SerializeField] private TMP_Text textEncabezadoInfoManejo;
    [SerializeField] private TMP_Text textRatón1InfoManejo;
    [SerializeField] private TMP_Text textRatón2InfoManejo;
    [SerializeField] private TMP_Text textRatón3InfoManejo;



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
                // Textos en canvas Editar Restaurante
                textNombreRestEdit.text = "Nombre Restaurante:";
                textHoraAperturaEdit.text = "Hora Apertura:";
                textHoraCierreEdit.text = "Hora Cierre:";
                textTiempoPermitidoEdit.text = "Tiempo\r\npermitido \r\npara \r\ncomer";
                textBtnSinTiempoEdit.text = "Sin tiempo límite";
                textGuardarEdit.text = "Guardar";
                textAñadirMesaEdit.text = "Añadir Mesa";
                textPapeleraEdit.text = "Papelera";
                textCuidadoEdit.text = "¡CUIDADO!";
                textAdvertencia1Edit.fontSize = 56;
                textAdvertencia1Edit.text = "La mesa tiene una reserva.";
                textAdvertencia2Edit.fontSize = 47;
                textAdvertencia2Edit.text = "Si elimina la mesa, se eliminarán sus reservas.";
                textBtnCancelarEdit.text = "Cancelar";
                textBtnEliminarEdit.text = "Sí, eliminar";
                textHayCambiosSinGuardarEdit.text = "Hay cambios sin guardar";
                textNoGuardarEdit.text = "No guardar";
                textGuardarEnHayCambiosEdit.text = "Guardar";
                textCantDeComensalesEdit.text = "Cantidad de comensales";
                textBtnCancelarCantComensalesEdit.text = "Cancelar";
                textBtnConfirmarCantComensalesEdit.text = "Confirmar";

                // Textos en canvas Info Manejo Mesas
                textEncabezadoInfoManejo.text = "INFORMACIÓN MANEJO DE MESAS";
                textRatón1InfoManejo.text = "Para mover una mesa a cualquier lugar del mapa, o desactivar una mesa marcada.";
                textRatón2InfoManejo.text = "Para agrandar o reducir el tamaño de una mesa.";
                textRatón3InfoManejo.text = "Para seleccionar una mesa y editar su cantidad de comensales o, activar la papelera en caso de querer eliminarla.";

                break;

            case "English":
                // Textos en canvas Editar Restaurante
                textNombreRestEdit.text = "Restaurant Name:";
                textHoraAperturaEdit.text = "Opening Time:";
                textHoraCierreEdit.text = "Closing Time:";
                textTiempoPermitidoEdit.text = "Time\r\nallowed \r\nfor \r\neating";
                textBtnSinTiempoEdit.text = "No time limit";
                textGuardarEdit.text = "Save";
                textAñadirMesaEdit.text = "Add Table";
                textPapeleraEdit.text = "Bin";
                textCuidadoEdit.text = "CAREFUL!";
                textAdvertencia1Edit.fontSize = 48;
                textAdvertencia1Edit.text = "The table has a reservation.";
                textAdvertencia2Edit.fontSize = 44;
                textAdvertencia2Edit.text = "If you delete the table, your reservations will be deleted.";
                textBtnCancelarEdit.text = "Cancel";
                textBtnEliminarEdit.text = "Yes, delete";
                textHayCambiosSinGuardarEdit.text = "There are unsaved changes";
                textNoGuardarEdit.text = "Don´t save";
                textGuardarEnHayCambiosEdit.text = "Save";
                textCantDeComensalesEdit.text = "Number of guests";
                textBtnCancelarCantComensalesEdit.text = "Cancel";
                textBtnConfirmarCantComensalesEdit.text = "Confirm";

                // Textos en canvas Info Manejo Mesas
                textEncabezadoInfoManejo.text = "TABLE HANDLING INFORMATION";
                textRatón1InfoManejo.text = "To move a table anywhere on the map, or deactivate a marked table.";
                textRatón2InfoManejo.text = "To enlarge or reduce the size of a table.";
                textRatón3InfoManejo.text = "To select a table and edit its number of guests, or activate the bin if you want to delete it.";

                break;
        }

    }
}
