using UnityEngine;

public class IceBlock : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isIceBlock = false;

    [SerializeField] private AudioClip iceBlockEnterSoundClip;
    [SerializeField] private GameObject iceBlockPrefab;       // Prefab bloku lodu
    [SerializeField] private GameObject penguinPrefab;        // Prefab pingwina
    [SerializeField] private Transform penguinRespawnPoint;   // Punkt respawnu nowego pingwina

    private GameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManager>();

        // Automatyczne wyszukanie punktu respawnu jeœli nie przypisany
        if (penguinRespawnPoint == null)
        {
            GameObject found = GameObject.Find("RespawnPoint");
            if (found != null)
            {
                penguinRespawnPoint = found.transform;
            }
            else
            {
                Debug.LogWarning("Nie znaleziono RespawnPoint w scenie!");
            }
        }
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
            Vector3 spawnPosition = transform.position;

            BoxCollider2D playerCollider = GetComponent<BoxCollider2D>();
            Transform iceChild = iceBlockPrefab.transform.GetChild(0);

            if (playerCollider != null && iceChild != null)
            {
                BoxCollider2D iceChildCollider = iceChild.GetComponent<BoxCollider2D>();
                if (iceChildCollider != null)
                {
                    float offsetY = (playerCollider.bounds.extents.y - iceChildCollider.bounds.extents.y);
                    spawnPosition.y += offsetY;
                }
            }

            Instantiate(iceBlockPrefab, spawnPosition, Quaternion.identity);
            Debug.Log("Blok lodu utworzony na pozycji: " + spawnPosition);

            SoundFXManager.instance.PlaySoundFXClip(iceBlockEnterSoundClip, transform, 1f);
        }
        else
        {
            Debug.LogError("Prefab bloku lodu nie jest przypisany!");
        }

        // Stwórz nowego pingwina w punkcie respawnu
        if (penguinPrefab != null && penguinRespawnPoint != null)
        {
            Instantiate(penguinPrefab, penguinRespawnPoint.position, Quaternion.identity);
            Debug.Log("Nowy pingwin zrespawnowany w punkcie: " + penguinRespawnPoint.position);
        }
        else
        {
            Debug.LogWarning("Brakuje prefab-u pingwina lub punktu respawnu!");
        }

        // Wy³¹cz star¹ postaæ
        gameObject.SetActive(false);
    }
}
