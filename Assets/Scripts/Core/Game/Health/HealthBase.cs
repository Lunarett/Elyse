using System;
using UnityEngine;
using Photon.Pun;

public abstract class BaseHealth : MonoBehaviourPunCallbacks
{
    [SerializeField] protected float _maxHealth = 100f;

    protected float _currentHealth;

    public float MaxHealth => _maxHealth;
    public float CurrentHealth => _currentHealth;
    


    protected virtual void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public bool IsDead()
    {
        return _currentHealth <= 0;
    }

    protected abstract void OnDeath(DamageCauserInfo damageCauserInfo);
}