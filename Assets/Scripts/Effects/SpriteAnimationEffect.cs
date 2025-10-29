using System;
using System.Collections;
using System.Collections.Generic;
using Alchemy.Inspector;
using BlackCatPool;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = System.Object;

namespace Effects
{
    public class SpriteAnimationEffect : MonoBehaviour, IPoolable
    {
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private List<Sprite> _spriteFrames = new();
        [SerializeField] private float _frameDuration;
        
        [SerializeField] private bool _returnToPoolAfterLastFrame = true;
        [SerializeField, HideIf(nameof(_returnToPoolAfterLastFrame))]
        private float _durationBeforeRePooled;

        private int _frameCount => _spriteFrames.Count;
        private Transform _anchorTransform;
        private Coroutine _frameRoutine;

#if UNITY_EDITOR
            
        [Button]
        private void LoadSpriteFromTexture(Texture2D texture)
        {
            if (texture is null) {
                Debug.LogError($"No input texture");
                return;
            }
            
            var data = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(texture));
            if (data == null)  {
                Debug.LogError($"No data for texture");
                return;
            }
            
            _spriteFrames.Clear();
            foreach (var obj in data)
            {
                var sprite = obj as Sprite;
                if (sprite is not null)
                {
                    _spriteFrames.Add(sprite);
                }
            }
        }
#endif
        
        public void OnCreated()
        {
            if (_spriteFrames.Count == 0)
            {
                Debug.LogError("No sprite frames for this effect");
                return;
            }
            _renderer.sprite = _spriteFrames[0];
        }

        public void OnObtained()
        {
            if (_frameRoutine is not null)
                StopCoroutine(_frameRoutine);
        }

        public void OnPooled()
        {
            _anchorTransform = null;
        }
        public void OnDestroyed() { }

        public void StartEffect()
        {
            StartCoroutine(EffectRoutine());
        }
        
        public void StartEffect(Vector3 position, Transform anchorTransform = null)
        {
            SetEffectPosition(position, anchorTransform);
            StartEffect();
        }

        private void SetEffectPosition(Vector3 position, Transform anchorTransform)
        {
            transform.position = position;
            if (anchorTransform != null)
            {
                _anchorTransform = anchorTransform;
                transform.SetParent(_anchorTransform);
            }
        }

        private IEnumerator EffectRoutine()
        {
            yield return PlayFrames();
            if  (!_returnToPoolAfterLastFrame)
                yield return new WaitForSeconds(_durationBeforeRePooled);
            gameObject.ReturnToPool(); 
        }

        private IEnumerator PlayFrames()
        {
            foreach (var frame in _spriteFrames)
            {
                _renderer.sprite = frame;
                yield return new WaitForSeconds(_frameDuration);
            }
        }
    }
}