using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlock : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isIceBlock = false; 

    [SerializeField] private GameObject iceBlockPrefab;  

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isIceBlock && Input.GetKeyDown(KeyCode.E))
        {
            ActivateIceBlock(); 
        }
    }

    private void ActivateIceBlock()
    {
        isIceBlock = true;  

        rb.velocity = Vector2.zero;  
        rb.isKinematic = true; 

        if (iceBlockPrefab != null)
        {
            Instantiate(iceBlockPrefab, transform.position, Quaternion.identity);
        }

        gameObject.SetActive(false); 
    }

    public void DeactivateIceBlock()
    {
        rb.isKinematic = false; 
        gameObject.SetActive(true);  

        isIceBlock = false;  
    }
}
