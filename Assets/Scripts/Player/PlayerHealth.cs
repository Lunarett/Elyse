using UnityEngine;
using Photon.Pun;
using Pulsar.Core;

namespace Pulsar.Player
{
    public class PlayerHealth : Health
    {
        private PhotonView _pv;

        private void Awake()
        {
            _pv = GetComponent<PhotonView>();
        }

        public override void Damage(float damage, PhotonMessageInfo info = new PhotonMessageInfo())
        {
            _currentHealth -= damage;

            // Trigger Events
            OnTakeDamage?.Invoke();
            OnHealthChanged?.Invoke();

            if (_currentHealth <= 0) Die();
        }

        public override void Heal(float heal, PhotonMessageInfo info = new PhotonMessageInfo())
        {
            _currentHealth += heal;
            _currentHealth = Mathf.Min(_currentHealth, _maxHealth);

            // Trigger Events
            OnHeal?.Invoke();
            OnHealthChanged?.Invoke();
        }

        protected override void Die()
        {
            _currentHealth = 0;
            OnDeath?.Invoke();
        }

        private void OnGUI()
        {
            if (_pv.IsMine) // Only display for the local player
            {
                GUI.Label(new Rect(10, 10, 100, 20), "Health: " + _currentHealth);
            }
        }
    }
}