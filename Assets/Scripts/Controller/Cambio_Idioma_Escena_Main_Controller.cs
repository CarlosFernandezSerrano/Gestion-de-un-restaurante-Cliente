using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using TMPro;
using UnityEngine;

public class Cambio_Idioma_Escena_Main_Controller : MonoBehaviour
{
    // Textos en canvas Log In
    [SerializeField] private TMP_Text textLogIn;
    [SerializeField] private TMP_Text textNombreLogIn;
    [SerializeField] private TMP_Text textPasswordLogIn;
    [SerializeField] private TMP_Text textInputFieldNombreLogIn;
    [SerializeField] private TMP_Text textInputFieldPasswordLogIn;
    [SerializeField] private TMP_Text textButtonInformación;
    [SerializeField] private TMP_Text textButtonAccederLogIn;
    [SerializeField] private TMP_Text textButtonSalirAppEnLogIn;


    // Textos en canvas Info
    [SerializeField] private TMP_Text textInfo;
    [SerializeField] private TMP_Text textArribaInfo;
    [SerializeField] private TMP_Text textNumTeléfono;
    [SerializeField] private TMP_Text textCorreoElectrónico;
    [SerializeField] private TMP_Text textAbajoInfo;
    [SerializeField] private TMP_Text textBtnVolver;
    [SerializeField] private TMP_Text textSalirDeLaApp;

    // Texto en canvas Inicio App
    [SerializeField] private TMP_Text textCrearRestauranteInicio;
    [SerializeField] private TMP_Text textPerfilInicio;
    [SerializeField] private TMP_Text textCerrarSesiónInicio;
    [SerializeField] private TMP_Text textIdiomaInicio;
    [SerializeField] private TMP_Text textRestauranteInicio;
    [SerializeField] private TMP_Text textNombreInicio;
    [SerializeField] private TMP_Text textRolUsuarioInicio;
    [SerializeField] private TMP_Text textEditarRestauranteInicio;
    [SerializeField] private TMP_Text textGestionarMesasInicio;
    [SerializeField] private TMP_Text textGestionarTrabajadoresInicio;
    [SerializeField] private TMP_Text textSalirDeLaAppInicio;

    // Texto en canvas crear restaurante
    [SerializeField] private TMP_Text textNombreCrearRest;
    [SerializeField] private TMP_Text textPlaceHolderNombreCrearRest;
    [SerializeField] private TMP_Text textHorarioCrearRest;
    [SerializeField] private TMP_Text textAperturaCrearRest;
    [SerializeField] private TMP_Text textCierreCrearRest;
    [SerializeField] private TMP_Text textConfirmarOpcionesCrearRest;
    [SerializeField] private TMP_Text textNotaCrearRest;
    [SerializeField] private TMP_Text textInfoServicioCrearRest;
    [SerializeField] private TMP_Text textBtnVerVídeoCrearRest;


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
                // Textos en canvas Log In
                textLogIn.text = "Iniciar Sesión";
                textNombreLogIn.text = "Nombre:";
                textPasswordLogIn.text = "Contraseña:";
                textInputFieldNombreLogIn.text = " Mínimo 3 caracteres...";
                textInputFieldPasswordLogIn.text = " Mínimo 6 caracteres...";
                textButtonInformación.text = "Información";
                textButtonAccederLogIn.text = "Acceder";
                textButtonSalirAppEnLogIn.text = "Salir de la app";

                // Textos en canvas Info
                textInfo.text = "INFORMACIÓN";
                textArribaInfo.text = "Para obtener el servicio, póngase en contacto con nosotros a través de los siguientes medios.";
                textNumTeléfono.text = "Número de teléfono";
                textCorreoElectrónico.text = "Correo electrónico";
                textAbajoInfo.text = "Si usted es empleado de un restaurante y no tiene una cuenta, hable con su gerente.";
                textBtnVolver.text = "Volver para iniciar sesión";
                textSalirDeLaApp.text = "Salir de la app";

