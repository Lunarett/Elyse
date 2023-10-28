using Pulsar.Debug;
using UnityEngine;

/*
 * Apply this script to you Character's body parts and set your custom damage multipliers
 */
public class BodyDamageMultiplier : MonoBehaviour
{
    [SerializeField] private float _damageMultiplier = 1.0f;
    private PlayerHealth _playerHealth;

    private void Awake()
    {
        _playerHealth = transform.root.GetComponent<PlayerHealth>();
        DebugUtils.CheckForNull<PlayerHealth>(_playerHealth);
    }

    public void TakeDamage(float baseDamage, WeaponInfo damageCauserInfo)
    {
        float modifiedDamage = baseDamage * _damageMultiplier;
        _playerHealth.TakeDamage(modifiedDamage, damageCauserInfo);
    }
}
