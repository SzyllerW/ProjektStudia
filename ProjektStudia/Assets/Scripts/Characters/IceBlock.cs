using UnityEngine;

public class IceBlock : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isIceBlock = false;

    [SerializeField] private GameObject iceBlockPrefab;
    private GameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (!isIceBlock && Input.GetKeyDown(KeyCode.E))
        {
            ActivateIceBlock();
            gameManager.SwitchToNextCharacter();
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
}