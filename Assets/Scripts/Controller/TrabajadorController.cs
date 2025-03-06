using Assets.Scripts.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    class TrabajadorController : MonoBehaviour
    {
        MétodosAPIController instanceMétodosAPIController;
        MainController instanceMainController;

        public static TrabajadorController InstanceTrabajadorController { get; private set; }

        private void Awake()
        {
            if (InstanceTrabajadorController == null)
            {
                InstanceTrabajadorController = this;
            }
        }

        public void Start()
        {
            instanceMétodosAPIController = MétodosAPIController.InstanceMétodosAPIController;
            instanceMainController = MainController.InstanceMainController;
        }

        public async void ObtenerDatosTrabajadorPorNombreAsync(Trabajador t)
        {
            // Es más seguro usar POST aunque solo necesito obtener información. Esto se debe a la protección de datos sensibles.
            // No quiero que cualquier persona pueda ver el id de cualquier trabajador usando la url.
            string cad = await instanceMétodosAPIController.PostDataAsync("trabajador/obtenerTrabajadorPorNombre", t);
            
            // Deserializo la respuesta
            Trabajador trabajador = JsonConvert.DeserializeObject<Trabajador>(cad);
            PlayerPrefs.SetInt("ID Usuario", trabajador.Id);
            PlayerPrefs.SetInt("Rol_ID Usuario", trabajador.Rol_ID);
            // Si el valor es 0, es que no está en ningún restaurante.
            PlayerPrefs.SetInt("Restaurante_ID Usuario", trabajador.Restaurante_ID);
            PlayerPrefs.Save();

            PonerDatosEnPerfilTrabajador(instanceMainController.getTextPerfilUserNombre(), instanceMainController.getTextPerfilUserRol(), instanceMainController.getTextPerfilUserRestaurante());
        }

        //Importante:
        //Quizás este método tenga que ponerlo en un Update() para que el user esté comprobando todo el rato si ha recibido cambios nuevos.
        public async void ObtenerDatosTrabajadorPorIdAsync(int id)
        {
            // Es más seguro usar POST aunque solo necesito obtener información. Esto se debe a la protección de datos sensibles.
            // No quiero que cualquier persona pueda ver el id de cualquier trabajador usando la url.
            string cad = await instanceMétodosAPIController.GetDataAsync("trabajador/obtenerTrabajadorPorId/"+id);

            // Deserializo la respuesta
            Trabajador trabajador = JsonConvert.DeserializeObject<Trabajador>(cad);
            
            PlayerPrefs.SetInt("Rol_ID Usuario", trabajador.Rol_ID);
            // Si el valor es 0, es que no está en ningún restaurante.
            PlayerPrefs.SetInt("Restaurante_ID Usuario", trabajador.Restaurante_ID);
            PlayerPrefs.Save();

            PonerDatosEnPerfilTrabajador(instanceMainController.getTextPerfilUserNombre(), instanceMainController.getTextPerfilUserRol(), instanceMainController.getTextPerfilUserRestaurante());
            Debug.Log("ID Usuario: " + PlayerPrefs.GetInt("ID Usuario") + ", Nombre Usuario: " + PlayerPrefs.GetString("Nombre Usuario") + ", Rol_ID Usuario: " + PlayerPrefs.GetInt("Rol_ID Usuario") + ", Restaurante_ID Usuario: " + PlayerPrefs.GetInt("Restaurante_ID Usuario"));
        }

        public void PonerDatosEnPerfilTrabajador(TMPro.TMP_Text textUserNombre, TMPro.TMP_Text textUserRol, TMPro.TMP_Text textUserRestaurante)
        {
            Debug.Log("Pasa por aquí");
            textUserNombre.text = PlayerPrefs.GetString("Nombre Usuario");
            switch (PlayerPrefs.GetInt("Rol_ID Usuario"))
            {
                case 1:
                    textUserRol.text = "Empleado";
                    break;
                case 2:
                    textUserRol.text = "Gerente";
                    break;
            }
            if (PlayerPrefs.GetInt("Restaurante_ID Usuario").Equals(0))
            {
                textUserRestaurante.text = "";
            }
            else
            {
                //textUserRestaurante.text = ObtenerNombreRestauranteTrabajador();
            }

        }
    }
}
