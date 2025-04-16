using Assets.Scripts.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GestionarTrabajadoresController : MonoBehaviour
{



    private void Awake()
    {
        SceneManager.LoadSceneAsync("General Controller", LoadSceneMode.Additive);
    }

    // Start is called before the first frame update
    void Start()
    {
        TrabajadorController.ComprobandoDatosTrabajador = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void IrALaEscenaMain()
    {
        SceneManager.LoadScene("Main");
    }

}
