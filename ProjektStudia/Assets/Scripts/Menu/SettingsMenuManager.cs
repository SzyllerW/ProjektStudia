using UnityEngine;

public class SettingsMenuManager : MonoBehaviour
{
    public GameObject[] panels;
    private int currentIndex = 0;

    private void Start()
    {
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
        currentIndex = (currentIndex + 1) % panels.Length;
        ShowPanel(currentIndex);
    }

    public void ShowPreviousPanel()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = panels.Length - 1;
        ShowPanel(currentIndex);
    }
}


