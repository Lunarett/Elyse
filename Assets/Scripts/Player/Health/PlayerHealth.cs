using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using Pulsar.Debug;

public class PlayerHealth : BaseHealth
{
    // Define an event for when the player dies
    public event Action<DamageCauserInfo> OnPlayerDied;
    public event Action<float, float> OnHealthChanged;

    private ElyseCharacter _elyseCharacter;
    
    protected HUD _hud;
    

    protected override void Awake()
    {
        base.Awake();
        _hud = HUD.Instance;
        _elyseCharacter = GetComponent<ElyseCharacter>();
        DebugUtils.CheckForNull<ElyseCharacter>(_elyseCharacter);
        OnHealthChanged += UpdateHealthBar;
    }

    private void Start()
    {
        _hud.SetHeath(_currentHealth, _maxHealth);
    }

    protected override void OnDeath(DamageCauserInfo damageCauserInfo)
    {
        photonView.RPC(nameof(RPC_OnDeath), RpcTarget.All, damageCauserInfo.Serialize());
    }

    [PunRPC]
    private void RPC_OnDeath(byte[] serializedDamageCauserInfo)
    {
        DamageCauserInfo damageCauserInfo = new DamageCauserInfo();
        damageCauserInfo.Deserialize(serializedDamageCauserInfo);
        _hud.BroadcastGameFeed(damageCauserInfo.CauserOwner.NickName, photonView.Owner.NickName);
        
        // Trigger the OnPlayerDied event
        OnPlayerDied?.Invoke(damageCauserInfo);
        
        if (!photonView.IsMine) return;
        _hud.SetDamageScreenAlpha(0.5f);
    }
    
    public void TakeDamage(float damage, DamageCauserInfo damageCauserInfo)
    {
        Debug.Log("HealthBase TakeDamage Local");
        _currentHealth -= damage;
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        
        if (_currentHealth <= 0)
        {
            OnDeath(damageCauserInfo);
        }

        photonView.RPC(nameof(RPC_TakeDamage), RpcTarget.Others, damage);
    }

    public void Heal(float amount)
    {
        if (!photonView.IsMine) return;
        _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        photonView.RPC(nameof(RPC_Heal), RpcTarget.Others, amount);
    }

    [PunRPC]
    public void RPC_TakeDamage(float damage)
    {
        // Apply damage remotely
        Debug.Log("HealthBase TakeDamage RPC");
        _currentHealth -= damage;
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        if(!photonView.IsMine) return;
        _hud.PlayDamageEffect();
        _hud.SetHeath(_currentHealth, _maxHealth);
    }

    [PunRPC]
    public void RPC_Heal(float amount)
    {
        // Heal remotely
        _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        if(!photonView.IsMine) return;
        _hud.SetHeath(_currentHealth, _maxHealth);
    }
    
    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        _hud.SetHeath(currentHealth, maxHealth);
    }
}