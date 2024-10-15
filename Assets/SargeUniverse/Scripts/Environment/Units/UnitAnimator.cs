using System.Collections;
using SargeUniverse.Scripts.Enums;
using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Units
{
    public class UnitAnimator : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        
        [Header("Idle Animations")]
        [SerializeField] private SpriteDirectionCollection _idleAnimations = null;
        [SerializeField] private float _delayBetweenIdleSlides = 0.1f;
        
        [Header("Movement Animations")]
        [SerializeField] private SpriteDirectionCollection _moveAnimations = null;
        [SerializeField] private float _delayBetweenMoveSlides = 0.1f;
        
        [Header("Attack Animations")]
        [SerializeField] private SpriteDirectionCollection _attackAnimations = null;
        [SerializeField] private float _delayBetweenAttackSlides = 0.1f;
        [SerializeField] private float _delayAfterAttackSlides = 0.1f;
        [SerializeField] private int _spriteOffset = 0;

        private MovementDirection _direction = MovementDirection.North;
        private IEnumerator _coroutine = null;

        public void SetMovementDirection(MovementDirection movementDirection)
        {
            _direction = movementDirection;
        }
        
        public void PlayIdleAnimation()
        {
            StopAnimation();
            _coroutine = PlayAnimation(_idleAnimations, _delayBetweenMoveSlides, _delayBetweenMoveSlides);
            StartCoroutine(_coroutine);
        }
        
        public void PlayMoveAnimation()
        {
            StopAnimation();
            _moveAnimations.SetDirection(_direction);
            _coroutine = PlayAnimation(_moveAnimations, _delayBetweenMoveSlides, _delayBetweenMoveSlides);
            StartCoroutine(_coroutine);
        }

        public void PlayAttackAnimation()
        {
            StopAnimation();
            _attackAnimations.SetDirection(_direction);
            _coroutine = PlayAnimation(_attackAnimations, _delayBetweenAttackSlides, _delayAfterAttackSlides, _spriteOffset);
            StartCoroutine(_coroutine);
        }

        public void PlayCombinedAttackAnimation()
        {
            StopAnimation();
            _attackAnimations.SetDirection(_direction);
            _moveAnimations.SetDirection(_direction);
            _coroutine = PlayCombinedAnimation(
                _attackAnimations, 
                _moveAnimations, 
                _delayBetweenAttackSlides,
                _delayBetweenMoveSlides,
                _delayAfterAttackSlides, 
                _spriteOffset);
            StartCoroutine(_coroutine);
        }

        private void StopAnimation()
        {
            if (_coroutine == null) return;
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
        
        private IEnumerator PlayAnimation(
            SpriteDirectionCollection directionCollection, 
            float delayBetweenSlides, 
            float delayAfterSlides,
            int spriteOffset = 0)
        {
            yield return new WaitForEndOfFrame();
            while (true)
            {
                _spriteRenderer.sprite = directionCollection.GetSprite(spriteOffset);
                if (directionCollection.IsLastSpriteInCollection())
                {
                    yield return new WaitForSeconds(delayAfterSlides);
                }
                else
                {
                    yield return new WaitForSeconds(delayBetweenSlides);
                }
            }
        }

        private IEnumerator PlayCombinedAnimation(
            SpriteDirectionCollection firstDirectionCollection,
            SpriteDirectionCollection secondDirectionCollection,
            float delayBetweenFirstSlides,
            float delayBetweenSecondSlides, 
            float delayAfterFirstSlides,
            int spriteOffset = 0
            )
        {
            yield return new WaitForEndOfFrame();
            var timer = delayAfterFirstSlides;
            
            while (true)
            {
                if (!firstDirectionCollection.IsLastSpriteInCollection())
                {
                    _spriteRenderer.sprite = firstDirectionCollection.GetSprite(spriteOffset);
                    yield return new WaitForSeconds(delayBetweenFirstSlides);

                    if (firstDirectionCollection.IsLastSpriteInCollection())
                    {
                        secondDirectionCollection.Reset();
                        timer = delayAfterFirstSlides - delayBetweenFirstSlides;
                    }
                }
                else
                {
                    while (timer > 0)
                    {
                        _spriteRenderer.sprite = secondDirectionCollection.GetSprite();
                        timer -= delayBetweenSecondSlides;
                        yield return new WaitForSeconds(delayBetweenSecondSlides);
                    }
                    firstDirectionCollection.Reset();
                }
            }
        }
    }
}