using System;
using UnityEngine;
using Photon.Pun;

public abstract class BaseHealth : MonoBehaviourPunCallbacks
{
    [SerializeField]
    protected float _maxHealth = 100f;

    protected float _currentHealth;

    public float MaxHealth => _maxHealth;
    public float CurrentHealth => _currentHealth;
    
    public event Action<float, float> OnHealthChanged;

    private GUIStyle _guiStyle = new GUIStyle();

    protected virtual void Awake()
    {
        _currentHealth = _maxHealth;  // Initialize current health to max health at the start
        _guiStyle.fontSize = 30;
        _guiStyle.fontStyle = FontStyle.Bold;
    }

    public void TakeDamage(float damage, WeaponInfo damageCauserInfo)
    {
        _currentHealth -= damage;
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        
        if (_currentHealth <= 0)
        {
            OnDeath(damageCauserInfo);
        }
        photonView.RPC(nameof(RPC_UpdateHealth), RpcTarget.Others, _currentHealth);
    }

    public void Heal(float amount)
    {
        _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        photonView.RPC(nameof(RPC_UpdateHealth), RpcTarget.Others, _currentHealth);
    }

    [PunRPC]
    protected void RPC_UpdateHealth(float newHealth)
    {
        _currentHealth = newHealth;
    }

    public bool IsDead()
    {
        return _currentHealth <= 0;
    }

    protected abstract void OnDeath(WeaponInfo damageCauserInfo);

    private void OnGUI()
    {
        if (!photonView.IsMine) return;  // Only display health for the local player

        float healthPercentage = _currentHealth / _maxHealth;
        if (healthPercentage > 0.6f)
        {
            _guiStyle.normal.textColor = Color.green;
        }
        else if (healthPercentage > 0.3f)
        {
            _guiStyle.normal.textColor = Color.yellow;
        }
        else
        {
            _guiStyle.normal.textColor = Color.red;
        }

        GUI.Label(new Rect(10, 10, 200, 50), "Health: " + _currentHealth.ToString("0"), _guiStyle);
    }
}