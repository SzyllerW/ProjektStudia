using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityManager : MonoBehaviour
{
    [SerializeField] private GameObject sideView; 
    [SerializeField] private GameObject frontView; 
    [SerializeField] private Animator animator;

    private void Update()
    {
        if (animator.GetBool("IsFacingForward"))
        {
            sideView.SetActive(false); 
            frontView.SetActive(true); 
        }
        else
        {
            sideView.SetActive(true);  
            frontView.SetActive(false); 
        }
    }
}
