using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level Config")]
public class LevelConfig : ScriptableObject
{
    public string levelID;
    public bool unlocksCharacter;
    public int characterIndexToUnlock = -1;
    public bool unlocksNextLevel;
    public string levelIdToUnlock;
    public int characterLimit = 3;

    public void ApplyUnlocks()
    {
        if (characterIndexToUnlock >= 0)
            SaveSystem.UnlockCharacter(characterIndexToUnlock);

        if (!string.IsNullOrEmpty(levelIdToUnlock))
            SaveSystem.UnlockLevel(levelIdToUnlock);
    }

    public int GetCharacterLimit() => characterLimit;
    public string GetLevelID() => levelID;
}



