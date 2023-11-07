using System;
using UnityEngine;

public class Power : MonoBehaviour
{
    [SerializeField] private float _maxPower = 100.0f;
    [SerializeField] private float _rechargeRate = 5.0f;
    [SerializeField] private float _rechargeDelay = 1.0f;
    
    private float _currentPower = 0;
    private float _lastConsumeTime = 0;
    private PlayerInputManager _inputManager;
    private HUD _hud;

    public float CurrentPower => _currentPower;
    public float MaxPower => _maxPower;

    private void Awake()
    {
        _currentPower = _maxPower;
        _inputManager = GetComponent<PlayerInputManager>();
        _hud = HUD.Instance;
    }

    private void Update()
    {
        if (_inputManager.GetFlyInputHeld() || !(Time.time >= _lastConsumeTime + _rechargeDelay)) return;
        _currentPower = Mathf.Clamp(_currentPower += (_rechargeRate * Time.deltaTime), 0, _maxPower);
        _hud.SetPower(_currentPower, _maxPower);
    }

    public bool ConsumePower(float amount)
    {
        if (!(_currentPower >= amount)) return false;
        _currentPower -= amount;
        _hud.SetPower(_currentPower, _maxPower);
        _lastConsumeTime = Time.time;
        return true;
    }
}