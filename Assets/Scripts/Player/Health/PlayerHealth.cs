using UnityEngine;
using System;
using Pulsar.Debug;
using QFSW.QC;

public class PlayerHealth : BaseHealth
{
    // Define an event for when the player dies
    public event Action OnPlayerDied;
    public event Action<float, float> OnHealthChanged;

    private ElyseCharacter _elyseCharacter;
    
    protected HUD _hud;
    

    protected override void Awake()
    {
        base.Awake();
        _hud = HUD.Instance;
        _elyseCharacter = GetComponent<ElyseCharacter>();
        DebugUtils.CheckForNull<ElyseCharacter>(_elyseCharacter);
    }

    private void Start()
    {
        _hud.SetHeath(_currentHealth, _maxHealth);
    }

    protected override void OnDeath()
    {
        Debug.LogWarning("PlayerHealth: Player has died!");
        _hud.BroadcastGameFeed("CauserName", "AffectedName");
        OnPlayerDied?.Invoke();
    }
    
    public void TakeDamage(float damage)
    {
        if (IsDead()) return;
        Debug.LogWarning($"PlayerHealth: Damage hit! Current Health: {_currentHealth}, Damage: {damage}");
        _currentHealth -= damage;
        _hud.SetHeath(_currentHealth, _maxHealth);
        if (_currentHealth <= 0)
        {
            _hud.SetDamageScreenAlpha(0.5f);
            OnDeath();
            return;
        }
        
        _hud.PlayDamageEffect();
    }

    public void Heal(float amount)
    {
        if (IsDead()) return;
        _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
        _hud.SetHeath(_currentHealth, _maxHealth);
    }
    
    [Command("kill.self", "Instantly kills you")]
    public void killSelf()
    {
        if (IsDead()) return;
        TakeDamage(_currentHealth);
    }
}