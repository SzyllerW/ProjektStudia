using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PortalsManager : MonoBehaviour
{
    public static PortalsManager instance;

    [Header("Interaction with portal")]
    [SerializeField] private GameObject portalCanvas;
    [SerializeField] private GameObject popUpPrefab;
    [SerializeField] private KeyCode interactionKey;
    [SerializeField] private float offsetY;
    [SerializeField] private Material buttonShaderMaterial;

    private GameObject popUpInstance;
    private float fillAmount = 0f;
    private bool isAnimationStarted = false;
    private GameObject activePortal;
    private CameraFollow cameraFollow;
    private string targetScene;
    private GameObject player;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        cameraFollow = FindObjectOfType<CameraFollow>();
    }

    public void DisplayPopUp(EnterPortal portal, string sceneName, GameObject playerObject)
    {
        activePortal = portal.gameObject;
        targetScene = sceneName;
        player = playerObject;

        popUpInstance = Instantiate(popUpPrefab, portalCanvas.transform);
        RectTransform rectTransform = popUpInstance.GetComponent<RectTransform>();
        rectTransform.position = activePortal.transform.position + new Vector3(0, offsetY, 0);

        cameraFollow.focusOnPortal(true);
    }

    public void DestroyPopUp()
    {
        if (popUpInstance != null)
        {
            Destroy(popUpInstance);
            popUpInstance = null;
            activePortal = null;
        }

        cameraFollow.focusOnPortal(false);
        isAnimationStarted = false;
    }

    private void Update()
    {
        if (popUpInstance != null && player != null)
        {
            if (Input.GetKey(interactionKey))
            {
                fillAmount += Time.deltaTime / 1f;
                fillAmount = Mathf.Clamp(fillAmount, 0f, 1f);
                buttonShaderMaterial.SetFloat("_FillAmount", fillAmount);

                if (fillAmount >= 1f && !isAnimationStarted)
                {
                    isAnimationStarted = true;
                    StartCoroutine(PlayerAnimation());
                    Destroy(popUpInstance);
                }
            }
            else
            {
                fillAmount = 0f;
                buttonShaderMaterial.SetFloat("_FillAmount", fillAmount);
                isAnimationStarted = false;
            }
        }
    }

    private IEnumerator PlayerAnimation()
    {
        float duration = 0.5f;
        float elapsedTime = 0;
        Vector3 startPosition = player.transform.position;
        Vector3 targetPosition = activePortal.transform.position;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
        }

        while (elapsedTime < duration)
        {
            float progress = elapsedTime / duration;
            player.transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        player.transform.position = targetPosition;
        StartCoroutine(ScalePlayer());
    }

    private IEnumerator ScalePlayer()
    {
        float scaleDuration = 0.5f;
        float scaleElapsedTime = 0;
        Vector3 startScale = player.transform.localScale;
        Vector3 targetScale = Vector3.zero;

        while (scaleElapsedTime < scaleDuration)
        {
            float progress = scaleElapsedTime / scaleDuration;
            player.transform.localScale = Vector3.Lerp(startScale, targetScale, progress);
            scaleElapsedTime += Time.deltaTime;
            yield return null;
        }

        player.transform.localScale = targetScale;
        yield return new WaitForSeconds(1f);

        LoadTargetScene();
    }

    private void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(targetScene))
        {
            SceneManager.LoadScene(targetScene);
        }
    }
}
