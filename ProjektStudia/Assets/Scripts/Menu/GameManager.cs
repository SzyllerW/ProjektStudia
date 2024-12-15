using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Transform respawnPoint;
    public Image[] iconSlots;
    public List<GameObject> characterPrefabs;

    private List<GameObject> activeCharacters = new List<GameObject>();
    private int currentCharacterIndex = 0;

    [SerializeField] private float switchDelay = 0.5f;

    private void Start()
    {
        LoadSelectedCharacters();
        ActivateCharacter(0);
        UpdateIcons();
    }

    private void LoadSelectedCharacters()
    {
        string selectedCharacters = PlayerPrefs.GetString("SelectedCharacters", "");
        string[] characterIndexes = selectedCharacters.Split(',');

        foreach (string index in characterIndexes)
        {
            int characterIndex = int.Parse(index);
            GameObject character = Instantiate(characterPrefabs[characterIndex], respawnPoint.position, Quaternion.identity);
            character.SetActive(false);
            activeCharacters.Add(character);
        }
    }

    private void ActivateCharacter(int index)
    {
        if (index >= activeCharacters.Count)
        {
            Debug.Log("No more characters available! End of game.");
            return;
        }

        currentCharacterIndex = index;
        GameObject newCharacter = activeCharacters[currentCharacterIndex];
        ResetCharacter(newCharacter);
        newCharacter.transform.position = respawnPoint.position;
        newCharacter.SetActive(true);
    }

    public void SwitchToNextCharacter()
    {
        if (activeCharacters.Count == 0) return;

        StartCoroutine(SwitchCharacterWithDelay());
    }

    private IEnumerator SwitchCharacterWithDelay()
    {
        GameObject currentCharacter = activeCharacters[currentCharacterIndex];
        currentCharacter.SetActive(false);

        yield return new WaitForSeconds(switchDelay);

        ActivateCharacter((currentCharacterIndex + 1) % activeCharacters.Count);
        UpdateIcons();
    }

    private void UpdateIcons()
    {
        for (int i = 0; i < iconSlots.Length; i++)
        {
            if (i < activeCharacters.Count)
            {
                iconSlots[i].color = (i == currentCharacterIndex) ? Color.white : Color.gray;
            }
            else
            {
                iconSlots[i].color = Color.clear;
            }
        }
    }

    private void ResetCharacter(GameObject character)
    {
        Rigidbody2D rb = character.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero; 
        }

        Animator animator = character.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetBool("IsJumping", false);
        }

    }
}
