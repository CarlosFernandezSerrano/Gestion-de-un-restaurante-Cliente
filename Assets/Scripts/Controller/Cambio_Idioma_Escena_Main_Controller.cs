using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private TMP_Text textButtonRegistrarseLogIn;
    [SerializeField] private TMP_Text textButtonAccederLogIn;

    // Textos en canvas Registrarse
    [SerializeField] private TMP_Text textRegistrarse;
    [SerializeField] private TMP_Text textNombreRegistrarse;
    [SerializeField] private TMP_Text textPasswordRegistrarse;
    [SerializeField] private TMP_Text textRepeatPasswordRegistrarse;
    [SerializeField] private TMP_Text textInputFieldNombreRegistrarse;
    [SerializeField] private TMP_Text textInputFieldPasswordRegistrarse;
    [SerializeField] private TMP_Text textInputFieldRepeatPasswordRegistrarse;
    [SerializeField] private TMP_Text textIniciarSesionRegistrarse;
    [SerializeField] private TMP_Text textConfirmarRegistrarse;

    // Texto en canvas Inicio App
    [SerializeField] private TMP_Text textComprarInicio;
    [SerializeField] private TMP_Text textPerfilInicio;
    [SerializeField] private TMP_Text textCerrarSesiónInicio;
    [SerializeField] private TMP_Text textIdiomaInicio;
    [SerializeField] private TMP_Text textRestauranteInicio;
    [SerializeField] private TMP_Text textNombreInicio;


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
                textLogIn.text = "Inicia Sesión";
                textNombreLogIn.text = "Nombre";
                textPasswordLogIn.text = "Contraseña";
                textInputFieldNombreLogIn.text = "Mínimo 3 caracteres...";
                textInputFieldPasswordLogIn.text = "Mínimo 6 caracteres...";
                textButtonRegistrarseLogIn.text = "Registrarse";
                textButtonAccederLogIn.text = "Acceder";
                
                // Textos en canvas Registrarse
                textRegistrarse.text = "Registrarse";
                textNombreRegistrarse.text = "Nombre";
                textPasswordRegistrarse.text = "Contraseña";
                textRepeatPasswordRegistrarse.text = "Confirmar Contraseña";
                textInputFieldNombreRegistrarse.text = "Mínimo 3 caracteres...";
                textInputFieldPasswordRegistrarse.text = "Mínimo 3 caracteres...";
                textInputFieldRepeatPasswordRegistrarse.text = "Repetir contraseña...";
                textIniciarSesionRegistrarse.text = "Iniciar Sesión";
                textConfirmarRegistrarse.text = "Confirmar";

                // Texto en canvas Inicio App
                textComprarInicio.text = "Comprar";
                textPerfilInicio.text = "Perfil";
                textCerrarSesiónInicio.text = "Cerrar sesión";
                textIdiomaInicio.text = "Idioma";
                textRestauranteInicio.text = "Restaurante";
                textNombreInicio.text = "Nombre";
                break;

            case "English":
                // Textos en canvas Log In
                textLogIn.text = "Login";
                textNombreLogIn.text = "Name";
                textPasswordLogIn.text = "Password";
                textInputFieldNombreLogIn.text = "Minimum 3 characters...";
                textInputFieldPasswordLogIn.text = "Minimum 6 characters...";
                textButtonRegistrarseLogIn.text = "Register";
                textButtonAccederLogIn.text = "Access";

                // Textos en canvas Registrarse
                textRegistrarse.text = "Register";
                textNombreRegistrarse.text = "Name";
                textPasswordRegistrarse.text = "Password";
                textRepeatPasswordRegistrarse.text = "Confirm password";
                textInputFieldNombreRegistrarse.text = "Minimum 3 characters...";
                textInputFieldPasswordRegistrarse.text = "Minimum 6 characters...";
                textInputFieldRepeatPasswordRegistrarse.text = "Repeat password...";
                textIniciarSesionRegistrarse.text = "Login";
                textConfirmarRegistrarse.text = "Confirm";

                // Texto en canvas Inicio App
                textComprarInicio.text = "Buy";
                textPerfilInicio.text = "Profile";
                textCerrarSesiónInicio.text = "Log out";
                textIdiomaInicio.text = "Language";
                textRestauranteInicio.text = "Restaurant";
                textNombreInicio.text = "Name";
                break;
        }

    }

    public void CambiarAIdiomaSpanish()
    {
        Usuario.Idioma = "Español";
        FicheroController.GestionarEncriptarFicheroUserInfo(Usuario.ID, Usuario.Idioma); // Guardo el idioma cambiado en el fichero "UserInfo"
        PonerTextosEnIdiomaCorrecto();
    }

    public void CambiarAIdiomaEnglish()
    {
        Usuario.Idioma = "English";
        FicheroController.GestionarEncriptarFicheroUserInfo(Usuario.ID, Usuario.Idioma); // Guardo el idioma cambiado en el fichero "UserInfo"
        PonerTextosEnIdiomaCorrecto();
    }
}
