using UnityEngine;
using UnityEngine.UI;

public class CharacterIconButton : MonoBehaviour
{
    private int characterIndex;
    private Button button;
    private GameManager gameManager;

    private void Awake()
    {
        button = GetComponent<Button>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void Setup(int index)
    {
        characterIndex = index;
        if (button == null)
            button = GetComponent<Button>();

        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        if (gameManager != null)
        {
            gameManager.SelectCharacter(characterIndex);
        }
    }
}
