using UnityEngine;

public struct DamageCauseInfo
{
    public float damage;
    public Pawn causer;
    public ECauseOfDeath CauseOfDeath;
}

public enum ECauseOfDeath
{
    KilledByPlayer,
    FellToDeath,
    FellOutOfWorld,
    SelfKill
}

public abstract class HealthBase : MonoBehaviour
{
    [SerializeField] protected float _maxHealth = 100f;

    protected float _currentHealth;
    protected Pawn _pawn;

    public float MaxHealth => _maxHealth;
    public float CurrentHealth => _currentHealth;
    
    
    protected virtual void Awake()
    {
        _pawn = GetComponent<Pawn>();
        _currentHealth = _maxHealth;
    }

    public bool IsDead()
    {
        return _currentHealth <= 0;
    }

    protected abstract void OnDeath();
    public abstract void ApplyDamage(DamageCauseInfo damageInfo);
    public abstract void ApplyHeal(float heal);
}