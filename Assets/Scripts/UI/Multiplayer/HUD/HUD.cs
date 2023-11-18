using System;
using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("Panel Manager")]
    [SerializeField] private int _pauseMenuPanelIndex = 1;
    [SerializeField] private int _heighScoreIndex = 2;
    
    [Header("Match Text")]
    [SerializeField] private TMP_Text _matchText;
    [SerializeField] private TMP_Text _timerText;
    
    [Header("Progress Bars")]
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _powerSlider;
    
    [Header("Damage Effect")]
    [SerializeField] private AnimationCurve _damageEffectCurve;
    [SerializeField] private float _damageEffectDuration = 0.2f;
    [SerializeField] private Image _damageScreen;

    [Header("Game Feed")]
    [SerializeField] private GameFeedHandler _gameFeedHandler;
    [SerializeField] private float _feedLifetime = 5.0f;
    [SerializeField] private int _maxFeedCount = 5;
    
    private PhotonView _photonView;
    private PanelManager _panelManager;
    
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
        _gameFeedHandler = GetComponentInChildren<GameFeedHandler>();
        _panelManager = GetComponent<PanelManager>();
    }

    public void SetHeath(float current, float max)
    {
        float percentage = current / max;
        _healthSlider.value = percentage;
    }

    public void SetPower(float current, float max)
    {
        float percentage = current / max;
        _powerSlider.value = percentage;
    }

    public void PlayDamageEffect()
    {
        StopCoroutine(nameof(DamageEffectCoroutine));
        StartCoroutine(DamageEffectCoroutine());
    }

    public void SetDamageScreenAlpha(float alpha)
    {
        Color color = new Color(_damageScreen.color.r, _damageScreen.color.g, _damageScreen.color.b, alpha);
        _damageScreen.color = color;
    }

    private IEnumerator DamageEffectCoroutine()
    {
        float timer = 0f;
        while (timer < _damageEffectDuration)
        {
            timer += Time.deltaTime;
            float alpha = _damageEffectCurve.Evaluate(timer / _damageEffectDuration);
            _damageScreen.color = new Color(_damageScreen.color.r, _damageScreen.color.g, _damageScreen.color.b, alpha);
            yield return null;
        }

        _damageScreen.color = new Color(_damageScreen.color.r, _damageScreen.color.g, _damageScreen.color.b, 0);
    }

    public void SetMatchText(string text)
    {
        _matchText.text = text;
    }

    public void SetTimerText(float seconds, bool isVisible = true)
    {
        if (!isVisible)
        {
            _timerText.text = "";
            return;
        }
        int minutes = Mathf.FloorToInt(seconds / 60F);
        int intSeconds = Mathf.FloorToInt(seconds - minutes * 60);
        _timerText.text = string.Format("{0:00}:{1:00}", minutes, intSeconds);
    }

    public void BroadcastGameFeed(string causerName, string affectedName)
    {
        if(_gameFeedHandler == null) Debug.LogError("GameFeed is null");
        _gameFeedHandler.AddMessage(causerName, affectedName, _maxFeedCount, _feedLifetime);
    }

    public void DisplayPauseMenu(bool show = true)
    {
        Debug.Log("Pause");
        _panelManager.ShowPanel(show ? _pauseMenuPanelIndex : 0);
    }
}