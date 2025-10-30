using System;
using System.Collections;
using BlackCatPool;
using Damage;
using Effects;
using UnityEngine;

namespace Plane
{
    public class Plane : MonoBehaviour, IDamageable, IDamageSource
    {
        [SerializeField] private ETeam _team;
        [SerializeField] protected PlaneMovement _movement;
        [SerializeField] protected Cannon _cannon;
        [SerializeField] protected SpriteAnimationEffect _destroyedEffect;
        
        public PlaneMovement planeMovement => _movement;
        public Cannon Cannon => _cannon;
        public HealthComponent HealthComponent { get; set; }
        public Collider2D HurtBox { get; set; }
        public ETeam Team
        {
            get => _team; 
            set => _team = value;
        }

        private void Awake()
        {
            HealthComponent = GetComponent<HealthComponent>();
            HurtBox = GetComponent<Collider2D>();
        }
        
        public void TakeDamage(float amount, IDamageSource source = null, IDamageSource causer = null)
        {
            HealthComponent.HandleDamage(amount, source, causer);
        }
        
        public void Destroyed()
        {
            ParallaxController.Instance.AddToParallax(transform);
            HurtBox.enabled = false;
            planeMovement.enabled = false;
            var fallSpeed = 2f;
            var groundZ = 5;
            var rotateSpeed = 0.5f;
            var sizeScaledDown = 0.1f;
            LTDescr rotateTween, scaleDown;
            rotateTween = LeanTween.rotateAroundLocal(gameObject, Vector3.forward,  360f, rotateSpeed).setRepeat(-1);
            scaleDown = LeanTween.scale(gameObject, Vector3.one * sizeScaledDown, fallSpeed);
            LeanTween.moveZ(gameObject, groundZ, fallSpeed).setOnComplete(() =>
            {
                // ==Reached ground==
                //Start destroy effect
                enabled = false;
                var destroyed = ObjectPoolManager.Instance.GetObject<SpriteAnimationEffect>(_destroyedEffect.gameObject, true);
                ParallaxController.Instance.AddToParallax(destroyed.transform);
                destroyed.transform.localScale = Vector3.one * 0.5f;
                destroyed.StartEffect(transform.position, () =>
                {
                    ParallaxController.Instance.RemoveFromParallax(destroyed.transform);
                    Destroy(gameObject);
                });
                
                LeanTween.cancel(rotateTween.id);
                LeanTween.cancel(scaleDown.id);
            });
        }

        private void OnDestroy()
        {
            ParallaxController.Instance.RemoveFromParallax(transform);
        }

        public float GetDamage()
        {
            // noop
            return 0f;
        }

        public void ApplyDamage(IDamageable damageable)
        {
            // noop
        }
    }
}