using System.Collections;
using UnityEngine;

public class EnterPortal : MonoBehaviour
{
    [Header("Scene to load")]
    [SerializeField] private string destinationScene;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PortalsManager.instance.DisplayPopUp(this, destinationScene, other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PortalsManager.instance.DestroyPopUp();
        }
    }
}
