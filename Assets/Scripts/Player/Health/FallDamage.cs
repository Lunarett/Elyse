using UnityEngine;
using FMODUnity; // Include the FMODUnity namespace

public class FallDamage : MonoBehaviour
{
    [Header("Fall Damage Settings")]
    [SerializeField] private bool _enableFallDamage = true;
    [SerializeField] private float _minFallSpeedThreshold = 10.0f;
    [SerializeField] private float _lethalFallSpeed = 30.0f;
    [SerializeField] private float _minFallDamage = 10.0f;
    [SerializeField] private float _maxFallDamage = 95.0f;

    [Header("FMOD Audio Events")]
    [SerializeField] private EventReference landingSoundEvent;
    [SerializeField] private EventReference damageSoundEvent;

    private PlayerMovement _movement;
    private PlayerHealth _playerHealth;
    private float peakFallSpeed = 0f;
    
    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (!_movement.IsGrounded)
        {
            float currentFallSpeed = -_movement.CharacterVelocity.y;
            peakFallSpeed = Mathf.Max(peakFallSpeed, currentFallSpeed);
        }
        else if (_movement.WasGrounded)
        {
            if (peakFallSpeed > _minFallSpeedThreshold)
            {
                PlayLandingSound();
                
                if (_enableFallDamage)
                {
                    float fallDamage = CalculateFallDamage(peakFallSpeed);
                    _playerHealth.TakeDamage(fallDamage);
                    PlayDamageSound(fallDamage);
                }
            }
            // Reset peak fall speed
            peakFallSpeed = 0f;
        }
    }

    private float CalculateFallDamage(float fallSpeed)
    {
        if (fallSpeed >= _lethalFallSpeed)
            return _maxFallDamage;

        float normalizedSpeed = (fallSpeed - _minFallSpeedThreshold) / (_lethalFallSpeed - _minFallSpeedThreshold);
        return Mathf.Lerp(_minFallDamage, _maxFallDamage, normalizedSpeed);
    }

    private void PlayLandingSound()
    {
        RuntimeManager.PlayOneShot(landingSoundEvent, transform.position);
    }

    private void PlayDamageSound(float damage)
    {
        if (damage > 0)
        {
            RuntimeManager.PlayOneShot(damageSoundEvent, transform.position);
        }
    }
}
