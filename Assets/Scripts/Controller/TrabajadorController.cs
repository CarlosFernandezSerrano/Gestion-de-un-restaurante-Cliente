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

        public static TrabajadorController instanceTrabajadorController { get; private set; }

        private void Awake()
        {
            if (instanceTrabajadorController == null)
            {
                instanceTrabajadorController = this;
            }
        }

        public void Start()
        {
            instanceMétodosAPIController = MétodosAPIController.instanceMétodosAPIController;
        }

        public IEnumerator ObtenerDatosTrabajador(Trabajador t)
        {
            // Es más seguro usar POST aunque solo necesito obtener información. Esto se debe a la protección de datos sensibles.
            // No quiero que cualquier persona pueda ver el id de cualquier trabajador usando la url.
            yield return StartCoroutine(instanceMétodosAPIController.PostData("trabajador/obtenerID", t));
            
            // Deserializo la respuesta
            Trabajador trabajador = JsonConvert.DeserializeObject<Trabajador>(instanceMétodosAPIController.respuestaPOST);
            PlayerPrefs.SetInt("ID Usuario", trabajador.Id);
            PlayerPrefs.SetInt("Rol_ID Usuario", trabajador.Rol_ID);
            // Si el trabajador está en algún restaurante. Si el valor es 0, es que no está en ningún restaurante.
            PlayerPrefs.SetInt("Restaurante_ID Usuario", trabajador.Restaurante_ID);
        }

    }
}
