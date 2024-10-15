using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Projectiles
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ProjectileExplosion : MonoBehaviour
    {
        [SerializeField] private List<Sprite> _sprites = new();
        [SerializeField] private float _animationDelay = 0.05f;
        
        private SpriteRenderer _spriteRenderer = null;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void PlayDestroyAnimation(Action callback)
        {
            StartCoroutine(PlayAnimation(callback));
        }
        
        private IEnumerator PlayAnimation(Action callback)
        {
            yield return new WaitForFixedUpdate();
            foreach (var sprite in _sprites)
            {
                _spriteRenderer.sprite = sprite;
                yield return new WaitForSeconds(_animationDelay);
            }
            
            callback.Invoke();
        }
    }
}