using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    class FicheroController
    {
        private static string rutaArchivo;


        public static void GestionarFicheros()
        {
            CrearFichero("KeyAndIV");

            CrearFichero("UserInfo.txt");

            LeerFichUserInfo();
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
                    GestionarEncriptarFicheroUserInfo(0, "Spanish");
                }

                Debug.Log("Archivo creado en: " + rutaArchivo);
            }
            else
            {
                if (cad.Contains("KeyAndIV"))
                {
                    string contenido = File.ReadAllText(rutaArchivo);

                    string cad2 = AESController.LeerKeyAndIV(contenido);
                    Debug.Log(cad2);
                }

                Debug.Log("Archivo leído en: " + rutaArchivo);
            }
        }

        private static void GestionarEncriptarFicheroUserInfo(int id, string language)
        {
            string contenido = "ID:"+id+"*\nLanguage:"+language;

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

            Debug.Log("Contenido Fich User info: " + AESController.Decrypt(contenido));
        }
    }
}
