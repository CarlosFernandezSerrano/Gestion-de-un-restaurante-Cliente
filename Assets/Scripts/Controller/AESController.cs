using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Controller
{
    class AESController
    {
        public static string KeyBase64;
        public static string IVBase64;

        //public static byte[] key = Convert.FromBase64String(KeyBase64);
        //public static byte[] iv = Convert.FromBase64String(IVBase64);

        public static void CrearKeyAndIV()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey(); // Genera una clave aleatoria
                aes.GenerateIV();  // Genera un IV aleatorio

                // Mostramos la clave y el IV en Base64 (para guardar o enviar)
                string claveBase64 = Convert.ToBase64String(aes.Key);
                string ivBase64 = Convert.ToBase64String(aes.IV);

                Console.WriteLine("Clave AES (Base64): " + claveBase64);
                Console.WriteLine("IV AES (Base64): " + ivBase64);

                KeyBase64 = claveBase64;
                IVBase64 = ivBase64;
            }
        }

        public static string LeerKeyAndIV(string contenido)
        {
            string[] partes = contenido.Split(new char[] { ':', '*' });
            AESController.KeyBase64 = partes[1];
            AESController.IVBase64 = partes[3];

            return "Key: " + AESController.KeyBase64 + "; IV: " + AESController.IVBase64;
        }

        public static string Encrypt(string simpleText)
        {
             byte[] key = Convert.FromBase64String(KeyBase64);
             byte[] iv = Convert.FromBase64String(IVBase64);


            byte[] cipheredtextInBytes;
            using (Aes aes = Aes.Create())
            {
                ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(simpleText);
                        }

                        cipheredtextInBytes = memoryStream.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(cipheredtextInBytes);
        }

        public static string Decrypt(string cipherTextBase64)
        {
            byte[] key = Convert.FromBase64String(KeyBase64);
            byte[] iv = Convert.FromBase64String(IVBase64);

            using Aes aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor();
            byte[] cipherBytes = Convert.FromBase64String(cipherTextBase64);

            using MemoryStream msDecrypt = new(cipherBytes);
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);

            return srDecrypt.ReadToEnd();
        }
    }
}
