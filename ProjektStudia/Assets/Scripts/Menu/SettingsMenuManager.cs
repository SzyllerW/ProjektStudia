using UnityEngine;

public class SettingsMenuManager : MonoBehaviour
{
    public GameObject[] panels;
    private int currentIndex = 0;
    [SerializeField] private AudioClip buttonSoundClip;

    private void Start()
    {
        if (panels.Length == 0) return;
        ShowPanel(currentIndex);
    }

    private void ShowPanel(int index)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == index);
        }
    }

    public void ShowNextPanel()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        currentIndex = (currentIndex + 1) % panels.Length;
        ShowPanel(currentIndex);
    }

    public void ShowPreviousPanel()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = panels.Length - 1;
        ShowPanel(currentIndex);
    }
}


