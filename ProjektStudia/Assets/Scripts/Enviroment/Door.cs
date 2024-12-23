using UnityEngine;


public class Door : MonoBehaviour
{
    private bool isOpen = false; 

    public void ToggleDoor()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            transform.position += new Vector3(0, 5, 0);
        }
        else
        {
            transform.position -= new Vector3(0, 5, 0);
        }

        Debug.Log("Drzwi s¹ teraz " + (isOpen ? "otwarte" : "zamkniête"));
    }
}

