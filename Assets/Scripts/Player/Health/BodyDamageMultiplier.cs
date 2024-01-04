using Pulsar.Debug;
using UnityEngine;

/*
 * Apply this script to you Character's body parts and set your custom damage multipliers
 */
public class BodyDamageMultiplier : MonoBehaviour
{
    [SerializeField] private float _damageMultiplier = 1.0f;
    [SerializeField] private bool _isHead;
    
    private HealthBase _health;
    
    private void Awake()
    {
        _health = transform.root.GetComponent<HealthBase>();
        DebugUtils.CheckForNull<HealthBase>(_health);
    }

    public void ApplyDamage(DamageCauseInfo damageInfo)
    {
        damageInfo.damage *= _damageMultiplier;
        _health.ApplyDamage(damageInfo);
    }
}
