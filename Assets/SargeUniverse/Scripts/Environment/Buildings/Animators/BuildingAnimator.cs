using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SargeUniverse.Scripts.Enums;
using SargeUniverse.Scripts.Utils;
using Server;
using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Buildings.Animators
{
    public class BuildingAnimator : MonoBehaviour, IBuildingAnimator
    {
        [SerializeField] protected SpriteRenderer _spriteRenderer = null;

        [Header("Idle animation settings")]
        [SerializeField] protected List<SpriteDirectionCollection> _idleSpriteCollection = null;
        [SerializeField] protected float _delayBetweenIdleSlides = 0.1f;
        
        [Header("Attack animation settings")]
        [SerializeField] protected List<SpriteDirectionCollection> _attackSpriteCollection = null;
        [SerializeField] protected float _delayBetweenAttackSlides = 0.1f;
        [SerializeField] protected float _callbackDelay = -1f;
        
        [Header("Destroy animation settings")]
        [SerializeField] protected List<Sprite> _destroySprites = new();
        [SerializeField] protected float _delayBetweenDestroySlides = 0.05f;

        protected MovementDirection _direction = MovementDirection.North;
        protected Queue<IEnumerator> _coroutines = new();
        protected IEnumerator _coroutine = null;

        public virtual void SetDirection(int level, MovementDirection direction)
        {
            RotateToDirection(direction, level);
        }

        public virtual void PlayIdleAnimation(int level)
        {
            if (_coroutines.Count == 0 && _coroutine == null)
            {
                PlayIdleAnimation(level, _direction);
            }
        }
        
        public virtual void PlayIdleAnimation(int level, MovementDirection direction)
        {
            if (_idleSpriteCollection.Count <= 0 || _idleSpriteCollection.Count < level)
            {
                return;
            }

            var collection = (SpriteDirectionCollection)_idleSpriteCollection[level - 1].Clone();
            collection.SetDirection(direction);
            _coroutines.Enqueue(PlayAnimation(collection, _delayBetweenIdleSlides, () => SetDirection(direction)));
            PlayAnimation();
        }
        
        public virtual void PlayAttackAnimation(int level, Action callback = null)
        {
            PlayAttackAnimation(level, _direction, callback);
        }
        
        public virtual void PlayAttackAnimation(int level, MovementDirection direction, Action callback = null)
        {
            if (_attackSpriteCollection.Count <= 0 || _attackSpriteCollection.Count < level)
            {
                return;
            }
            
            var collection = _attackSpriteCollection[level - 1];
            collection.SetDirection(direction);
            _coroutines.Enqueue(PlayAnimation(collection, _delayBetweenAttackSlides, callback));
            PlayAnimation();
        }
        
        public virtual void PlayDestroyAnimation()
        {
            _coroutines.Enqueue(PlayAnimation(_destroySprites, _delayBetweenDestroySlides));
            PlayAnimation();
        }
        
        public virtual void StopAnimation()
        {
        }

        protected virtual IEnumerator PlayAnimation(
            SpriteDirectionCollection directionCollection,
            float delayBetweenSlides, 
            Action callback = null)
        {
            yield return new WaitForEndOfFrame();

            if (_callbackDelay > 0)
            {
                StartCoroutine(InvokeCallbackWithDelay(callback, _callbackDelay));
            }
            
            if (directionCollection == null)
            {
                yield return null;
            }
            else
            {
                while (!directionCollection.IsLastSpriteInCollection())
                {
                    _spriteRenderer.sprite = directionCollection.GetSprite();
                    yield return new WaitForSeconds(delayBetweenSlides);
                }
                _spriteRenderer.sprite = directionCollection.GetSprite();
                yield return new WaitForSeconds(delayBetweenSlides);
            }

            if (_callbackDelay < 0)
            {
                callback?.Invoke();
            }
            
            _coroutine = null;
            PlayAnimation();
        }

        private IEnumerator InvokeCallbackWithDelay(Action callback, float delay)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }
        
        protected virtual IEnumerator PlayAnimation(
            List<Sprite> sprites, 
            float delayBetweenSlides)
        {
            yield return new WaitForEndOfFrame();
            if (sprites.Count == 0)
            {
                _spriteRenderer.sprite = null;
            }
            else
            {
                foreach (var sprite in sprites)
                {
                    _spriteRenderer.sprite = sprite;
                    yield return new WaitForSeconds(delayBetweenSlides);
                }
            }
            
            _coroutine = null;
            PlayAnimation();
        }
        
        private void RotateToDirection(MovementDirection direction, int level)
        {
            if (_direction == direction)
            {
                return;
            }
            
            var directions = DirectionUtils.GetRotationList(_direction, direction);
            foreach (var dir in directions)
            {
                PlayIdleAnimation(level, dir);
            }
        }

        protected void PlayAnimation()
        {
            if (_coroutines.Count == 0 || _coroutine != null)
            {
                return;
            }
            
            _coroutine = _coroutines.Dequeue();
            StartCoroutine(_coroutine);
        }

        protected void SetDirection(MovementDirection direction)
        {
            _direction = direction;
        }
    }
}