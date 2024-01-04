using UnityEngine;

public class AIHealth : HealthBase
{
    protected override void OnDeath()
    {
        gameObject.SetActive(false);
    }

    public override void ApplyDamage(DamageCauseInfo damageInfo)
    {
        if (IsDead()) return;
        _currentHealth -= damageInfo.damage;
        if (!(_currentHealth <= 0)) return;
        OnDeath();
    }

    public override void ApplyHeal(float heal)
    {
        if (IsDead()) return;
        _currentHealth = Mathf.Min(_currentHealth + heal, _maxHealth);
    }
}
