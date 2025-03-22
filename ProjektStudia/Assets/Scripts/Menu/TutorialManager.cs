using UnityEngine;
using UnityEngine.UI;

public class ChangeButtonsColor : MonoBehaviour
{
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private KeyCode[] keys;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color pressedColor = Color.black;

    void Update()
    {
        for (int i = 0; i < keys.Length; i++)
        {
            // Zmiana koloru na czarny po wciœniêciu klawisza
            if (Input.GetKeyDown(keys[i]))
            {
                ChangeColor(buttons[i], pressedColor);
            }

            // Powrót do normalnego koloru po puszczeniu klawisza
            if (Input.GetKeyUp(keys[i]))
            {
                ChangeColor(buttons[i], normalColor);
            }
        }
    }

    void ChangeColor(GameObject button, Color newColor)
    {
        Image imageComponent = button.GetComponent<Image>();
        if (imageComponent != null)
        {
            imageComponent.color = newColor;
        }
    }
}