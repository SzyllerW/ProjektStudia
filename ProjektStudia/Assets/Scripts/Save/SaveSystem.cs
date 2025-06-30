using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string SavePath => Application.persistentDataPath + "/save.json";
    public static SaveData CurrentData { get; private set; }

    public static bool IsDevMode
    {
        get
        {
#if UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
    }

    public static void Load()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            CurrentData = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            CurrentData = new SaveData();
            CurrentData.unlockedCharacters.Add(0);     // <- odblokuj ¯abê
            CurrentData.unlockedLevels.Add("FrogTutorial");     // <- odblokuj poziom
            Save();
        }
    }

    public static void Save()
    {

        string json = JsonUtility.ToJson(CurrentData, true);
        File.WriteAllText(SavePath, json);
    }

    public static void UnlockCharacter(int index)
    {
        if (!CurrentData.unlockedCharacters.Contains(index))
        {
            CurrentData.unlockedCharacters.Add(index);
            Save();
        }
    }

    public static void UnlockLevel(string levelName)
    {
        if (!CurrentData.unlockedLevels.Contains(levelName))
        {
            CurrentData.unlockedLevels.Add(levelName);
            Save();
        }
    }

    public static List<int> GetUnlockedCharacters()
    {
        return CurrentData.unlockedCharacters;
    }

    public static void Reset()
    {
        if (File.Exists(SavePath)) File.Delete(SavePath);
        Load();
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Debug/Reset Save Data")]
    public static void Dev_Reset()
    {
        Reset();
        Debug.Log("Zresetowano zapis gry.");
    }
#endif
}
