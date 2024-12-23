using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIInteractionDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Klikniêcie lewym przyciskiem myszy
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (var result in results)
            {
                Debug.Log("Klikniêto: " + result.gameObject.name);
            }
        }
    }
}