using Assets.Scripts.Model;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    class FicheroController : MonoBehaviour
    {
        [SerializeField] private Cambio_Idioma_Escena_Main_Controller scriptCambioIdiomaEscenaMainController;

        private static string rutaArchivo;

        public static FicheroController InstanceFicheroController { get; private set; }

        private void Awake()
        {
            if (InstanceFicheroController == null)
            {
                InstanceFicheroController = this;
            }
        }



        public void GestionarFicheros()
        {
            CrearFichero("KeyAndIV");

            CrearFichero("UserInfo.txt");

            LeerFichUserInfo();

            scriptCambioIdiomaEscenaMainController.PonerTextosEnIdiomaCorrecto();
        }

        private static void CrearFichero(string cad)
        {
            // Ruta completa de la carpeta nueva dentro de persistentDataPath
            string rutaCarpeta = Path.Combine(Application.persistentDataPath, "Fichs");

            // Creo la carpeta si no existe
            if (!Directory.Exists(rutaCarpeta))
            {
                Directory.CreateDirectory(rutaCarpeta);
                Debug.Log("Carpeta creada: " + rutaCarpeta);
            }

            // Creo archivo si no existe dentro de esa carpeta
            rutaArchivo = Path.Combine(rutaCarpeta, cad);
            if (!File.Exists(rutaArchivo))
            {
                if (cad.Contains("KeyAndIV"))
                {
                    GestionarCrearFicheroKeyAndIV(rutaArchivo);                    
                }

                if (cad.Contains("UserInfo"))
                {
                    GestionarEncriptarFicheroUserInfo(Usuario.ID, Usuario.Restaurante_ID, "Español", Usuario.Token);
                }

                Debug.Log("Archivo creado en: " + rutaArchivo);
            }
            else
            {
                if (cad.Contains("KeyAndIV"))
                {
                    string contenido = File.ReadAllText(rutaArchivo);

                    string cad2 = LeerKeyAndIV(contenido);
                    Debug.Log(cad2);
                }

                Debug.Log("Archivo leído en: " + rutaArchivo);
            }
        }

        public static void GestionarEncriptarFicheroUserInfo(int id, int idRestaurante, string language, string token)
        {
            string contenido = "ID:"+id+"*\nLanguage:"+language+"*\nToken:"+token;

            // Creo el archivo y escribo el contenido encriptado
            File.WriteAllText(rutaArchivo, AESController.Encrypt(contenido));
        }

        private static void GestionarCrearFicheroKeyAndIV(string rutaArchivo)
        {
            AESController.CrearKeyAndIV();

            Debug.Log("Key: " + AESController.KeyBase64 + "; IV:" + AESController.IVBase64);

            // Texto a guardar
            string textoEnFich = "Key:" + AESController.KeyBase64 + "*" + "IV:" + AESController.IVBase64;

            PonerTextoEnBinarioEnFich(rutaArchivo, textoEnFich);
        }

        private static void PonerTextoEnBinarioEnFich(string rutaArchivo, string cad)
        {
            // Guardo el texto como binario (no cifrado)
            using (BinaryWriter writer = new BinaryWriter(File.Open(rutaArchivo, FileMode.Create)))
            {
                writer.Write(cad); 
            }

            // Pongo el archivo como sólo lectura 
            File.SetAttributes(rutaArchivo, FileAttributes.ReadOnly);
        }

        private static void LeerFichUserInfo()
        {
            // Ruta completa del archivo dentro de persistentDataPath
            string rutaFichero = Path.Combine(Application.persistentDataPath, "Fichs/UserInfo.txt");
            string contenido = File.ReadAllText(rutaFichero);
            string contenidoDecrypted = AESController.Decrypt(contenido);

            string[] partes = contenidoDecrypted.Split(new char[] { ':', '*' });
            Usuario.ID = int.Parse(partes[1]);
            Usuario.Idioma = partes[3];
            Usuario.Token = partes[5];


            Debug.Log("ID:" + Usuario.ID + "; Idioma:" + Usuario.Idioma+ "; Token: "+Usuario.Token);
            Debug.Log("Contenido Fich User info: " + contenidoDecrypted);
        }

        private static string LeerKeyAndIV(string contenido)
        {
            string[] partes = contenido.Split(new char[] { ':', '*' });
            AESController.KeyBase64 = partes[1];
            AESController.IVBase64 = partes[3];

            return "Key: " + AESController.KeyBase64 + "; IV: " + AESController.IVBase64;
        }
    }
}
