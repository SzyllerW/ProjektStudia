using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class ResolutionSettings : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    [SerializeField] private GameObject dropdown;
    [SerializeField] private Image img;
    [SerializeField] private AudioClip buttonSoundClip;
    private Resolution[] resolutions;
    private bool wasExpanded = false;

    void Start()
    {
        img.enabled = false;
        resolutions = Screen.resolutions.OrderByDescending(r => r.width * r.height).ToArray();
        resolutionDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    void Update()
    {
        bool isExpanded = dropdown.transform.childCount > 3;

        if (isExpanded && !wasExpanded)
        {
            ShowImage();
            PlayButtonSound();
        }
        else if (!isExpanded && wasExpanded)
        {
            img.enabled = false;
            PlayButtonSound();
        }

        wasExpanded = isExpanded;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    private void ShowImage()
    {
        img.enabled = true;
    }

    private void PlayButtonSound()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
    }
}
