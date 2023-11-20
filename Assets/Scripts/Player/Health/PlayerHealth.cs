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

        // Assuming damageCauserInfo.CauserOwner is the Photon.Realtime.Player who caused the death
        var attackerController = Controller.Find(damageCauserInfo.CauserOwner);
        if (attackerController != null)
        {
            var attackerElysePlayerState = attackerController.GetComponent<ElysePlayerState>();
            if (attackerElysePlayerState != null)
            {
                attackerElysePlayerState.AddKill();
            }
        }
    }

    [PunRPC]
    private void RPC_OnDeath(byte[] serializedDamageCauserInfo)
    {
        DamageCauserInfo damageCauserInfo = new DamageCauserInfo();
        damageCauserInfo.Deserialize(serializedDamageCauserInfo);
        _hud.BroadcastGameFeed(damageCauserInfo.CauserOwner.NickName, photonView.Owner.NickName);
    
        // Trigger the OnPlayerDied event
        OnPlayerDied?.Invoke(damageCauserInfo);

        // Check if this is the local player
        if (photonView.IsMine)
        {
            _hud.SetDamageScreenAlpha(0.5f);
        }

        if (PhotonNetwork.LocalPlayer == damageCauserInfo.CauserOwner)
        {
            var attackerElysePlayerState = GetComponent<ElysePlayerState>();
            if (attackerElysePlayerState != null)
            {
                attackerElysePlayerState.AddKill();
            }
        }
    }
    
    public void TakeDamage(float damage, DamageCauserInfo damageCauserInfo)
    {
        if (IsDead()) return;
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
        if (IsDead()) return;
        _currentHealth -= damage;
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        if(!photonView.IsMine) return;
        _hud.PlayDamageEffect();
        _hud.SetHeath(_currentHealth, _maxHealth);
    }

    [PunRPC]
    public void RPC_Heal(float amount)
    {
        if (IsDead()) return;
        _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        if(!photonView.IsMine) return;
        _hud.SetHeath(_currentHealth, _maxHealth);
    }
    
    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (IsDead()) return;
        if (photonView.IsMine)
        {
            _hud.SetHeath(currentHealth, maxHealth);
        }
    }
}