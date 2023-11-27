using Pulsar.Debug;
using UnityEngine;

/*
 * Apply this script to you Character's body parts and set your custom damage multipliers
 */
public class BodyDamageMultiplier : MonoBehaviour
{
    [SerializeField] private float _damageMultiplier = 1.0f;
    [SerializeField] private bool _isHead;
    
    private PlayerHealth _playerHealth;
    
    private void Awake()
    {
        _playerHealth = transform.root.GetComponent<PlayerHealth>();
        DebugUtils.CheckForNull<PlayerHealth>(_playerHealth);
    }

    public void TakeDamage(float baseDamage)
    {
        float modifiedDamage = baseDamage * _damageMultiplier;
        Debug.Log($"BodyDamageMultiplier: Base damage {baseDamage}, new damage {modifiedDamage}");
        _playerHealth.TakeDamage(modifiedDamage);
    }
}
