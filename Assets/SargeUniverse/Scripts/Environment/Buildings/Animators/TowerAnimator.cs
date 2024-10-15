using System;
using System.Collections;
using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Buildings.Animators
{
    public class TowerAnimator : BuildingAnimator
    {
        [Header("Tower Settings")]
        [SerializeField] private SpriteRenderer _gunSpriteRenderer = null;
        [SerializeField] private SpriteRenderer _roofSpriteRenderer = null;
        
        public override void PlayDestroyAnimation()
        {
            base.PlayDestroyAnimation();
            _gunSpriteRenderer.enabled = false;
            _roofSpriteRenderer.enabled = false;
        }
        
        protected override IEnumerator PlayAnimation(
            SpriteDirectionCollection directionCollection,
            float delayBetweenSlides,
            Action callback = null)
        {
            yield return new WaitForEndOfFrame();
            if (directionCollection == null)
            {
                yield return null;
            }
            else
            {
                while (!directionCollection.IsLastSpriteInCollection())
                {
                    _gunSpriteRenderer.sprite = directionCollection.GetSprite();
                    yield return new WaitForSeconds(delayBetweenSlides);
                }
                _gunSpriteRenderer.sprite = directionCollection.GetSprite();
                yield return new WaitForSeconds(delayBetweenSlides);
            }

            callback?.Invoke();
            
            _coroutine = null;
            PlayAnimation();
        }
    }
}