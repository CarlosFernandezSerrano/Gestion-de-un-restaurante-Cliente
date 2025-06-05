using Assets.Scripts.Model;
using Newtonsoft.Json;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    class TrabajadorController : MonoBehaviour
    {
        public static bool ComprobandoDatosTrabajador { get; set; } = false;


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

        private void Update()
        {
            ComprobarDatosTrabajador();
            
        }

        private void ComprobarDatosTrabajador()
        {
            int id = Usuario.ID;
            // Si el usuario ya tiene asignado un ID, puede realizar esta función
            if (id > 0)
            {
                if (!ComprobandoDatosTrabajador)
                {
                    Debug.Log("2");
                    ComprobandoDatosTrabajador = true;
                    ObtenerDatosTrabajadorPorIdAsync(id);
                    StartCoroutine(EsperoUnTiempoAntesDeVolverAComprobarLosDatosDelTrabajador());
                }
                
            }
        }

        private IEnumerator EsperoUnTiempoAntesDeVolverAComprobarLosDatosDelTrabajador()
        {
            yield return new WaitForSeconds(2f); // Cada 2 segundos actualizo
            ComprobandoDatosTrabajador = false;
        }

        public async void ObtenerDatosTrabajadorPorNombreAsync(Trabajador t)
        {
            // Es más seguro usar POST aunque solo necesito obtener información. Esto se debe a la protección de datos sensibles.
            // No quiero que cualquier persona pueda ver el id de cualquier trabajador usando la url.
            string cad = await instanceMétodosAPIController.PostDataAsync("trabajador/obtenerTrabajadorPorNombre", t);
            
            // Deserializo la respuesta
            Trabajador trabajador = JsonConvert.DeserializeObject<Trabajador>(cad);
            
            Usuario.ID = trabajador.Id;
            FicheroController.GestionarEncriptarFicheroUserInfo(Usuario.ID, Usuario.Restaurante_ID, Usuario.Idioma, Usuario.Token);
            Usuario.Rol_ID = trabajador.Rol_ID;
            // Si el valor es 0, es que no está en ningún restaurante.
            Usuario.Restaurante_ID = trabajador.Restaurante_ID;

            instanceMainController.QuitarYPonerBotonesSegúnElTrabajador();

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

            Usuario.Nombre = trabajador.Nombre;
            Usuario.Rol_ID = trabajador.Rol_ID;
            // Si el valor es 0, es que no está en ningún restaurante.
            Usuario.Restaurante_ID =  trabajador.Restaurante_ID;

            if (instanceMainController != null)
            {
                instanceMainController.QuitarYPonerBotonesSegúnElTrabajador();

                PonerDatosEnPerfilTrabajador(instanceMainController.getTextPerfilUserNombre(), instanceMainController.getTextPerfilUserRol(), instanceMainController.getTextPerfilUserRestaurante());
            }

            GeneralController.NoHayConexion = false;
            Debug.Log("ID Usuario: " + Usuario.ID + ", Nombre Usuario: " + Usuario.Nombre + ", Rol_ID Usuario: " + Usuario.Rol_ID + ", Restaurante_ID Usuario: " + Usuario.Restaurante_ID);
        }

        public void PonerDatosEnPerfilTrabajador(TMPro.TMP_Text textUserNombre, TMPro.TMP_Text textUserRol, TMPro.TMP_Text textUserRestaurante)
        {
            textUserNombre.text = Usuario.Nombre;
            switch (Usuario.Rol_ID)
            {
                case 1:
                    if (Usuario.Idioma.CompareTo("Español") == 0)
                    {
                        textUserRol.text = "Empleado";
                    }
                    else
                    {
                        textUserRol.text = "Employee";
                    }
                    break;
                case 2:
                    if (Usuario.Idioma.CompareTo("Español") == 0)
                    {
                        textUserRol.text = "Gerente";
                    }
                    else
                    {
                        textUserRol.text = "Manager";
                    }
                    break;
            }
            if (Usuario.Restaurante_ID.Equals(0))
            {
                textUserRestaurante.text = "";
            }
            else
            {
                ObtenerNombreRestauranteTrabajador(textUserRestaurante);
            }
        }

        private async void ObtenerNombreRestauranteTrabajador(TMPro.TMP_Text textUserRestaurante)
        {
            if (instanceMétodosAPIController != null)
            {
                string cad = await instanceMétodosAPIController.GetDataAsync("restaurante/getRestaurantePorId/" + Usuario.Restaurante_ID);

                // Deserializo la respuesta
                Restaurante restaurante = JsonConvert.DeserializeObject<Restaurante>(cad);

                switch (restaurante.Nombre.Length)
                {
                    case <= 14:
                        textUserRestaurante.fontSize = 48;
                        break;
                    case <= 16:
                        textUserRestaurante.fontSize = 41;
                        break;
                    case 17:
                        textUserRestaurante.fontSize = 39;
                        break;
                }
                textUserRestaurante.text = restaurante.Nombre;
            }            
        }

        public async Task ActualizarDatosTrabajadorPorIdAsync(Trabajador trabajador)
        {
            string cad = await instanceMétodosAPIController.PutDataAsync("trabajador/actualizarTrabajador/", trabajador);

            // Deserializo la respuesta
            Resultado resultado = JsonConvert.DeserializeObject<Resultado>(cad);

            if (resultado.Result.Equals(1)) {
                Debug.Log("Actualización exitosa de datos del trabajador");
            }

            instanceMainController.QuitarYPonerBotonesSegúnElTrabajador();

            PonerDatosEnPerfilTrabajador(instanceMainController.getTextPerfilUserNombre(), instanceMainController.getTextPerfilUserRol(), instanceMainController.getTextPerfilUserRestaurante());
        }
    }
}
