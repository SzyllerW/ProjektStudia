using System.Collections;
using UnityEngine;

public class MoleAbility : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Rigidbody2D rb;

    [SerializeField] private GameObject dirtMoundPrefab;
    [SerializeField] private float moundHeight = 2f;

    private bool isMole = false;
    private bool isDigging = false;
    private bool canControl = true;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isMole && playerMovement.IsGrounded() && !isDigging)
        {
            StartDigging();
        }
    }

    private void StartDigging()
    {
        isDigging = true;
        canControl = false;

        rb.velocity = Vector2.zero;  
        rb.isKinematic = true; 

        StartCoroutine(WaitAndCreateMound());
    }

    private IEnumerator WaitAndCreateMound()
    {
        yield return new WaitForSeconds(1f);

        Instantiate(dirtMoundPrefab, transform.position, Quaternion.identity);

        rb.isKinematic = false;  
        canControl = false;
        isMole = true;

        gameObject.SetActive(false); 
    }

    public void ResetMoleAbility()
    {
        gameObject.SetActive(true);
        canControl = true;
        isMole = false;
    }
}