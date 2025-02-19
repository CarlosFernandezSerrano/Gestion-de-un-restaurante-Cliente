using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    [SerializeField] private GameObject canvasLogInUsuario;


    // Start is called before the first frame update
    void Start()
    {
        int usuarioRegistrado = PlayerPrefs.GetInt("UsuarioRegistrado", 0); // 1 es sí, 0 es no
        //Si el usuario no se ha registrado, le aparece el canvas de iniciar sesión
        if (usuarioRegistrado.Equals(0))
        {
            canvasLogInUsuario.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
