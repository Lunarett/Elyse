using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

namespace Pulsar.Core
{
    public abstract class Health : MonoBehaviour
    {
        [Header("Health Properties")]
        [SerializeField] protected float _maxHealth = 100.0f;

        [Header("Regeneration Properties")]
        [SerializeField] private bool _regenerate = true;
        [SerializeField] private float _regenerationSpeed = 1.0f;
        [SerializeField] private float _regenerationDelay = 1.0f;
        [SerializeField] private float _regenerationLimit = 90.0f;

        protected float _currentHealth;
        private float _timeSinceLastDamage;

        public UnityAction OnHealthChanged;
        public UnityAction OnTakeDamage;
        public UnityAction OnHeal;
        public UnityAction OnDeath;

        public float MaxHealth => _maxHealth;
        public float CurrentHealth => _currentHealth;

        private void Start()
        {
            _currentHealth = _maxHealth;
            _timeSinceLastDamage = _regenerationDelay;
        }

        private void Update()
        {
            // Update the time since last damage
            _timeSinceLastDamage += Time.deltaTime;

            // Regeneration logic
            if (_regenerate && _timeSinceLastDamage >= _regenerationDelay && _currentHealth < _regenerationLimit)
            {
                _currentHealth += _regenerationSpeed * Time.deltaTime;
                _currentHealth = Mathf.Min(_currentHealth, _maxHealth, _regenerationLimit);
                OnHealthChanged?.Invoke();
            }
        }

        public abstract void Damage(float damage, PhotonMessageInfo info = new PhotonMessageInfo());
        public abstract void Heal(float heal, PhotonMessageInfo info = new PhotonMessageInfo());

        protected abstract void Die();

        protected void ResetRegenerationDelay()
        {
            _timeSinceLastDamage = 0;
        }
    }
}
