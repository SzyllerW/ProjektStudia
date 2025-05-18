using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;

public class ResolutionSettings : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;
    private int SelectedResolution;
    private List<Resolution> SelectedResolutionList = new List<Resolution>();
    private bool wasExpanded = false;

    [SerializeField] private GameObject dropdown;
    [SerializeField] private Image img;
    [SerializeField] private AudioClip buttonSoundClip;

    void Start()
    {
        img.enabled = false;
        resolutions = Screen.resolutions.OrderByDescending(r => r.width * r.height).ToArray();
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        string newRes;

        foreach (Resolution res in resolutions)
        {
            newRes = res.width.ToString() + "x" + res.height.ToString();
            if (!options.Contains(newRes))
            {
                options.Add(newRes);
                SelectedResolutionList.Add(res);
            }
            if (res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = options.Count - 1;
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

    public void SetResolution()
    {
        SelectedResolution = resolutionDropdown.value;
        Screen.SetResolution(SelectedResolutionList[SelectedResolution].width, SelectedResolutionList[SelectedResolution].height, true);
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
