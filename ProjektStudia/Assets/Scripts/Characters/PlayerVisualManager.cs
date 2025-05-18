using UnityEngine;

public class PlayerVisualManager : MonoBehaviour
{
    [SerializeField] private GameObject sideView;
    [SerializeField] private GameObject frontView;
    [SerializeField] private GameObject climbView; 
    [SerializeField] private bool supportsClimbView = false;

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Animator animator;

    void Update()
    {
        if (animator.GetBool("Impact") && !animator.GetBool("IsKilledBySpikes"))
        {
            SetFrontView();
            return;
        }

        if (supportsClimbView && animator.GetBool("IsHoldingWall"))
        {
            SetClimbView();
        }
        else if (!animator.GetBool("IsKilledBySpikes"))
        {
            if (Mathf.Abs(playerMovement.horizontal) > 0.1f || animator.GetBool("Dig") || animator.GetBool("IsJumping"))
            {
                SetSideView();
            }
            else
            {
                SetFrontView();
            }
        }
    }

    private void SetSideView()
    {
        sideView.SetActive(true);
        frontView.SetActive(false);
        if (climbView != null) climbView.SetActive(false);
    }

    private void SetFrontView()
    {
        sideView.SetActive(false);
        frontView.SetActive(true);
        if (climbView != null) climbView.SetActive(false);
    }

    private void SetClimbView()
    {
        Debug.Log("[Visual] ClimbView aktywowane");
        sideView.SetActive(false);
        frontView.SetActive(false);
        if (climbView != null) climbView.SetActive(true);
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

        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.UnlockCharacterSelection();
        }
    }
}

