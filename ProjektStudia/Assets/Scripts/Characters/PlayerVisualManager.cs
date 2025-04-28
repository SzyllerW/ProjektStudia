using UnityEngine;

public class PlayerVisualManager : MonoBehaviour
{
    [SerializeField] private GameObject sideView; 
    [SerializeField] private GameObject frontView; 
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Animator animator;

    void Update()
    {
        if (animator.GetBool("Impact") && !animator.GetBool("IsKilledBySpikes"))
        {
            SetFrontView();
            return;
        }

        if (Mathf.Abs(playerMovement.horizontal) > 0.1f || animator.GetBool("Dig") || animator.GetBool("IsJumping"))
        {
            SetSideView();
        }
        else if (!animator.GetBool("IsKilledBySpikes"))
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

    public void PlayerTouchedSpikes()
    {
        animator.SetBool("Dig", false);
        animator.SetBool("Impact", false);
        SetSideView();
        animator.SetBool("IsKilledBySpikes", true);
    }

    public void KilledBySpikes()
    {
        gameObject.SetActive(false);

        if (animator != null)
        {
            animator.SetBool("IsKilledBySpikes", false);
            animator.Play("Movement", 0, 0f);
            animator.Update(0);
        }

        // Prze³¹cz na nastêpn¹ postaæ
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.UnlockCharacterSelection();

        }
    }
}
