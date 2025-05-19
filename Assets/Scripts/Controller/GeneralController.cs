using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GeneralController : MonoBehaviour
{
    [SerializeField] private GameObject imgNoConexión;

    public static bool NoHayConexion { get; set; } = true;

    public static GeneralController InstanceGeneralController { get; private set; }

    void Awake()
    {
        if (InstanceGeneralController == null)
        {
            InstanceGeneralController = this;
        }      
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator HacerAparecerLentamente(GameObject gameObject, float duration)
    {
        float elapsedTime = 0f; //duration = 1f; //Lo normal es que esté a 2f, pero aquí lo dejo a 1f 

        Image image = gameObject.GetComponent<Image>();

        //Muestro partes del juego (como el teclado) lentamente
        while (elapsedTime < duration)
        {
            if (!GeneralController.NoHayConexion)
            {
                break;
            }

            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration); // Interpola de 0 a 1 (totalmente invisible a visible)

            // Aplica el valor alfa interpolado a cada imagen
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            gameObject.SetActive(true);

            yield return null; // Espera un frame antes de continuar el bucle
        }
    }

    public IEnumerator HacerDesaparecerLentamente(GameObject gameObject, float duration)
    {
        float elapsedTime = 0f; //duration = 1f; //Lo normal es que esté a 2f, pero aquí lo dejo a 1f 

        Image image = gameObject.GetComponent<Image>();

        //Muestro partes del juego (como el teclado) lentamente
        while (elapsedTime < duration)
        {
            if (!GeneralController.NoHayConexion)
            {
                break;
            }

            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration); // Interpola de 1 a 0 (totalmente visible a invisible)

            // Aplica el valor alfa interpolado a cada imagen
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);

            yield return null; // Espera un frame antes de continuar el bucle
        }
    }

    public GameObject getImgNoConexión()
    {
        return imgNoConexión;
    }

    
}
