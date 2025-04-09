using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistorialReservasController : MonoBehaviour
{
    [SerializeField] private GameObject canvasHistorialReservas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DesactivarCanvasHistorialReservas()
    {
        canvasHistorialReservas.SetActive(false);
    }

}
