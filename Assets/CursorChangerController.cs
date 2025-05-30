using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorChangerController : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture;
    public Vector2 hotspot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPointerOverUIButton())
        {
            //Cursor.SetCursor(cursorTexture, hotspot, cursorMode); 
            Cursor.SetCursor(cursorTexture, new Vector2(25, 6), cursorMode);
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, cursorMode);
        }
    }

    private bool IsPointerOverUIButton()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            GameObject hoveredObject = GetUIObjectUnderPointer();
            if (hoveredObject != null)
            {
                // Buscar el componente Button en el objeto o en su padre
                Button button = hoveredObject.GetComponentInParent<Button>();
                if (button != null && button.interactable)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private GameObject GetUIObjectUnderPointer()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        if (raycastResults.Count > 0)
        {
            return raycastResults[0].gameObject;
        }

        return null;
    }
}
