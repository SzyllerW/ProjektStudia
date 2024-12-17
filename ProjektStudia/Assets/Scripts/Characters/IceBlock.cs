using UnityEngine;

public class IceBlock : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isIceBlock = false;

    [SerializeField] private GameObject iceBlockPrefab; // Prefab bloku lodu
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

        // Wy³¹cz fizykê postaci
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        if (iceBlockPrefab != null)
        {
            // Pozycja bazuj¹ca na œrodku BoxCollider postaci i childa kostki lodu
            Vector3 spawnPosition = transform.position;

            BoxCollider2D playerCollider = GetComponent<BoxCollider2D>();
            Transform iceChild = iceBlockPrefab.transform.GetChild(0); // Pobranie childa kostki lodu

            if (playerCollider != null && iceChild != null)
            {
                BoxCollider2D iceChildCollider = iceChild.GetComponent<BoxCollider2D>();
                if (iceChildCollider != null)
                {
                    float offsetY = (playerCollider.bounds.extents.y - iceChildCollider.bounds.extents.y);
                    spawnPosition.y += offsetY;
                }
            }

            // Tworzenie kostki lodu z poprawn¹ pozycj¹
            GameObject iceBlock = Instantiate(iceBlockPrefab, spawnPosition, Quaternion.identity);
            Debug.Log("Blok lodu utworzony na pozycji: " + spawnPosition);
        }
        else
        {
            Debug.LogError("Prefab bloku lodu nie jest przypisany!");
        }

        // Wy³¹cz postaæ
        gameObject.SetActive(false);
    }
}