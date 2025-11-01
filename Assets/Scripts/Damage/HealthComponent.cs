using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using ValueSystem;

namespace Damage
{
    public class HealthComponent : MonoBehaviour
    {
        public UnityAction<float, IDamageSource> onHurt;
        public UnityAction<float, IDamageSource, IDamageSource> onDamageReceived;
        public UnityAction onRecover;
        public UnityAction<float> onHeal;
        public UnityAction<float> onHealthChanged;
        public UnityAction<IDamageSource> onDie;

        public float CurrentHealth() => _currentHealth;

        private IDamageable _owner;
        private float _currentHealth;
        private float _currentMaxHealth;
        [SerializeField] private FloatSharedValueWithFallback _maxHealth = 100f;

        [SerializeField] private bool _canHeal = true;
        private bool _isDead;

        [SerializeField] private FloatSharedValueWithFallback _hurtTime = 0.1f;
        private bool _isHurt;
        private Coroutine _hurtCoroutine;

        private IDamageSource _lastDamageSource;

        private void Awake()
        {
            _owner = GetComponent<IDamageable>();
        }

        private void Start()
        {
            _currentMaxHealth = _maxHealth.Get();
            _currentHealth = _currentMaxHealth;
        }

        public void HandleDamage(float damage, IDamageSource source = null, IDamageSource causer = null)
        {
            if (_isDead) return;

            if (_isHurt) return;

            onDamageReceived?.Invoke(damage, source, causer);

            _lastDamageSource = source;

            var newHealth = _currentHealth - damage;
            _currentHealth = Mathf.Clamp(newHealth, 0, newHealth);
            onHealthChanged?.Invoke(_currentHealth);

            if (_currentHealth <= 0)
            {
                _isDead = true;
                onDie?.Invoke(_lastDamageSource);
                _owner.Destroyed();
            }
            else
            {
                onHurt?.Invoke(damage, causer);
                if (_hurtCoroutine is not null)
                    StopCoroutine(_hurtCoroutine);
                _hurtCoroutine = StartCoroutine(Hurting());
            }
        }

        public void Heal(float amount, bool shouldRecover = false)
        {
            if (!_canHeal) return;

            var newHealth = _currentHealth + amount;
            _currentHealth = Mathf.Clamp(newHealth, 0, _currentMaxHealth);
            onHealthChanged?.Invoke(_currentHealth);

            onHeal?.Invoke(amount);

            if (shouldRecover)
            {
                if (_hurtCoroutine is not null)
                    StopCoroutine(_hurtCoroutine);
                Recover();
            }
        }

        private IEnumerator Hurting()
        {
            _isHurt = true;
            var hurtTime = _hurtTime.Get();
            if (hurtTime > 0f)
                yield return new WaitForSeconds(hurtTime);
            Recover();
        }

        private void Recover()
        {
            _isHurt = false;
            onRecover?.Invoke();
        }
    }
}