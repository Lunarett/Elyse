using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _powerSlider;
    [SerializeField] private TMP_Text _matchText;
    [SerializeField] private TMP_Text _timerText;

    private PhotonView _photonView;
    
    // Position for the power slider text display
    private Rect _powerTextRect = new Rect(Screen.width - 110, 10, 100, 20);
    private GUIStyle _powerTextStyle = new GUIStyle();

    private float cpower;

    public static HUD Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _photonView = GetComponent<PhotonView>();

        // Set up the GUIStyle for the power text
        _powerTextStyle.alignment = TextAnchor.UpperRight;
        _powerTextStyle.fontSize = 30;
        _powerTextStyle.normal.textColor = Color.white;
    }

    public void SetHeath(float current, float max)
    {
        if (!_photonView.IsMine) return;
        float percentage = current / max;
        _healthSlider.value = percentage;
    }

    public void SetPower(float current, float max)
    {
        if (!_photonView.IsMine) return;

        //Debug.Log($"HUD SetPower called with current: {current}, max: {max}");
        float percentage = current / max;
        _powerSlider.value = percentage;
    }


    public void SetMatchText(string text)
    {
        _matchText.text = text;
    }

    public void SetTimerText(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60F);
        int intSeconds = Mathf.FloorToInt(seconds - minutes * 60);
        _timerText.text = string.Format("{0:00}:{1:00}", minutes, intSeconds);
    }

    void OnGUI()
    {
        // Display the power slider value in the top right corner
        if (_powerSlider != null)
        {
            string powerText = $"Power: {cpower}%";
            GUI.Label(_powerTextRect, powerText, _powerTextStyle);
        }
    }
}