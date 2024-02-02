using System;
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
    private DamageCauseInfo _info;
    private Pawn _pawn;
    private float peakFallSpeed = 0f;
    
    private void Awake()
    {
        _pawn = GetComponent<Pawn>();
        _movement = GetComponent<PlayerMovement>();
        _playerHealth = GetComponent<PlayerHealth>();
    }

    private void Start()
    {
        _info = new DamageCauseInfo()
        {
            damage = 0.0f,
            causer = _pawn,
            CauseOfDeath = ECauseOfDeath.FellToDeath
        };
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
                    _info.damage = CalculateFallDamage(peakFallSpeed);
                    _playerHealth.ApplyDamage(_info);
                    PlayDamageSound(_info.damage);
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
