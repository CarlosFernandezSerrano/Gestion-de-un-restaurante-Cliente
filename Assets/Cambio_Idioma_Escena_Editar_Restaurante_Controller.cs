using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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

                // Textos en canvas Info Manejo Mesas
                textEncabezadoInfoManejo.text = "INFORMACIÓN MANEJO DE MESAS";
                textRatón1InfoManejo.text = "Para mover una mesa a cualquier lugar del mapa.";
                textRatón2InfoManejo.text = "Para agrandar o reducir el tamaño de una mesa.";
                textRatón3InfoManejo.text = "Para seleccionar una mesa y activar la papelera en el caso de querer eliminarla.";

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

                // Textos en canvas Info Manejo Mesas
                textEncabezadoInfoManejo.text = "TABLE HANDLING INFORMATION";
                textRatón1InfoManejo.text = "To move a table to any location on the map.";
                textRatón2InfoManejo.text = "To enlarge or reduce the size of a table.";
                textRatón3InfoManejo.text = "To select a table and activate the bin in case you want to delete it.";

                break;
        }

    }
}
