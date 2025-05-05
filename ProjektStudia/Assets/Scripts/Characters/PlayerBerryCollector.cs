using UnityEngine;

public class PlayerBerryCollector : MonoBehaviour
{
    public bool HasCollectedBerry { get; set; } = false;

    public void ResetCollector()
    {
        HasCollectedBerry = false;
    }
}