                // Texto en canvas Inicio App
                textCrearRestauranteInicio.text = "Crear Restaurante";
                textPerfilInicio.text = "Perfil";
                textCerrarSesiónInicio.text = "Cerrar sesión";
                textIdiomaInicio.text = "Idioma";
                textRestauranteInicio.text = "Restaurante";
                textNombreInicio.text = "Nombre";
                if (Usuario.Rol_ID > 0)
                {
                    if (Usuario.Rol_ID.Equals(1))
                    {
                        textRolUsuarioInicio.text = "Empleado";
                    }
                    else
                    {
                        textRolUsuarioInicio.text = "Gerente";
                    }
                }
                textEditarRestauranteInicio.text = "Editar \r\nRestaurante";
                textGestionarMesasInicio.text = "Gestionar \r\nMesas";
                textGestionarTrabajadoresInicio.text = "Gestionar\r\nTrabajadores";
                textSalirDeLaAppInicio.text = "Salir de la app";

                // Texto en canvas crear restaurante
                textNombreCrearRest.text = "Nombre";
                textPlaceHolderNombreCrearRest.text = "Max. 17 caracteres...";
                textHorarioCrearRest.text = "Horario";
                textAperturaCrearRest.text = "Apertura";
                textCierreCrearRest.text = "Cierre";
                textConfirmarOpcionesCrearRest.text = "Confirmar opciones";
                textNotaCrearRest.text = "Nota:";
                textInfoServicioCrearRest.text = "Este servicio ofrece una excelente gestión de su restaurante.";
                textBtnVerVídeoCrearRest.text = "Ver vídeo";

                break;

            case "English":
                // Textos en canvas Log In
                textLogIn.text = "Login";
                textNombreLogIn.text = "Name:";
                textPasswordLogIn.text = "Password:";
                textInputFieldNombreLogIn.text = " Minimum 3 characters...";
                textInputFieldPasswordLogIn.text = " Minimum 6 characters...";
                textButtonInformación.text = "Information";
                textButtonAccederLogIn.text = "Access";
                textButtonSalirAppEnLogIn.text = "Exit the app";

                // Textos en canvas Info
                textInfo.text = "INFORMATION";
                textArribaInfo.text = "To obtain the service, please contact us through the following means.";
                textNumTeléfono.text = "Phone number";
                textCorreoElectrónico.text = "Email";
                textAbajoInfo.text = "If you are a restaurant employee and do not have a check, speak to your manager.";
                textBtnVolver.text = "Return to log in";
                textSalirDeLaApp.text = "Exit the app";


                // Texto en canvas Inicio App
                textCrearRestauranteInicio.text = "Create Restaurant";
                textPerfilInicio.text = "Profile";
                textCerrarSesiónInicio.text = "Log out";
                textIdiomaInicio.text = "Language";
                textRestauranteInicio.text = "Restaurant";
                textNombreInicio.text = "Name";
                if (Usuario.Rol_ID > 0)
                {
                    if (Usuario.Rol_ID.Equals(1))
                    {
                        textRolUsuarioInicio.text = "Employee";
                    }
                    else
                    {
                        textRolUsuarioInicio.text = "Manager";
                    }
                }
                textEditarRestauranteInicio.text = "Edit \r\nRestaurant";
                textGestionarMesasInicio.text = "Manage \r\nTables";
                textGestionarTrabajadoresInicio.text = "Manage\r\nWorkers";
                textSalirDeLaAppInicio.text = "Exit the app";

                // Texto en canvas crear restaurante
                textNombreCrearRest.text = "Name";
                textPlaceHolderNombreCrearRest.text = "Max. 17 characters...";
                textHorarioCrearRest.text = "Schedule";
                textAperturaCrearRest.text = "Opening";
                textCierreCrearRest.text = "Closing";
                textConfirmarOpcionesCrearRest.text = "Confirm options";
                textNotaCrearRest.text = "Note:";
                textInfoServicioCrearRest.text = "This service offers excellent management of your restaurant.";
                textBtnVerVídeoCrearRest.text = "Watch video";

                break;
        }

    }

    public void CambiarAIdiomaSpanish()
    {
        Usuario.Idioma = "Español";
        FicheroController.GestionarEncriptarFicheroUserInfo(Usuario.ID, Usuario.Restaurante_ID, Usuario.Idioma, Usuario.Token); // Guardo el idioma cambiado en el fichero "UserInfo"
        PonerTextosEnIdiomaCorrecto();
    }

    public void CambiarAIdiomaEnglish()
    {
        Usuario.Idioma = "English";
        FicheroController.GestionarEncriptarFicheroUserInfo(Usuario.ID, Usuario.Restaurante_ID, Usuario.Idioma, Usuario.Token); // Guardo el idioma cambiado en el fichero "UserInfo"
        PonerTextosEnIdiomaCorrecto();
    }
}
