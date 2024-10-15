using System.Collections;
using System.Collections.Generic;
using SargeUniverse.Scripts.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace SargeUniverse.Scripts.Environment.Projectiles
{
    public class ProjectileAnimator : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer = null;
        [SerializeField] private SpriteDirectionCollection _moveAnimations = null;
        [SerializeField] private List<Sprite> _destroySprites = new();
        [SerializeField] private float _animationDelay = 0.05f;

        private MovementDirection _direction = MovementDirection.North;
        private IEnumerator _coroutine = null;

        public void SetProjectileDirection(MovementDirection direction, float time = 0)
        {
            _direction = direction;
            _moveAnimations.SetDirection(_direction);
            if (time > 0)
            {
                _coroutine = PlayTimedAnimation(time);
            }
            else
            {
                _coroutine = PlayLoopAnimation();
            }

            StartCoroutine(_coroutine);
        }
        
        public void PlayDestroyAnimation(UnityAction callback)
        {
            StopCoroutine(_coroutine);
            StartCoroutine(PlayAnimation(callback));
        }

        private IEnumerator PlayTimedAnimation(float time)
        {
            yield return new WaitForFixedUpdate();
            var animationLength = time;
            var collectionSize = _moveAnimations.GetCollectionSize();
            var spriteIndex = -1;
            
            while (time > 0)
            {
                if (collectionSize > 0)
                {
                    var index = Mathf.FloorToInt((animationLength - time) / animationLength * collectionSize);
                    if (index > spriteIndex)
                    {
                        _spriteRenderer.sprite = _moveAnimations.GetSprite();
                        spriteIndex = index;
                    }
                }

                time -= _animationDelay;
                yield return new WaitForSeconds(_animationDelay);
            }
        }

        private IEnumerator PlayLoopAnimation()
        {
            yield return new WaitForFixedUpdate();
            while (true)
            {
                _spriteRenderer.sprite = _moveAnimations.GetSprite();
                yield return new WaitForSeconds(_animationDelay);
            }
        }

        private IEnumerator PlayAnimation(UnityAction callback)
        {
            yield return new WaitForFixedUpdate();
            foreach (var sprite in _destroySprites)
            {
                _spriteRenderer.sprite = sprite;
                yield return new WaitForSeconds(_animationDelay);
            }
            
            callback.Invoke();
        }
    }
}