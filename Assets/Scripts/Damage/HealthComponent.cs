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
        public bool IsHurt => _isHurt;

        private IDamageable _owner;
        private float _currentHealth;
        [SerializeField] private float _maxHealth = 100;

        [SerializeField] private bool _canHeal = true;
        private bool _isDead;

        [SerializeField] private float _hurtTime = 0.1f;
        private bool _isHurt;
        private Coroutine _hurtCoroutine;

        private IDamageSource _lastDamageSource;

        private void Awake()
        {
            _owner = GetComponent<IDamageable>();
            _currentHealth = _maxHealth;
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
            _currentHealth = Mathf.Clamp(newHealth, 0, _maxHealth);
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
            if (_hurtTime > 0f)
                yield return new WaitForSeconds(_hurtTime);
            Recover();
        }

        private void Recover()
        {
            _isHurt = false;
            onRecover?.Invoke();
        }
    }
}