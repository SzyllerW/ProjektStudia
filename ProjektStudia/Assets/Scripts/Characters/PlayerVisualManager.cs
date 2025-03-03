using UnityEngine;

public class PlayerVisualManager : MonoBehaviour
{
    [SerializeField] private GameObject sideView; 
    [SerializeField] private GameObject frontView; 
    [SerializeField] private PlayerMovement playerMovement;

    void Update()
    {
        if (Mathf.Abs(playerMovement.horizontal) > 0.1f)
        {
            SetSideView();
        }
        else
        {
            SetFrontView();
        }
    }

    private void SetSideView()
    {
        sideView.SetActive(true);
        frontView.SetActive(false);
    }

    private void SetFrontView()
    {
        sideView.SetActive(false);
        frontView.SetActive(true);
    }
}
