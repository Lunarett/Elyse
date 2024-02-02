using UnityEngine;
using System;
using Pulsar.Debug;
using QFSW.QC;

public class PlayerHealth : HealthBase
{
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

    public override void ApplyDamage(DamageCauseInfo damageInfo)
    {
        if (IsDead()) return;
        _currentHealth -= damageInfo.damage;
        _hud.SetHeath(_currentHealth, _maxHealth);
        if (_currentHealth <= 0)
        {
            _hud.SetDamageScreenAlpha(0.5f);
            OnDeath();
            return;
        }
        
        _hud.PlayDamageEffect();
    }

    public override void ApplyHeal(float heal)
    {
        if (IsDead()) return;
        _currentHealth = Mathf.Min(_currentHealth + heal, _maxHealth);
        _hud.SetHeath(_currentHealth, _maxHealth);
    }
    
    [Command("kill.self", "Instantly kills you")]
    public void killSelf()
    {
        if (IsDead()) return;

        DamageCauseInfo info = new DamageCauseInfo();
        info.damage = _currentHealth;
        info.causer = _pawn;
        info.CauseOfDeath = ECauseOfDeath.SelfKill;
        
        ApplyDamage(info);
    }
}