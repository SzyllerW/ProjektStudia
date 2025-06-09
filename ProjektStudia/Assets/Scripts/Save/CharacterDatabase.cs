using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Character Database")]
public class CharacterDatabase : ScriptableObject
{
    public List<GameObject> allCharacters = new List<GameObject>();
    public List<string> characterDescriptions = new List<string>();
}
