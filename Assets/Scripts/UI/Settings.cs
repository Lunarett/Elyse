using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private TMP_Dropdown _screenModeDropdown;
    [SerializeField] private TMP_Dropdown _qualityDropdown;
    [SerializeField] private TMP_Dropdown _vsyncDropdown;
    [SerializeField] private TMP_Dropdown _antiAliasingDropdown;
    [SerializeField] private TMP_Dropdown _shadowQualityDropdown;

    private void Start()
    {
        PopulateResolutionDropdown();
        PopulateScreenModeDropdown();
        PopulateQualityDropdown();
        PopulateVSyncDropdown();
        PopulateAntiAliasingDropdown();
        PopulateShadowQualityDropdown();

        SetCurrentSettings();

        _resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        _screenModeDropdown.onValueChanged.AddListener(OnScreenModeChanged);
        _qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        _vsyncDropdown.onValueChanged.AddListener(OnVSyncChanged);
        _antiAliasingDropdown.onValueChanged.AddListener(OnAntiAliasingChanged);
        _shadowQualityDropdown.onValueChanged.AddListener(OnShadowQualityChanged);
    }

    private void SetCurrentSettings()
    {
        // Set current resolution
        string currentResolution = Screen.currentResolution.width + "x" + Screen.currentResolution.height;
        _resolutionDropdown.value = _resolutionDropdown.options.FindIndex(option => option.text == currentResolution);

        // Set current screen mode
        _screenModeDropdown.value = (int)Screen.fullScreenMode;

        // Set current quality
        _qualityDropdown.value = QualitySettings.GetQualityLevel();

        // Set current VSync setting
        _vsyncDropdown.value = QualitySettings.vSyncCount;

        // Set current anti-aliasing
        int aaIndex = 0;
        switch (QualitySettings.antiAliasing)
        {
            case 0: aaIndex = 0; break;
            case 2: aaIndex = 1; break;
            case 4: aaIndex = 2; break;
            case 8: aaIndex = 3; break;
        }
        _antiAliasingDropdown.value = aaIndex;

        // Set current shadow quality
        int shadowIndex = 0;
        switch (QualitySettings.shadows)
        {
            case ShadowQuality.Disable: shadowIndex = 0; break;
            case ShadowQuality.HardOnly: shadowIndex = 1; break;
            case ShadowQuality.All: shadowIndex = 2; break;
        }
        _shadowQualityDropdown.value = shadowIndex;
    }
    
    private void PopulateResolutionDropdown()
    {
        _resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        foreach (Resolution resolution in Screen.resolutions)
        {
            string option = resolution.width + "x" + resolution.height;
            options.Add(option);
        }
        _resolutionDropdown.AddOptions(options);
    }

    private void PopulateScreenModeDropdown()
    {
        _screenModeDropdown.ClearOptions();
        List<string> options = new List<string> { "Fullscreen", "Windowed", "Borderless" };
        _screenModeDropdown.AddOptions(options);
    }

    private void PopulateQualityDropdown()
    {
        _qualityDropdown.ClearOptions();
        List<string> options = new List<string>(QualitySettings.names);
        _qualityDropdown.AddOptions(options);
    }

    private void PopulateVSyncDropdown()
    {
        _vsyncDropdown.ClearOptions();
        List<string> options = new List<string> { "Off", "On" };
        _vsyncDropdown.AddOptions(options);
    }

    private void PopulateAntiAliasingDropdown()
    {
        _antiAliasingDropdown.ClearOptions();
        List<string> options = new List<string> { "Off", "2x", "4x", "8x" };
        _antiAliasingDropdown.AddOptions(options);
    }

    private void PopulateShadowQualityDropdown()
    {
        _shadowQualityDropdown.ClearOptions();
        List<string> options = new List<string> { "Off", "Hard Shadows", "Soft Shadows" };
        _shadowQualityDropdown.AddOptions(options);
    }

    private void OnResolutionChanged(int index)
    {
        // ... existing code ...
    }

    private void OnScreenModeChanged(int index)
    {
        switch (index)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
        }
    }

    private void OnQualityChanged(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    private void OnVSyncChanged(int index)
    {
        QualitySettings.vSyncCount = index;
    }

    private void OnAntiAliasingChanged(int index)
    {
        QualitySettings.antiAliasing = index == 0 ? 0 : (int)Mathf.Pow(2, index);
    }

    private void OnShadowQualityChanged(int index)
    {
        switch (index)
        {
            case 0:
                QualitySettings.shadows = ShadowQuality.Disable;
                break;
            case 1:
                QualitySettings.shadows = ShadowQuality.HardOnly;
                break;
            case 2:
                QualitySettings.shadows = ShadowQuality.All;
                break;
        }
    }
}
